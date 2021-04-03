using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.RunningModel
{
    /// <summary>
    /// Type of low-level value. There are more types than in high-level block I/O because of memory and time optimilazition.
    /// </summary>
    public enum DeviceValueType
    {
        /// <summary>
        /// An event was fired. Typical use case is when a pressed button. low-level implementation is bool. high-level implementation is <see cref="bool"/>
        /// </summary>
        Pulse,

        /// <summary>
        /// A true/false value. low-level implementation is bool. high-level implementation is <see cref="bool"/>
        /// </summary>
        Bool,

        /// <summary>
        /// Value 0 to 255. low-level implementation is uint8_t. high-level implementation is <see cref="uint"/>
        /// </summary>
        Uint8,

        /// <summary>
        /// Value 0 to 65535. low-level implementation is uint16-t. high-level implementation is <see cref="uint"/>
        /// </summary>
        Uint16,

        /// <summary>
        /// Value 0 to 4294267296. low-level implementation is uint32-t. high-level implementation is <see cref="uint"/>
        /// </summary>
        Uint32,

        /// <summary>
        /// Value -256 to 255. low-level implementation is int8_t. high-level implementation is <see cref="int"/>
        /// </summary>
        Int8,

        /// <summary>
        /// Value -32768 to 32767. low-level implementation is int16_t. high-level implementation is <see cref="int"/>
        /// </summary>
        Int16,

        /// <summary>
        /// Value -2147483648 to 2147483647. low-level implementation is int32_t. high-level implementation is <see cref="int"/>
        /// </summary>
        Int32,

        /// <summary>
        /// Value -327.68 to 327.67. low-level implementation is int16_t with value multiplied by 100. high-level implementation is <see cref="float"/>
        /// </summary>
        Float2,

        /// <summary>
        /// Value -3.2768 to 3.2767. low-level implementation is int16_t with value multiplied by 10000. high-level implementation is <see cref="float"/>
        /// </summary>
        Float4,

        /// <summary>
        /// True 4-byte IEEE 754 little endian floating-point value. low-level implementation is float. high-level implementation is <see cref="float"/>
        /// </summary>
        Float,

        /// <summary>
        /// String value. low-level implementation is uint16_t as size and char[]. high-level implementation is <see cref="string"/>
        /// </summary>
        String,

        /// <summary>
        /// Byte array. low-level implementation is uint16_t as size and uin8_t[]. high-level implementation is <see cref="byte[]"/>
        /// </summary>
        Binary
    }
}
