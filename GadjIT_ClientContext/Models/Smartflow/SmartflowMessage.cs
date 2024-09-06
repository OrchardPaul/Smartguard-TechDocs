using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GadjIT_ClientContext.Models.Smartflow
{
    public partial class SmartflowMessage
    {
        public int? SeqNo { get; set; }

        [StringLength(150)]
        [Required(ErrorMessage = "Name field is required")]
        public string Message { get; set; }

        [Required]
        public string FromDate { get; set; }

        [Required]
        public string ToDate { get; set; }
    }
}
