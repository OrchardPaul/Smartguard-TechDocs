using System;
using System.Collections.Generic;

namespace Gizmo_V1_00.Models
{
    public partial class UsrOrResiMtChapterControl
    {
        public int Id { get; set; }
        public string EntityRef { get; set; }
        public int MatterNo { get; set; }
        public string StepsToRun { get; set; }
        public DateTime? DateScheduleFor { get; set; }
        public string CurrentChapter { get; set; }
        public string DefaultStep { get; set; }
        public string DefaultStepAsName { get; set; }
        public string SubViewName { get; set; }
        public string ScheduleAsName { get; set; }
        public string DoNotReschedule { get; set; }
        public string CompleteAsName { get; set; }
    }
}
