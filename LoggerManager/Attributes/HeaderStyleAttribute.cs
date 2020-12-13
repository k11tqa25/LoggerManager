using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// The attribute that defines the style of the header
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class HeaderStyleAttribute: Attribute
    {
        private string stylename;

        /// <summary>
        /// The name of the style
        /// </summary>
        public virtual string StyleName
        {
            get => stylename;
            set => stylename = value;
        }

        /// <summary>
        /// Define the style of the header
        /// </summary>
        /// <param name="styleName">The name of the style. </param>
        public HeaderStyleAttribute(string styleName )
        {
            StyleName = styleName;
        }
    }
}
