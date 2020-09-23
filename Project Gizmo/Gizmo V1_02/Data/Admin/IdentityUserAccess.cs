using Gizmo.Context.Gizmo_Authentification;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Data.Admin
{
    public interface IIdentityUserAccess
    {
        Task<IdentityResult> Delete(AspNetUsers item);
        Task<List<AspNetUsers>> GetUsers();
        Task<IList<string>> GetSelectedUserRoles(AspNetUsers selectedUser);
        Task<AspNetUsers> SubmitChanges(AspNetUsers item, List<AspNetRoles> allRoles, IList<String> selectedRoles);
    }

    public class IdentityUserAccess : IIdentityUserAccess
    {
        private readonly UserManager<IdentityUser> userManager;
        private IdentityUser selectedUser { get; set; }

        public IdentityUserAccess(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IList<string>> GetSelectedUserRoles(AspNetUsers item)
        {
            selectedUser = new IdentityUser
            {
                Id = item.Id,
                UserName = item.UserName,
                NormalizedUserName = item.NormalizedUserName,
                Email = item.Email,
                NormalizedEmail = item.NormalizedEmail,
                EmailConfirmed = item.EmailConfirmed,
                PasswordHash = item.PasswordHash,
                SecurityStamp = item.SecurityStamp,
                ConcurrencyStamp = item.ConcurrencyStamp,
                PhoneNumber = item.PhoneNumber,
                PhoneNumberConfirmed = item.PhoneNumberConfirmed,
                TwoFactorEnabled = item.TwoFactorEnabled,
                LockoutEnd = item.LockoutEnd,
                LockoutEnabled = item.LockoutEnabled,
                AccessFailedCount = item.AccessFailedCount
            };

            return await userManager.GetRolesAsync(selectedUser);
        }

        public async Task<List<AspNetUsers>> GetUsers()
        {
            return await userManager.Users
                .Select(U => new AspNetUsers 
                        {     
                            Id = U.Id,
                            UserName = U.UserName,
                            NormalizedUserName = U.NormalizedUserName,
                            Email = U.Email,
                            NormalizedEmail = U.NormalizedEmail,
                            EmailConfirmed = U.EmailConfirmed,
                            PasswordHash = U.PasswordHash,
                            SecurityStamp = U.SecurityStamp,
                            ConcurrencyStamp = U.ConcurrencyStamp,
                            PhoneNumber = U.PhoneNumber,
                            PhoneNumberConfirmed = U.PhoneNumberConfirmed,
                            TwoFactorEnabled = U.TwoFactorEnabled,
                            LockoutEnd = U.LockoutEnd,
                            LockoutEnabled = U.LockoutEnabled,
                            AccessFailedCount = U.AccessFailedCount
                        })
                .ToListAsync();
        }

        public async Task<AspNetUsers> SubmitChanges(AspNetUsers item, List<AspNetRoles> allRoles, IList<String> selectedRoles)
        {
            selectedUser = await userManager.FindByIdAsync(item.Id);

            if (selectedUser is null)
            {
                IdentityUser NewUser = new IdentityUser
                {
                    UserName = item.UserName,
                    NormalizedUserName = item.NormalizedUserName,
                    Email = item.Email,
                    NormalizedEmail = item.NormalizedEmail,
                    EmailConfirmed = item.EmailConfirmed,
                    PasswordHash = item.PasswordHash,
                    SecurityStamp = item.SecurityStamp,
                    ConcurrencyStamp = item.ConcurrencyStamp,
                    PhoneNumber = item.PhoneNumber,
                    PhoneNumberConfirmed = item.PhoneNumberConfirmed,
                    TwoFactorEnabled = item.TwoFactorEnabled,
                    LockoutEnd = item.LockoutEnd,
                    LockoutEnabled = item.LockoutEnabled,
                    AccessFailedCount = item.AccessFailedCount
                };

                var CreateResult = await userManager.CreateAsync(NewUser);

                if (!CreateResult.Succeeded)
                {
                    return null;
                }
                else
                {
                    await userManager.AddToRolesAsync(NewUser, selectedRoles);

                    return item;
                }
            }
            else
            {
                selectedUser.UserName = item.UserName;
                selectedUser.NormalizedUserName = item.NormalizedUserName;
                selectedUser.Email = item.Email;
                selectedUser.NormalizedEmail = item.NormalizedEmail;
                selectedUser.EmailConfirmed = item.EmailConfirmed;
                selectedUser.SecurityStamp = item.SecurityStamp;
                selectedUser.ConcurrencyStamp = item.ConcurrencyStamp;
                selectedUser.PhoneNumber = item.PhoneNumber;
                selectedUser.PhoneNumberConfirmed = item.PhoneNumberConfirmed;
                selectedUser.TwoFactorEnabled = item.TwoFactorEnabled;
                selectedUser.LockoutEnd = item.LockoutEnd;
                selectedUser.LockoutEnabled = item.LockoutEnabled;
                selectedUser.AccessFailedCount = item.AccessFailedCount;

                await userManager.UpdateAsync(selectedUser);

                var resetToken = await userManager.GeneratePasswordResetTokenAsync(selectedUser);
                
                await userManager.ResetPasswordAsync(
                        selectedUser,
                        resetToken,
                        item.PasswordHash);

                foreach (AspNetRoles checkRole in allRoles)
                {
                    var queryResult = await userManager.IsInRoleAsync(selectedUser, checkRole.Name);

                    if (queryResult)
                    {
                        if (!selectedRoles.Contains(checkRole.Name))
                        {
                            await userManager.RemoveFromRoleAsync(selectedUser, checkRole.Name);
                        }
                    }
                    else
                    {
                        if (selectedRoles.Contains(checkRole.Name))
                        {
                            await userManager.AddToRoleAsync(selectedUser, checkRole.Name);
                        }
                    }
                }


                await userManager.AddToRolesAsync(selectedUser, selectedRoles);

                item = new AspNetUsers
                {
                    Id = selectedUser.Id,
                    UserName = selectedUser.UserName,
                    NormalizedUserName = selectedUser.NormalizedUserName,
                    Email = selectedUser.Email,
                    NormalizedEmail = selectedUser.NormalizedEmail,
                    EmailConfirmed = selectedUser.EmailConfirmed,
                    PasswordHash = selectedUser.PasswordHash,
                    SecurityStamp = selectedUser.SecurityStamp,
                    ConcurrencyStamp = selectedUser.ConcurrencyStamp,
                    PhoneNumber = selectedUser.PhoneNumber,
                    PhoneNumberConfirmed = selectedUser.PhoneNumberConfirmed,
                    TwoFactorEnabled = selectedUser.TwoFactorEnabled,
                    LockoutEnd = selectedUser.LockoutEnd,
                    LockoutEnabled = selectedUser.LockoutEnabled,
                    AccessFailedCount = selectedUser.AccessFailedCount
                };

                return item;
            }
        }

        public async Task<IdentityResult> Delete(AspNetUsers item)
        {
            selectedUser = await userManager.FindByIdAsync(item.Id);

            if (selectedUser is null)
            {
                return IdentityResult.Failed();
            }
            else
            {
                return await userManager.DeleteAsync(selectedUser);
            }
        }
    }
}
