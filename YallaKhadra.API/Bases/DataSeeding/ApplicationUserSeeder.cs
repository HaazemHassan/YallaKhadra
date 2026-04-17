using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Enums;

namespace YallaKhadra.API.Bases.DataSeeding
{
    public static class ApplicationUserSeeder
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> _userManager)
        {
            string usersJson = await File.ReadAllTextAsync("ApplicationUsers.json");
            List<SeedApplicationUser>? users = JsonSerializer.Deserialize<List<SeedApplicationUser>>(usersJson);

            int rolesInDb = await _userManager.Users.CountAsync();
            if (users is null || rolesInDb > 0)
                return;

            foreach (var user in users)
            {
                var appUser = new ApplicationUser {
                    Email = user.Email,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed
                };

                await _userManager.CreateAsync(appUser, "12345678");

                var role = Enum.TryParse<UserRole>(user.Role, true, out var parsedRole)
                    ? parsedRole.ToString()
                    : UserRole.User.ToString();

                await _userManager.AddToRoleAsync(appUser, role);
            }
        }

        private sealed class SeedApplicationUser
        {
            public string Email { get; set; } = string.Empty;
            public string UserName { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public bool EmailConfirmed { get; set; }
            public bool PhoneNumberConfirmed { get; set; }
            public string Role { get; set; } = UserRole.User.ToString();
        }


    }
}
