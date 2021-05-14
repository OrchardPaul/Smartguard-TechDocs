using GadjIT.GadjitContext.GadjIT_App;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using GadjIT.GadjitContext.GadjIT_App.Custom;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Gizmo_V1_02.Services.SessionState;
using AutoMapper;

namespace Gizmo_V1_02.Data.Admin
{
    public interface IIdentityUserAccess
    {
        Task<AspNetUsers> SwitchSelectedCompany(AspNetUsers user);
        Task<IdentityResult> Delete(AspNetUsers item);
        Task<IList<Claim>> GetCompanyClaims(AspNetUsers user);
        Task<IList<string>> GetSelectedUserRoles(AspNetUsers item);
        Task<IList<Claim>> GetSignedInUserClaims();
        Task<IList<Claim>> GetSignedInUserClaimViaType(string Type);
        Task<AspNetUsers> GetUserByName(string userName);
        Task<List<AspNetUsers>> GetUsers();
        Task<List<UserDataCollectionItem>> GetUsersWithCompanyInfo();
        Task<AspNetUsers> SubmitChanges(AspNetUsers item, List<RoleItem> selectedRoles);
        Task<AspNetUsers> SubmitCompanyCliams(List<CompanyItem> companies, AspNetUsers user);
        Task<AspNetUsers> UpdateUserDetails(AspNetUsers user);

        bool Lock { get; set; }
    }

    public class IdentityUserAccess : IIdentityUserAccess
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        private ApplicationUser selectedUser { get; set; }


        public bool Lock { get; set; } = false;

        public IdentityUserAccess(UserManager<ApplicationUser> userManager
            , RoleManager<ApplicationRole> roleManager
            , AuthenticationStateProvider authenticationStateProvider
            , ApplicationDbContext context
            , IMapper mapper)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.authenticationStateProvider = authenticationStateProvider;
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<AspNetUsers> SwitchSelectedCompany(AspNetUsers user)
        {
            var signedInUserState = await authenticationStateProvider.GetAuthenticationStateAsync();

            selectedUser = await userManager.FindByNameAsync(user.UserName);
            selectedUser.SelectedCompanyId = user.SelectedCompanyId;
            selectedUser.SelectedUri = user.SelectedUri;

            if (!signedInUserState.User.IsInRole("Super User"))
            {
                var selectedCompanyUserRoles = await context.AppCompanyUserRoles
                                                    .Where(A => A.UserId == selectedUser.Id
                                                                & A.CompanyId == selectedUser.SelectedCompanyId)
                                                    .Select(A => A.RoleId)
                                                    .ToListAsync();

                var newRoles = await roleManager.Roles
                                            .Where(R => selectedCompanyUserRoles.Contains(R.Id))
                                            .Select(R => R.Name)
                                            .ToListAsync();



                var currentRoles = await userManager.GetRolesAsync(selectedUser);

                if (currentRoles.Count > 0)
                {
                    await userManager.RemoveFromRolesAsync(selectedUser, currentRoles);
                }

                if (newRoles.Count > 0)
                {
                    await userManager.AddToRolesAsync(selectedUser, newRoles);
                }

            }

 


            await userManager.UpdateAsync(selectedUser);



            return mapper.Map(selectedUser, new AspNetUsers());
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
            //Get signed in user in proper format
            var signedInUserState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var signedInUserAsp = await userManager.FindByNameAsync(signedInUserState.User.Identity.Name);
            var signedInUser = mapper.Map(signedInUserAsp, new AspNetUsers());

            var returnedUsers = new List<AspNetUsers>();

            if (signedInUserState.User.IsInRole("Super User"))
            {
                return await userManager.Users
                            .Select(U => mapper.Map(U, new AspNetUsers()))
                            .ToListAsync(); 
            }
            else
            {
                var signedInUsersCompany = await GetCompanyClaims(signedInUser);

                foreach(var company in signedInUsersCompany)
                {
                    /*
                     * 1. Grab all users in company
                     * 2. format returned applicationusers into usefull aspnetusers
                     * 3. check if users exist in return list
                     * 4. add any applicable return users to list
                     */

                    var usersInCompanyUnformatted = await userManager.GetUsersForClaimAsync(company);
                    var usersInCompanyFormatted = usersInCompanyUnformatted.Select(U => mapper.Map(U, new AspNetUsers())).ToList();
                    var usersInCompanySorted = usersInCompanyFormatted.Where(U => !returnedUsers.Contains(U)).ToList();

                    returnedUsers.AddRange(usersInCompanySorted);
                }

                return returnedUsers;
            }
        }

        public async Task<List<UserDataCollectionItem>> GetUsersWithCompanyInfo()
        {
            /*
             * 
             * 1. Get Signed in user
             * 2. Get all users in signed in user company
             * 3. Gather User,Role and company data in single class
             * 
             */

            //Can't get from session state (Circular dependany error)
            var signedInUserState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var signedInUserAsp = await userManager.FindByNameAsync(signedInUserState.User.Identity.Name);
            var signedInUser = mapper.Map(signedInUserAsp, new AspNetUsers());

            var UserInFormat = await userManager.FindByNameAsync(signedInUser.UserName); 

            var allUsers = await userManager.Users
                                        .Select(U => mapper.Map(U, new AspNetUsers()))
                                        .ToListAsync();

            var allCompanies = await context
                                        .AppCompanyDetails
                                        .ToListAsync();

            var signedInUserSelectedCompany = await context
                                            .AppCompanyDetails
                                            .Where(C => C.Id == signedInUser.SelectedCompanyId)
                                            .SingleOrDefaultAsync();

            var companyUserRoles = await context
                                            .AppCompanyUserRoles
                                            .Where(C => C.CompanyId == signedInUserSelectedCompany.Id)
                                            .ToListAsync();

            var companyUserIds = companyUserRoles.Select(C => C.UserId).ToList();

            var companyUsers = allUsers
                                    .Where(U => companyUserIds.Contains(U.Id))
                                    .ToList();

            if (signedInUserSelectedCompany.CompanyName == "Orchard Rock")
            {
                return companyUsers
                            .Select(U => new UserDataCollectionItem
                            {
                                User = U,

                                UserCompanies = allCompanies
                                                        .Where(C => companyUserRoles
                                                                            .Where(UR => UR.UserId == U.Id)
                                                                            .Select(UR => UR.CompanyId)
                                                                            .ToList()
                                                                            .Contains(C.Id))
                                                        .ToList(),

                                UserRoles = companyUserRoles
                                                        .Where(UR => UR.UserId == U.Id)
                                                        .ToList()
                            })
                            .ToList();
            }
            else 
            {
                var superUserRole = await roleManager.FindByNameAsync("Super User");

                return companyUsers
                            .Select(U => new UserDataCollectionItem
                            {
                                User = U,

                                UserCompanies = allCompanies
                                                        .Where(C => companyUserRoles
                                                                            .Where(UR => UR.UserId == U.Id)
                                                                            .Select(UR => UR.CompanyId)
                                                                            .ToList()
                                                                            .Contains(C.Id))
                                                        .ToList(),

                                UserRoles = companyUserRoles
                                                        .Where(UR => UR.UserId == U.Id)
                                                        .ToList()
                            })
                            .Where(CU => !CU.UserRoles
                                                .Select(UR => UR.RoleId)
                                                .ToList()
                                                .Contains(superUserRole.Id))
                            .ToList();
            }


        }

        public async Task<AspNetUsers> GetUserByName(string userName)
        {
            return await userManager.Users
                .Where(U => U.UserName == userName)
                .Select(U => mapper.Map(U, new AspNetUsers()))
                .SingleAsync();
        }

        public async Task<AspNetUsers> UpdateUserDetails(AspNetUsers user)
        {
            Lock = true;

            try
            {
                selectedUser = await userManager.FindByIdAsync(user.Id);

                selectedUser.UserName = user.UserName;
                selectedUser.NormalizedUserName = user.NormalizedUserName;
                selectedUser.FullName = user.FullName;
                selectedUser.Email = user.UserName;
                selectedUser.NormalizedEmail = user.NormalizedEmail;
                selectedUser.EmailConfirmed = user.EmailConfirmed;
                selectedUser.SecurityStamp = user.SecurityStamp;
                selectedUser.ConcurrencyStamp = user.ConcurrencyStamp;
                selectedUser.PhoneNumber = user.PhoneNumber;
                selectedUser.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                selectedUser.TwoFactorEnabled = user.TwoFactorEnabled;
                selectedUser.LockoutEnd = user.LockoutEnd;
                selectedUser.LockoutEnabled = user.LockoutEnabled;
                selectedUser.AccessFailedCount = user.AccessFailedCount;
                selectedUser.SelectedUri = user.SelectedUri;
                selectedUser.SelectedCompanyId = user.SelectedCompanyId;
                selectedUser.MainBackgroundImage = user.MainBackgroundImage;
                selectedUser.DisplaySmartflowPreviewImage = user.DisplaySmartflowPreviewImage;

                await userManager.UpdateAsync(selectedUser);

                mapper.Map(selectedUser, user);
            }
            finally
            {
                Lock = false;
            }

            return user;

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
                    Email = item.UserName,
                    NormalizedEmail = item.UserName.ToUpper(),
                    PhoneNumber = item.PhoneNumber,
                    EmailConfirmed = true,
                    SelectedUri = item.SelectedUri,
                    SelectedCompanyId = item.SelectedCompanyId,
                    MainBackgroundImage = item.MainBackgroundImage,
                    DisplaySmartflowPreviewImage = item.DisplaySmartflowPreviewImage
                };

                var CreateResult = await userManager.CreateAsync(NewUser, item.PasswordHash);

                if (!CreateResult.Succeeded)
                {
                    return null;
                }
                else
                {
                    var strSelectedRoles = selectedRoles
                                                    .Where(S => S.IsSubscribed)
                                                    .Select(S => S.RoleName)
                                                    .ToList();

                    await userManager.AddToRolesAsync(NewUser, strSelectedRoles);

                    var selectedCompanyUserRoles = selectedRoles
                                                        .Where(S => S.IsSubscribed)
                                                        .Select(S => new AppCompanyUserRoles
                                                        {
                                                            UserId = NewUser.Id,
                                                            CompanyId = NewUser.SelectedCompanyId,
                                                            RoleId = S.RoleId
                                                        })
                                                        .ToList();

                    context.AppCompanyUserRoles.AddRange(selectedCompanyUserRoles);

                    mapper.Map(NewUser, item);

                    return item;
                }
            }
            else
            {
                selectedUser.UserName = item.UserName;
                selectedUser.NormalizedUserName = item.NormalizedUserName;
                selectedUser.FullName = item.FullName;
                selectedUser.Email = item.UserName;
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
                selectedUser.SelectedCompanyId = item.SelectedCompanyId;
                selectedUser.MainBackgroundImage = item.MainBackgroundImage;
                selectedUser.DisplaySmartflowPreviewImage = item.DisplaySmartflowPreviewImage;
                

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

                var addSelectedCompanyUserRoles = selectedRoles
                                                        .Where(S => S.IsSubscribed)
                                                        .Select(S => new AppCompanyUserRoles
                                                        {
                                                            UserId = selectedUser.Id,
                                                            CompanyId = selectedUser.SelectedCompanyId,
                                                            RoleId = S.RoleId
                                                        })
                                                        .ToList();

                foreach(var company in addSelectedCompanyUserRoles) 
                {
                    var selectedCompany = await context.AppCompanyUserRoles
                                                    .Where(A => A.RoleId == company.RoleId
                                                                & A.CompanyId == company.CompanyId
                                                                & A.UserId == company.UserId)
                                                    .SingleOrDefaultAsync();

                    if(selectedCompany is null)
                    {
                        context.AppCompanyUserRoles.Add(company);
                    }
                }

                var removeSelectedCompanyUserRoles = selectedRoles
                                                            .Where(S => !S.IsSubscribed)
                                                            .Select(S => new AppCompanyUserRoles
                                                            {
                                                                UserId = selectedUser.Id,
                                                                CompanyId = selectedUser.SelectedCompanyId,
                                                                RoleId = S.RoleId
                                                            })
                                                            .ToList();

                foreach (var company in removeSelectedCompanyUserRoles)
                {
                    var selectedCompany = await context.AppCompanyUserRoles
                                                    .Where(A => A.RoleId == company.RoleId
                                                                & A.CompanyId == company.CompanyId
                                                                & A.UserId == company.UserId)
                                                    .SingleOrDefaultAsync();

                    if (!(selectedCompany is null))
                    {
                        context.AppCompanyUserRoles.Remove(selectedCompany);
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
