using Blazored.Modal;
using Blazored.Modal.Services;
using GadjIT.GadjitContext.GadjIT_App;
using GadjIT.GadjitContext.GadjIT_App.Custom;
using Gizmo_V1_02.Data.Admin;
using Gizmo_V1_02.Pages.Shared.Modals;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.SystemNav.CompanyManagement
{
    public partial class ManageCompanies
    {
        public class companyInfo
        {
            public VmCompanyDetails Company { get; set; }

            public List<AspNetUsers> Users { get; set; }

            public List<SmartflowRecords> SmartflowsDev { get; set; }

            public List<SmartflowRecords> SmartflowsLive { get; set; }

            public string LastUpdated { get; set; }
        }

        public class userInfo
        {
            public AspNetUsers user { get; set; }

            public IList<Claim> claims { get; set; }
        }

        [Inject]
        IModalService Modal { get; set; }

        [Inject]
        ICompanyDbAccess companyDbAccess { get; set; }

        [Inject]
        IIdentityUserAccess userAccess { get; set; }


        [Inject]
        IUserSessionState sessionState { get; set; }

        private List<VmCompanyDetails> lstCompanyDetails;

        private List<companyInfo> AllCompanies { get; set; }

        private List<AspNetUsers> AllUsers { get; set; }

        private List<userInfo> AllUserinfo { get; set; }

        private List<SmartflowRecords> AllSmartflows { get; set; }

        public AppCompanyDetails editCompany = new AppCompanyDetails();


        private string LastUpdated { get; set; }

        protected override async Task OnInitializedAsync()
        {
            bool gotLock = sessionState.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = sessionState.Lock;
            }

            var lstAppCompanyDetails = await companyDbAccess.GetCompanies();
            lstCompanyDetails = lstAppCompanyDetails
                                            .Select(A => new VmCompanyDetails { Company = A, OnHover = false })
                                            .ToList();


            AllUsers = await userAccess.GetUsers();
            AllUserinfo = AllUsers.Select(U => new userInfo
                                            { 
                                                user = U
                                            })
                                    .ToList();

            foreach (var user in AllUserinfo)
            {
                var claims = await userAccess.GetCompanyClaims(user.user);

                user.claims = claims;
            }
            



            AllSmartflows = await companyDbAccess.GetAllSmartflowRecordsForAllCompanies();



            AllCompanies = lstCompanyDetails
                                    .Select(C => new companyInfo
                                            {
                                                Company = C,
                                                SmartflowsDev = AllSmartflows
                                                                    .Where(S => S.CompanyId == C.Company.Id)
                                                                    .Where(S => S.System == "Dev")
                                                                    .ToList(),
                                                SmartflowsLive = AllSmartflows
                                                                    .Where(S => S.CompanyId == C.Company.Id)
                                                                    .Where(S => S.System == "Live")
                                                                    .ToList(),
                                                Users = AllUserinfo.Where(U => U
                                                                                .claims
                                                                                .Select(C => C.Value)
                                                                                .ToList()
                                                                                .Contains(C.Company.Id.ToString()))
                                                                    .Select(U => U.user)
                                                                    .ToList()
                                                
                                                

                                    })
                                    .ToList();


            foreach (var company in AllCompanies)
            {
                var lastDevDate = DateTime.Now;
                bool isDevDate = true;

                if (!(company.SmartflowsDev is null) && company.SmartflowsDev.Count() > 0)
                {
                    lastDevDate = company.SmartflowsDev.OrderBy(S => S.LastModifiedDate).Select(S => S.LastModifiedDate).FirstOrDefault();
                }
                else
                {
                    isDevDate = false;
                }

                var lastLiveDate = DateTime.Now;
                bool isLiveDate = true;

                if (!(company.SmartflowsLive is null) && company.SmartflowsLive.Count() > 0)
                {
                    lastLiveDate = company.SmartflowsDev.OrderBy(S => S.LastModifiedDate).Select(S => S.LastModifiedDate).FirstOrDefault();
                }
                else
                {
                    isLiveDate = false;
                }

                if(isDevDate && isLiveDate)
                {
                    company.LastUpdated = lastDevDate > lastLiveDate ? lastDevDate.ToString("dd MMM yyyy") : lastLiveDate.ToString("dd MMM yyyy");
                }
                else if(isDevDate && !isLiveDate)
                {
                    company.LastUpdated = lastDevDate.ToString("dd MMM yyyy");
                }
                else if (!isDevDate && isLiveDate)
                {
                    company.LastUpdated = lastLiveDate.ToString("dd MMM yyyy");
                }
                else
                {
                    company.LastUpdated = "Not Used";
                }




            }

        }

        private async void DataChanged()
        {
            var lstAppCompanyDetails = await companyDbAccess.GetCompanies();
            lstCompanyDetails = lstAppCompanyDetails
                                            .Select(A => new VmCompanyDetails { Company = A })
                                            .ToList();

            StateHasChanged();
        }

        protected void PrepareForEdit(AppCompanyDetails seletedRole)
        {
            editCompany = seletedRole;

            ShowEditModal();
        }

        protected void PrepareForDelete(companyInfo seletedRole)
        {
            if (seletedRole.Users is null || seletedRole.Users.Count != 0)
            {
                var errorDets = new List<string>();

                errorDets.Add("Users exist with claims to this company");

                var parameters = new ModalParameters();
                parameters.Add("ErrorDesc", "Cannot delete company for the following reasons:");
                parameters.Add("ErrorDetails", errorDets);

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-chapter-import"
                };

                Modal.Show<ModalErrorInfo>("Delete Company Issue", parameters, options);
            }
            else
            {
                editCompany = seletedRole.Company.Company;

                Action SelectedDeleteAction = HandleValidDelete;
                var parameters = new ModalParameters();
                parameters.Add("InfoHeader", "Delete?");
                parameters.Add("ModalHeight", "300px");
                parameters.Add("ModalWidth", "500px");
                parameters.Add("DeleteAction", SelectedDeleteAction);


                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-chapter-import"
                };

                Modal.Show<ModalDelete>("Delete?", parameters, options);
            }



        }

        protected void PrepareForInsert()
        {
            editCompany = new AppCompanyDetails();

            ShowEditModal();
        }

        protected void ShowEditModal()
        {
            Action Action = DataChanged;

            var parameters = new ModalParameters();
            parameters.Add("TaskObject", editCompany);
            parameters.Add("DataChanged", Action);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-system-company"
            };

            Modal.Show<CompanyEdit>("Edit Company", parameters,options);
        }

        private async void HandleValidDelete()
        {
            await companyDbAccess.DeleteCompany(editCompany);

            DataChanged();
        }
    }
}
