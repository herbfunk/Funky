using System.IO;
using System.Reflection;

namespace FunkyBot
{
	public static class FolderPaths
	{
		internal static string DemonBuddyPath=Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		internal static string PluginPath=DemonBuddyPath+@"\Plugins\FunkyBot\";
		internal static string SettingsDefaultPath=Path.Combine(PluginPath, "Config", "Defaults");

		internal static string ProfileStatsPath
		{
			get
			{
				string outputPath = Path.Combine(LoggingFolderPath, "ProfileStats");
				CheckFolderExists(outputPath);
				return outputPath;
			}
		}
		internal static string LoggingFolderPath
		{
			get
			{
				string folderpath = Path.Combine(DemonBuddyPath, "FunkyStats", Bot.Character.Account.CurrentAccountName);
				CheckFolderExists(folderpath);
				return folderpath;
			}
		}
		internal static string sFunkySettingsPath
		{
			get
			{
				string sFunkyCharacterFolder = Path.Combine(DemonBuddyPath, "Settings", "FunkyBot", Bot.Character.Account.CurrentAccountName);
				CheckFolderExists(sFunkyCharacterFolder);
				return sFunkyCharacterFolder;
			}
		}

		internal static void CheckFolderExists(string path)
		{
			if (!Directory.Exists(path))
			{
				Logger.DBLog.DebugFormat("Creating new Folder @ {0}", path);
				Directory.CreateDirectory(path);
			}
		}

		internal static string sFunkySettingsCurrentPath
		{
			get
			{
				return Path.Combine(sFunkySettingsPath, Bot.Character.Account.CurrentHeroName + ".xml");

			}
		}
	}
}