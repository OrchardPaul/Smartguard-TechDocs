using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GadjIT_ClientContext.P4W.Custom
{
    public class VmUsrOrsfSmartflows
    {
        public UsrOrsfSmartflows SmartflowObject { get; set; }

        public string ComparisonResult { get; set; }
        public string ComparisonIcon { get; set; }
        public List<string> ComparisonList { get; set; } = new List<string>();

        
        public string StatisticsStatus {get; set;} = "Not Set";
        public int? NumAgenda {get; set;}
        public int? NumStatus {get; set;}
        public int? NumDocs {get; set;}
        public int? NumFees {get; set;}
        public int? NumDataViews {get; set;}
        public int? NumMessages {get; set;}

        public string BackgroundImage { get; set; }
        public string BackgroundImageName { get; set; }




        public void SetSmartflowStatistics()
        {
            NumAgenda = 0;
            NumStatus = 0;
            NumDocs = 0;
            NumFees = 0;
            NumDataViews = 0;
            NumMessages = 0;
            StatisticsStatus = "Not Set";

            try
            {
                if(!string.IsNullOrEmpty(SmartflowObject.SmartflowData))
                {
                    var vmSmartflow = JsonConvert.DeserializeObject<VmSmartflow>(SmartflowObject.SmartflowData);

                    if(vmSmartflow != null)
                    {
                        if(vmSmartflow.Items != null)
                        {
                            NumAgenda = vmSmartflow.Items.Where(I => I.Type == "Agenda").Count();
                            NumStatus = vmSmartflow.Items.Where(I => I.Type == "Status").Count();
                            NumDocs = vmSmartflow.Items.Where(I => I.Type == "Doc").Count();
                        }
                        if(vmSmartflow.Fees != null)
                        {
                            NumFees = vmSmartflow.Fees.Count();
                        }
                        if(vmSmartflow.DataViews != null)
                        {
                            NumDataViews = vmSmartflow.DataViews.Count();
                        }
                        if(vmSmartflow.TickerMessages != null)
                        {
                            NumMessages = vmSmartflow.TickerMessages.Count();
                        }

                        StatisticsStatus = "Set";

                        BackgroundImage = vmSmartflow.BackgroundImage;
                        BackgroundImageName = vmSmartflow.BackgroundImageName;

                    }
                }
            }
            catch(Exception e)
            {
                StatisticsStatus = "Failed";
            }
        }
    }
}
