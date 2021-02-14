using DotHome.Core.Services;
using DotHome.RunningModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DotHome.Core.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private IProgramCore programCore;

        public LoginModel(IProgramCore programCore)
        {
            this.programCore = programCore;
        }

        [BindProperty]
        [Required]
        public string Username { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
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
                    return Redirect(returnUrl ?? "/");
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
