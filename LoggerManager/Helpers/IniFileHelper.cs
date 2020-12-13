using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Schema.Generation;
using System.Collections;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// The helper for the initial files
    /// </summary>
    public static class IniFileHelper 
    {
        //Win32 dll
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileSection", SetLastError = true)]
        private static extern uint GetPrivateProfileSection(string section, IntPtr retVal, uint size, string filePath);

        /// <summary>
        /// The filename of the file
        /// </summary>
        public static string Filename { get; set; }

        /// <summary>
        /// Set the value to the key.
        /// </summary>
        /// <param name="IN_Section">Section。</param>
        /// <param name="IN_Key">Key。</param>
        /// <param name="IN_Value">Value。</param>
        public static void setKeyValue(string IN_Section, string IN_Key, string IN_Value)
        {
            WritePrivateProfileString(IN_Section, IN_Key, IN_Value, Filename);
        }

        /// <summary>
        /// Get the value for the key.
        /// </summary>
        /// <param name="IN_Section">Section。</param>
        /// <param name="IN_Key">Key。</param>
        public static string getKeyValue(string IN_Section, string IN_Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(IN_Section, IN_Key, "", temp, 255, Filename);
            return temp.ToString();
        }

        /// <summary>
        /// Get the value for the key. Return a specified default value if the value doesn't exist.
        /// </summary>
        /// <param name="Section">Section。</param>
        /// <param name="Key">Key。</param>
        /// <param name="DefaultValue">DefaultValue。</param>
        public static string getKeyValue(string Section, string Key, string DefaultValue)
        {
            StringBuilder sbResult = null;

            try
            {
                sbResult = new StringBuilder(255);

                GetPrivateProfileString(Section, Key, "", sbResult, 255, Filename);

                return (sbResult.Length > 0) ? sbResult.ToString() : DefaultValue;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get all the key value pairs for a specific section.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        public static string[] GetSection(string section)
        {
            uint MAX_BUFFER = 32767;
            IntPtr pReturnedString = Marshal.AllocCoTaskMem((int)MAX_BUFFER);
            uint bytesReturned = GetPrivateProfileSection(section, pReturnedString, MAX_BUFFER, Filename);
            return IntPtrToStringArray(pReturnedString, bytesReturned);
        }

        //指標資料轉字串陣列
        private static string[] IntPtrToStringArray(IntPtr pReturnedString, uint bytesReturned)
        {
            //use of Substring below removes terminating null for split
            if (bytesReturned == 0)
            {
                Marshal.FreeCoTaskMem(pReturnedString);
                return null;
            }
            string local = Marshal.PtrToStringAnsi(pReturnedString, (int)bytesReturned).ToString();
            Marshal.FreeCoTaskMem(pReturnedString);
            return local.Substring(0, local.Length - 1).Split('\0');
        }


        /// <summary>
        /// Convert a class object to the ini file. <br></br><br></br>
        /// <strong>Does NOT support List type</strong>
        /// </summary>
        /// <param name="classInstance">The class object to be convert. </param>
        /// <returns></returns>
        public static bool ToIniFile<T>(this T classInstance)
            where T : class, new()
        {
            if (classInstance == null) return false;

            bool success = true;

            PropertyInfo[] propertyInfos;

            // Set the name of the section 
            string section = classInstance.GetType().Name;

            // Convert object to the ini file
            helper(classInstance);

            return success;

            // The helper for the recursive functionality
            void helper(object subject, string prepend = "")
            {
                // Get all properties of the subject
                propertyInfos = subject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                // Loop through all the properties
                foreach (var p in propertyInfos)
                {
                    // If this property contains the skip attribute
                    if (Attribute.IsDefined(p, typeof(SkipAttribute)))
                        // Then skip it.
                        continue;

                    // If this property contains the NewSheet attributes..
                    if (Attribute.IsDefined(p, typeof(NewSheetAttribute)))
                    {
                        // Switch the section to current property name
                        section = p.Name;
                        // Recursively log the result 
                        helper(p.GetValue(subject));
                    }
                    // Handle List Type
                    else if (p.PropertyType.IsGenericType || p.PropertyType is IList)
                    {
                        // Doesn't support list type
                        success = false;
                        return;
                    }
                    // Handle none-system types
                    else if (p.PropertyType.Namespace != "System")
                    {
                        // Recursively log the result 
                        helper(p.GetValue(subject), prepend + p.Name + ".");
                    }
                    // If it's not nested with other properties, add the value
                    else
                    {
                        var val = p.GetValue(subject);
                        if(val == null) setKeyValue(section, prepend + p.Name, "null");
                        else setKeyValue(section, prepend + p.Name, Convert.ToString(val));

                    }
                }

                // Return the current col and row
                return;
            }
        }

        /// <summary>
        /// Convert a class object from the ini file. <br></br><br></br>
        /// <strong>Does NOT support List type</strong>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="classInstance"></param>
        /// <returns></returns>
        public static bool FromIniFile<T>(this T classInstance)

            where T : class, new()
        {
            PropertyInfo[] propertyInfos;

            bool success = true;

            // Set the name of the section 
            string section = classInstance.GetType().Name;

            // Convert object to the ini file
            helper(classInstance);

            return success;

            // The helper for the recursive functionality
            object helper(object subject, string prepend = "")
            {
                // Get all properties of the subject
                propertyInfos = subject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                // Loop through all the properties
                foreach (var p in propertyInfos)
                {
                    // If this property contains the skip attribute
                    if (Attribute.IsDefined(p, typeof(SkipAttribute)))
                        // Then skip it.
                        continue;

                    // If this property contains the NewSheet attributes..
                    if (Attribute.IsDefined(p, typeof(NewSheetAttribute)))
                    {
                        // Switch the section to current property name
                        section = p.Name;
                        // Recursively log the result 
                        helper(p.GetValue(subject));
                    }
                    // Handle List Type
                    else if (p.PropertyType.IsGenericType || p.PropertyType is IList)
                    {
                        // Doesn't support list type
                        success = false;
                        return subject;
                    }
                    // Handle none-system types
                    else if (p.PropertyType.Namespace != "System")
                    {
                        // Recursively log the result 
                        object obj = helper(p.GetValue(subject), prepend + p.Name + ".");
                        if (p.PropertyType.IsValueType) p.SetValue(subject, obj);
                    }
                    // If it's not nested with other properties, set the value
                    else
                    {
                        try
                        {
                            var value = getKeyValue(section, prepend + p.Name);

                            IoC.Logger.Log($"Subject = {subject}, Property = {p.Name}, Type = {p.PropertyType}, Set to {value}");
                            p.SetValue(subject, Convert.ChangeType(value, p.PropertyType));                           

                        }
                        catch(Exception ex)
                        {
                            IoC.Logger.Log(ex.Message);
                        }
                    }
                }

                return subject;
            }
        }
    }
}