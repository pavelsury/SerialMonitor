using SerialMonitor.Business.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SerialMonitor.Business
{
    public class CommandVariablesResolver
    {
        public string ResolveTextVariables(string text)
        {
            if (!ContainsVariableDelimiter(text))
            {
                return text;
            }

            var localNow = DateTime.Now;
            var winterNow = GetWinterTime(localNow);
            var utcNow = localNow.ToUniversalTime();

            foreach (var (localTimeName, winterTimeName, utcTimeName, resolver) in _timeResolvers)
            {
                text = text.Replace(MakeVar(localTimeName), resolver(localNow));
                if (!ContainsVariableDelimiter(text))
                {
                    return text;
                }

                text = text.Replace(MakeVar(winterTimeName), resolver(winterNow));
                if (!ContainsVariableDelimiter(text))
                {
                    return text;
                }

                text = text.Replace(MakeVar(utcTimeName), resolver(utcNow));
                if (!ContainsVariableDelimiter(text))
                {
                    return text;
                }
            }

            return text;
        }

        private static bool ContainsVariableDelimiter(string text) => text.Contains('%');

        private static string MakeVar(string varName) => $"%{varName}%";

        public bool IsEolOverridden(string text)
        {
            if (!ContainsVariableDelimiter(text))
            {
                return false;
            }

            return _eolMapping.Any(p => text.EndsWith(MakeVar(p.Key)));
        }

        public string ResolveEolVariables(string text)
        {
            if (!ContainsVariableDelimiter(text))
            {
                return text;
            }

            _eolMapping.ForEach(p => text = text.Replace(MakeVar(p.Key), p.Value));
            return text;
        }

        private static DateTime GetWinterTime(DateTime localNow)
        {
            if (!localNow.IsDaylightSavingTime())
            {
                return localNow;
            }

            var today = localNow.Date;
            var rule = TimeZoneInfo.Local.GetAdjustmentRules().First(x => today >= x.DateStart && today <= x.DateEnd);
            return localNow - rule.DaylightDelta;
        }

        private readonly List<(string localTimeName, string winterTimeName, string utcTimeName, Func<DateTime, string> resolver)> _timeResolvers = new List<(string, string, string, Func<DateTime, string>)>
        {
            ("NOW", "WINTER_NOW", "UTC_NOW", d => d.ToString("s", System.Globalization.CultureInfo.InvariantCulture)),
            ("NOW_DATE", "WINTER_NOW_DATE", "UTC_NOW_DATE",  d => d.ToString("s", System.Globalization.CultureInfo.InvariantCulture).Split('T')[0]),
            ("NOW_TIME", "WINTER_NOW_TIME", "UTC_NOW_TIME", d => d.ToString("T", System.Globalization.CultureInfo.InvariantCulture)),
            ("NOW_YEAR", "WINTER_NOW_YEAR", "UTC_NOW_YEAR", d => d.Year.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            ("NOW_MONTH", "WINTER_NOW_MONTH", "UTC_NOW_MONTH", d => d.Month.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            ("NOW_DAY", "WINTER_NOW_DAY", "UTC_NOW_DAY", d => d.Day.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            ("NOW_HOUR", "WINTER_NOW_HOUR", "UTC_NOW_HOUR", d => d.Hour.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            ("NOW_MINUTE", "WINTER_NOW_MINUTE", "UTC_NOW_MINUTE", d => d.Minute.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            ("NOW_SECOND", "WINTER_NOW_SECOND", "UTC_NOW_SECOND", d => d.Second.ToString(System.Globalization.CultureInfo.InvariantCulture))
        };

        private Dictionary<string, string> _eolMapping = new Dictionary<string, string> 
        {
            { "EOL_SKIP", "" },
            { "EOL_CR", "\r" },
            { "EOL_LF", "\n" },
            { "EOL_CRLF", "\r\n" },
        };
    }
}
