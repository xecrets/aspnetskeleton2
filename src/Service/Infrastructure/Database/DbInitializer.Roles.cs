﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Roles;
using WebApp.Core.Helpers;
using WebApp.DataAccess.Entities;
using WebApp.Service.Helpers;

namespace WebApp.Service.Infrastructure.Database
{
    public partial class DbInitializer
    {
        public async Task SeedRolesAsync(CancellationToken cancellationToken)
        {
            var roles = await _context.Roles.ToDictionarySafeAsync(entity => entity.RoleName, AsExistingEntity, _caseInsensitiveComparer, cancellationToken).ConfigureAwait(false);

            foreach (var (roleName, enumMetadata) in EnumMetadata<RoleEnum>.Members)
            {
                var descriptionAttribute = enumMetadata.Attributes.OfType<DescriptionAttribute>().FirstOrDefault();

                if (descriptionAttribute != null)
                    AddOrUpdateRole(roles,
                        id: (int)enumMetadata.Value,
                        roleName,
                        description: descriptionAttribute.Description);
            }

            _context.Roles.AddRange(GetEntitesToAdd(roles.Values));
            foreach (var entity in GetEntitesToRemove(roles.Values))
                _context.Roles.Remove(entity);
        }

        private static void AddOrUpdateRole(Dictionary<string, EntityInfo<Role>> roles, int id, string roleName, string? description)
        {
            if (!roles.TryGetValue(roleName, out var role))
            {
                roles.Add(roleName, role = AsNewEntity(new Role
                {
                    Id = id,
                    RoleName = roleName,
                }));
            }
            else
                role.State = EntityState.Seen;

            role.Entity.Description = description;
        }
    }
}
