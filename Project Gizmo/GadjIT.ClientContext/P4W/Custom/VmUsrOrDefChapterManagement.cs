using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GadjIT.ClientContext.P4W.Custom
{
    public partial class VmUsrOrDefChapterManagement
    {

        public GenSmartflowItem ChapterObject { get; set; }

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
                if (string.IsNullOrEmpty(ChapterObject.AltDisplayName))
                {
                    return ChapterObject.Name;
                }
                else
                {
                    return ChapterObject.AltDisplayName;
                }
            }
        }
        
        public string DocOrigName
        {
            get
            {
                if (string.IsNullOrEmpty(ChapterObject.AltDisplayName))
                {
                    return "";
                }
                else
                {
                    return ChapterObject.Name;
                }
            }
        }



        public bool IsChapterItemMatch(VmUsrOrDefChapterManagement vmCompItem)
        {
            AltObject = vmCompItem.ChapterObject;
            vmCompItem.Compared = true;

            ComparisonList = new List<string>();
            bool isSame = true;
            GenSmartflowItem compItem = vmCompItem.ChapterObject;

            if (ChapterObject.SeqNo != compItem.SeqNo)
            {
                isSame = false;
                ComparisonList.Add("SeqNo");
            }

            if (ChapterObject.AsName != compItem.AsName)
            {
                isSame = false;
                ComparisonList.Add("AsName");
            }

            if (ChapterObject.RescheduleDays != compItem.RescheduleDays)
            {
                isSame = false;
                ComparisonList.Add("RescheduleDays");
            }

            if (ChapterObject.CompleteName != compItem.CompleteName)
            {
                isSame = false;
                ComparisonList.Add("CompleteName");
            }

            if (ChapterObject.SuppressStep != compItem.SuppressStep)
            {
                isSame = false;
                ComparisonList.Add("SuppressStep");
            }

            if (ChapterObject.EntityType != compItem.EntityType)
            {
                isSame = false;
                ComparisonList.Add("EntityType");
            }

            if (ChapterObject.AltDisplayName != compItem.AltDisplayName)
            {
                isSame = false;
                ComparisonList.Add("AltDisplayName");
            }

            if (ChapterObject.UserMessage != compItem.UserMessage)
            {
                isSame = false;
                ComparisonList.Add("UserMessage");
            }

            if (ChapterObject.PopupAlert != compItem.PopupAlert)
            {
                isSame = false;
                ComparisonList.Add("PopupAlert");
            }

            if (ChapterObject.NextStatus != compItem.NextStatus)
            {
                isSame = false;
                ComparisonList.Add("NextStatus");
            }

            if (ChapterObject.Action != compItem.Action)
            {
                isSame = false;
                ComparisonList.Add("Action");
            }


            if(!(ChapterObject.FollowUpDocs is null))
            {
                if(!(compItem.FollowUpDocs is null))
                {
                    if (ChapterObject.FollowUpDocs.Count == compItem.FollowUpDocs.Count)
                    {
                        
                        foreach (var doc in ChapterObject.FollowUpDocs)
                        {
                            var compDoc = compItem.FollowUpDocs.Where(F => F.DocName == doc.DocName).FirstOrDefault();

                            if(compDoc is null)
                            {
                                isSame = false;
                                ComparisonList.Add("FollowUpDocs");
                            }
                            else
                            {
                                if(!(compDoc.Action == doc.Action))
                                {
                                    isSame = false;
                                    ComparisonList.Add("FollowUpDocs");
                                }

                                if (!(compDoc.DocAsName == doc.DocAsName))
                                {
                                    isSame = false;
                                    ComparisonList.Add("FollowUpDocs");
                                }
                            }
    
                        }
                    }
                    else
                    {
                        isSame = false;
                        ComparisonList.Add("FollowUpDocs");
                    }
                }
                else
                {
                    isSame = false;
                    ComparisonList.Add("FollowUpDocs");
                }
            }
            else if (!(compItem.FollowUpDocs is null))
            {
                isSame = false;
                ComparisonList.Add("FollowUpDocs");
            }




            return isSame;
        }
    }
}