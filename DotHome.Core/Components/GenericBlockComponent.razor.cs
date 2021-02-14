using DotHome.RunningModel;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DotHome.Core.Components
{
    public partial class GenericBlockComponent
    {
        [Parameter]
        public AVisualBlock Block { get; set; }

        public RenderFragment Renderer => ((builder) =>
        {
            Type type = Block.GetType();
            Type componentType = type.Assembly.GetTypes().Where(t => typeof(VisualBlockComponent<>).MakeGenericType(type).IsAssignableFrom(t)).FirstOrDefault();
            if (componentType != null)
            {
                builder.OpenComponent(0, componentType);
                builder.AddAttribute(0, "Block", Block);
                builder.CloseComponent();
            }
            else if (type.IsGenericType)
            {
                Type argumentType = type.GetGenericArguments().First();
                Type genericComponentType = type.Assembly.GetTypes().Where(t => t.IsGenericType && typeof(VisualBlockComponent<>).MakeGenericType(type).IsAssignableFrom(t.MakeGenericType(argumentType))).FirstOrDefault();
                if (genericComponentType != null)
                {
                    builder.OpenComponent(0, genericComponentType.MakeGenericType(argumentType));
                    builder.AddAttribute(0, "Block", Block);
                    builder.CloseComponent();
                }
            }
        });
    }
}
