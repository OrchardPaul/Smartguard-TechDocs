using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GadjIT.ClientContext.P4W
{
    [Table("Usr_OR_DEF_Chapter_Management")]
    public partial class UsrOrDefChapterManagement
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("ParentID")]
        public int? ParentId { get; set; }
        [StringLength(100)]
        public string CaseTypeGroup { get; set; }
        [StringLength(100)]
        public string CaseType { get; set; }
        [StringLength(100)]
        [Required(ErrorMessage = "Item type must be set")]
        public string Type { get; set; }
        [StringLength(250)]
        [Required]
        public string Name { get; set; }
        [NotMapped]
        public string ChapterName { get; set; }
        [Required]
        public int? SeqNo { get; set; }
        [Column("Suppress_Step")]
        [StringLength(1)]
        public string SuppressStep { get; set; }
        [Column("Entity_Type")]
        [StringLength(100)]
        public string EntityType { get; set; }
        [StringLength(250)]
        public string AsName { get; set; }
        [StringLength(250)]
        public string CompleteName { get; set; }
        public int? RescheduleDays { get; set; }
        [Column("Alt_Display_Name")]
        [StringLength(300)]
        public string AltDisplayName { get; set; }

        [Column("Next_Status")]
        [StringLength(100)]
        public string NextStatus { get; set; }
        [NotMapped]
        public string Action { get; set; }


        public string ChapterData { get; set; }

        [NotMapped]
        public string UserMessage { get; set; }
        [NotMapped]
        public string PopupAlert { get; set; }
        [NotMapped]
        public string DeveloperNotes { get; set; }
        [NotMapped]
        public string StoryNotes { get; set; }

        [NotMapped]
        public List<FollowUpDoc> FollowUpDocs { get; set; }
        
    }

    public class FollowUpDoc
    {
        public string DocName { get; set; }
        public string DocAsName { get; set; }
        public string Action { get; set; }
        public int? ScheduleDays { get; set; }

    }

}
