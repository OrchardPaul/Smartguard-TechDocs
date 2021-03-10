using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT.ClientContext.P4W.Custom
{
    public partial class VmDataViews
    {
        public DataViews DataView { get; set; }

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

    }
}
