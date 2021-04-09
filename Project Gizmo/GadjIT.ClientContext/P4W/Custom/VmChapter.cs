using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT.ClientContext.P4W.Custom
{
    public class VmChapter
    {
        public string CaseTypeGroup { get; set; }
        public string CaseType { get; set; }
        public string P4WCaseTypeGroup { get; set; }
        public string StepName { get; set; }
        public string Name { get; set; }
        public int SeqNo { get; set; }
        public string BackgroundColour { get; set; }
        public string BackgroundColourName { get; set; }
        public string BackgroundImage { get; set; }
        public string BackgroundImageName { get; set; }
        public string ShowPartnerNotes { get; set; }
        public string GeneralNotes { get; set; }
        public string DeveloperNotes { get; set; }
        public string SelectedView { get; set; }
        public string SelectedStep { get; set; }
        public List<UsrOrDefChapterManagement> ChapterItems { get; set; }
        public List<DataViews> DataViews { get; set; }
        public List<Fee> Fees { get; set; }
    }
}
