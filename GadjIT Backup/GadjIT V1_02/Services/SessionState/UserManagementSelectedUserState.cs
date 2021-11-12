using GadjIT.AppContext.GadjIT_App;
using GadjIT.AppContext.GadjIT_App.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace GadjIT_V1_02.Services.SessionState
{
    public interface IUserManagementSelectedUserState
    {
        List<AppCompanyDetails> allCompanies { get; set; }
        List<AspNetRoles> allRoles { get; set; }
        List<CompanyItem> companyItems { get; set; }
        Action DataChanged { get; set; }
        bool enablePasswordSet { get; set; }
        string selectedOption { get; set; }
        List<RoleItem> selectedRoles { get; set; }
        AspNetUsers TaskObject { get; set; }

        event Action OnChange;

        void SetallCompanies(List<AppCompanyDetails> allCompanies);
        void SetallRoles(List<AspNetRoles> allRoles);
        void SetcompanyItems(List<CompanyItem> companyItems);
        void SetDataChanged(Action DataChanged);
        void SetenablePasswordSet(bool enablePasswordSet);
        void SetselectedOption(string selectedOption);
        void SetselectedRoles(List<RoleItem> selectedRoles);
        void SetTaskObject(AspNetUsers TaskObject);
    }

    public class UserManagementSelectedUserState : IUserManagementSelectedUserState
    {
        public event Action OnChange;

        public AspNetUsers TaskObject { get; set; }

        public Action DataChanged { get; set; }

        public List<CompanyItem> companyItems { get; set; }

        public List<AppCompanyDetails> allCompanies { get; set; }

        public List<AspNetRoles> allRoles { get; set; }

        public List<RoleItem> selectedRoles { get; set; }

        public string selectedOption { get; set; }

        public bool enablePasswordSet { get; set; } = false;

        public void SetTaskObject(AspNetUsers TaskObject)
        {
            this.TaskObject = TaskObject;
            NotifyStateChanged();
        }
        public void SetDataChanged(Action DataChanged)
        {
            this.DataChanged = DataChanged;
            NotifyStateChanged();
        }
        public void SetcompanyItems(List<CompanyItem> companyItems)
        {
            this.companyItems = companyItems;
            NotifyStateChanged();
        }
        public void SetallCompanies(List<AppCompanyDetails> allCompanies)
        {
            this.allCompanies = allCompanies;
            NotifyStateChanged();
        }
        public void SetallRoles(List<AspNetRoles> allRoles)
        {
            this.allRoles = allRoles;
            NotifyStateChanged();
        }
        public void SetselectedRoles(List<RoleItem> selectedRoles)
        {
            this.selectedRoles = selectedRoles;
            NotifyStateChanged();
        }
        public void SetselectedOption(string selectedOption)
        {
            this.selectedOption = selectedOption;
            NotifyStateChanged();
        }
        public void SetenablePasswordSet(bool enablePasswordSet)
        {
            this.enablePasswordSet = enablePasswordSet;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
