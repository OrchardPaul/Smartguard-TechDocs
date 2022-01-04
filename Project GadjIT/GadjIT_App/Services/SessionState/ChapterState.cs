using GadjIT.ClientContext.P4W.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Services.SessionState
{
    public interface IChapterState
    {
        List<VmUsrOrDefChapterManagement> lstChapterItems { get; set; }
        
    }

    public class ChapterState : IChapterState
    {
        public List<VmUsrOrDefChapterManagement> lstChapterItems { get; set; } = new List<VmUsrOrDefChapterManagement>();


    }
}
