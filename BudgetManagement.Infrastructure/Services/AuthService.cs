using BudgetManagement.Application.DTOs;
using BudgetManagement.Application.Interfaces;
using BudgetManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BudgetManagement.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditLogger _auditLogger;

        public AuthService(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IAuditLogger auditLogger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _auditLogger = auditLogger;
        }

        public async Task<AuthResultDto> LoginAsync(string usernameOrEmail, string password, bool rememberMe)
        {
            var user = await _userManager.FindByNameAsync(usernameOrEmail)
                       ?? await _userManager.FindByEmailAsync(usernameOrEmail);

            if (user == null)
            {
                await _auditLogger.LogAsync("", "Unknown", "LoginFailed", usernameOrEmail);
                return new AuthResultDto { Succeeded = false, ErrorMessage = "نام کاربری یا رمز عبور اشتباه است." };
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                password,
                rememberMe,
                lockoutOnFailure: true
            );

            if (result.Succeeded)
            {
                await _auditLogger.LogAsync(user.Id, user.UserName!, "LoginSuccess");
                return new AuthResultDto { Succeeded = true };
            }

            if (result.IsLockedOut)
            {
                await _auditLogger.LogAsync(user.Id, user.UserName!, "AccountLocked");
                return new AuthResultDto { Succeeded = false, ErrorMessage = "حساب کاربری شما قفل شده است." };
            }

            // پسورد اشتباه ولی کاربر وجود دارد
            await _auditLogger.LogAsync(user.Id, user.UserName!, "LoginFailed");
            return new AuthResultDto { Succeeded = false, ErrorMessage = "نام کاربری یا رمز عبور اشتباه است." };
        }

        public async Task LogoutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _auditLogger.LogAsync(user.Id, user.UserName!, "Logout");
            }

            await _signInManager.SignOutAsync();
        }
    }
}
