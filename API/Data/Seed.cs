using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;

namespace API.Data
{
    public class Seed
    {
        public async static Task<int> SeedUsers(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager
            )
        {
            if (userManager.Users.Any())
            {
                return 0;
            }
            var userData = File.ReadAllText(@"Data\UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            if (users == null)
            {
                return 0;
            }
            var roles = new List<AppRole>
            {
                new AppRole(){ Name="Member"},
                new AppRole(){ Name="Admin"},
                new AppRole(){ Name="Moderator"},
            };
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            foreach (var user in users)
            {
                user.UserName = user.UserName.ToLower();
                await userManager.CreateAsync(user, "Abc12345@");
                await userManager.AddToRoleAsync(user, "Member");
            }
            var admin = new AppUser
            {
                UserName = "admin"
            };
            await userManager.CreateAsync(admin, "Abc12345@");
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
            return users.Count;
        }
    }
}