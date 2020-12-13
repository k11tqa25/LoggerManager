using System;
using System.Collections.Generic;
using System.Text;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// Apply this attribute to the property that you don't want to record its header. <br></br>
    /// Usually this will be apply on a list type.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]

    public class SkipHeaderAttribute : Attribute
    {
    }
}
