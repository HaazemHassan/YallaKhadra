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

            if (users is null || users.Count == 0)
                return;

            var existingKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var existingEmails = await _userManager.Users
                .Where(u => u.Email != null)
                .Select(u => u.Email!)
                .ToListAsync();

            var existingUserNames = await _userManager.Users
                .Where(u => u.UserName != null)
                .Select(u => u.UserName!)
                .ToListAsync();

            foreach (var email in existingEmails)
                existingKeys.Add($"email:{email}");

            foreach (var userName in existingUserNames)
                existingKeys.Add($"username:{userName}");

            foreach (var user in users)
            {
                string emailKey = $"email:{user.Email}";
                string userNameKey = $"username:{user.UserName}";

                if (existingKeys.Contains(emailKey) || existingKeys.Contains(userNameKey))
                    continue;

                var appUser = new ApplicationUser {
                    Email = user.Email,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed
                };

                var createResult = await _userManager.CreateAsync(appUser, "12345678");
                if (!createResult.Succeeded)
                    continue;

                existingKeys.Add(emailKey);
                existingKeys.Add(userNameKey);

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
