using GadjIT_ClientContext.Models.Smartflow;
using System.Collections.Generic;


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
