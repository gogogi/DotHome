using DotHome.Core.Services;
using DotHome.RunningModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotHome.Core.Pages
{
    public partial class CategoryPage
    {
        [Parameter]
        public string CategoryName { get; set; }

        private string username;
        private Category category;
        private List<VisualBlock> visualBlocks;

        [Inject]
        public IProgramCore ProgramCore { get; set; }

        [Inject]
        public IHttpContextAccessor HttpContextAccessor { get; set; }

        [Inject]
        public PageReloader PageReloader { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            category = ProgramCore.Categories.SingleOrDefault(c => c.Name == CategoryName);
            string username = HttpContextAccessor.HttpContext.User.Identity.Name;
            User user = ProgramCore.Users.SingleOrDefault(u => u.Name == username);
            if (category != null && user != null)
            {
                visualBlocks = ProgramCore.VisualBlocks.Where(b => b.AllowedUsers.Contains(user) && b.Category == category).ToList();
            }
            else
            {
                visualBlocks = null;
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            PageReloader.ReloadForced += () =>
            {
                if (!ProgramCore.Users.Any(u => u.Name == username))
                {
                    NavigationManager.NavigateTo("/logout", true);
                }
                else
                {
                    NavigationManager.NavigateTo(NavigationManager.Uri, true);
                }
            };
        }
    }
}
