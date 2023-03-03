using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GadjIT_ClientContext.Models.Smartflow
{
    public partial class SmartflowAgenda
    {
        [StringLength(250)]
        [Required]
        public string Name { get; set; }

    }

}
