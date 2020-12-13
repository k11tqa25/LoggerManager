using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// The attribute that defines the style of the cell. <br></br><br></br>
    /// <strong>NOTE: This will override the <see cref="ConditionCellStyleAttribute"/></strong>
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class ValueCellStyle: Attribute
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
        /// Define the style of the value cell
        /// </summary>
        /// <param name="styleName">The name of the style. </param>
        public ValueCellStyle(string styleName)
        {
            StyleName = styleName;
        }
    }
}
