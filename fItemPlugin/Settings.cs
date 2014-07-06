using System.IO;
using System.Xml.Serialization;
using fItemPlugin.Player;
using fItemPlugin.Townrun;

namespace fItemPlugin
{
	public class Settings
	{
		public bool UseItemRulesPickup { get; set; }
		public bool UseItemRules { get; set; }
		public bool ItemRulesSalvaging { get; set; }
		public bool ItemRulesUnidStashing { get; set; }
		public bool ItemRuleUseItemIDs { get; set; }
		public bool ItemRuleDebug { get; set; }
		public string ItemRuleType { get; set; }
		public string ItemRuleCustomPath { get; set; }
		public string ItemRuleLogPickup { get; set; }
		public string ItemRuleLogKeep { get; set; }
	

		public bool StashHoradricCache { get; set; }
		public bool BuyPotionsDuringTownRun { get; set; }
		public int PotionsCount { get; set; }
		public bool IdentifyLegendaries { get; set; }
		public bool UseItemManagerEvaluation { get; set; }

		public bool EnableBloodShardGambling { get; set; }
		public int MinimumBloodShards { get; set; }
		public BloodShardGambleItems BloodShardGambleItems { get; set; }
		

		//0 == Ignore, 1 == All, 61 == ROS Only
		public int SalvageWhiteItemLevel { get; set; }
		public int SalvageMagicItemLevel { get; set; }
		public int SalvageRareItemLevel { get; set; }
		public int SalvageLegendaryItemLevel { get; set; }

		

		public Settings()
		{
			ItemRuleDebug=false;
			ItemRuleUseItemIDs=false;
			ItemRuleType="hard";
			ItemRuleLogKeep="Rare";
			ItemRuleLogPickup="Rare";
		
			UseItemRulesPickup=true;
			UseItemRules=true;
			ItemRulesUnidStashing=true;
			ItemRulesSalvaging=true;
			ItemRuleCustomPath="";

			StashHoradricCache = false;
			EnableBloodShardGambling = false;
			MinimumBloodShards = 100;
			BloodShardGambleItems = BloodShardGambleItems.All;
			BuyPotionsDuringTownRun = false;
			PotionsCount = 100;
			SalvageWhiteItemLevel = 0;
			SalvageMagicItemLevel = 0;
			SalvageRareItemLevel = 0;
			SalvageLegendaryItemLevel = 0;
			IdentifyLegendaries = false;
			UseItemManagerEvaluation = false;
		}

		public static void SerializeToXML(Settings settings)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(Settings));
			TextWriter textWriter = new StreamWriter(FolderPaths.sFunkySettingsCurrentPath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
		public static Settings DeserializeFromXML(string Path)
		{
			XmlSerializer deserializer = new XmlSerializer(typeof(Settings));
			 TextReader textReader=new StreamReader(Path);
			 Settings settings;
			 settings = (Settings)deserializer.Deserialize(textReader);
			 textReader.Close();
			 return settings;
		}


		public static void LoadSettings()
		{
			Character.UpdateAccoutDetails();

			string sFunkyCharacterConfigFile = FolderPaths.sFunkySettingsCurrentPath;

			//Check for Config file
			if (!File.Exists(sFunkyCharacterConfigFile))
			{
				FunkyTownRunPlugin.DBLog.InfoFormat("No config file found, now creating a new config from defaults at: " + sFunkyCharacterConfigFile);
				SerializeToXML(FunkyTownRunPlugin.PluginSettings);
			}

			FunkyTownRunPlugin.PluginSettings = DeserializeFromXML(FolderPaths.sFunkySettingsCurrentPath);
		}
	}
}
