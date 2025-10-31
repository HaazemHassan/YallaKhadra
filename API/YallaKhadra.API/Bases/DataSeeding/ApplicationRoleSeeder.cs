using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text.Json;
using YallaKhadra.Core.Entities.IdentityEntities;

namespace YallaKhadra.API.Bases.DataSeeding {
    public class RoleDefinition {
        public string RoleName { get; set; }
        public List<string> Permissions { get; set; }
    }

    public static class ApplicationRoleSeeder {
        private const string PermissionClaimType = "Permission";

        public static async Task SeedAsync(RoleManager<ApplicationRole> _roleManager) {
            string rolesJson = await File.ReadAllTextAsync("ApplicationRoles.json");

            List<RoleDefinition>? roleDefinitions =
                JsonSerializer.Deserialize<List<RoleDefinition>>(rolesJson);

            if (roleDefinitions is null)
                return;

            foreach (var roleDef in roleDefinitions) {
                var roleInDb = await _roleManager.FindByNameAsync(roleDef.RoleName);
                if (roleInDb is null) {
                    roleInDb = new ApplicationRole(roleDef.RoleName);
                    await _roleManager.CreateAsync(roleInDb);
                }

                if (roleDef.Permissions is null || !roleDef.Permissions.Any())
                    continue;


                var existingClaims = await _roleManager.GetClaimsAsync(roleInDb);

                foreach (var permissionValue in roleDef.Permissions) {
                    if (!existingClaims.Any(c => c.Type == PermissionClaimType && c.Value == permissionValue)) {
                        var claimToAdd = new Claim(PermissionClaimType, permissionValue);
                        await _roleManager.AddClaimAsync(roleInDb, claimToAdd);
                    }
                }
            }
        }
    }
}