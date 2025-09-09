using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace BudgetManagement.Infrastructure.Identity
{
    public static class IdentitySeed
    {
        public static async Task EnsureSeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // نقش‌ها
            var roles = new[] { "Admin", "UserManager", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = role,
                        Description = $"Role: {role}"
                    });
                }
            }

            // کاربر ادمین
            var adminUserName = "admin";
            var adminEmail = "admin@budget.com";
            var adminNationalCode = "1234567890";
            var admin = await userManager.FindByNameAsync(adminUserName);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    NationalCode = adminNationalCode,
                    EmailConfirmed = true,
                    FullName = "System Administrator"
                };
                var createResult = await userManager.CreateAsync(admin, "Admin@12345");// Change Password Later!
                if (createResult.Succeeded)
                {
                    await userManager.AddToRolesAsync(admin, new[] { "Admin", "UserManager" });
                }
            }
        }
    }
}
