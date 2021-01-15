using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.ProgrammingModel
{
    [Serializable]
    public class Wire
    {
        public IInput Input { get; set; }

        public IOutput Output { get; set; }
    }
}
