using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// The extension methods for <see cref="Color"/>
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Convert the <see cref="Color"/> to hex string.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static String ToHexString(this Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        /// <summary>
        /// Convert the <see cref="Color"/> to RGB string.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static String ToRgbString(Color c)
        {
            return "RGB(" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString() + ")";
        }
    }
}
