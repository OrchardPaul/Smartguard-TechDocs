using GadjIT.ClientContext.P4W.Custom;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Gizmo_V1_02.Pages.Chapters
{
    public partial class ChapterExport
    {
        [Parameter]
        public List<VmUsrOrDefChapterManagement> lstChapterItems { get; set; }

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        ElementReference MeasureMe { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                await JSRuntime.InvokeVoidAsync("fncPrepDataTables");
            }
        }

    }

}
