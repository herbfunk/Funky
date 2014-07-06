using System.IO;
using System.Reflection;
using fItemPlugin.Player;

namespace fItemPlugin
{
	public static class FolderPaths
	{
		internal static string DemonBuddyPath=Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		internal static string RoutinePath = DemonBuddyPath + @"\Plugins\fItemPlugin\";
		internal static string SettingsDefaultPath=Path.Combine(RoutinePath, "Config", "Defaults");

		internal static string LoggingFolderPath
		{
			get
			{
				string folderpath = Path.Combine(DemonBuddyPath, "FunkyStats", Character.CurrentAccountName);
				CheckFolderExists(folderpath);
				return folderpath;
			}
		}
		internal static string sFunkyTownRunSettingsRootFolder
		{
			get
			{
				string path=Path.Combine(DemonBuddyPath, "Settings", "FunkyTownRun");
				CheckFolderExists(path);
				return path;
			}
		}
		internal static string sFunkySettingsPath
		{
			get
			{
				string sFunkyCharacterFolder = Path.Combine(sFunkyTownRunSettingsRootFolder, Character.CurrentAccountName);
				CheckFolderExists(sFunkyCharacterFolder);
				return sFunkyCharacterFolder;
			}
		}

		internal static void CheckFolderExists(string path)
		{
			if (!Directory.Exists(path))
			{
				FunkyTownRunPlugin.DBLog.DebugFormat("Creating new Folder @ {0}", path);
				Directory.CreateDirectory(path);
			}
		}

		internal static string sFunkySettingsCurrentPath
		{
			get
			{
				return Path.Combine(sFunkySettingsPath, Character.CurrentHeroName + ".xml");

			}
		}
	}
}