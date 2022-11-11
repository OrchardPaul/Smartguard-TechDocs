
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GadjIT_App.Pages.Chapters
{
    public class ChapterP4WStepSchema
    {

        [StringLength(200)]
        public string StepName { get; set; }

        [StringLength(200)]
        public string P4WCaseTypeGroup { get; set; }

        [StringLength(200)]
        public string GadjITCaseTypeGroup { get; set; }
        
        [StringLength(200)]
        public string GadjITCaseType { get; set; }
        
        [StringLength(200)]
        public string Smartflow { get; set; }

        [StringLength(30)]
        public string SFVersion { get ; set;} = "";


        public List<ChapterP4WStepQuestion> Questions { get; set; }
        public List<ChapterP4WStepAnswer> Answers { get; set; }

    }

    public class ChapterP4WStepQuestion
    {
        
        public int QNo { get; set; }

        [StringLength(2000)]
        public string QText { get; set; }
    }

    public class ChapterP4WStepAnswer
    {
       
        public int QNo { get; set; }

        [StringLength(2000)]
        public string GoToData { get; set; }
    }
}

