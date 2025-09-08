using System.ComponentModel.DataAnnotations;
using BudgetManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BudgetManagement.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
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
                return LocalRedirect(ReturnUrl);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "حساب شما قفل شده است.");
                return Page();
            }

            ModelState.AddModelError(string.Empty, "نام کاربری یا رمز عبور اشتباه است.");
            return Page();
        }
    }
}
