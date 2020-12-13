using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// The attribute that defines a style for the ODS file.
    /// </summary>

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class DefineStyleAttribute: Attribute
    {
        private string stylename;
        private string foregroundColor;
        private string backgroundColor;

        /// <summary>
        /// The name of the style
        /// </summary>
        public virtual string StyleName
        {
            get => stylename;
            set => stylename = value;
        }
        /// <summary>
        /// The foreground color of the cell. <br></br>
        /// Use nameof(<see cref="Color"/>) for the string input.
        /// </summary>
        public virtual string ForegroundColor
        {
            get => foregroundColor;
            set => foregroundColor = value;
        }

        /// <summary>
        /// The background color of the cell. <br></br>
        /// Use nameof(<see cref="Color"/>) for the string input.
        /// </summary>
        public virtual string BackgroundColor
        {
            get => backgroundColor;
            set => backgroundColor = value;
        }


        /// <summary>
        /// Define the style of the header
        /// </summary>
        /// <param name="styleName">The name of the style. </param>
        /// <param name="foreground">
        /// The foreground color of the cell. <br></br>
        /// Use nameof(<see cref="Color"/>) for the string input.
        /// </param>
        /// <param name="background">
        /// The background color of the cell. <br></br>
        /// Use nameof(<see cref="Color"/>) for the string input.
        /// </param>
        public DefineStyleAttribute(string styleName, string foreground = "", string background = "" )
        {
            StyleName = styleName;
            ForegroundColor = foreground;
            BackgroundColor = background;
        }
    }
}
