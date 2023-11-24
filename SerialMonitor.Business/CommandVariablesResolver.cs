using SerialMonitor.Business.Data;
using SerialMonitor.Business.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SerialMonitor.Business
{
    public class CommandVariablesResolver
    {
        public CommandVariablesResolver(IEndiannessProvider endiannessProvider, IReadOnlyList<CustomCommandVariable> customVariables)
        {
            _dataVariableResolver = new DataVariablesResolver(endiannessProvider);
            _otherVariablesMapping.Add("EOF", $"{_dataVariableResolver.MakeVar(@"4")}{MakeVar(_eolSkipVariableName)}");
            _customVariables = customVariables;
        }

        public string ResolveTextVariables(string text)
        {
            text = ResolveCustomVariables(text);

            if (!ContainsVariableDelimiter(text))
            {
                return text;
            }

            text = ResolveTimeVariables(text);
            if (!ContainsVariableDelimiter(text))
            {
                return text;
            }

            return ResolveOtherTextVariables(text);
        }

        public bool IsEolOverridden(string text)
        {
            if (!ContainsVariableDelimiter(text))
            {
                return false;
            }

            return _eolMapping.Any(p => text.EndsWith(MakeVar(p.Key), StringComparison));
        }

        public string ResolveEolVariables(string text)
        {
            if (!ContainsVariableDelimiter(text))
            {
                return text;
            }

            _eolMapping.ForEach(p => text = text.Replace(MakeVar(p.Key), p.Value, StringComparison));
            return text;
        }

        public List<(string token, byte[] tokenBytes)> ResolveDataVariables(string text)
        {
            var option = AppSettings.IsVariableCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
            var pattern = $@"{DataVariablesResolver.StartDelimiter}[^{AppSettings.VariableEndDelimiter}]*{AppSettings.VariableEndDelimiter}";
            var matches = new Regex(pattern, option)
                .Matches(text)
                .Cast<Match>();

            int startIndex = 0;
            var resultList = new List<(string, byte[])>();

            foreach (var match in matches)
            {
                var length = match.Index - startIndex;
                if (length > 0)
                {
                    resultList.Add((text.Substring(startIndex, length), null));
                }

                resultList.Add((match.Value, _dataVariableResolver.Resolve(match.Value)));
                startIndex = match.Index + match.Length;
            }

            if (startIndex < text.Length)
            {
                resultList.Add((text.Substring(startIndex), null));
            }

            return resultList;
		}

        private static bool ContainsVariableDelimiter(string text) => text.Contains(AppSettings.VariableStartDelimiter);

        private static string MakeVar(string varName) => $"{AppSettings.VariableStartDelimiter}{varName}{AppSettings.VariableEndDelimiter}";

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

        private static StringComparison StringComparison => AppSettings.IsVariableCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

        private string ResolveCustomVariables(string text)
        {
            var oldText = text;

            while (true)
            {
                _customVariables
                    .Where(v => !string.IsNullOrEmpty(v.CommandVariable))
                    .ForEach(v => text = text.Replace(v.CommandVariable, v.Content ?? string.Empty, StringComparison));
                
                if (text == oldText)
                {
                    break;
                }
                oldText = text;
            }

            return text;
        }

        private string ResolveTimeVariables(string text)
        {
            var localNow = DateTime.Now;
            var winterNow = GetWinterTime(localNow);
            var utcNow = localNow.ToUniversalTime();

            foreach (var (localTimeName, winterTimeName, utcTimeName, resolver) in _timeResolvers)
            {
                text = text.Replace(MakeVar(localTimeName), resolver(localNow), StringComparison);
                if (!ContainsVariableDelimiter(text))
                {
                    return text;
                }

                text = text.Replace(MakeVar(winterTimeName), resolver(winterNow), StringComparison);
                if (!ContainsVariableDelimiter(text))
                {
                    return text;
                }

                text = text.Replace(MakeVar(utcTimeName), resolver(utcNow), StringComparison);
                if (!ContainsVariableDelimiter(text))
                {
                    return text;
                }
            }

            return text;
        }

        private string ResolveOtherTextVariables(string text)
        {
            _otherVariablesMapping.ForEach(p => text = text.Replace(MakeVar(p.Key), p.Value, StringComparison));
            return text;
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

        private const string _eolSkipVariableName = "EOL_SKIP";

        private readonly Dictionary<string, string> _eolMapping = new Dictionary<string, string>
        {
            { _eolSkipVariableName, "" },
            { "EOL_CR", "\r" },
            { "EOL_LF", "\n" },
            { "EOL_CRLF", "\r\n" },
        };

        private readonly Dictionary<string, string> _otherVariablesMapping = new Dictionary<string, string>();

        private readonly DataVariablesResolver _dataVariableResolver;
        private readonly IReadOnlyList<CustomCommandVariable> _customVariables;
    }
}
