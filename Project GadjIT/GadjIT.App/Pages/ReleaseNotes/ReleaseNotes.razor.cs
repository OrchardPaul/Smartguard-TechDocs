using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.ReleaseNotes
{
    public partial class ReleaseNotes
    {
        private class VersionItem
        {
            public string VersionName { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public string Notes { get; set; }
        }

        public string CurrentVersion { get; set; } = "";

        private List<VersionItem> ListVersions
        {
            get
            {
                List<VersionItem> listVersions = new List<VersionItem>
                {
                    new VersionItem { VersionName = "V01_03", FromDate = new DateTime(2021, 11, 12), ToDate= new DateTime(), Notes="Current - In Development" }
                    ,new VersionItem { VersionName = "V01_02", FromDate = new DateTime(2021, 06, 01), ToDate= new DateTime(2021, 11, 11), Notes="First Official Release of Smartflow" } 
                   
                };
                return listVersions;
            }
        }

        public void SelectVersion(string version)
        {
            CurrentVersion = version;
            StateHasChanged();
        }
    }
}

   
