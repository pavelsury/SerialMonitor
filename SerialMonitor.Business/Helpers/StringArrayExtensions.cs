using System;
using System.Linq;

namespace SerialMonitor.Business.Helpers
{
    public static class StringArrayExtensions
    {
        public static string GetOptionalArgument(this string[] args, string argumentName)
        {
            var foundArgs = args.Where(a => a.StartsWith($"-{argumentName}{Separator}", StringComparison.InvariantCulture));
            var arg = foundArgs.FirstOrDefault();
            var separatorIndex = arg?.IndexOf(Separator);
            return separatorIndex != null ? arg.Substring(separatorIndex.Value + 1) : null;
        }

        public static string GetMandatoryArgument(this string[] args, string argumentName)
        {
            return GetOptionalArgument(args, argumentName);
        }

        private const char Separator = '=';
    }
}