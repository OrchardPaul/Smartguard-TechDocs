using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GadjIT_ClientContext.P4W
{
    public partial class DataView
    {
        public int? SeqNo { get; set; } //BlockNo refers to the tab blocks on the XAML form. 
                                         //NOTE: PT 20/12/2022 - Changed BlockNo to SeqNo  

        [StringLength(100)]
        [Required(ErrorMessage = "Name field is required")]
        public string ViewName { get; set; }

        [StringLength(200)]
        public string DisplayName { get; set; }

        [NotMapped]
        public string DisplayNameOrViewName
        {
            get
            {
                if (string.IsNullOrEmpty(DisplayName))
                {
                    return ViewName;
                }
                else
                {
                    return DisplayName;
                }
            }
        }

    }
}
