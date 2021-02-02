using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GadjIT.ClientContext.P4W.Functions
{
    public partial class fnORCHAGetFeeDefinitions
    {
		[StringLength(300)]
		public string FeeDesc { get; set; }
		[StringLength(300)]
		public string Suffix { get; set; }
		[StringLength(50)]
		public string FeeType { get; set; }
		[StringLength(50)]
		public string Category { get; set; }
		[StringLength(50)]
		public string BaseType { get; set; }
		[StringLength(3000)]
		public string WorkProvider { get; set; }
		[StringLength(300)]
		public string OtherVariant { get; set; }
		public int SortGroup { get; set; }
	}
}
