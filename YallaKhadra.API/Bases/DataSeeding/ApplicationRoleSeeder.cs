using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using YallaKhadra.Core.Entities.IdentityEntities;

namespace YallaKhadra.API.Bases.DataSeeding {


    public static class ApplicationRoleSeeder {

        public static async Task SeedAsync(RoleManager<ApplicationRole> _roleManager) {
            string rolesJson = await File.ReadAllTextAsync("ApplicationRoles.json");

            List<string>? roles =
                JsonSerializer.Deserialize<List<string>>(rolesJson);

            if (roles is null)
                return;

            var normalizedRoles = roles.Where(r => !string.IsNullOrWhiteSpace(r))
                                                 .Select(r => r.Trim())
                                                 .Distinct(StringComparer.OrdinalIgnoreCase);

            foreach (var role in normalizedRoles) {
                var roleInDb = await _roleManager.FindByNameAsync(role);
                if (roleInDb is null) {
                    roleInDb = new ApplicationRole(role);
                    await _roleManager.CreateAsync(roleInDb);
                }



            }
        }
    }
}