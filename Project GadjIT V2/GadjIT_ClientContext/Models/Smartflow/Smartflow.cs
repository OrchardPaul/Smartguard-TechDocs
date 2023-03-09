using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT_ClientContext.Models.Smartflow
{
    public class SmartflowV1
    {
        public string CaseTypeGroup { get; set; }
        public string CaseType { get; set; }
        public string Name { get; set; }
        public int SeqNo { get; set; }
        public string P4WCaseTypeGroup { get; set; }
        public string StepName { get; set; }
        public string BackgroundColour { get; set; }
        public string BackgroundColourName { get; set; }
        public string BackgroundImage { get; set; }
        public string BackgroundImageName { get; set; }
        public string ShowPartnerNotes { get; set; }
        public string ShowDocumentTracking { get; set; }
        public string GeneralNotes { get; set; }
        public string DeveloperNotes { get; set; }
        public string SelectedView { get; set; }
        public string SelectedStep { get; set; }
        public List<GenSmartflowItem> Items { get; set; } = new List<GenSmartflowItem>();
        public List<SmartflowDataView> DataViews { get; set; } = new List<SmartflowDataView>();
        public List<SmartflowFee> Fees { get; set; } = new List<SmartflowFee>();
        public List<SmartflowMessage> TickerMessages { get; set; } = new List<SmartflowMessage>();
    }

    public class SmartflowV2
    {
        public string CaseTypeGroup { get; set; }
        public string CaseType { get; set; }
        public string Name { get; set; }
        public int SeqNo { get; set; }
        public string P4WCaseTypeGroup { get; set; }
        public string StepName { get; set; }
        public string BackgroundColour { get; set; }
        public string BackgroundColourName { get; set; }
        public string BackgroundImage { get; set; }
        public string BackgroundImageName { get; set; }
        public string ShowPartnerNotes { get; set; }
        public string ShowDocumentTracking { get; set; }
        public string GeneralNotes { get; set; }
        public string DeveloperNotes { get; set; }
        public string SelectedView { get; set; }
        public string SelectedStep { get; set; }
        public List<SmartflowAgenda> Agendas { get; set; } = new List<SmartflowAgenda>();
        public List<SmartflowStatus> Status { get; set; } = new List<SmartflowStatus>();
        public List<SmartflowDocument> Documents { get; set; } = new List<SmartflowDocument>();
        public List<SmartflowDataView> DataViews { get; set; } = new List<SmartflowDataView>();
        public List<SmartflowFee> Fees { get; set; } = new List<SmartflowFee>();
        public List<SmartflowMessage> Messages { get; set; } = new List<SmartflowMessage>();
    }
}
