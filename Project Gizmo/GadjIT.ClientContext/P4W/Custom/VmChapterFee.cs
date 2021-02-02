using GadjIT.ClientContext.P4W.Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT.ClientContext.P4W.Custom
{
    public partial class VmChapterFee
    {
        public UsrOrDefChapterManagement FeeItem { get; set; }

        public fnORCHAGetFeeDefinitions feeDefinition { get; set; }

        public bool selected { get; set; }
    }
}
