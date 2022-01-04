using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT.ClientContext.P4W.Custom
{
    public partial class VmDataViews
    {
        public DataViews DataView { get; set; }

        public DataViews AltDataView { get; set; }

        public string ComparisonResult { get; set; }
        public string ComparisonIcon { get; set; }

        public bool Compared { get; set; }
        public List<string> ComparisonList { get; set; } = new List<string>();

        public string DataViewDisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(DataView.DisplayName))
                {
                    return DataView.ViewName;
                }
                else
                {
                    return DataView.DisplayName;
                }
            }
        }

        public bool IsDataViewMatch(VmDataViews dataItem)
        {
            AltDataView = dataItem.DataView;
            dataItem.Compared = true;

            ComparisonList = new List<string>();
            bool isSame = true;

            if (DataView.BlockNo != AltDataView.BlockNo)
            {
                isSame = false;
                ComparisonList.Add("BlockNo");
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
