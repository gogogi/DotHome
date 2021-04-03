using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.RunningModel
{
    /// <summary>
    /// The low-level error received from end device
    /// </summary>
    public enum DeviceValueError
    {
        /// <summary>
        /// No error
        /// </summary>
        Ok, 

        /// <summary>
        /// The value was not yet read
        /// </summary>
        Unset, 

        /// <summary>
        /// Connection error with the sensor chip occured
        /// </summary>
        ErrorConnection, 

        /// <summary>
        /// There was a timeout reading the value
        /// </summary>
        ErrorTimeout
    }
}
