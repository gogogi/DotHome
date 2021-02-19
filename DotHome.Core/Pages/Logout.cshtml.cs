using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotHome.Core.Pages
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGet(string returnUrl = null)
        {
            await HttpContext.SignOutAsync();
            return Redirect($"/AfterLogout/{HttpUtility.UrlEncode(returnUrl)}");
        }
    }
}
