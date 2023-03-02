
using GadjIT_App.Services.SessionState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GadjIT_ClientContext.Models.Smartflow;
using GadjIT_AppContext.GadjIT_App;

namespace GadjIT_App.Services.AppState
{
    
    public interface IAppSmartflowsState 
    {
        List<IAppStateSmartflow> LstAppStateSmartflows { get; }


        // bool IsSmartflowLocked(int _companyId, string _system, string _caseTypeGroup, string _caseType, string _smartflowName);

        // bool IsSmartflowLockedByOtherUser(AspNetUser _user, int _companyId, string _system, string _caseTypeGroup, string _caseType, string _smartflowName);

        // IAppStateSmartflow GetSmartflowOpenedBy(AspNetUser _user, int _companyId, string _system);

        // AspNetUser SmartflowIsLockedBy(int _companyId, string _system, string _caseTypeGroup, string _caseType, string _smartflowName);

        // void LockSmartflow(AspNetUser _user, int _companyId, string _system, string _caseTypeGroup, string _caseType, string _smartflowName, bool _lockForEdit);

        // void UnlockSmartflow(int _companyId, string _system, string _caseTypeGroup, string _caseType, string _smartflowName);
        
        // void UnlockSmartflowForUser(AspNetUser _user, int _companyId, string _system, string _caseTypeGroup, string _caseType, string _smartflowName);

        // void DisposeUser(AspNetUser _user);

        bool IsSmartflowLocked(int _smartflowId);

        /// <summary>
        /// Checks if User has the Smartflow Locked for edit
        /// </summary>
        /// <param name="_user"></param>
        /// <param name="_smartflowId"></param>
        /// <returns></returns>
        bool IsSmartflowLockedByUser(AspNetUser _user, int _smartflowId);

        /// <summary>
        /// Checks if a User other than the one passed (_user) has the Smartflow Locked for edit
        /// </summary>
        /// <param name="_user"></param>
        /// <param name="_smartflowId"></param>
        /// <returns></returns>
        bool IsSmartflowLockedByOtherUser(AspNetUser _user, int _smartflowId);

        IAppStateSmartflow GetSmartflowOpenedBy(AspNetUser _user, int _companyId, string _system);

        AspNetUser SmartflowIsLockedBy(int _smartflowId);

        void LockSmartflow(AspNetUser _user, int _smartflowId, bool _lockForEdit);

        void UnlockSmartflow(int _smartflowId);
        
        void UnlockSmartflowForUser(AspNetUser _user, int _smartflowId);

        void DisposeUser(AspNetUser _user);
        



    }
    public interface IAppStateSmartflow
    {
        public int SmartflowId {get; set; }

        int CompanyID { get; set; }

        string SelectedSystem { get; set; }

        string CaseTypeGroup { get; set; }

        string CaseType { get; set; }

        string SmartflowName { get; set; }

        bool LockedForEdit { get; set; }

        public AspNetUser CurrentUser {get; set;}

        public DateTime TimeLocked { get; set; }



    }


    public class AppSmartflowsStateList : IAppSmartflowsState
    {
        
        public List<IAppStateSmartflow> LstAppStateSmartflows { get; set; } = new List<IAppStateSmartflow>();


        private class AppStateSmartflow : IAppStateSmartflow
        {
            public int SmartflowId {get; set; }

            public int CompanyID { get; set; }

            public string SelectedSystem { get; set; }

            public string CaseTypeGroup { get; set; }

            public string CaseType { get; set; }

            public string SmartflowName { get; set; }

            public bool LockedForEdit { get; set; }

            public AspNetUser CurrentUser {get; set;}

            public DateTime TimeLocked { get; set; }

        }

        public bool IsSmartflowLocked(int _smartflowId)
        {

            int numLocked = LstAppStateSmartflows
                                        .Where(CI => CI.SmartflowId == _smartflowId)
                                        .Where(CI => CI.LockedForEdit == true)
                                        .Count();

            return numLocked == 0 ? false : true;
        }


        public bool IsSmartflowLockedByUser(AspNetUser _user, int _smartflowId)
        {

            int numLocked = LstAppStateSmartflows
                                        .Where(CI => CI.SmartflowId == _smartflowId)
                                        .Where(CI => CI.CurrentUser.Id == _user.Id)
                                        .Where(CI => CI.LockedForEdit == true)
                                        .Count();

            return numLocked == 0 ? false : true;
        }

        public bool IsSmartflowLockedByOtherUser(AspNetUser _user, int _smartflowId)
        {

            int numLocked = LstAppStateSmartflows
                                        .Where(CI => CI.SmartflowId == _smartflowId)
                                        .Where(CI => CI.CurrentUser.Id != _user.Id)
                                        .Where(CI => CI.LockedForEdit == true)
                                        .Count();

            return numLocked == 0 ? false : true;
        }

        public AspNetUser SmartflowIsLockedBy(int _smartflowId)
        {

            var appSmartflow = LstAppStateSmartflows
                                        .Where(CI => CI.SmartflowId == _smartflowId)
                                        .Where(CI => CI.LockedForEdit == true)
                                        .FirstOrDefault();
            
            if(appSmartflow == null || appSmartflow.CurrentUser == null)
            {
                return null;
            }

            return appSmartflow.CurrentUser;
        }

        public IAppStateSmartflow GetSmartflowOpenedBy(AspNetUser _user, int _companyId, string _system)
        {

            var appSmartflow = LstAppStateSmartflows
                                        .Where(CI => CI.CompanyID == _companyId
                                                && CI.SelectedSystem == _system
                                                && CI.CurrentUser.Id == _user.Id)
                                        .FirstOrDefault();
            
            if(appSmartflow == null || appSmartflow.CurrentUser == null)
            {
                return null;
            }

            return appSmartflow;
        }

        public void LockSmartflow(AspNetUser _user, int _smartflowId, bool _lockForEdit)
        {
            int numFound = LstAppStateSmartflows
                                        .Where(CI => CI.SmartflowId == _smartflowId
                                                && CI.CurrentUser.Id == _user.Id)
                                        .Count();
            if(numFound == 0)
            {
                LstAppStateSmartflows.Add(
                                            new AppStateSmartflow{
                                                SmartflowId = _smartflowId
                                                ,LockedForEdit = _lockForEdit
                                                ,TimeLocked = DateTime.Now
                                                ,CurrentUser = new AspNetUser{
                                                                            Id = _user.Id
                                                                            , FullName = _user.FullName
                                                                            , UserName = _user.UserName
                                                                            , Email = _user.Email
                                                                            }
                                            }
                                        );
                                        
            }
        }

        public void UnlockSmartflow(int _smartflowId)
        {

            LstAppStateSmartflows.RemoveAll(
                                        CI => CI.SmartflowId == _smartflowId);
                                        
        }

        public void UnlockSmartflowForUser(AspNetUser _user,int _smartflowId)
        {

            LstAppStateSmartflows.RemoveAll(
                                        CI => CI.CurrentUser.Id == _user.Id
                                        && CI.SmartflowId == _smartflowId);
                                        
        }

        public void DisposeUser(AspNetUser _user)
        {
            LstAppStateSmartflows.RemoveAll(A => A.CurrentUser.Id == _user.Id);                                                    

        }

        // public bool IsSmartflowLocked(int _companyId, string _system, string _caseTypeGroup, string _caseType, string _smartflowName)
        // {

        //     int numLocked = LstAppStateSmartflows
        //                                 .Where(CI => CI.CompanyID == _companyId)
        //                                 .Where(CI => CI.SelectedSystem == _system)
        //                                 .Where(CI => CI.CaseTypeGroup == _caseTypeGroup)
        //                                 .Where(CI => CI.CaseType == _caseType)
        //                                 .Where(CI => CI.SmartflowName == _smartflowName)
        //                                 .Where(CI => CI.LockedForEdit == true)
        //                                 .Count();

        //     return numLocked == 0 ? false : true;
        // }

        // public bool IsSmartflowLockedByOtherUser(AspNetUser _user, int _companyId, string _system, string _caseTypeGroup, string _caseType, string _smartflowName)
        // {

        //     int numLocked = LstAppStateSmartflows
        //                                 .Where(CI => CI.CompanyID == _companyId)
        //                                 .Where(CI => CI.SelectedSystem == _system)
        //                                 .Where(CI => CI.CaseTypeGroup == _caseTypeGroup)
        //                                 .Where(CI => CI.CaseType == _caseType)
        //                                 .Where(CI => CI.SmartflowName == _smartflowName)
        //                                 .Where(CI => CI.CurrentUser.Id != _user.Id)
        //                                 .Where(CI => CI.LockedForEdit == true)
        //                                 .Count();

        //     return numLocked == 0 ? false : true;
        // }

        // public AspNetUser SmartflowIsLockedBy(int _companyId, string _system, string _caseTypeGroup, string _caseType, string _smartflowName)
        // {

        //     var appSmartflow = LstAppStateSmartflows
        //                                 .Where(CI => CI.CompanyID == _companyId)
        //                                 .Where(CI => CI.SelectedSystem == _system)
        //                                 .Where(CI => CI.CaseTypeGroup == _caseTypeGroup)
        //                                 .Where(CI => CI.CaseType == _caseType)
        //                                 .Where(CI => CI.SmartflowName == _smartflowName)
        //                                 .Where(CI => CI.LockedForEdit == true)
        //                                 .FirstOrDefault();
            
        //     if(appSmartflow == null || appSmartflow.CurrentUser == null)
        //     {
        //         return null;
        //     }

        //     return appSmartflow.CurrentUser;
        // }

        // public IAppStateSmartflow GetSmartflowOpenedBy(AspNetUser _user, int _companyId, string _system)
        // {

        //     var appSmartflow = LstAppStateSmartflows
        //                                 .Where(CI => CI.CompanyID == _companyId)
        //                                 .Where(CI => CI.SelectedSystem == _system)
        //                                 .Where(CI => CI.CurrentUser.Id == _user.Id)
        //                                 .FirstOrDefault();
            
        //     if(appSmartflow == null || appSmartflow.CurrentUser == null)
        //     {
        //         return null;
        //     }

        //     return appSmartflow;
        // }

        // public void LockSmartflow(AspNetUser _user, int _companyId, string _system, string _caseTypeGroup, string _caseType, string _smartflowName, bool _lockForEdit)
        // {
        //     int numFound = LstAppStateSmartflows
        //                                 .Where(CI => CI.CompanyID == _companyId)
        //                                 .Where(CI => CI.SelectedSystem == _system)
        //                                 .Where(CI => CI.CaseTypeGroup == _caseTypeGroup)
        //                                 .Where(CI => CI.CaseType == _caseType)
        //                                 .Where(CI => CI.SmartflowName == _smartflowName)
        //                                 .Where(CI => CI.CurrentUser.Id == _user.Id)
        //                                 .Count();
        //     if(numFound == 0)
        //     {
        //         LstAppStateSmartflows.Add(
        //                                     new AppStateSmartflow{
        //                                         CompanyID = _companyId
        //                                         ,SelectedSystem = _system
        //                                         ,CaseTypeGroup = _caseTypeGroup
        //                                         ,CaseType = _caseType
        //                                         ,SmartflowName = _smartflowName
        //                                         ,LockedForEdit = _lockForEdit
        //                                         ,TimeLocked = DateTime.Now
        //                                         ,CurrentUser = new AspNetUser{
        //                                                                     Id = _user.Id
        //                                                                     , FullName = _user.FullName
        //                                                                     , UserName = _user.UserName
        //                                                                     , Email = _user.Email
        //                                                                     }
        //                                     }
        //                                 );
                                        
        //     }
        // }

        // public void UnlockSmartflow(int _companyId, string _system, string _caseTypeGroup, string _caseType, string _smartflowName)
        // {

        //     LstAppStateSmartflows.RemoveAll(
        //                                 CI => CI.CompanyID == _companyId
        //                                 && CI.SelectedSystem == _system
        //                                 && CI.CaseTypeGroup == _caseTypeGroup
        //                                 && CI.CaseType == _caseType
        //                                 && CI.SmartflowName == _smartflowName);
                                        
        // }

        // public void UnlockSmartflowForUser(AspNetUser _user,int _companyId, string _system, string _caseTypeGroup, string _caseType, string _smartflowName)
        // {

        //     LstAppStateSmartflows.RemoveAll(
        //                                 CI => CI.CompanyID == _companyId
        //                                 && CI.SelectedSystem == _system
        //                                 && CI.CaseTypeGroup == _caseTypeGroup
        //                                 && CI.CaseType == _caseType
        //                                 && CI.CurrentUser.Id == _user.Id
        //                                 && CI.SmartflowName == _smartflowName);
                                        
        // }

        // public void DisposeUser(AspNetUser _user)
        // {
        //     LstAppStateSmartflows.RemoveAll(A => A.CurrentUser.Id == _user.Id);                                                    

        // }

        


    }



}
