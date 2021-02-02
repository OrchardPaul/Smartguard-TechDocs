using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GadjIT.ClientContext.P4W
{
    [Table("Usr_OR_RESI_MT_Chapter_Control")]
    public partial class UsrOrResiMtChapterControl
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [StringLength(15)]
        public string EntityRef { get; set; }
        public int MatterNo { get; set; }
        [Column("Steps_to_Run")]
        public string StepsToRun { get; set; }
        [Column("Date_Schedule_For", TypeName = "datetime")]
        public DateTime? DateScheduleFor { get; set; }
        [Column("Current_Chapter")]
        [StringLength(200)]
        public string CurrentChapter { get; set; }
        [Column("Default_Step")]
        [StringLength(300)]
        public string DefaultStep { get; set; }
        [Column("Default_Step_AsName")]
        [StringLength(300)]
        public string DefaultStepAsName { get; set; }
        [Column("Sub_View_Name")]
        [StringLength(500)]
        public string SubViewName { get; set; }
        [Column("Schedule_AsName")]
        [StringLength(500)]
        public string ScheduleAsName { get; set; }
        [Column("Do_Not_Reschedule")]
        [StringLength(1)]
        public string DoNotReschedule { get; set; }
        [Column("Complete_AsName")]
        [StringLength(500)]
        public string CompleteAsName { get; set; }
    }
}
