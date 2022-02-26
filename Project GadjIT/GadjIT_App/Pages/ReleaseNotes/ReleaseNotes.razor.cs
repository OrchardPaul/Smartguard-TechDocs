using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.ReleaseNotes
{
    public partial class ReleaseNotes
    {
        public class VersionItem
        {
            public string VersionName { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public string Notes { get; set; }
        }

        public string CurrentVersion { get; set; } = "";

        public VersionItem SelectedVersion { get; set; }

        private List<VersionItem> ListVersions
        {
            get
            {
                List<VersionItem> listVersions = new List<VersionItem>
                {
                    new VersionItem { VersionName = "V01_05", Notes="In Development" }
                    , new VersionItem { VersionName = "V01_04", FromDate = new DateTime(2022, 02, 25), Notes="Current: inc. Agenda Management and Optional Documents" }
                    , new VersionItem { VersionName = "V01_03", FromDate = new DateTime(2021, 11, 12), ToDate= new DateTime(2022,02,24), Notes="Minor upgrade including Global Status and Bulk Step Creation" }
                    , new VersionItem { VersionName = "V01_02", FromDate = new DateTime(2021, 06, 01), ToDate= new DateTime(2021, 11, 11), Notes="First Official Release of Smartflow" } 
                   
                };
                return listVersions;
            }
        }

        public void SelectVersion(string version)
        {
            SelectedVersion = ListVersions.Where(V => V.VersionName == version).FirstOrDefault();
            CurrentVersion = version;
            StateHasChanged();
        }

        void SelectHome()
        {
            CurrentVersion = "";
            StateHasChanged();
        }
    }
}

   
