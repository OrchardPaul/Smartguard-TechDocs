using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GadjIT_ClientContext.Models.Smartflow
{
    public partial class VmGenSmartflowItem
    {

        public GenSmartflowItem SmartflowObject { get; set; }

        public GenSmartflowItem AltObject { get; set; }

        public string DocType { get; set; }

        public string ComparisonResult { get; set; }
        public string ComparisonIcon { get; set; }

        public bool Compared { get; set; }

        

        public List<string> ComparisonList { get; set; } = new List<string>();

        public string DocDisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(SmartflowObject.AltDisplayName))
                {
                    return SmartflowObject.Name;
                }
                else
                {
                    return SmartflowObject.AltDisplayName;
                }
            }
        }
        
        public string DocOrigName
        {
            get
            {
                if (string.IsNullOrEmpty(SmartflowObject.AltDisplayName))
                {
                    return "";
                }
                else
                {
                    return SmartflowObject.Name;
                }
            }
        }



        public bool IsChapterItemMatch(VmGenSmartflowItem vmCompItem)
        {
            AltObject = vmCompItem.SmartflowObject;
            vmCompItem.Compared = true;

            ComparisonList = new List<string>();
            bool isSame = true;
            GenSmartflowItem compItem = vmCompItem.SmartflowObject;

            if ((SmartflowObject.SeqNo ?? 0) != (compItem.SeqNo ?? 0))
            {
                isSame = false;
                ComparisonList.Add("SeqNo");
            }

            if ((SmartflowObject.AsName ?? "") != (compItem.AsName ?? ""))
            {
                isSame = false;
                ComparisonList.Add("AsName");
            }

            if ((SmartflowObject.RescheduleDays ?? 0) != (compItem.RescheduleDays ?? 0))
            {
                isSame = false;
                ComparisonList.Add("RescheduleDays");
            }

            if ((SmartflowObject.RescheduleDataItem ?? "") != (compItem.RescheduleDataItem ?? ""))
            {
                isSame = false;
                ComparisonList.Add("RescheduleDataItem");
            }

            if ((SmartflowObject.CompleteName ?? "") != (compItem.CompleteName ?? ""))
            {
                isSame = false;
                ComparisonList.Add("CompleteName");
            }

            if ((SmartflowObject.SuppressStep ?? "") != (compItem.SuppressStep ?? ""))
            {
                isSame = false;
                ComparisonList.Add("SuppressStep");
            }

            if ((SmartflowObject.EntityType ?? "") != (compItem.EntityType ?? ""))
            {
                isSame = false;
                ComparisonList.Add("EntityType");
            }

            if ((SmartflowObject.AltDisplayName ?? "") != (compItem.AltDisplayName ?? ""))
            {
                isSame = false;
                ComparisonList.Add("AltDisplayName");
            }

            if ((SmartflowObject.UserMessage ?? "") != (compItem.UserMessage ?? ""))
            {
                isSame = false;
                ComparisonList.Add("UserMessage");
            }

            if ((SmartflowObject.PopupAlert ?? "") != (compItem.PopupAlert ?? ""))
            {
                isSame = false;
                ComparisonList.Add("PopupAlert");
            }

            if ((SmartflowObject.NextStatus ?? "") != (compItem.NextStatus ?? ""))
            {
                isSame = false;
                ComparisonList.Add("NextStatus");
            }

            if ((SmartflowObject.Action ?? "") != (compItem.Action ?? ""))
            {
                isSame = false;
                ComparisonList.Add("Action");
            }

            if ((SmartflowObject.MilestoneStatus ?? "") != (compItem.MilestoneStatus ?? ""))
            {
                isSame = false;
                ComparisonList.Add("MilestoneStatus");
            }

            if(!(SmartflowObject.LinkedItems is null) && !(compItem.LinkedItems is null))
            {
                if(!(SmartflowObject.LinkedItems is null))
                {
                    if(!(compItem.LinkedItems is null))
                    {
                        if (SmartflowObject.LinkedItems.Count == compItem.LinkedItems.Count)
                        {
                            
                            foreach (var doc in SmartflowObject.LinkedItems)
                            {
                                var compDoc = compItem.LinkedItems.Where(F => F.DocName == doc.DocName).FirstOrDefault();

                                if(compDoc is null)
                                {
                                    isSame = false;
                                    ComparisonList.Add("LinkedItems");
                                }
                                else
                                {
                                    if(!(compDoc.Action == doc.Action))
                                    {
                                        isSame = false;
                                        ComparisonList.Add("LinkedItems");
                                    }

                                    if (!(compDoc.ScheduleDataItem == doc.ScheduleDataItem))
                                    {
                                        isSame = false;
                                        ComparisonList.Add("LinkedItems");
                                    }


                                    if (!(compDoc.DocAsName == doc.DocAsName))
                                    {
                                        isSame = false;
                                        ComparisonList.Add("LinkedItems");
                                    }
                                }

                            }
                        }
                        else
                        {
                            isSame = false;
                            ComparisonList.Add("LinkedItems");
                        }
                    }
                    else
                    {
                        isSame = false;
                        ComparisonList.Add("LinkedItems");
                    }
                }
                else if (!(compItem.LinkedItems is null))
                {
                    isSame = false;
                    ComparisonList.Add("LinkedItems");
                }
            }

            return isSame;
        }
    }
}