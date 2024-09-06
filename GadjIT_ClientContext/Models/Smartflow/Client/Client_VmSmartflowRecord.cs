using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GadjIT_ClientContext.Models.Smartflow.Client
{
    public class Client_VmSmartflowRecord
    {
        public Client_SmartflowRecord ClientSmartflowRecord { get; set; }

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
                if(!string.IsNullOrEmpty(ClientSmartflowRecord.SmartflowData))
                {
                    var vmSmartflow = JsonConvert.DeserializeObject<SmartflowV2>(ClientSmartflowRecord.SmartflowData);

                    if(vmSmartflow != null)
                    {
                        // if(vmSmartflow.Items != null)
                        // {
                        //     NumAgenda = vmSmartflow.Items.Where(I => I.Type == "Agenda").Count();
                        //     NumStatus = vmSmartflow.Items.Where(I => I.Type == "Status").Count();
                        //     NumDocs = vmSmartflow.Items.Where(I => I.Type == "Doc").Count();
                        // }
                        if(vmSmartflow.Agendas != null)
                        {
                            NumAgenda = vmSmartflow.Agendas.Count();
                        }
                        if(vmSmartflow.Status != null)
                        {
                            NumStatus = vmSmartflow.Status.Count();
                        }
                        if(vmSmartflow.Documents != null)
                        {
                            NumDocs = vmSmartflow.Documents.Count();
                        }
                        if(vmSmartflow.Fees != null)
                        {
                            NumFees = vmSmartflow.Fees.Count();
                        }
                        if(vmSmartflow.DataViews != null)
                        {
                            NumDataViews = vmSmartflow.DataViews.Count();
                        }
                        if(vmSmartflow.Messages != null)
                        {
                            NumMessages = vmSmartflow.Messages.Count();
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
