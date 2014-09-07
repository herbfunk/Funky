using System.IO;
using System.Reflection;
using fBaseXtensions.Game;


namespace fBaseXtensions.Helpers
{
	public static class FolderPaths
	{
		public static string DemonBuddyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		internal static string PluginPath = Path.Combine(DemonBuddyPath, "Plugins", "fBaseXtensions");
		internal static string RoutinePath
		{
			get
			{
				string path = Path.Combine(DemonBuddyPath, "Routines", "Funky");
				CheckFolderExists(path);
				return path;
			}
		}

		public static string LoggingFolderPath
		{
			get
			{
				string folderpath = Path.Combine(DemonBuddyPath, "FunkyLogs", FunkyGame.CurrentAccountName, FunkyGame.CurrentActorClass.ToString() + "_" + FunkyGame.CurrentHeroName);
				CheckFolderExists(folderpath);
				return folderpath;
			}
		}
		public static string sFunkySettingsPath
		{
			get
			{
				string sFunkyCharacterFolder = Path.Combine(DemonBuddyPath, "Settings", "Funky", FunkyGame.CurrentAccountName);
				CheckFolderExists(sFunkyCharacterFolder);
				return sFunkyCharacterFolder;
			}
		}

		public static bool CheckFolderExists(string path, bool createfolder=true)
		{
			if (!Directory.Exists(path))
			{
				if (!createfolder) return false;
				Logger.DBLog.DebugFormat("Creating new Folder @ {0}", path);
				Directory.CreateDirectory(path);
			}

			return true;
		}


	}
}