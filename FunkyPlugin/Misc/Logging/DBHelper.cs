using System;
using Zeta.Common;
using System.Windows.Media;


namespace FunkyTrinity
{
    public partial class Funky
    {
        /// <summary>
        /// Utilities help developer interact with DemonBuddy
        /// </summary>
        internal static class DbHelper
        {
            /// <summary>
            /// Enumerate severity level for logging 
            /// </summary>
            internal enum TrinityLogLevel
            {
                Critical = 5,
                Error = 4,
                Normal = 3,
                Verbose = 2,
                Debug = 1
            }

            /// <summary>
            /// Enumerate all log categories
            /// </summary>
            [Flags]
            public enum LogCategory
            {
                UserInformation = 0,
                ItemValuation = 1,
                CacheManagement = 2,
                ScriptRule = 4,
                Configuration = 8,
                UI = 16,
                WeaponSwap = 32,
                Behavior = 64,
                Performance = 128,
                Targetting = 256,
                Weight = 512,
                XmlTag = 1024,
                Moving = 2048,
                GlobalHandler = 4096
            }

            /// <summary>Logs the specified level.</summary>
            /// <param name="level">The logging level.</param>
            /// <param name="category">The category.</param>
            /// <param name="formatMessage">The format message.</param>
            /// <param name="args">The parameters used when format message.</param>
            public static void Log(TrinityLogLevel level, LogCategory category, string formatMessage, params object[] args)
            {
                if (category == LogCategory.UserInformation || level >= TrinityLogLevel.Error)
                {
						  string msg=string.Format("[Funky]{0}{1}", category!=LogCategory.UserInformation?"["+category.ToString()+"]":string.Empty, formatMessage);
                    if (level == TrinityLogLevel.Critical)
                    {
                        Logging.Write(ConvertToLogLevel(level), Colors.Red, msg, args);
                    }
                    else
                    {
                        Logging.Write(ConvertToLogLevel(level), msg, args);
                    }
                }
            }

            /// <summary>Logs the specified level.</summary>
            /// <param name="category">The category.</param>
            /// <param name="formatMessage">The format message.</param>
            /// <param name="args">The parameters used when format message.</param>
            public static void Log(LogCategory category, string formatMessage, params object[] args)
            {
                Log(TrinityLogLevel.Debug, category, formatMessage, args);
            }

            /// <summary>Converts <see cref="TrinityLogLevel"/> to <see cref="LogLevel"/>.</summary>
            /// <param name="level">The trinity logging level.</param>
            /// <returns>DemonBuddy logging level.</returns>
            private static LogLevel ConvertToLogLevel(TrinityLogLevel level)
            {
                LogLevel logLevel = LogLevel.Diagnostic;
                switch (level)
                {
                    case TrinityLogLevel.Critical:
                        logLevel = LogLevel.None;
                        break;
                    case TrinityLogLevel.Error:
                        logLevel = LogLevel.Quiet;
                        break;
                    case TrinityLogLevel.Normal:
                        logLevel = LogLevel.Normal;
                        break;
                    case TrinityLogLevel.Verbose:
                        logLevel = LogLevel.Verbose;
                        break;
                    case TrinityLogLevel.Debug:
                        logLevel = LogLevel.Diagnostic;
                        break;
                }
                return logLevel;
            }
        }
    }
}