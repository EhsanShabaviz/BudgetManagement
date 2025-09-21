using BudgetManagement.Application.Interfaces;
using BudgetManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BudgetManagement.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditLogger _auditLogger;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IAuditLogger auditLogger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _auditLogger = auditLogger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "نام کاربری یا ایمیل الزامی است")]
            [Display(Name = "نام کاربری یا ایمیل")]
            public string UserNameOrEmail { get; set; } = "";

            [Required(ErrorMessage = "رمز عبور الزامی است")]
            [DataType(DataType.Password)]
            [Display(Name = "رمز عبور")]
            public string Password { get; set; } = "";

            [Display(Name = "مرا به خاطر بسپار")]
            public bool RememberMe { get; set; }
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByNameAsync(Input.UserNameOrEmail)
                       ?? await _userManager.FindByEmailAsync(Input.UserNameOrEmail);


            if (user == null)
            {
                await _auditLogger.LogAsync("", "Unknown", "LoginFailed", Input.UserNameOrEmail);
                ModelState.AddModelError(string.Empty, "نام کاربری یا رمز عبور اشتباه است.");
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                Input.Password,
                Input.RememberMe,
                lockoutOnFailure: true
            );

            if (result.Succeeded)
            {
                await _auditLogger.LogAsync(user.Id, user.UserName!, "LoginSuccess");
                //return LocalRedirect(ReturnUrl);
                return Redirect("~/");

            }

            if (result.IsLockedOut)
            {
                await _auditLogger.LogAsync(user.Id, user.UserName!, "AccountLocked");
                ModelState.AddModelError(string.Empty, "حساب کاربری شما قفل شده است.");
                return Page();
            }


            //پسورد اشتباه ولی کاربر وجود دارد
            await _auditLogger.LogAsync(user.Id, user.UserName!, "LoginFailed");
            ModelState.AddModelError(string.Empty, "نام کاربری یا رمز عبور اشتباه است.");
            return Page();
        }
    }
}
