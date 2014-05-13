using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FunkyBot.Cache
{
	public class CacheObjectIDs
	{
		public int[] Doors { get; set; }
		public int[] SpecialInteractables { get; set; }
		public int[] ResplendantChests { get; set; }

		public CacheObjectIDs()
		{
			Doors = new[] { 5854 };

			ResplendantChests = new[] 
			{
				62873, 95011, 81424, 108230, 111808, 111809, 199583, 109264,101500,96993, 62866, 108230, 211861, 62860, 96993,112182, 363725,357509
			};

			SpecialInteractables = new[] { 174754, 174753, 102079 };


		}

		private static string DefaultFilePath = Path.Combine(FolderPaths.PluginPath, "Cache", "Dictionaries", "SNOId_Cache_Objects.xml");
		public static CacheObjectIDs DeserializeFromXML()
		{
			XmlSerializer deserializer = new XmlSerializer(typeof(CacheObjectIDs));
			TextReader textReader = new StreamReader(DefaultFilePath);
			CacheObjectIDs settings;
			settings = (CacheObjectIDs)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
		public static void SerializeToXML(CacheObjectIDs settings)
		{
			// Type[] Settings=new Type[] {typeof(SettingCluster),typeof(SettingFleeing),typeof(SettingGrouping),typeof(SettingItemRules),typeof(SettingLoot),typeof(SettingRanges) };
			XmlSerializer serializer = new XmlSerializer(typeof(CacheObjectIDs));
			StreamWriter textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
	}
}
