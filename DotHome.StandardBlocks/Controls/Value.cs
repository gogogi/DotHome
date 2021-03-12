using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.StandardBlocks.Controls
{
    [Category("Controls"), Description("Displays the value from input"), Color("#8040fe")]
    public class Value<T> : VisualBlock
    {
        internal T Val { get; set; }

        [Description("Input")]
        public Input<T> I { get; set; }

        public override void Init()
        {
            
        }

        public override void Run()
        {
            if(!Equals(Val, I.Value))
            {
                Val = I.Value;
                VisualStateHasChanged();
            }
        }
    }
}
