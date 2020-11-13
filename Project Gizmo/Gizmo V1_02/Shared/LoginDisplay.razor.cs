using Gizmo_V1_02.Data;
using Gizmo_V1_02.Data.Admin;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Shared
{
    public partial class LoginDisplay
    {
        [Parameter]
        public string userFullName { get; set; }

    }
}
