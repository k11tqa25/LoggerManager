using System.Collections.Generic;

namespace LoggerManagerExample
{
    public static class ListExtensions
    {
        public static string JoinWith<T>(this List<T> list, string deliminator = " ")
        {
            string s = "";

            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    s += list[i].ToString();
                    if (i != list.Count - 1) s += deliminator;
                }
            }
            return s;
        }
    }
}
