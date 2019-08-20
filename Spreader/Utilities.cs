using System;
using System.Collections.Generic;
using System.Text;

namespace Spreader
{
    public class Utilities
    {
        public const int WorkerPingLimit = 180000;
        public const int WorkerTimeoutLimit = 600000;
        public const char CommandSeparator = '|';
        public const string CommandSeparatorEncoded = "&#124;";
        public const char InitSeparator = ';';

        public enum SpreaderLogLevel
        {
            LOG_DEBUG = 0,
            LOG_MESSAGE = 1,
            LOG_WARNING = 2,
            LOG_ERROR = 3,
            LOG_FATAL = 4
        }

        public static string EncodeParameters(string DecodedData)
        {
            return DecodedData.Replace(CommandSeparator.ToString(), CommandSeparatorEncoded);
        }

        public static string DecodeParameters(string EncodedData)
        {
            return EncodedData.Replace(CommandSeparatorEncoded, CommandSeparator.ToString());
        }
    }
}
