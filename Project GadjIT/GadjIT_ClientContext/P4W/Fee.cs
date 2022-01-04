using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GadjIT.ClientContext.P4W
{
    public partial class Fee
    {
        [Required(ErrorMessage = "Name field is required")]
        public string FeeName { get; set; }
        public int? SeqNo { get; set; }
        public string FeeCategory { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public string VATable { get; set; }
        public string PostingType { get; set; }
    }
}
