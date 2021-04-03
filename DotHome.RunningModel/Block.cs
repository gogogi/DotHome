using System;
using System.Collections.Generic;
using System.Drawing;

namespace DotHome.RunningModel
{
    /// <summary>
    /// Organization unit of DotHome project. Every block must inherit this class
    /// </summary>
    public abstract class Block
    {
        private Color debugColor = Color.Black;

        /// <summary>
        /// Id which is responsible for attachig programming model block to running model block during debug
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Debug string with escaped color code to show in Config when in debug mode
        /// </summary>
        public string DebugString { get; private set; } = "";

        /// <summary>
        /// Incoming values to this block
        /// </summary>
        public List<BlockValue> Inputs { get; } = new List<BlockValue>();

        /// <summary>
        /// Outcoming values from this block
        /// </summary>
        public List<BlockValue> Outputs { get; } = new List<BlockValue>();

        /// <summary>
        /// This method is run only once during initialization of the program. Must be overriden in every derived class.
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// This method is being run in loop during exetution of the program. Must be overriden in every derived class.
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// Writes debug string terminated with '\n' to debug channel, to be displayed via Config. Should be called from <see cref="Run"/>.
        /// </summary>
        /// <param name="parameter">Value to be printed</param>
        /// <param name="color">Color to use</param>
        protected void DebugWriteLine(object parameter, Color color)
        {
            DebugWrite(parameter.ToString() + "\n", color);
        }

        /// <summary>
        /// Writes debug string terminated with '\n' to debug channel, to be displayed via Config. Should be called from <see cref="Run"/>.
        /// </summary>
        /// <param name="parameter">Value to be printed</param>
        protected void DebugWriteLine(object parameter)
        {
            DebugWriteLine(parameter, Color.Black);
        }

        /// <summary>
        /// Writes debug string to debug channel, to be displayed via Config. Should be called from <see cref="Run"/>.
        /// </summary>
        /// <param name="parameter">Value to be printed</param>
        /// <param name="color">Color to use</param>
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

        /// <summary>
        /// Writes debug string to debug channel, to be displayed via Config. Should be called from <see cref="Run"/>.
        /// </summary>
        /// <param name="parameter">Value to be printed</param>
        protected void DebugWrite(object parameter)
        {
            DebugWrite(parameter, Color.Black);
        }

        /// <summary>
        /// Is called after each program loop to reset the debug string and color
        /// </summary>
        public void ClearDebugString()
        {
            DebugString = "";
            debugColor = Color.Black;
        }

        /// <summary>
        /// Creates escape sequence \esc+'c'+r+g+b to specify color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private string ColorToEscapeString(Color color)
        {
            return "\u001bc" + (char)color.R + (char)color.G + (char)color.B;
        }
    }
}
