using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT.ClientContext.P4W.Custom
{
    public class VmUsrOrsfSmartflows
    {
        public UsrOrsfSmartflows SmartflowObject { get; set; }

        public string ComparisonResult { get; set; }
        public string ComparisonIcon { get; set; }
        public List<string> ComparisonList { get; set; } = new List<string>();

    }
}
