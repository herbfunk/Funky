using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using fBaseXtensions.Cache.External.Debugging.EntryObjects;
using fBaseXtensions.Cache.External.Enums;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Items;
using fBaseXtensions.Items.Enums;
using Zeta.Game.Internals.SNO;

namespace fBaseXtensions.Cache.External.Debugging
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
			else if (d.ActorType == PluginActorType.Item || d.TargetType == TargetType.Item)
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
			List<DebugEntry> Entries = new List<DebugEntry>();

			
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
				var ActorType = (PluginActorType)Enum.Parse(typeof(PluginActorType), sActorType);

				string name = splitStrings.FirstOrDefault(str => str.TrimStart().StartsWith("<Name>"));
				if (name == null) return String.Empty;
				name = ExtractDataFromXMLTag(name);
				if (name == String.Empty) return String.Empty;

				PluginGizmoType gizmotype = PluginGizmoType.None;
				if (ActorType == PluginActorType.Gizmo)
				{
					string sGizmoType = splitStrings.FirstOrDefault(str => str.TrimStart().StartsWith("<GizmoType>"));
					if (sGizmoType == null) return String.Empty;
					sGizmoType = ExtractDataFromXMLTag(sGizmoType);
					if (sGizmoType == String.Empty) return String.Empty;
					gizmotype = (PluginGizmoType)Enum.Parse(typeof(PluginGizmoType), sGizmoType);
				}

				Entries.Add(new DebugEntry(iSNOID, name, ActorType, gizmotype));

				//string returnString = "";
				//if (ActorType == ActorType.Item)
				//{
				//	returnString = "new DroppedItemEntry(" + sSnoID + ", ";

				//	PluginDroppedItemTypes itemtype = ItemFunc.DetermineDroppedItemType(name, iSNOID);
				//	returnString = returnString + "PluginDroppedItemTypes." + itemtype.ToString() + @", """ + name + @"""),";
				//}
				//else if (ActorType == ActorType.Gizmo)
				//{
				//	returnString = "new GizmoEntry(" + sSnoID + ", ";

					
				//	returnString = returnString + "GizmoType." + sGizmoType + @", """ + name + @"""),";
				//}



				//finalString = finalString + returnString + "\r\n";
			}


			string finalString = String.Empty;
			foreach (var entry in Entries.OrderBy(e => e.DroppedItemType))
			{
				finalString = finalString + entry.ReturnCacheEntryString() + "\r\n";
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
			
			List<DebugItemDataEntry> Entries = new List<DebugItemDataEntry>();

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
				var Quality = (PluginItemQuality)Enum.Parse(typeof(PluginItemQuality), sQuality);

				Entries.Add(new DebugItemDataEntry(iSNOID, name, ItemType, Quality));

				//string returnString = "";

				//returnString = "new ItemDataEntry(" + sSnoID + ", PluginItemTypes." + ItemType.ToString();

				//if (Quality == ItemQuality.Legendary)
				//{
				//	try
				//	{
				//		var LegendaryItemType = (LegendaryItemTypes)Enum.Parse(typeof(LegendaryItemTypes), name);
				//		returnString = returnString + ", LegendaryItemTypes." + LegendaryItemType.ToString() + "),";
				//	}
				//	catch (ArgumentException)
				//	{
				//		returnString = returnString + ", LegendaryItemTypes.None), //" + name;

				//	}
				//}
				//else
				//	returnString = returnString + "),";



				//finalString = finalString + returnString + "\r\n";
			}

			string finalString = String.Empty;
			foreach (var entry in Entries.OrderBy(e => e.Type))
			{
				finalString = finalString + entry.ReturnCacheEntryString() + "\r\n";
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
}
