﻿using Microsoft.AspNetCore.Identity;

namespace YallaKhadra.Core.Entities.IdentityEntities {
    public class ApplicationRole : IdentityRole<Guid> {
        public ApplicationRole() {

        }
        public ApplicationRole(string role) {
            Name = role;
        }
    }
}
