using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GadjIT_ClientContext.Models.Smartflow
{
    public partial class VmSmartflowAgenda
    {

        public SmartflowAgenda SmartflowObject { get; set; }

        public SmartflowAgenda AltObject { get; set; }

        public string ComparisonResult { get; set; }
        public string ComparisonIcon { get; set; }

        public bool Compared { get; set; }

        

        public List<string> ComparisonList { get; set; } = new List<string>();

        



        public bool IsChapterItemMatch(VmSmartflowAgenda vmCompItem)
        {
            AltObject = vmCompItem.SmartflowObject;
            vmCompItem.Compared = true;

            ComparisonList = new List<string>();
            bool isSame = true;
            SmartflowAgenda compItem = vmCompItem.SmartflowObject;

            
            

            return isSame;
        }
    }
}