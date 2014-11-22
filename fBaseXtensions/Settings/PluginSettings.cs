using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using System.Xml.Serialization;
using fBaseXtensions.Game;
using fBaseXtensions.Helpers;
using Zeta.Game;

namespace fBaseXtensions.Settings
{
	public class PluginSettings
	{
		public GeneralSettings General { get; set; }
		public LoggerSettings Logging { get; set; }
		public MonitorSettings Monitoring { get; set; }
		public DebugSettings Debugging { get; set; }
		public SettingAdventureMode AdventureMode { get; set; }
		public SettingTargeting Targeting { get; set; }
		public SettingCombat Combat { get; set; }
		public SettingAvoidance Avoidance { get; set; }
		public SettingBacktrack Backtracking { get; set; }
		public SettingFleeing Fleeing { get; set; }
		public SettingGrouping Grouping { get; set; }
		public SettingLoot Loot { get; set; }
		public SettingRanges Ranges { get; set; }
		public SettingCluster Cluster { get; set; }
		public SettingLOSMovement LOSMovement { get; set; }
		public SettingDeath Death { get; set; }
		public SettingPlugin Plugin { get; set; }
		public SettingBarbarian Barbarian { get; set; }
		public SettingCrusader Crusader { get; set; }
		public SettingDemonHunter DemonHunter { get; set; }
		public SettingMonk Monk { get; set; }
		public SettingWitchDoctor WitchDoctor { get; set; }
		public SettingWizard Wizard { get; set; }

		public PluginSettings()
		{
			General = new GeneralSettings();
			Logging = new LoggerSettings();
			Monitoring = new MonitorSettings();
			Debugging = new DebugSettings();
			AdventureMode = new SettingAdventureMode();
			Grouping = new SettingGrouping();
			Fleeing = new SettingFleeing();
			Ranges = new SettingRanges();
			Loot = new SettingLoot();
			Cluster = new SettingCluster();
			Targeting = new SettingTargeting();
			Combat = new SettingCombat();
			Avoidance = new SettingAvoidance();
			LOSMovement = new SettingLOSMovement();
			Plugin = new SettingPlugin();
			Death = new SettingDeath();
			Backtracking = new SettingBacktrack();



            CreateClassSettings(FunkyGame.CurrentActorClass);
		}

	    public void CreateClassSettings()
	    {
            CreateClassSettings(FunkyGame.CurrentActorClass);
	    }

	    public void CreateClassSettings(ActorClass actorclass)
		{
            if (actorclass != ActorClass.Invalid)
			{
                switch (actorclass)
				{
					case ActorClass.Barbarian:
						Barbarian = new SettingBarbarian();
						break;
					case ActorClass.Crusader:
						Crusader = new SettingCrusader();
						break;
					case ActorClass.DemonHunter:
						DemonHunter = new SettingDemonHunter();
						break;
					case ActorClass.Monk:
						Monk = new SettingMonk();
						break;
					case ActorClass.Witchdoctor:
						WitchDoctor = new SettingWitchDoctor();
						break;
					case ActorClass.Wizard:
						Wizard = new SettingWizard();
						break;
				}
			}
		}

	    internal static List<string> ReturnAllSettingFiles()
	    {
	        var files = Directory.EnumerateFiles(FolderPaths.sFunkySettingsPath).ToList();
	        List<string> settingFiles = new List<string>();
	        foreach (var f in files)
	        {
	            if (f.EndsWith("_Plugin.xml"))
	            {
                    settingFiles.Add(f.Replace(FolderPaths.sFunkySettingsPath + @"\",""));
	            }
	        }

	        return settingFiles;
	    }
		internal static string sFunkySettingsCurrentPath
		{
			get
			{
				return Path.Combine(FolderPaths.sFunkySettingsPath, FunkyGame.CurrentHeroName + "_Plugin.xml");

			}
		}
		public static void LoadSettings()
		{
			string sFunkyCharacterConfigFile = sFunkySettingsCurrentPath;

			//Check for Config file
			if (!File.Exists(sFunkyCharacterConfigFile))
			{
				Logger.DBLog.InfoFormat("No config file found, now creating a new config from defaults at: " + sFunkyCharacterConfigFile);
				SerializeToXML(FunkyBaseExtension.Settings);
			}

			FunkyBaseExtension.Settings = DeserializeFromXML(sFunkySettingsCurrentPath);
		}

	    public static void SerializeToXML(PluginSettings settings)
	    {
            SerializeToXML(settings, sFunkySettingsCurrentPath);
	    }

	    public static void SerializeToXML(PluginSettings settings, string path)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(PluginSettings));
            TextWriter textWriter = new StreamWriter(path);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
		public static PluginSettings DeserializeFromXML(string path)
		{
			XmlSerializer deserializer = new XmlSerializer(typeof(PluginSettings));
			TextReader textReader = new StreamReader(path);
			PluginSettings settings;
			settings = (PluginSettings)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
		public static PluginSettings DeserializeFromXML()
		{
			return DeserializeFromXML(sFunkySettingsCurrentPath);
		}
	}
}
