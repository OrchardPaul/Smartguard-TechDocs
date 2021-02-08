using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT.ClientContext.P4W.Custom
{
    public class VmChapter
    {
        public string CaseTypeGroup { get; set; }
        public string CaseType { get; set; }
        public string Name { get; set; }
        public int SeqNo { get; set; }
        public string BackgroundColour { get; set; }
        public string GeneralNotes { get; set; }
        public string DelvolperNotes { get; set; }
        public List<UsrOrDefChapterManagement> ChapterItems { get; set; }
    }
}
