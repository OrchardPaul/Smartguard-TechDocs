
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GadjIT_App.Pages.Smartflows
{
    public class SmartflowP4WStepSchema
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


        public List<SmartflowP4WStepQuestion> Questions { get; set; }
        public List<SmartflowP4WStepAnswer> Answers { get; set; }

    }

    public class SmartflowP4WStepQuestion
    {
        
        public int QNo { get; set; }

        [StringLength(2000)]
        public string QText { get; set; }
    }

    public class SmartflowP4WStepAnswer
    {
       
        public int QNo { get; set; }

        [StringLength(2000)]
        public string GoToData { get; set; }
    }
}

