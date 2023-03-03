using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GadjIT_ClientContext.Models.Smartflow
{
    public partial class SmartflowStatus
    {
        
        [StringLength(250)]
        [Required]
        public string Name { get; set; }

        [Required]
        public int? SeqNo { get; set; }

        [StringLength(1)]
        public string SuppressStep { get; set; }

        public string MilestoneStatus { get; set; }


    }

}
