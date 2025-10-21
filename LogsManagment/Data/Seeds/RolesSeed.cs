using LogsManagment.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace LogsManagment.Data.Seeds
{
    public static class RolesSeed
    {
        public static void Seed(RoleManager<IdentityRole<int>> roleManager, UserManager<AppUser> userManager)
        {
            string[] roleNames = { "Admin", "User" };

            // ✅ إنشاء الأدوار لو مش موجودة
            foreach (var roleName in roleNames)
            {
                var roleExists = roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult();
                if (!roleExists)
                {
                    roleManager.CreateAsync(new IdentityRole<int>(roleName)).GetAwaiter().GetResult();
                }
            }

            // ✅ إنشاء Admin افتراضي
            var adminEmail = "admin@system.com";
            var adminUser = userManager.FindByEmailAsync(adminEmail).GetAwaiter().GetResult();

            if (adminUser == null)
            {
                var user = new AppUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = userManager.CreateAsync(user, "Admin@123").GetAwaiter().GetResult();
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
                }
            }
        }
    }
}
