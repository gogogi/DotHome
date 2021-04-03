using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.RunningModel
{
    /// <summary>
    /// The Blazor framework component that represents the actual user control attached to a <see cref="VisualBlock"/> <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class VisualBlockComponent<T> : ComponentBase where T : VisualBlock
    {
        [Parameter]
        public T Block { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Block.VisualStateChanged += async () => await InvokeAsync(() => StateHasChanged());
        }
    }
}
