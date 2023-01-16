using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GadjIT_ClientContext.P4W
{
    public partial class GenSmartflowItem
    {
        [StringLength(100)]
        [Required(ErrorMessage = "Item type must be set")] //Agenda, Status, Doc, Fee, TickerMessage
        public string Type { get; set; }

        [StringLength(250)]
        [Required]
        public string Name { get; set; }

        [Required]
        public int? SeqNo { get; set; }

        [StringLength(1)]
        public string SuppressStep { get; set; }

        public string MilestoneStatus { get; set; }

        [StringLength(100)]
        public string EntityType { get; set; }
        
        [StringLength(250)]
        public string AsName { get; set; }
        
        [StringLength(250)]
        public string CompleteName { get { return string.IsNullOrEmpty(completeName) ? "" : completeName.ToUpper(); } set { completeName = string.IsNullOrEmpty(value) ? "" : value.ToUpper(); } } 


        private string completeName;


        public int? RescheduleDays { get; set; }

        [StringLength(250)]
        public string RescheduleDataItem { get; set; }

        [StringLength(300)]
        public string AltDisplayName { get; set; }
        
        [StringLength(100)]
        public string NextStatus { get; set; }
        
        public string Action { get; set; }
        public string UserMessage { get; set; }
        public string PopupAlert { get; set; }
        public string DeveloperNotes { get; set; }
        public string StoryNotes { get; set; }
        public string TrackingMethod { get; set; }
        public string ChaserDesc { get; set; }
        public string OptionalDocument { get; set; } = "N";
        public string CustomItem { get; set; } = "N";
        public string Agenda { get; set; } = "";
        public List<LinkedItems> LinkedItems { get; set; }


    }

    public class LinkedItems
    {
        [Required (ErrorMessage = "Name field is required")]
        public string DocName { get; set; }
        public string DocAsName { get; set; }
        public string DocType { get; set; } = "";

        [Required]
        public string Action { get; set; }

        public string TrackingMethod { get; set; }
        public string ChaserDesc { get; set; }
        public int ScheduleDays { get; set; }
        public string OptionalDocument { get; set; } = "N";

        public string CustomItem { get; set; } = "N";
        public string Agenda { get; set; } = "";

        [StringLength(250)]
        public string ScheduleDataItem { get; set; }
    }
}
