using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT.ClientContext.P4W.Custom
{
    public partial class VmTickerMessages
    {
        public TickerMessages Message { get; set; }
        public TickerMessages AltMessage { get; set; }

        public string ComparisonResult { get; set; }
        public string ComparisonIcon { get; set; }

        public string msgValidation { get; set; }

        public bool Compared { get; set; }

        public string msgTooltip { get; set; }
        public List<string> ComparisonList { get; set; } = new List<string>();


        public bool IsTickerMessageMatch(VmTickerMessages vmCompItem)
        {
            AltMessage = vmCompItem.Message;
            vmCompItem.Compared = true;

            ComparisonList = new List<string>();
            bool isSame = true;

            if (Message.SeqNo != AltMessage.SeqNo)
            {
                isSame = false;
                ComparisonList.Add("SeqNo");
            }

            if (Message.Message != AltMessage.Message)
            {
                isSame = false;
                ComparisonList.Add("AsName");
            }

            if (Message.ToDate != AltMessage.ToDate)
            {
                isSame = false;
                ComparisonList.Add("RescheduleDays");
            }

            if (Message.FromDate != AltMessage.FromDate)
            {
                isSame = false;
                ComparisonList.Add("CompleteName");
            }

            return isSame;
        }
    }
}
