namespace GadjIT_App.Pages.Smartflows.ComponentsCaseTypeDetail._Documents
{
    public class LinkedCaseItem
    {
        
        public string ItemName { get; set; }
        public string AltName { get; set; }
        public string OrigItemName { get; set; }
        public bool IsAttachment { get; set; } 
        public int? OrigSeqNo { get; set; }
        

        //#########################
        //Case Type Maintenance Items
        //#########################
        public bool IsItemLinked { get; set; } /// No Matching entry in Document Library.  Will need addressing if true.
        public string SmartflowName { get; set; } /// Used to indicate in the maintenance list which Smartflow the document is used in. 

        
    }
}