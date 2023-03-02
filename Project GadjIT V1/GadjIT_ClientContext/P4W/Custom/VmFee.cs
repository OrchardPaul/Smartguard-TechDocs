using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT_ClientContext.P4W.Custom
{
    public partial class VmFee
    {
        public Fee FeeObject { get; set; }
        public Fee AltObject { get; set; }
        public string ComparisonResult { get; set; }
        public string ComparisonIcon { get; set; }

        public bool Compared { get; set; }
        public List<string> ComparisonList { get; set; } = new List<string>();

        public bool IsChapterItemMatch(VmFee vmCompItem)
        {
            AltObject = vmCompItem.FeeObject;
            vmCompItem.Compared = true;

            ComparisonList = new List<string>();
            bool isSame = true;
            var compItem = vmCompItem.FeeObject;

            if (FeeObject.SeqNo != compItem.SeqNo)
            {
                isSame = false;
                ComparisonList.Add("SeqNo");
            }

            if (FeeObject.FeeName != compItem.FeeName)
            {
                isSame = false;
                ComparisonList.Add("FeeName");
            }

            if (FeeObject.FeeCategory != compItem.FeeCategory)
            {
                isSame = false;
                ComparisonList.Add("FeeCategory");
            }

            if (FeeObject.Amount != compItem.Amount)
            {
                isSame = false;
                ComparisonList.Add("Amount");
            }

            if (FeeObject.VATable != compItem.VATable)
            {
                isSame = false;
                ComparisonList.Add("VATable");
            }

            if (FeeObject.PostingType != compItem.PostingType)
            {
                isSame = false;
                ComparisonList.Add("PostingType");
            }

            return isSame;
        }

    }
}
