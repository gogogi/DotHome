using DotHome.Core.Services;
using DotHome.Core.Tools;
using DotHome.RunningModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace DotHome.Core.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private IProgramCore programCore;
        private IJSRuntime jsRuntime;
        private NotificationManager notificationManager;

        public LoginModel(IProgramCore programCore, IJSRuntime jsRuntime, NotificationManager notificationManager)
        {
            this.programCore = programCore;
            this.jsRuntime = jsRuntime;
            this.notificationManager = notificationManager;
        }

        [BindProperty]
        [Required]
        public string Username { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                User user = programCore.Users.SingleOrDefault(u => u.Name == Username);
                if(user != null && Password == user.Password)
                {
                    await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, Username)
                    }, "Password")));
                    var u = HttpContext.User;                    
                    return Redirect("/AfterLogin");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid credentials");
                }
            }
            return Page();
        }
    }
}
