using System;
using System.Collections.Generic;
using System.Text;

namespace Spreader
{
    class Utilities
    {
        public const int WorkerPingLimit = 180000;
        public const int WorkerTimeoutLimit = 600000;

        public enum SpreaderLogLevel
        {
            SPREADER_LOG_DEBUG = 0,
            SPREADER_LOG_MESSAGE = 1,
            SPREADER_LOG_WARNING = 2,
            SPREADER_LOG_ERROR = 3,
            SPREADER_LOG_FATAL = 4
        }

        public static string EncodeParameters(string DecodedData)
        {
            return DecodedData.Replace("|", "&#124;");
        }

        public static string DecodeParameters(string EncodedData)
        {
            return EncodedData.Replace("&#124;", "|");
        }
    }
}
