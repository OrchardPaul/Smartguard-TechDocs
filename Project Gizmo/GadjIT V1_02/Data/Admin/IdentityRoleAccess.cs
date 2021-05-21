using AutoMapper;
using GadjIT.GadjitContext.GadjIT_App;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_V1_02.Data.Admin
{
    public interface IIdentityRoleAccess
    {
        Task<IdentityResult> Delete(AspNetRoles item);
        Task<List<AspNetRoles>> GetUserRoles();
        Task<IList<string>> GetCurrentUserRolesForCompany(AspNetUsers user, int selectedCompany);
        Task<AspNetRoles> SubmitChanges(AspNetRoles item);

        void Dispose();
    }

    public class IdentityRoleAccess : IIdentityRoleAccess
    {
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly AuthorisationDBContext dBContext;

        private ApplicationRole selectedRole { get; set; }

        public IdentityRoleAccess(RoleManager<ApplicationRole> roleManager
                                    , AuthorisationDBContext dBContext)
        {
            this.roleManager = roleManager;
            this.dBContext = dBContext;
        }

        public async Task<List<AspNetRoles>> GetUserRoles()
        {
            return await roleManager.Roles
                .Select(x => new AspNetRoles
                {
                    Id = x.Id,
                    Name = x.Name,
                    NormalizedName = x.NormalizedName,
                    ConcurrencyStamp = x.ConcurrencyStamp,
                    RoleDescription = x.RoleDescription
                })
                .ToListAsync();
        }

        public async Task<IList<string>> GetCurrentUserRolesForCompany(AspNetUsers user, int selectedCompany)
        {
            var SelectedUserRoles = await dBContext.AppCompanyUserRoles
                                            .Where(A => A.UserId == user.Id
                                                        & A.CompanyId == selectedCompany)
                                            .Select(A => A.RoleId)
                                            .ToListAsync();

            return await roleManager.Roles
                                    .Where(x => SelectedUserRoles.Contains(x.Id))
                                    .Select(x => x.Name)
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
                ApplicationRole newItem = new ApplicationRole
                {
                    Name = item.Name,
                    NormalizedName = item.NormalizedName,
                    RoleDescription = item.RoleDescription
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
                        ConcurrencyStamp = newItem.ConcurrencyStamp,
                        RoleDescription = newItem.RoleDescription
                    };

                    return item;
                }
            }
            else
            {
                selectedRole.Name = item.Name;
                selectedRole.NormalizedName = item.NormalizedName;
                selectedRole.ConcurrencyStamp = item.ConcurrencyStamp;
                selectedRole.RoleDescription = item.RoleDescription;

                await roleManager.UpdateAsync(selectedRole);

                item = new AspNetRoles
                {
                    Id = selectedRole.Id,
                    Name = selectedRole.Name,
                    NormalizedName = selectedRole.NormalizedName,
                    ConcurrencyStamp = selectedRole.ConcurrencyStamp,
                    RoleDescription = selectedRole.RoleDescription
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
                var userRoles = await dBContext.AppCompanyUserRoles
                                                .Where(C => C.RoleId == selectedRole.Id)
                                                .ToListAsync();

                if (userRoles.Count > 0)
                {
                    dBContext.AppCompanyUserRoles.RemoveRange(userRoles);
                }

                return await roleManager.DeleteAsync(selectedRole);
            }

        }


    }
}
