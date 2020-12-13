using System;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// The attribute that helps to add a new sheet in a dataset from a class property
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class NewSheetAttribute: Attribute
    {
    }
}
