using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.Admin.UserManagement
{
    public partial class ManageUsersMainPage
    {

        public bool showDetail { get; set; } = false;

        public void ToggleDetail() 
        {
            showDetail = !showDetail;

            StateHasChanged();
        }


    }
}
