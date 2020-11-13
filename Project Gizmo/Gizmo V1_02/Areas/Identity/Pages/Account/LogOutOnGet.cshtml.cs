using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo_V1_02.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Gizmo_V1_02.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public class LogoutModelOnGet : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly UserManager<ApplicationUser> userManager;

        public LogoutModelOnGet(SignInManager<ApplicationUser> signInManager
            , ILogger<LogoutModel> logger
            , UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            this.userManager = userManager;
        }

        public async Task<IActionResult> OnGet(string returnUrl = "~/")
        {
            var signedInUser = await userManager.FindByNameAsync(User.Identity.Name);

            await _signInManager.RefreshSignInAsync(signedInUser);
            _logger.LogInformation("User Switched Companies.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage();
            }
        }
    }
}
