using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicoPay.Web.Helpers
{
    public static class TicoPayStringIndexOfExtension
    {
        public static List<int> AllIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }
    }
}