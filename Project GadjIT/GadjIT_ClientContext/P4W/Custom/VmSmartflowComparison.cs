using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT_ClientContext.P4W.Custom
{
    public partial class VmSmartflowComparison
    {
        public VmSmartflow CurrentChapter { get; set; }

        public VmSmartflow AltChapter { get; set; }

      
        public string ComparisonResult { get; set; }
        public string ComparisonIcon { get; set; }
        public List<string> ComparisonList { get; set; } = new List<string>();

        public bool IsChapterMatch(VmSmartflow vmCompItem)
        {
            AltChapter = vmCompItem;

            ComparisonList = new List<string>();
            bool isSame = true;
            VmSmartflow compItem = vmCompItem;

            if (CurrentChapter.CaseTypeGroup != compItem.CaseTypeGroup)
            {
                isSame = false;
                ComparisonList.Add("CaseTypeGroup");
            }
            if (CurrentChapter.CaseType != compItem.CaseType)
            {
                isSame = false;
                ComparisonList.Add("CaseType");
            }
            if (CurrentChapter.StepName != compItem.StepName)
            {
                isSame = false;
                ComparisonList.Add("StepName");
            }
            if (CurrentChapter.BackgroundColour != compItem.BackgroundColour)
            {
                isSame = false;
                ComparisonList.Add("BackgroundColour");
            }
            if (CurrentChapter.BackgroundImage != compItem.BackgroundImage)
            {
                isSame = false;
                ComparisonList.Add("BackgroundImage");
            }
            
            return isSame;
        }

    }


}
