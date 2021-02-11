using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo_V1_02.Services.SessionState;
using GadjIT.ClientContext.P4W.Custom;

namespace Gizmo_V1_02.Pages.Chapters
{
    public partial class ChapterData
    {
        [Inject]
        public IChapterState ChapterState { get; set; }

        public List<VmUsrOrDefChapterManagement> lstChapterItems { get; set; } 

        [Parameter]
        public string ListToDisplay { get; set; } = "";

        [Parameter]
        public string PageTitle { get; set; } = "";

        [Parameter]
        public string PageSubTitle { get; set; } = "";

        
        protected override void OnInitialized()
        {
            lstChapterItems = ChapterState.lstChapterItems;
            
        }

    }

}
