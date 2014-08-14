using System;
using System.IO;
using System.Text;
using fBaseXtensions.Game;

namespace fBaseXtensions.Helpers
{
	[Flags]
	public enum LogLevel
	{
		None = 0,
		User = 1,
		LineOfSight = 2,
		Cluster = 4,
		Grouping = 8,
		Movement = 16,
		Ability = 32,
		Target = 64,
		Items = 128,
		OutOfGame = 256,
		OutOfCombat = 512,
		Event = 1024,
		Cache = 2048,
		Bounty = 4096,

		All = User | LineOfSight | Cluster | Grouping | Movement | Ability | Target | Items | OutOfGame | OutOfCombat | Event | Cache | Bounty,
	}
	public static class Logger
	{
		public static readonly log4net.ILog DBLog = Zeta.Common.Logger.GetLoggerInstanceForType();
		public static readonly string LoggingStamp = DateTime.Now.ToString("yyyy-MM-dd hh.mm") + ".txt";

		internal static string LoggingPrefixString
		{
			get
			{

				return FunkyGame.CurrentActorClass + " _ " + FunkyGame.CurrentHeroName;
			}
		}

		internal delegate string GetLogLevelName(object obj);

		internal static readonly string FileNamePrefix = "FunkyLog - ";

		private static string filename = Path.Combine(FolderPaths.DemonBuddyPath, "Logs", FileNamePrefix + LoggingStamp);
		public static string FunkyLogFilename
		{
			get { return filename; }
			set { filename = value; }
		}
		public static void Write(LogLevel level, string Message, bool WriteToMainLog, params object[] args)
		{
			if (!LogLevelEnabled(level)) return;
			string prefix = "[" + DateTime.Now.ToString("hh:mm:ss.fff") + " " + level.ToString() + "]";
			string message = String.Format(Message, args);
			WriteLine(String.Format("{0} {1}", prefix, message), true);
			if (WriteToMainLog)
				DBLog.InfoFormat("{0} {1}", "[Funky]", message);
		}
		public static void Write(LogLevel level, string Message, params object[] args)
		{
			if (!LogLevelEnabled(level)) return;
			string prefix = "[" + DateTime.Now.ToString("hh:mm:ss.fff") + " " + level.ToString() + "]";
			string message = String.Format(Message, args);
			WriteLine(String.Format("{0} {1}", prefix, message), true);
		}
		public static void Write(string Message, params object[] args)
		{
			string prefix = "[" + DateTime.Now.ToString("hh:mm:ss.fff") + " N]";
			string message = String.Format(Message, args);
			WriteLine(String.Format("{0} {1}", prefix, message), true);
		}
		private static void WriteLine(string text, bool append)
		{
			// open file
			// If an error occurs throw it to the caller.
			try
			{

				StreamWriter Writer = new StreamWriter(FunkyLogFilename, append, Encoding.UTF8);
				if (!String.IsNullOrEmpty(text)) Writer.WriteLine(text);
				Writer.Flush();
				Writer.Close();
			}
			catch
			{
				throw;
			}
		}
		private static bool LogLevelEnabled(LogLevel level)
		{
			return (FunkyBaseExtension.Settings.Logging.LogFlags & level) != 0;
		}
	}
}
