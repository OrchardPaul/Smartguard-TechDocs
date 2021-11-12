using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT.AppContext.GadjIT_App.Custom
{
    /*
     * Class for checkbox list to determin which group the user is to be assigned too 
     */

    public partial class WorkTypeAssignment
    {
        public AppWorkTypeGroups WorkTypeGroup { get; set; }
        public bool IsAssigned { get; set; }
    }
}
