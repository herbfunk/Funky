using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FunkyTrinity
{
	 public enum LogLevel
	 {
		  User=1,
		  Execption=2,
		  Cluster=4,
		  Grouping=8,
		  Movement=16,
		  Ability=32,
		  Target=64,
		  Items=128,
		  OutOfGame=256,
		  OutOfCombat=512,
	 }
	 public static class Logger
	 {
		  internal static readonly string FileNamePrefix="FunkyLog - ";
		  private static string filename=Path.Combine(Funky.FolderPaths.sDemonBuddyPath, "Logs", Logger.FileNamePrefix+DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")+".txt");
		  public static string Filename
		  {
				get { return filename; }
				set { filename=value; }
		  }
		  private static string FolderPath
		  {
				get
				{
					 return Path.Combine(Funky.FolderPaths.sDemonBuddyPath, "Logs");
				}
		  }
		  private static string FilePath
		  {
				get
				{
					 return Path.Combine(FolderPath,Filename);
				}
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
					 StreamWriter Writer=new StreamWriter(FilePath, append, Encoding.UTF8);
					 if (text!="") Writer.WriteLine(text);
					 Writer.Flush();
					 Writer.Close();
				} catch
				{
					 throw;
				}
		  }
	 }
}
