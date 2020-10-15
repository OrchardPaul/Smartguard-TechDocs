using Gizmo.Context.Gizmo_Authentification;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Context.Gizmo_Authentification.Custom;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Gizmo_V1_02.Services.SessionState;
using AutoMapper;

namespace Gizmo_V1_02.Data.Admin
{
    public interface IIdentityUserAccess
    {
        Task<IdentityResult> Delete(AspNetUsers item);
        Task<IList<Claim>> GetCompanyClaims(AspNetUsers user);
        Task<IList<string>> GetSelectedUserRoles(AspNetUsers item);
        Task<IList<Claim>> GetSignedInUserClaims();
        Task<IList<Claim>> GetSignedInUserClaimViaType(string Type);
        Task<AspNetUsers> GetUserByName(string userName);
        Task<List<AspNetUsers>> GetUsers();
        Task<AspNetUsers> SubmitChanges(AspNetUsers item, List<RoleItem> selectedRoles);
        Task<AspNetUsers> SubmitCompanyCliams(List<CompanyItem> companies, AspNetUsers user);
    }

    public class IdentityUserAccess : IIdentityUserAccess
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly IUserSessionState userSessionState;
        private readonly IMapper mapper;

        private ApplicationUser selectedUser { get; set; }

        public IdentityUserAccess(UserManager<ApplicationUser> userManager
            , AuthenticationStateProvider authenticationStateProvider
            , IUserSessionState userSessionState
            , IMapper mapper)
        {
            this.userManager = userManager;
            this.authenticationStateProvider = authenticationStateProvider;
            this.userSessionState = userSessionState;
            this.mapper = mapper;
        }

        public async Task<AspNetUsers> SubmitCompanyCliams(List<CompanyItem> companies, AspNetUsers user)
        {
            selectedUser = await userManager.FindByIdAsync(user.Id);

            var removeClaims = companies.Where(C => !C.IsSubscribed)
                .Select(C => new Claim("Company", C.Id.ToString()))
                .ToList();

            await userManager.RemoveClaimsAsync(selectedUser, removeClaims);

            var claims = await userManager.GetClaimsAsync(selectedUser);

            var claimsId = claims.Where(C => C.Type == "Company")
                                .Select(C => C.Value)
                                .ToList();

            var addClaims = companies.Where(C => C.IsSubscribed)
                .Where(C => !claimsId.Contains(C.Id.ToString()))
                .Select(C => new Claim("Company", C.Id.ToString()))
                .ToList();

            await userManager.AddClaimsAsync(selectedUser, addClaims);

            return user;
        }

        public async Task<IList<Claim>> GetSignedInUserClaims()
        {
            var auth = await authenticationStateProvider.GetAuthenticationStateAsync();

            if (!(auth is null))
            {
                var user = auth.User;
                var userName = user.Identity.Name;

                if (!(userName is null))
                {
                    var currentUser = await userManager.Users
                                            .Where(U => U.UserName == userName)
                                            .SingleAsync();

                    if (!(currentUser is null))
                    {
                        var claims = await userManager.GetClaimsAsync(currentUser);

                        return claims.ToList();
                    }
                }
            }

            return null;
        }


        public async Task<IList<Claim>> GetSignedInUserClaimViaType(string Type)
        {
            var auth = await authenticationStateProvider.GetAuthenticationStateAsync();

            if (!(auth is null))
            {
                var user = auth.User;
                var userName = user.Identity.Name;

                if (!(userName is null))
                {
                    var currentUser = await userManager.Users
                                            .Where(U => U.UserName == userName)
                                            .SingleAsync();

                    if (!(currentUser is null))
                    {
                        var claims = await userManager.GetClaimsAsync(currentUser);

                        return claims.Where(C => C.Type == Type)
                                                .ToList();
                    }
                }
            }

            return null;
        }

        public async Task<IList<Claim>> GetCompanyClaims(AspNetUsers user)
        {
            selectedUser = new ApplicationUser();

            mapper.Map(user, selectedUser);

            var claims = await userManager.GetClaimsAsync(selectedUser);
            var claimsReturn = claims.Where(C => C.Type == "Company").ToList();

            return claimsReturn;
        }


        public async Task<IList<string>> GetSelectedUserRoles(AspNetUsers item)
        {
            selectedUser = new ApplicationUser();

            mapper.Map(item, selectedUser);

            return await userManager.GetRolesAsync(selectedUser);
        }

        public async Task<List<AspNetUsers>> GetUsers()
        {
            return await userManager.Users
                .Select(U => mapper.Map(U,new AspNetUsers()))
                .ToListAsync();
        }

        public async Task<AspNetUsers> GetUserByName(string userName)
        {
            return await userManager.Users
                .Where(U => U.UserName == userName)
                .Select(U => mapper.Map(U, new AspNetUsers()))
                .SingleAsync();
        }

        public async Task<AspNetUsers> SubmitChanges(AspNetUsers item, List<RoleItem> selectedRoles)
        {
            selectedUser = await userManager.FindByIdAsync(item.Id);

            if (selectedUser is null)
            {
                ApplicationUser NewUser = new ApplicationUser
                {
                    UserName = item.UserName,
                    NormalizedUserName = item.UserName.ToUpper(),
                    FullName = item.FullName,
                    Email = item.Email,
                    NormalizedEmail = item.Email.ToUpper(),
                    PhoneNumber = item.PhoneNumber,
                    EmailConfirmed = true,
                    SelectedUri = item.SelectedUri
                };

                var CreateResult = await userManager.CreateAsync(NewUser, item.PasswordHash);

                if (!CreateResult.Succeeded)
                {
                    return null;
                }
                else
                {
                    var strSelectedRoles = selectedRoles.Where(S => S.IsSubscribed == true).Select(S => S.RoleName).ToList();

                    await userManager.AddToRolesAsync(NewUser, strSelectedRoles);

                    mapper.Map(NewUser, item);

                    return item;
                }
            }
            else
            {
                selectedUser.UserName = item.UserName;
                selectedUser.NormalizedUserName = item.NormalizedUserName;
                selectedUser.FullName = item.FullName;
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
                selectedUser.SelectedUri = item.SelectedUri;

                await userManager.UpdateAsync(selectedUser);

                if (!(item.PasswordHash == "PasswordNotChanged115592!"))
                {
                    var resetToken = await userManager.GeneratePasswordResetTokenAsync(selectedUser);

                    await userManager.ResetPasswordAsync(
                            selectedUser,
                            resetToken,
                            item.PasswordHash);
                }

                foreach (RoleItem checkRole in selectedRoles)
                {
                    var queryResult = await userManager.IsInRoleAsync(selectedUser, checkRole.RoleName);

                    if (queryResult)
                    {
                        if (!checkRole.IsSubscribed)
                        {
                            await userManager.RemoveFromRoleAsync(selectedUser, checkRole.RoleName);
                        }
                    }
                    else
                    {
                        if (checkRole.IsSubscribed)
                        {
                            await userManager.AddToRoleAsync(selectedUser, checkRole.RoleName);
                        }
                    }
                }

                mapper.Map(selectedUser, item);

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
