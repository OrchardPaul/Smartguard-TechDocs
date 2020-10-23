using AutoMapper;
using Gizmo.Context.Gizmo_Authentification;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Data.Admin
{
    public interface IIdentityRoleAccess
    {
        Task<IdentityResult> Delete(AspNetRoles item);
        Task<List<AspNetRoles>> GetUserRoles();
        Task<AspNetRoles> SubmitChanges(AspNetRoles item);

        void Dispose();
    }

    public class IdentityRoleAccess : IIdentityRoleAccess
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;

        private IdentityRole selectedRole { get; set; }

        public IdentityRoleAccess(RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            this.roleManager = roleManager;
            this.mapper = mapper;
        }

        public async Task<List<AspNetRoles>> GetUserRoles()
        {
            return await roleManager.Roles
                .Select(x => new AspNetRoles
                {
                    Id = x.Id,
                    Name = x.Name,
                    NormalizedName = x.NormalizedName,
                    ConcurrencyStamp = x.ConcurrencyStamp
                })
                .ToListAsync();
        }

        public void Dispose()
        {
            roleManager.Dispose();
        }

        public async Task<AspNetRoles> SubmitChanges(AspNetRoles item)
        {
            selectedRole = await roleManager.FindByIdAsync(item.Id);

            if (selectedRole is null)
            {
                IdentityRole newItem = new IdentityRole 
                {
                    Name = item.Name,
                    NormalizedName = item.NormalizedName,
                    ConcurrencyStamp = item.ConcurrencyStamp
                };

                var CreateResult = await roleManager.CreateAsync(newItem);

                if (!CreateResult.Succeeded)
                {
                    return null;
                }
                else
                {
                    item = new AspNetRoles
                    {
                        Id = newItem.Id,
                        Name = newItem.Name,
                        NormalizedName = newItem.NormalizedName,
                        ConcurrencyStamp = newItem.ConcurrencyStamp
                    };

                    return item;
                }
            }
            else
            {
                selectedRole.Name = item.Name;
                selectedRole.NormalizedName = item.NormalizedName;
                selectedRole.ConcurrencyStamp = item.ConcurrencyStamp;

                await roleManager.UpdateAsync(selectedRole);

                item = new AspNetRoles
                {
                    Id = selectedRole.Id,
                    Name = selectedRole.Name,
                    NormalizedName = selectedRole.NormalizedName,
                    ConcurrencyStamp = selectedRole.ConcurrencyStamp
                };

                return item;
            }
        }

        public async Task<IdentityResult> Delete(AspNetRoles item)
        {
            selectedRole = await roleManager.FindByIdAsync(item.Id);

            if (selectedRole is null)
            {
                return IdentityResult.Failed();
            }
            else
            {
                return await roleManager.DeleteAsync(selectedRole);
            }

        }


    }
}
