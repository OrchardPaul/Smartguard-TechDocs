using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GadjIT.AppContext.GadjIT_App
{
    [Table("GadjIT_Log")]
    public partial class GadjITLog
    {
        [Key]
        public int Id { get; set; }
        public string Message { get; set; }
        public string MessageTemplate { get; set; }
        public string Level { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? TimeStamp { get; set; }
        public string Exception { get; set; }
        public string Properties { get; set; }
        [StringLength(75)]
        public string SourceContext { get; set; }
        [StringLength(75)]
        public string SourceUserId { get; set; }
    }
}
