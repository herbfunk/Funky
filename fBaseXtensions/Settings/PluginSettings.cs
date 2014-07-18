using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using fBaseXtensions.Game;
using fBaseXtensions.Helpers;

namespace fBaseXtensions.Settings
{
	public class PluginSettings
	{
		public LoggerSettings Logging { get; set; }
		public MonitorSettings Monitoring { get; set; }

		public PluginSettings()
		{
			Logging = new LoggerSettings();
			Monitoring = new MonitorSettings();
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
			XmlSerializer serializer = new XmlSerializer(typeof(PluginSettings));
			TextWriter textWriter = new StreamWriter(sFunkySettingsCurrentPath);
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
