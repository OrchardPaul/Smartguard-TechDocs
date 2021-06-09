using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GadjIT.ClientContext.P4W
{
    public partial class TickerMessages
    {
        public int SeqNo { get; set; }

        [StringLength(150)]
        [Required(ErrorMessage = "Name field is required")]
        public string Message { get; set; }

        [Required]
        public string FromDate { get; set; }

        [Required]
        public string ToDate { get; set; }
    }
}
