using GadjIT_ClientContext.P4W.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Services.SessionState
{
    public interface IChapterState
    {
        List<VmGenSmartflowItem> lstChapterItems { get; set; }
        
    }

    public class ChapterState : IChapterState
    {
        public List<VmGenSmartflowItem> lstChapterItems { get; set; } = new List<VmGenSmartflowItem>();


    }
}
