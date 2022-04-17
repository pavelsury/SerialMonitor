using System;
using System.Collections.Generic;

namespace SerialMonitor.Business
{
    public class CommandVariablesResolver
    {
        public string Resolve(string text)
        {
            if (!ContainsVariableDelimiter(text))
            {
                return text;
            }

            var utcNow = DateTime.UtcNow;
            var localNow = utcNow.ToLocalTime();

            foreach (var (localTimeName, utcTimeName, resolver) in _timeResolvers)
            {
                text = text.Replace($"%{localTimeName}%", resolver(localNow));
                
                if (!ContainsVariableDelimiter(text))
                {
                    return text;
                }

                text = text.Replace($"%{utcTimeName}%", resolver(utcNow));
                
                if (!ContainsVariableDelimiter(text))
                {
                    return text;
                }
            }

            return text;
        }

        private static bool ContainsVariableDelimiter(string text) => text.Contains("%");

        private readonly List<(string localTimeName, string utcTimeName, Func<DateTime, string> resolver)> _timeResolvers = new List<(string, string, Func<DateTime, string>)>
        {
            ("NOW", "UTC_NOW", d => d.ToString("s", System.Globalization.CultureInfo.InvariantCulture)),
            ("NOW_DATE", "UTC_NOW_DATE",  d => d.ToString("s", System.Globalization.CultureInfo.InvariantCulture).Split('T')[0]),
            ("NOW_TIME", "UTC_NOW_TIME", d => d.ToString("T", System.Globalization.CultureInfo.InvariantCulture)),
            ("NOW_YEAR", "UTC_NOW_YEAR", d => d.Year.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            ("NOW_MONTH", "UTC_NOW_MONTH", d => d.Month.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            ("NOW_DAY", "UTC_NOW_DAY", d => d.Day.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            ("NOW_HOUR", "UTC_NOW_HOUR", d => d.Hour.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            ("NOW_MINUTE", "UTC_NOW_MINUTE", d => d.Minute.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            ("NOW_SECOND", "UTC_NOW_SECOND", d => d.Second.ToString(System.Globalization.CultureInfo.InvariantCulture))
        };
    }
}
