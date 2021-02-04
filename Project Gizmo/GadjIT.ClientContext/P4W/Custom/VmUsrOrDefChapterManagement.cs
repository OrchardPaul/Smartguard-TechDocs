using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT.ClientContext.P4W.Custom
{
    public partial class VmUsrOrDefChapterManagement
    {
        public UsrOrDefChapterManagement ChapterObject { get; set; }

        public UsrOrDefChapterManagement AltObject { get; set; }

        public string ComparisonResult { get; set; }
        public string ComparisonIcon { get; set; }
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

            ComparisonList = new List<string>();
            bool isSame = true;
            UsrOrDefChapterManagement compItem = vmCompItem.ChapterObject;

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

            return isSame;
        }
    }
}