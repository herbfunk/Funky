using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FunkyBot.Misc
{
	 [Flags]
	 public enum LogLevel
	 {
		  None=0,
		  User=1,
		  LineOfSight=2,
		  Cluster=4,
		  Grouping=8,
		  Movement=16,
		  Ability=32,
		  Target=64,
		  Items=128,
		  OutOfGame=256,
		  OutOfCombat=512,
		  Event=1024,
		  Cache=2048,
		  Bounty=4096,

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
				 return Bot.Character.Account.ActorClass.ToString() + " _ " + Bot.Character.Account.CurrentHeroName;
			 }
		 }

		  internal static void CleanLogs()
		  {
				List<string> deleteList=new List<string>();
				if (string.IsNullOrEmpty(FolderPaths.DemonBuddyPath))
				{
					DBLog.Info("Failure to reconigze demon buddy path!");

				}
				else
				{
					 foreach (string file in Directory.GetFiles(FolderPaths.DemonBuddyPath+@"\Logs\"))
					 {
						  DateTime curFileCreated=Directory.GetCreationTime(file);
						  if (DateTime.Now.Subtract(curFileCreated).TotalHours>=24)
						  {
								deleteList.Add(file);
						  }
					 }

					 if (deleteList.Count>0)
					 {
						  foreach (string item in deleteList)
						  {
								File.Delete(item);
						  }
						  DBLog.DebugFormat("Total DB logs deleted "+deleteList.Count);
					 }
				}

				string ItemRulesPath=@"\Plugins\FunkyBot\ItemRules\Log\Archive\";
				deleteList=new List<string>();
				try
				{
					 foreach (string file in Directory.GetFiles(FolderPaths.DemonBuddyPath+ItemRulesPath))
					 {
						  DateTime curFileCreated=Directory.GetCreationTime(file);
						  if (DateTime.Now.Subtract(curFileCreated).TotalHours>=24)
						  {
								deleteList.Add(file);
						  }
					 }

					 if (deleteList.Count>0)
					 {
						  foreach (string item in deleteList)
						  {
								File.Delete(item);
						  }

						  DBLog.DebugFormat("Total item rule logs deleted " + deleteList.Count);
					 }

				} catch { DBLog.DebugFormat("Failure to clean log files @ path: "+ItemRulesPath); }


				deleteList=new List<string>();
				try
				{
					foreach (string file in Directory.GetFiles(FolderPaths.ProfileStatsPath))
					 {
						  DateTime curFileCreated=Directory.GetCreationTime(file);
						  if (DateTime.Now.Subtract(curFileCreated).TotalDays>=1)
						  {
								deleteList.Add(file);
						  }
					 }

					 if (deleteList.Count>0)
					 {
						  foreach (string item in deleteList)
						  {
								File.Delete(item);
						  }
						  DBLog.DebugFormat("Total game stat logs deleted "+deleteList.Count);
					 }

				}
				catch { DBLog.DebugFormat("Failure to clean log files @ path: " + FolderPaths.ProfileStatsPath); }

		  }

		  internal delegate string GetLogLevelName(object obj);

		  internal static readonly string FileNamePrefix="FunkyLog - ";

		  private static string filename = Path.Combine(FolderPaths.DemonBuddyPath, "Logs", FileNamePrefix + LoggingStamp);
		  public static string FunkyLogFilename
		  {
				get { return filename; }
				set { filename=value; }
		  }
		  public static void Write(LogLevel level, string Message,bool WriteToMainLog, params object[] args)
		  {
			  if (!LogLevelEnabled(level)) return;
				string prefix="["+DateTime.Now.ToString("hh:mm:ss.fff")+" "+level.ToString()+"]";
				string message=String.Format(Message, args);
				WriteLine(String.Format("{0} {1}", prefix, message), true);
				if (WriteToMainLog)
					DBLog.InfoFormat("{0} {1}", "[Funky]", message);
		  }
		  public static void Write(LogLevel level, string Message, params object[] args)
		  {
			  if (!LogLevelEnabled(level)) return;
				string prefix="["+DateTime.Now.ToString("hh:mm:ss.fff")+" "+level.ToString()+"]";
				string message=String.Format(Message, args);
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

					 StreamWriter Writer=new StreamWriter(FunkyLogFilename, append, Encoding.UTF8);
					 if (!String.IsNullOrEmpty(text)) Writer.WriteLine(text);
					 Writer.Flush();
					 Writer.Close();
				} catch
				{
					 throw;
				}
		  }
		 private static bool LogLevelEnabled(LogLevel level)
		  {
			  return (Bot.Settings.Debug.FunkyLogFlags & level) != 0;
		  }
	 }
}
