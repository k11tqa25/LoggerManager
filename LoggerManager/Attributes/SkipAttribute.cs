using System;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// Apply this attribute if you don't want certain property to be documented.<br></br><br></br>
    /// <strong>This doesn't work on JSON logger.</strong>
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class SkipAttribute: Attribute
    {
    }
}
