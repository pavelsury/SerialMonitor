using System;
using System.Linq;
using System.Text;

namespace SerialMonitor.Business.Helpers
{
    public static class StringExtensions
    {
        public static string RemoveWhitespaces(this string text) => new string(text.Where(c => !char.IsWhiteSpace(c)).ToArray());

        public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
        {
            if (string.IsNullOrEmpty(oldValue)) 
            {
                return oldValue;
            }
            
            StringBuilder sb = new StringBuilder();
            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);

            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }

            sb.Append(str.Substring(previousIndex));
            return sb.ToString();
        }
    }
}
