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
        public string Message { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }
    }
}
