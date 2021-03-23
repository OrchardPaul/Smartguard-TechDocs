using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT.ClientContext.P4W
{
    public partial class Fee
    {
        public string FeeName { get; set; }
        public int? SeqNo { get; set; }
        public string FeeCategory { get; set; }
        public decimal Amount { get; set; }
        public string VATable { get; set; }
        public string PostingType { get; set; }
    }
}
