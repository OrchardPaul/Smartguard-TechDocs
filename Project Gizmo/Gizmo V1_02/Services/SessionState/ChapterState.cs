using GadjIT.ClientContext.P4W.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Services.SessionState
{
    public interface IChapterState
    {
        List<VmUsrOrDefChapterManagement> lstAgendas { get; set; }
        List<VmUsrOrDefChapterManagement> lstAll { get; set; }
        List<VmUsrOrDefChapterManagement> lstChapters { get; set; }
        List<VmUsrOrDefChapterManagement> lstDocs { get; set; }
        List<VmUsrOrDefChapterManagement> lstFees { get; set; }
        List<VmUsrOrDefChapterManagement> lstStatus { get; set; }
        List<VmChapterFee> lstVmFeeModalItems { get; set; }
    }

    public class ChapterState : IChapterState
    {
        public List<VmUsrOrDefChapterManagement> lstChapters { get; set; } = new List<VmUsrOrDefChapterManagement>();

        public List<VmUsrOrDefChapterManagement> lstAll { get; set; } = new List<VmUsrOrDefChapterManagement>();

        public List<VmUsrOrDefChapterManagement> lstAgendas { get; set; } = new List<VmUsrOrDefChapterManagement>();
        public List<VmUsrOrDefChapterManagement> lstFees { get; set; } = new List<VmUsrOrDefChapterManagement>();
        public List<VmChapterFee> lstVmFeeModalItems { get; set; } = new List<VmChapterFee>();
        public List<VmUsrOrDefChapterManagement> lstDocs { get; set; } = new List<VmUsrOrDefChapterManagement>();
        public List<VmUsrOrDefChapterManagement> lstStatus { get; set; } = new List<VmUsrOrDefChapterManagement>();

    }
}
