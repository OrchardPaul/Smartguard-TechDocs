using GadjIT.ClientContext.P4W.Custom;
using GadjIT_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_V1_02.Services.AppState
{
    
    public interface IAppChapterState
    {
        List<IAppChapterItemState> lstAppChapterStateItems { get; }

        void SetLastUpdated(IUserSessionState sessionState, VmChapter selectedChapter);

        DateTime GetLastUpdatedDate(IUserSessionState sessionState, VmChapter selectedChapter);

        IAppChapterItemState GetChapterItemState(IUserSessionState sessionState, VmChapter selectedChapter);

        void SetUsersCurrentChapter(IUserSessionState sessionState, VmChapter selectedChapter);

    }
    public interface IAppChapterItemState
    {
        int CompanyID { get; set; }

        string SelectedSystem { get; set; }

        string CaseTypeGroup { get; set; }

        string CaseType { get; set; }

        string ChapterName { get; set; }

        DateTime LastUpdated { get; set; }

        string UpdatedBy { get; set; }

        List<ActiveUsers> ActiveUsers { get; set; }

    }

    public class ActiveUsers
    {
        public string UserName { get; set; }

        public DateTime LastActive { get; set; }

    }

    public class AppChapterStateList : IAppChapterState
    {
        
        public List<IAppChapterItemState> lstAppChapterStateItems { get; set; } = new List<IAppChapterItemState>();


        private class AppChapterItemState : IAppChapterItemState
        {
            public int CompanyID { get; set; }

            public string SelectedSystem { get; set; }

            public string CaseTypeGroup { get; set; }

            public string CaseType { get; set; }

            public string ChapterName { get; set; }

            public DateTime LastUpdated { get; set; }

            public string UpdatedBy { get; set; }

            public List<ActiveUsers> ActiveUsers { get; set; }



        }

        public DateTime GetLastUpdatedDate(IUserSessionState sessionState,VmChapter selectedChapter)
        {
            IAppChapterItemState appChapterItemState;

            appChapterItemState = GetChapterItemState(sessionState, selectedChapter);

            return appChapterItemState.LastUpdated;

        }

        public void SetLastUpdated(IUserSessionState sessionState, VmChapter selectedChapter)
        {
            IAppChapterItemState appChapterItemState;
            DateTime currentDate = DateTime.Now;

            appChapterItemState = GetChapterItemState(sessionState, selectedChapter);


            appChapterItemState.LastUpdated = currentDate;
            sessionState.ChapterLastCompared = currentDate;


        }

        public IAppChapterItemState GetChapterItemState(IUserSessionState sessionState, VmChapter selectedChapter)
        {

            IAppChapterItemState appChapterItemState;

            appChapterItemState = lstAppChapterStateItems
                                        .Where(CI => CI.CompanyID == sessionState.Company.Id)
                                        .Where(CI => CI.SelectedSystem == sessionState.selectedSystem)
                                        .Where(CI => CI.CaseTypeGroup == selectedChapter.CaseTypeGroup)
                                        .Where(CI => CI.CaseType == selectedChapter.CaseType)
                                        .Where(CI => CI.ChapterName == selectedChapter.Name)
                                        .FirstOrDefault();

            if (appChapterItemState is null)
            {
                appChapterItemState = new AppChapterItemState
                {
                    CompanyID = sessionState.Company.Id
                    ,SelectedSystem = sessionState.selectedSystem
                    ,CaseTypeGroup = selectedChapter.CaseTypeGroup
                    ,CaseType = selectedChapter.CaseType
                    ,ChapterName = selectedChapter.Name
                    ,LastUpdated = DateTime.Now //by setting to current date time a full refresh of data will be invoked (once only)
                    ,ActiveUsers = new List<ActiveUsers> { new ActiveUsers { UserName= sessionState.User.FullName, LastActive = DateTime.Now} }
                };

                lstAppChapterStateItems.Add(appChapterItemState);

            }

            return appChapterItemState;
        }

        public void SetUsersCurrentChapter(IUserSessionState sessionState, VmChapter selectedChapter)
        {
            IAppChapterItemState appChapterItemState;

            appChapterItemState = GetChapterItemState(sessionState, selectedChapter);

            ActiveUsers activeUser = appChapterItemState.ActiveUsers
                                                    .Where(U => U.UserName == sessionState.User.FullName)
                                                    .FirstOrDefault();
            if (activeUser is null)
            {
                appChapterItemState.ActiveUsers.Add(new ActiveUsers { UserName = sessionState.User.FullName, LastActive = DateTime.Now });
                appChapterItemState.LastUpdated = DateTime.Now;
            }
            else
            {
                activeUser.LastActive = DateTime.Now;
            }

            FlushInactiveUsers(sessionState, selectedChapter);

        }

        public void FlushInactiveUsers(IUserSessionState sessionState, VmChapter selectedChapter)
        {
            IAppChapterItemState appChapterItemState;

            appChapterItemState = GetChapterItemState(sessionState, selectedChapter);

            int userCount = appChapterItemState.ActiveUsers.Count;

            appChapterItemState.ActiveUsers.RemoveAll(U => (DateTime.Now - U.LastActive).TotalSeconds > 30);
            if(appChapterItemState.ActiveUsers.Count != userCount)
            {
                appChapterItemState.LastUpdated = DateTime.Now;
            }                        
        }

    }



}
