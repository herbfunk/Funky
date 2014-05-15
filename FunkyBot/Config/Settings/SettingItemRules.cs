using System;
using System.IO;
using System.Xml.Serialization;

namespace FunkyBot.Config.Settings
{
	public class SettingItemRules
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
		public bool ItemRuleGilesScoring { get; set; }

		public SettingItemRules()
		{
			ItemRuleDebug=false;
			ItemRuleUseItemIDs=false;
			ItemRuleType="Hard";
			ItemRuleLogKeep="Rare";
			ItemRuleLogPickup="Rare";
			ItemRuleGilesScoring=true;
			UseItemRulesPickup=true;
			UseItemRules=true;
			ItemRulesUnidStashing=true;
			ItemRulesSalvaging=true;
			ItemRuleCustomPath=String.Empty;
		}

		private static string DefaultFilePath=Path.Combine(FolderPaths.SettingsDefaultPath, "Specific", "ItemRules_Default.xml");
		public static SettingItemRules DeserializeFromXML()
		{
			 XmlSerializer deserializer=new XmlSerializer(typeof(SettingItemRules));
			 TextReader textReader=new StreamReader(DefaultFilePath);
			 SettingItemRules settings;
			 settings=(SettingItemRules)deserializer.Deserialize(textReader);
			 textReader.Close();
			 return settings;
		}
		public static SettingItemRules DeserializeFromXML(string Path)
		{
			 XmlSerializer deserializer=new XmlSerializer(typeof(SettingItemRules));
			 TextReader textReader=new StreamReader(Path);
			 SettingItemRules settings;
			 settings=(SettingItemRules)deserializer.Deserialize(textReader);
			 textReader.Close();
			 return settings;
		}
	}
}