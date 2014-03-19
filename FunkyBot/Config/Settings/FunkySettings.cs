using System.IO;
using System.Xml.Serialization;
using Zeta.Game;


namespace FunkyBot.Settings
{

	public class Settings_Funky
	{
		public SettingDemonBuddy Demonbuddy { get; set; }
		public SettingDebug Debug { get; set; }
		public SettingTargeting Targeting { get; set; }
		public SettingCombat Combat { get; set; }
		public SettingAvoidance Avoidance { get; set; }
		public SettingBacktrack Backtracking { get; set; }


		public bool BuyPotionsDuringTownRun { get; set; }
		public bool EnableWaitAfterContainers { get; set; }


		//Character Related


		public SettingFleeing Fleeing { get; set; }

		//public int FleeDistance { get; set; }


		public SettingGrouping Grouping { get; set; }




		public int AfterCombatDelay { get; set; }


		public bool OutOfCombatMovement { get; set; }
		public bool AllowBuffingInTown { get; set; }



		public SettingItemRules ItemRules { get; set; }
		public SettingLoot Loot { get; set; }
		public SettingRanges Ranges { get; set; }








		public SettingCluster Cluster { get; set; }

		public SettingLOSMovement LOSMovement { get; set; }

		public SettingPlugin Plugin { get; set; }

		//Class Settings
		public ClassSettings Class { get; set; }
		public Settings_Funky()
		{

			Demonbuddy = new SettingDemonBuddy();
			Debug = new SettingDebug();
			Grouping = new SettingGrouping();
			Fleeing = new SettingFleeing();
			Ranges = new SettingRanges();
			ItemRules = new SettingItemRules();
			Loot = new SettingLoot();
			Cluster = new SettingCluster();
			Targeting = new SettingTargeting();
			Combat = new SettingCombat();
			Avoidance = new SettingAvoidance();
			LOSMovement = new SettingLOSMovement();
			Plugin = new SettingPlugin();
			Backtracking = new SettingBacktrack();


			BuyPotionsDuringTownRun = false;
			EnableWaitAfterContainers = false;

			AfterCombatDelay = 500;
			OutOfCombatMovement = false;
			AllowBuffingInTown = false;

			Class = new ClassSettings();
		}

		//TODO:: Create Abstract Base Class and Derieved Classes for each D3 Class.
		public class ClassSettings
		{
			//barb
			public bool bSelectiveWhirlwind { get; set; }
			public bool bWaitForWrath { get; set; }
			public bool bGoblinWrath { get; set; }
			public bool bFuryDumpWrath { get; set; }
			public bool bFuryDumpAlways { get; set; }
			public bool bBarbUseWOTBAlways { get; set; }
			//DH
			public int iDHVaultMovementDelay { get; set; }

			//Monk
			public bool bMonkInnaSet { get; set; }
			public bool bMonkSpamMantra { get; set; }
			public bool bMonkComboStrike { get; set; }
			public int iMonkComboStrikeAbilities { get; set; }
			public bool bMonkMaintainSweepingWind { get; set; }

			//Wiz
			public bool bWaitForArchon { get; set; }
			public bool bKiteOnlyArchon { get; set; }
			public bool bCancelArchonRebuff { get; set; }
			public bool bTeleportIntoGrouping { get; set; }
			public bool bTeleportFleeWhenLowHP { get; set; }

			//WD+Wiz
			//public bool bEnableCriticalMass { get; set; }

			//Range Class
			public int GoblinMinimumRange { get; set; }
			//Combat All Class
			public bool AllowDefaultAttackAlways { get; set; }

			public ClassSettings()
			{
				bTeleportIntoGrouping = false;
				bTeleportFleeWhenLowHP = true;
				bCancelArchonRebuff = false;
				bBarbUseWOTBAlways = false;
				bSelectiveWhirlwind = false;
				bWaitForWrath = false;
				bGoblinWrath = false;
				bFuryDumpWrath = false;
				bFuryDumpAlways = false;
				iDHVaultMovementDelay = 400;
				bMonkInnaSet = false;
				bMonkSpamMantra = false;
				bMonkComboStrike = false;
				bMonkMaintainSweepingWind = false;
				iMonkComboStrikeAbilities = 0;
				bWaitForArchon = false;
				bKiteOnlyArchon = true;
				GoblinMinimumRange = 40;
				AllowDefaultAttackAlways = false;
			}
		}


		public static void LoadFunkyConfiguration()
		{
			string sFunkyCharacterConfigFile = FolderPaths.sFunkySettingsCurrentPath;

			//Check for Config file
			if (!File.Exists(sFunkyCharacterConfigFile))
			{
				Logger.DBLog.InfoFormat("No config file found, now creating a new config from defaults at: " + sFunkyCharacterConfigFile);


				if (Bot.Character.Account.CurrentLevel < 60)
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
					if (Bot.Character.Account.ActorClass == ActorClass.Barbarian || Bot.Character.Account.ActorClass == ActorClass.Monk)
					{
						Logger.DBLog.InfoFormat("Using Melee Inferno Default Settings");
						DeserializeFromXML(Path.Combine(FolderPaths.SettingsDefaultPath, "InfernoMelee.xml"));
					}
					else
					{
						Logger.DBLog.InfoFormat("Using Ranged Inferno Default Settings");
						DeserializeFromXML(Path.Combine(FolderPaths.SettingsDefaultPath, "InfernoRanged.xml"));
					}
				}

				SerializeToXML(Bot.Settings);
			}
			else
			{
				//When new properties are added to exisiting classes.. a check should be done here to append those new settings with old file.

				Settings_Funky testSettings = DeserializeFromXML();

				//Avoidance Check
				if (testSettings.Avoidance.Avoidances.Length != Cache.AvoidanceCache.AvoidancesDefault.Length)
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
			TextWriter textWriter = new StreamWriter(FolderPaths.sFunkySettingsCurrentPath);
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
			return DeserializeFromXML(FolderPaths.sFunkySettingsCurrentPath);
		}
	}

}