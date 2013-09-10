using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FunkyTrinity
{
	 [Flags]
	 public enum LogLevel
	 {
		  None=0,
		  User=1,
		  Execption=2, //doh?
		  Cluster=4,
		  Grouping=8,
		  Movement=16,
		  Ability=32,
		  Target=64,
		  Items=128,
		  OutOfGame=256,
		  OutOfCombat=512,

		  All=User|Execption|Cluster|Grouping|Movement|Ability|Target|Items|OutOfGame|OutOfCombat,
	 }
	 public static class Logger
	 {
		  internal delegate string GetLogLevelName(object obj);

		  private static string[] FilePath=Path.GetFileName(Funky.DBLogFile).Split(Char.Parse(" "));

		  internal static readonly string FileNamePrefix="FunkyLog - ";

		  private static string filename=Path.Combine(Funky.FolderPaths.sDemonBuddyPath, "Logs", FileNamePrefix+FilePath[1]+" "+FilePath[2]);
		  public static string FunkyLogFilename
		  {
				get { return filename; }
				set { filename=value; }
		  }

		  public static void Init()
		  {

		  }
		  public static void Write(LogLevel level, string Message,bool WriteToMainLog, params object[] args)
		  {
				string prefix="["+DateTime.Now.ToString("hh:mm:ss.fff")+" "+level.ToString()+"]";
				string message=String.Format(Message, args);
				WriteLine(String.Format("{0} {1}", prefix, message), true);
				if (WriteToMainLog)
					 Zeta.Common.Logging.Write("{0} {1}","[Funky]", message);
		  }
		  public static void Write(LogLevel level, string Message, params object[] args)
		  {
				string prefix="["+DateTime.Now.ToString("hh:mm:ss.fff")+" "+level.ToString()+"]";
				string message=String.Format(Message, args);
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
	 }
}
