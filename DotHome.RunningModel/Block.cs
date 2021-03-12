using System;
using System.Collections.Generic;
using System.Drawing;

namespace DotHome.RunningModel
{
    public abstract class Block
    {
        private Color debugColor = Color.Black;

        public int Id { get; set; }

        public string DebugString { get; private set; } = "";

        public List<BlockValue> Inputs { get; } = new List<BlockValue>();

        public List<BlockValue> Outputs { get; } = new List<BlockValue>();

        public abstract void Init();

        public abstract void Run();

        protected void DebugWriteLine(object parameter, Color color)
        {
            DebugWrite(parameter.ToString() + "\n", color);
        }

        protected void DebugWriteLine(object parameter)
        {
            DebugWriteLine(parameter, Color.Black);
        }

        protected void DebugWrite(object parameter, Color color)
        {
            if(color == debugColor)
            {
                DebugString += parameter.ToString();
            }
            else
            {
                debugColor = color;
                DebugString += ColorToEscapeString(color) + parameter.ToString();
            }
        }

        protected void DebugWrite(object parameter)
        {
            DebugWrite(parameter, Color.Black);
        }

        public void ClearDebugString()
        {
            DebugString = "";
            debugColor = Color.Black;
        }

        private string ColorToEscapeString(Color color)
        {
            return "\u001bc" + (char)color.R + (char)color.G + (char)color.B;
        }
    }
}
