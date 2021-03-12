using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.StandardBlocks.Controls
{
    [Category("Controls"), Description("Output is HIGH when the button is pressed"), Color("#8040fe")]
    public class Button : VisualBlock
    {
        private bool pressed, released;

        [Description("Output")]
        public Output<bool> O { get; set; }

        public override void Init()
        {

        }

        public override void Run()
        {
            if(pressed)
            {
                pressed = false;
                O.Value = true;
            }
            else if(released)
            {
                released = false;
                O.Value = false;
            }
        }

        internal void Pressed() => pressed = true;

        internal void Released() => released = true;
    }
}
