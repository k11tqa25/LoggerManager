using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// The attribute that defines the style of a cell.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class ConditionCellStyleAttribute: Attribute
    {
        private string booleanproperty;
        private string stylenameTrue;
        private string stylenameFalse;

        /// <summary>
        /// The boolean property that will then determine the style to use.
        /// </summary>
        public virtual string BooleanProperty
        {
            get => booleanproperty;
            set => booleanproperty = value;
        }

        /// <summary>
        /// The name of the style
        /// </summary>
        public virtual string StyleNameIfTrue
        {
            get => stylenameTrue;
            set => stylenameTrue = value;
        }

        /// <summary>
        /// The name of the style
        /// </summary>
        public virtual string StyleNameIfFalse
        {
            get => stylenameFalse;
            set => stylenameFalse = value;
        }

        /// <summary>
        /// Set a condition to a cell according to the specified property name.
        /// </summary>
        /// <param name="booleanPropertyName">The name of a boolean property  That will then determines the style to use. Use <paramref name="styleIfTrue"/> if True; else <paramref name="styleIfFalse"/></param>
        /// <param name="styleIfTrue"></param>
        /// <param name="styleIfFalse"></param>
        public ConditionCellStyleAttribute(string booleanPropertyName, string styleIfTrue, string styleIfFalse = "")
        {
            BooleanProperty = booleanPropertyName;
            StyleNameIfTrue = styleIfTrue;
            StyleNameIfFalse = styleIfFalse;
        }
    }
}
