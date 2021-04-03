using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.RunningModel
{
    /// <summary>
    /// Service that can be injected to <see cref="Block"/> as parameter in (single) constructor, is created as singleton for entire program lifetime.
    /// Any class can be injected to <see cref="Block"/>, but if it implements <see cref="IBlockService"/>, then <see cref="Init"/> and <see cref="Run"/> methods are called
    /// </summary>
    public interface IBlockService
    {
        /// <summary>
        /// Is called only once at start of program
        /// </summary>
        void Init();

        /// <summary>
        /// Is called in every program loop
        /// </summary>
        void Run();
    }
}
