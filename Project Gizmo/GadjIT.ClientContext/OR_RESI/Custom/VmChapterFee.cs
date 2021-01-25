using GadjIT.ClientContext.OR_RESI.Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT.ClientContext.OR_RESI.Custom
{
    public partial class VmChapterFee
    {
        public UsrOrDefChapterManagement FeeItem { get; set; }

        public fnORCHAGetFeeDefinitions feeDefinition { get; set; }

        public bool selected { get; set; }
    }
}
