using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        [StringLength(256)]
        public string SelectedUri { get; set; }

        public int SelectedCompanyId { get; set; }

        [StringLength(500)]
        public string MainBackgroundImage { get; set; }

        public bool DisplaySmartflowPreviewImage { get; set; }
    }
}
