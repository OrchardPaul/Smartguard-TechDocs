using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GadjIT_ClientContext.Models.Smartflow
{
    public partial class VmSmartflowStatus
    {

        public SmartflowStatus SmartflowObject { get; set; }

        public SmartflowStatus AltObject { get; set; }


        public string ComparisonResult { get; set; }
        public string ComparisonIcon { get; set; }

        public bool Compared { get; set; }

        

        public List<string> ComparisonList { get; set; } = new List<string>();

        

        public bool IsChapterItemMatch(VmSmartflowStatus vmCompItem)
        {
            AltObject = vmCompItem.SmartflowObject;
            vmCompItem.Compared = true;

            ComparisonList = new List<string>();
            bool isSame = true;
            SmartflowStatus compItem = vmCompItem.SmartflowObject;

            if ((SmartflowObject.SeqNo ?? 0) != (compItem.SeqNo ?? 0))
            {
                isSame = false;
                ComparisonList.Add("SeqNo");
            }

            if ((SmartflowObject.SuppressStep ?? "") != (compItem.SuppressStep ?? ""))
            {
                isSame = false;
                ComparisonList.Add("SuppressStep");
            }

            if ((SmartflowObject.MilestoneStatus ?? "") != (compItem.MilestoneStatus ?? ""))
            {
                isSame = false;
                ComparisonList.Add("MilestoneStatus");
            }
            return isSame;
        }
    }
}