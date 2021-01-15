using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.ProgrammingModel
{
    [Serializable]
    public abstract class ABlock
    {
        public int Id { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public virtual IInput[] GetInputs() => new IInput[0];

        public virtual IOutput[] GetOutputs() => new IOutput[0];
    }
}