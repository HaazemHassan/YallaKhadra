using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using YallaKhadra.Core.Entities.IdentityEntities;

namespace YallaKhadra.API.Bases.DataSeeding {
    public static class ApplicationRoleSeeder {
        public static async Task SeedAsync(RoleManager<ApplicationRole> _roleManager) {
            string rolesJson = await File.ReadAllTextAsync("ApplicationRoles.json");
            List<string>? roles = JsonSerializer.Deserialize<List<string>>(rolesJson);

            int rolesInDb = await _roleManager.Roles.CountAsync();
            if (roles is null || rolesInDb > 0)
                return;

            foreach (var role in roles) {
                await _roleManager.CreateAsync(new ApplicationRole(role));
            }
        }

    }
}
