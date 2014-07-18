using System.IO;
using System.Xml.Serialization;
using fBaseXtensions.Game;
using fBaseXtensions.Helpers;
using FunkyBot.Cache.Avoidance;
using FunkyBot.Config.Settings.Class;
using Zeta.Game;
using Logger = fBaseXtensions.Helpers.Logger;

namespace FunkyBot.Config.Settings
{

	public class Settings_Funky
	{
		public SettingDeath Death { get; set; }
		public SettingAdventureMode AdventureMode { get; set; }
		public SettingDebug Debug { get; set; }
		public SettingTargeting Targeting { get; set; }
		public SettingCombat Combat { get; set; }
		public SettingAvoidance Avoidance { get; set; }
		public SettingBacktrack Backtracking { get; set; }
		public SettingFleeing Fleeing { get; set; }
		public SettingGrouping Grouping { get; set; }
		public SettingGeneral General { get; set; }
		public SettingLoot Loot { get; set; }
		public SettingRanges Ranges { get; set; }
		public SettingCluster Cluster { get; set; }
		public SettingLOSMovement LOSMovement { get; set; }
		public SettingPlugin Plugin { get; set; }
		public SettingBarbarian Barbarian { get; set; }
		public SettingCrusader Crusader { get; set; }
		public SettingDemonHunter DemonHunter { get; set; }
		public SettingMonk Monk { get; set; }
		public SettingWitchDoctor WitchDoctor { get; set; }
		public SettingWizard Wizard { get; set; }

		public Settings_Funky()
		{
			General = new SettingGeneral();
			Death = new SettingDeath();
			AdventureMode=new SettingAdventureMode();
			Debug = new SettingDebug();
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
			Backtracking = new SettingBacktrack();
			


			CreateClassSettings();

		}

		public void CreateClassSettings()
		{
			if (FunkyGame.CurrentActorClass != ActorClass.Invalid)
			{
				switch (FunkyGame.CurrentActorClass)
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

		private static string sFunkySettingsCurrentPath
		{
			get
			{
				return Path.Combine(FolderPaths.sFunkySettingsPath,FunkyGame.CurrentHeroName + "_Combat.xml");

			}
		}
		public static void LoadFunkyConfiguration()
		{
			string sFunkyCharacterConfigFile = sFunkySettingsCurrentPath;

			//Check for Config file
			if (!File.Exists(sFunkyCharacterConfigFile))
			{
				Logger.DBLog.InfoFormat("No config file found, now creating a new config from defaults at: " + sFunkyCharacterConfigFile);


				if (FunkyGame.CurrentHeroLevel < 60)
				{
					Logger.DBLog.InfoFormat("Using Low Level Settings");
					Bot.Settings = new Settings_Funky
					{
						 Grouping = new SettingGrouping(false),
						 Cluster = new SettingCluster(false),
						 Fleeing = new SettingFleeing(false),
					};
				}
				else
				{
					var path = Path.Combine(Funky.RoutinePath, "Config", "Defaults");
					if (FunkyGame.CurrentActorClass == ActorClass.Barbarian || FunkyGame.CurrentActorClass == ActorClass.Monk)
					{
						Logger.DBLog.InfoFormat("Using Melee Inferno Default Settings");
						DeserializeFromXML(Path.Combine(path, "Melee.xml"));
					}
					else
					{
						Logger.DBLog.InfoFormat("Using Ranged Inferno Default Settings");
						DeserializeFromXML(Path.Combine(path, "Ranged.xml"));
					}
				}

				SerializeToXML(Bot.Settings);
			}
			else
			{
				//When new properties are added to exisiting classes.. a check should be done here to append those new settings with old file.

				Settings_Funky testSettings = DeserializeFromXML();

				//Avoidance Check
				if (testSettings.Avoidance.Avoidances.Length != AvoidanceCache.AvoidancesDefault.Length)
				{
					Logger.DBLog.Info("[Funky] Settings found missing Avoidances.. reseting avoidance settings to default!");
					testSettings.Avoidance = new SettingAvoidance();
					SerializeToXML(testSettings);
				}

				//LOS Check
				if (!testSettings.LOSMovement.EnableLOSMovementBehavior)
				{
					Logger.DBLog.Info("[Funky] Settings found disabled LOS Movement -- reseting to default values!");
					testSettings.LOSMovement = new SettingLOSMovement();
					SerializeToXML(testSettings);
				}
			}



			Bot.Settings = DeserializeFromXML();
		}
		public static void SerializeToXML(Settings_Funky settings)
		{
			// Type[] Settings=new Type[] {typeof(SettingCluster),typeof(SettingFleeing),typeof(SettingGrouping),typeof(SettingItemRules),typeof(SettingLoot),typeof(SettingRanges) };
			XmlSerializer serializer = new XmlSerializer(typeof(Settings_Funky));
			TextWriter textWriter = new StreamWriter(sFunkySettingsCurrentPath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
		public static Settings_Funky DeserializeFromXML(string path)
		{
			// Type[] Settings=new Type[] { typeof(SettingCluster), typeof(SettingFleeing), typeof(SettingGrouping), typeof(SettingItemRules), typeof(SettingLoot), typeof(SettingRanges) };
			XmlSerializer deserializer = new XmlSerializer(typeof(Settings_Funky));
			TextReader textReader = new StreamReader(path);
			Settings_Funky settings;
			settings = (Settings_Funky)deserializer.Deserialize(textReader);
			textReader.Close();

			return settings;
		}
		public static Settings_Funky DeserializeFromXML()
		{
			return DeserializeFromXML(sFunkySettingsCurrentPath);
		}
	}

}