using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using fBaseXtensions.Cache.External.Objects;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Helpers;
using fBaseXtensions.Items;
using fBaseXtensions.Items.Enums;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace fBaseXtensions.Cache.Internal.Objects
{
	public class DebugData
	{
		//public HashSet<DebugEntry> Objects { get; set; }

		public DebugData_Doors Doors { get; set; }
		public DebugData_Containers Containers { get; set; }
		public DebugData_Destructibles Destructibles { get; set; }
		public DebugData_DroppedItems Items { get; set; }
		public DebugData_Items ItemsData { get; set; }
		public DebugData_Units Units { get; set; }

		public DebugData()
		{
			Doors = DebugData_Doors.DeserializeFromXML();
			Containers = DebugData_Containers.DeserializeFromXML();
			Destructibles = DebugData_Destructibles.DeserializeFromXML();
			Items = DebugData_DroppedItems.DeserializeFromXML();
			ItemsData = DebugData_Items.DeserializeFromXML();
			Units = DebugData_Units.DeserializeFromXML();
		}

		public void CheckEntry(CachedSNOEntry entry)
		{
			var d = new DebugEntry(entry);
			if (entry.Gizmotype.HasValue)
			{
				if (entry.Gizmotype.Value == GizmoType.Door)
				{
					if (!FunkyBaseExtension.Settings.Debugging.DebuggingDataTypes.HasFlag(DebugDataTypes.Doors))
						return;

					if (Doors.Entries.Contains(d)) return;
					Doors.Entries.Add(d);
					DebugData_Doors.SerializeToXML(Doors);
				}
				else if (entry.Gizmotype.Value == GizmoType.Chest)
				{
					if (!FunkyBaseExtension.Settings.Debugging.DebuggingDataTypes.HasFlag(DebugDataTypes.Containers))
						return;

					if (Containers.Entries.Contains(d)) return;
					Containers.Entries.Add(d);
					DebugData_Containers.SerializeToXML(Containers);
				}
				else if (entry.Gizmotype.Value == GizmoType.BreakableChest || entry.Gizmotype.Value == GizmoType.DestroyableObject || entry.Gizmotype.Value == GizmoType.BreakableDoor)
				{
					if (!FunkyBaseExtension.Settings.Debugging.DebuggingDataTypes.HasFlag(DebugDataTypes.Destructibles))
						return;

					if (Destructibles.Entries.Contains(d)) return;
					Destructibles.Entries.Add(d);
					DebugData_Destructibles.SerializeToXML(Destructibles);
				}
			}
			else if (d.ActorType == ActorType.Item || d.TargetType == TargetType.Item)
			{
				if (!FunkyBaseExtension.Settings.Debugging.DebuggingDataTypes.HasFlag(DebugDataTypes.Items))
					return;

				if (Items.Entries.Contains(d)) return;
				Items.Entries.Add(d);
				DebugData_DroppedItems.SerializeToXML(Items);
			}
		}
		public void CheckEntry(CacheACDItem entry)
		{
			var d = new DebugItemDataEntry(entry);
			if (ItemsData.Entries.Contains(d)) return;
			ItemsData.Entries.Add(d);
			DebugData_Items.SerializeToXML(ItemsData);
		}
		public void CheckEntry(CacheUnit entry)
		{
			var d = new DebugUnitEntry(entry);
			if (Units.Entries.Contains(d)) return;
			Units.Entries.Add(d);
			DebugData_Units.SerializeToXML(Units);
		}



		public class DebugEntry
		{
			public int SNOID { get; set; }
			public string Name { get; set; }
			public ActorType ActorType { get; set; }
			public GizmoType GizmoType { get; set; }
			public TargetType TargetType { get; set; }


			public DebugEntry()
			{
				SNOID = 0;
				Name = String.Empty;
				ActorType = ActorType.Invalid;
				GizmoType = GizmoType.None;
				TargetType = TargetType.None;

			}
			public DebugEntry(CachedSNOEntry entry)
			{
				SNOID = entry.SNOID;
				Name = entry.InternalName;
				ActorType = entry.Actortype.HasValue ? entry.Actortype.Value : ActorType.Invalid;
				GizmoType = entry.Gizmotype.HasValue ? entry.Gizmotype.Value : GizmoType.None;
				TargetType = entry.targetType.HasValue ? entry.targetType.Value : TargetType.None;
			}

			public override int GetHashCode()
			{
				return SNOID;
			}

			public override bool Equals(object obj)
			{
				if (obj == null)
					return false;

				var p = obj as DebugEntry;
				if (p == null)
					return false;
				return (SNOID == p.SNOID);
			}
		}
		public class DebugItemDataEntry
		{
			public int SNOID { get; set; }
			public string Name { get; set; }
			public PluginItemTypes Type { get; set; }
			public ItemQuality Quality { get; set; }

			public DebugItemDataEntry()
			{
				SNOID = 0;
				Name = String.Empty;
				Type = PluginItemTypes.Unknown;
				Quality = ItemQuality.Invalid;
			}
			public DebugItemDataEntry(CacheACDItem item)
			{
				SNOID = item.SNO;
				Name = FormatString(item.ThisRealName);
				Type = item.ItemType;
				Quality = item.ThisQuality;
			}

			private string FormatString(string s)
			{
				var chararray = s.ToCharArray().ToList();
				int chararrayLength = chararray.Count;
				List<int> RemovalIndexList = new List<int>();
				for (int i = 0; i < chararrayLength; i++)
				{
					Char c = chararray[i];
					if (!Char.IsLetter(c))
					{
						RemovalIndexList.Add(i);
					}
				}

				RemovalIndexList = RemovalIndexList.OrderByDescending(i => i).ToList();
				foreach (var i in RemovalIndexList)
				{
					chararray.RemoveAt(i);
				}

				string retString = String.Empty;
				foreach (var c in chararray)
				{
					retString = retString + c;
				}

				return retString;
			}

			public override int GetHashCode()
			{
				return SNOID;
			}
			public override bool Equals(object obj)
			{
				if (obj == null)
					return false;

				var p = obj as DebugItemDataEntry;
				if (p == null)
					return false;
				return (SNOID == p.SNOID);
			}
		}
		public class DebugUnitEntry
		{
			public int SNOID { get; set; }
			public string Name { get; set; }
			public ActorType ActorType { get; set; }
			public UnitFlags UnitFlags { get; set; }
			public TargetType TargetType { get; set; }


			public DebugUnitEntry()
			{
				SNOID = 0;
				Name = String.Empty;
				ActorType = ActorType.Invalid;
				UnitFlags = UnitFlags.None;
				TargetType = TargetType.None;

			}
			public DebugUnitEntry(CacheUnit entry)
			{
				SNOID = entry.SNOID;
				Name = entry.InternalName;
				ActorType = ActorType.Monster;
				UnitFlags = entry.UnitPropertyFlags.HasValue ? entry.UnitPropertyFlags.Value : UnitFlags.None;
				TargetType = TargetType.Unit;
			}

			public override int GetHashCode()
			{
				return SNOID;
			}

			public override bool Equals(object obj)
			{
				if (obj == null)
					return false;

				var p = obj as DebugUnitEntry;
				if (p == null)
					return false;
				return (SNOID == p.SNOID);
			}
		}

		public static string ConvertDebugData()
		{
			try
			{
				string s = Clipboard.GetText();
				if (s.Contains("<DebugEntry>"))
				{
					var debugEntryString = _ConvertDebugEntries(s);
					Clipboard.SetText(debugEntryString);
					return debugEntryString;
				}
				if (s.Contains("<DebugItemDataEntry>\r\n"))
				{
					var debugEntryString = _ConvertDebugItemDataEntries(s);
					Clipboard.SetText(debugEntryString);
					return debugEntryString;
				}
				if (s.Contains("<DebugUnitEntry>"))
				{
					var debugEntryString = _ConvertDebugUnitEntries(s);
					Clipboard.SetText(debugEntryString);
					return debugEntryString;
				}
			}
			catch (Exception ex)
			{
				return ex.Message;
			}

			return String.Empty;
		}
		private static string _ConvertDebugEntries(string s)
		{
			string[] delimiter = new string[] { "<DebugEntry>\r\n" };
			string[] entryStrings = s.TrimStart().Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
			string finalString = String.Empty;
			foreach (var entryString in entryStrings)
			{
				string[] splitStrings = entryString.Split(Convert.ToChar("\n"));

				string sSnoID = splitStrings.FirstOrDefault(str => str.TrimStart().StartsWith("<SNOID>"));
				if (sSnoID == null) return String.Empty;
				sSnoID = ExtractDataFromXMLTag(sSnoID);
				if (sSnoID == String.Empty) return String.Empty;
				int iSNOID = Convert.ToInt32(sSnoID);

				string sActorType = splitStrings.FirstOrDefault(str => str.TrimStart().StartsWith("<ActorType>"));
				if (sActorType == null) return String.Empty;
				sActorType = ExtractDataFromXMLTag(sActorType);
				if (sActorType == String.Empty) return String.Empty;
				var ActorType = (ActorType)Enum.Parse(typeof(ActorType), sActorType);

				string name = splitStrings.FirstOrDefault(str => str.TrimStart().StartsWith("<Name>"));
				if (name == null) return String.Empty;
				name = ExtractDataFromXMLTag(name);
				if (name == String.Empty) return String.Empty;

				string returnString = "";
				if (ActorType == ActorType.Item)
				{
					returnString = "new DroppedItemEntry(" + sSnoID + ", ";

					PluginDroppedItemTypes itemtype = ItemFunc.DetermineDroppedItemType(name, iSNOID);
					returnString = returnString + "PluginDroppedItemTypes." + itemtype.ToString() + @", """ + name + @"""),";
				}
				else if (ActorType == ActorType.Gizmo)
				{
					returnString = "new GizmoEntry(" + sSnoID + ", ";

					string sGizmoType = splitStrings.FirstOrDefault(str => str.TrimStart().StartsWith("<GizmoType>"));
					if (sGizmoType == null) return String.Empty;
					sGizmoType = ExtractDataFromXMLTag(sGizmoType);
					if (sGizmoType == String.Empty) return String.Empty;
					returnString = returnString + "GizmoType." + sGizmoType + @", """ + name + @"""),";
				}



				finalString = finalString + returnString + "\r\n";
			}

			return finalString;
		}
		private static string _ConvertDebugUnitEntries(string s)
		{

			string[] delimiter = new string[] { "<DebugUnitEntry>\r\n" };
			string[] entryStrings = s.TrimStart().Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
			string finalString = String.Empty;
			foreach (var entryString in entryStrings)
			{
				string[] splitStrings = entryString.Split(Convert.ToChar("\n"));

				string sSnoID = splitStrings.FirstOrDefault(str => str.TrimStart().StartsWith("<SNOID>"));
				if (sSnoID == null) return String.Empty;
				sSnoID = ExtractDataFromXMLTag(sSnoID);
				if (sSnoID == String.Empty) return String.Empty;
				int iSNOID = Convert.ToInt32(sSnoID);

				string name = splitStrings.FirstOrDefault(str => str.TrimStart().StartsWith("<Name>"));
				if (name == null) return String.Empty;
				name = ExtractDataFromXMLTag(name);
				if (name == String.Empty) return String.Empty;

				string sUnitFlags = splitStrings.FirstOrDefault(str => str.TrimStart().StartsWith("<UnitFlags>"));
				if (sUnitFlags == null) return String.Empty;
				sUnitFlags = ExtractDataFromXMLTag(sUnitFlags);
				if (sUnitFlags == String.Empty) return String.Empty;
				sUnitFlags = sUnitFlags.Replace(" ", " | UnitFlags.");


				string returnString = "new UnitEntry(" + sSnoID + ", UnitFlags." + sUnitFlags + @", """ + name + @"""),";

				finalString = finalString + returnString + "\r\n";
			}

			return finalString;
		}
		private static string _ConvertDebugItemDataEntries(string s)
		{
			string[] delimiter = new string[] { "<DebugItemDataEntry>\r\n" };
			string[] entryStrings = s.TrimStart().Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
			string finalString = String.Empty;
			foreach (var entryString in entryStrings)
			{
				string[] splitStrings = entryString.Split(Convert.ToChar("\n"));

				string sSnoID = splitStrings.FirstOrDefault(str => str.TrimStart().StartsWith("<SNOID>"));
				if (sSnoID == null) return String.Empty;
				sSnoID = ExtractDataFromXMLTag(sSnoID);
				if (sSnoID == String.Empty) return String.Empty;
				int iSNOID = Convert.ToInt32(sSnoID);

				string sItemType = splitStrings.FirstOrDefault(str => str.TrimStart().StartsWith("<Type>"));
				if (sItemType == null) return String.Empty;
				sItemType = ExtractDataFromXMLTag(sItemType);
				if (sItemType == String.Empty) return String.Empty;
				var ItemType = (PluginItemTypes)Enum.Parse(typeof(PluginItemTypes), sItemType);
				//var ItemType = ItemFunc.DBItemTypeToPluginItemType((ItemType)Enum.Parse(typeof(ItemType), sItemType));


				string name = splitStrings.FirstOrDefault(str => str.TrimStart().StartsWith("<Name>"));
				if (name == null) return String.Empty;
				name = ExtractDataFromXMLTag(name);
				if (name == String.Empty) return String.Empty;

				string sQuality = splitStrings.FirstOrDefault(str => str.TrimStart().StartsWith("<Quality>"));
				if (sQuality == null) return String.Empty;
				sQuality = ExtractDataFromXMLTag(sQuality);
				if (sQuality == String.Empty) return String.Empty;
				var Quality = (ItemQuality)Enum.Parse(typeof(ItemQuality), sQuality);

				string returnString = "";

				returnString = "new ItemDataEntry(" + sSnoID + ", PluginItemTypes." + ItemType.ToString();

				if (Quality == ItemQuality.Legendary)
				{
					try
					{
						var LegendaryItemType = (LegendaryItemTypes)Enum.Parse(typeof(LegendaryItemTypes), name);
						returnString = returnString + ", LegendaryItemTypes." + LegendaryItemType.ToString() + "),";
					}
					catch (ArgumentException)
					{
						returnString = returnString + ", LegendaryItemTypes.None), //" + name;

					}
				}
				else
					returnString = returnString + "),";



				finalString = finalString + returnString + "\r\n";
			}

			return finalString;
		}
		private static string ExtractDataFromXMLTag(string tag)
		{
			try
			{

				int startIndex = tag.IndexOf(Convert.ToChar(">")) + 1;
				int endIndex = tag.LastIndexOf(Convert.ToChar("<"));
				return tag.Substring(startIndex, (endIndex - startIndex));

			}
			catch
			{

			}

			return String.Empty;
		}
	}

	public class DebugData_Doors
	{
		public DebugData_Doors()
		{
			Entries = new HashSet<DebugData.DebugEntry>();
		}
		public HashSet<DebugData.DebugEntry> Entries { get; set; }
		private static readonly string DefaultFolderPath = Path.Combine(FolderPaths.PluginPath, "Log");
		private static readonly string DefaultFilePath = DefaultFolderPath + @"\DebugData_Doors.xml";
		internal static void SerializeToXML(DebugData_Doors settings)
		{
			FolderPaths.CheckFolderExists(DefaultFolderPath);

			var serializer = new XmlSerializer(typeof(DebugData_Doors));
			var textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
		internal static DebugData_Doors DeserializeFromXML()
		{

			if (!File.Exists(DefaultFilePath))
			{
				Logger.DBLog.DebugFormat("Could not load Data Debugging File! {0}", "DebugData_Doors.xml");
				var debugData = new DebugData_Doors();
				SerializeToXML(debugData);
				return debugData;
			}

			var deserializer = new XmlSerializer(typeof(DebugData_Doors));
			TextReader textReader = new StreamReader(DefaultFilePath);
			var settings = (DebugData_Doors)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
	}

	public class DebugData_Containers
	{
		public DebugData_Containers()
		{
			Entries = new HashSet<DebugData.DebugEntry>();
		}
		public HashSet<DebugData.DebugEntry> Entries { get; set; }
		private static readonly string DefaultFolderPath = Path.Combine(FolderPaths.PluginPath, "Log");
		private static readonly string DefaultFilePath = DefaultFolderPath + @"\DebugData_Containers.xml";

		internal static void SerializeToXML(DebugData_Containers settings)
		{
			FolderPaths.CheckFolderExists(DefaultFolderPath);

			var serializer = new XmlSerializer(typeof(DebugData_Containers));
			var textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
		internal static DebugData_Containers DeserializeFromXML()
		{
			if (!File.Exists(DefaultFilePath))
			{
				Logger.DBLog.DebugFormat("Could not load Data Debugging File! {0}", "DebugData_Containers.xml");
				var debugData = new DebugData_Containers();
				SerializeToXML(debugData);
				return debugData;
			}

			var deserializer = new XmlSerializer(typeof(DebugData_Containers));
			TextReader textReader = new StreamReader(DefaultFilePath);
			var settings = (DebugData_Containers)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
	}

	public class DebugData_Destructibles
	{
		public DebugData_Destructibles()
		{
			Entries = new HashSet<DebugData.DebugEntry>();
		}
		public HashSet<DebugData.DebugEntry> Entries { get; set; }
		private static readonly string DefaultFolderPath = Path.Combine(FolderPaths.PluginPath, "Log");
		private static readonly string DefaultFilePath = DefaultFolderPath + @"\DebugData_Destructibles.xml";

		internal static void SerializeToXML(DebugData_Destructibles settings)
		{
			FolderPaths.CheckFolderExists(DefaultFolderPath);

			var serializer = new XmlSerializer(typeof(DebugData_Destructibles));
			var textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
		internal static DebugData_Destructibles DeserializeFromXML()
		{
			if (!File.Exists(DefaultFilePath))
			{
				Logger.DBLog.DebugFormat("Could not load Data Debugging File! {0}", "DebugData_Destructibles.xml");
				var debugData = new DebugData_Destructibles();
				SerializeToXML(debugData);
				return debugData;
			}

			var deserializer = new XmlSerializer(typeof(DebugData_Destructibles));
			TextReader textReader = new StreamReader(DefaultFilePath);
			var settings = (DebugData_Destructibles)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
	}

	public class DebugData_DroppedItems
	{
		public DebugData_DroppedItems()
		{
			Entries = new HashSet<DebugData.DebugEntry>();
		}
		public HashSet<DebugData.DebugEntry> Entries { get; set; }
		private static readonly string DefaultFolderPath = Path.Combine(FolderPaths.PluginPath, "Log");
		private static readonly string DefaultFilePath = DefaultFolderPath + @"\DebugData_DroppedItems.xml";

		internal static void SerializeToXML(DebugData_DroppedItems settings)
		{
			FolderPaths.CheckFolderExists(DefaultFolderPath);

			var serializer = new XmlSerializer(typeof(DebugData_DroppedItems));
			var textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
		internal static DebugData_DroppedItems DeserializeFromXML()
		{
			if (!File.Exists(DefaultFilePath))
			{
				Logger.DBLog.DebugFormat("Could not load Data Debugging File! {0}", "DebugData_DroppedItems.xml");
				var debugData = new DebugData_DroppedItems();
				SerializeToXML(debugData);
				return debugData;
			}

			var deserializer = new XmlSerializer(typeof(DebugData_DroppedItems));
			TextReader textReader = new StreamReader(DefaultFilePath);
			var settings = (DebugData_DroppedItems)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
	}

	public class DebugData_Items
	{
		public DebugData_Items()
		{
			Entries = new HashSet<DebugData.DebugItemDataEntry>();
		}
		public HashSet<DebugData.DebugItemDataEntry> Entries { get; set; }
		private static readonly string DefaultFolderPath = Path.Combine(FolderPaths.PluginPath, "Log");
		private static readonly string DefaultFilePath = DefaultFolderPath + @"\DebugData_Items.xml";

		internal static void SerializeToXML(DebugData_Items settings)
		{
			FolderPaths.CheckFolderExists(DefaultFolderPath);

			var serializer = new XmlSerializer(typeof(DebugData_Items));
			var textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
		internal static DebugData_Items DeserializeFromXML()
		{
			if (!File.Exists(DefaultFilePath))
			{
				Logger.DBLog.DebugFormat("Could not load Data Debugging File! {0}", "DebugData_Items.xml");
				var debugData = new DebugData_Items();
				SerializeToXML(debugData);
				return debugData;
			}

			var deserializer = new XmlSerializer(typeof(DebugData_Items));
			TextReader textReader = new StreamReader(DefaultFilePath);
			var settings = (DebugData_Items)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
	}

	public class DebugData_Units
	{
		public DebugData_Units()
		{
			Entries = new HashSet<DebugData.DebugUnitEntry>();
		}
		public HashSet<DebugData.DebugUnitEntry> Entries { get; set; }
		private static readonly string DefaultFolderPath = Path.Combine(FolderPaths.PluginPath, "Log");
		private static readonly string DefaultFilePath = DefaultFolderPath + @"\DebugData_Units.xml";

		internal static void SerializeToXML(DebugData_Units settings)
		{
			FolderPaths.CheckFolderExists(DefaultFolderPath);

			var serializer = new XmlSerializer(typeof(DebugData_Units));
			var textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
		internal static DebugData_Units DeserializeFromXML()
		{
			if (!File.Exists(DefaultFilePath))
			{
				Logger.DBLog.DebugFormat("Could not load Data Debugging File! {0}", "DebugData_Units.xml");
				var debugData = new DebugData_Units();
				SerializeToXML(debugData);
				return debugData;
			}

			var deserializer = new XmlSerializer(typeof(DebugData_Units));
			TextReader textReader = new StreamReader(DefaultFilePath);
			var settings = (DebugData_Units)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
	}
}
