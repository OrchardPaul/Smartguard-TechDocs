using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT_ClientContext.P4W.Custom
{
    public partial class VmDataView
    {
        public DataView DataView { get; set; }

        public DataView AltDataView { get; set; }

        public string ComparisonResult { get; set; }
        public string ComparisonIcon { get; set; }

        public bool Compared { get; set; }
        public List<string> ComparisonList { get; set; } = new List<string>();

        public bool IsChapterItemMatch(VmDataView dataItem)
        {
            AltDataView = dataItem.DataView;
            dataItem.Compared = true;

            ComparisonList = new List<string>();
            bool isSame = true;

            if (DataView.SeqNo != AltDataView.SeqNo)
            {
                isSame = false;
                ComparisonList.Add("SeqNo");
            }
            if (DataView.DisplayName != AltDataView.DisplayName)
            {
                isSame = false;
                ComparisonList.Add("DisplayName");
            }
            if (DataView.ViewName != AltDataView.ViewName)
            {
                isSame = false;
                ComparisonList.Add("ViewName");
            }

            return isSame;
        }


    }
}
