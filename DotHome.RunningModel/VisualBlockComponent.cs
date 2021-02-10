﻿using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.RunningModel
{
    public class VisualBlockComponent<T> : ComponentBase where T : AVisualBlock
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