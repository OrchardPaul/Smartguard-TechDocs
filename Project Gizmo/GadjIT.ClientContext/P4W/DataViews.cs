using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GadjIT.ClientContext.P4W
{
    public partial class DataViews
    {
        public int BlockNo { get; set; }

        [StringLength(100)]
        public string ViewName { get; set; }

        [StringLength(200)]
        public string DisplayName { get; set; }

    }
}
