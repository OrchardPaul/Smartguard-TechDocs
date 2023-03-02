using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT_ClientContext.P4W.Custom
{
    public class VmSmartflow
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
        public List<DataView> DataViews { get; set; } = new List<DataView>();
        public List<Fee> Fees { get; set; } = new List<Fee>();
        public List<TickerMessage> TickerMessages { get; set; } = new List<TickerMessage>();
    }
}
