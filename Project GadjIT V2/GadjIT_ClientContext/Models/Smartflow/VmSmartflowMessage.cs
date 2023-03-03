using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT_ClientContext.Models.Smartflow
{
    public partial class VmSmartflowMessage
    {
        public SmartflowMessage Message { get; set; }
        public SmartflowMessage AltMessage { get; set; }

        public string ComparisonResult { get; set; }
        public string ComparisonIcon { get; set; }

        public string MsgValidation { get; set; }

        public bool Compared { get; set; }

        public string MsgTooltip { get; set; }
        public List<string> ComparisonList { get; set; } = new List<string>();


        public bool IsChapterItemMatch(VmSmartflowMessage vmCompItem)
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
                ComparisonList.Add("Message");
            }

            if (Message.FromDate != AltMessage.FromDate)
            {
                isSame = false;
                ComparisonList.Add("FromDate");
            }
            
            if (Message.ToDate != AltMessage.ToDate)
            {
                isSame = false;
                ComparisonList.Add("ToDate");
            }


            return isSame;
        }
    }
}
