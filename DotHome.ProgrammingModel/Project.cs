using System;
using System.Collections.Generic;

namespace DotHome.ProgrammingModel
{
    [Serializable]
    public class Project
    {
        public List<Page> Pages { get; set; } = new List<Page>();
    }
}
