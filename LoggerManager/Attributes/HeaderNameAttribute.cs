using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// This is used to change the default name of the header.<br></br>
    /// The default name is the property name itself.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class HeaderNameAttribute: Attribute
    {
        private string headername;

        /// <summary>
        /// The name of the header
        /// </summary>
        public virtual string HeaderName
        {
            get => headername;
            set => headername = value;
        }

        /// <summary>
        /// Rename the header
        /// </summary>
        /// <param name="headerName">The name of the header. </param>
        public HeaderNameAttribute(string headerName )
        {
            HeaderName = headerName;
        }
    }
}
