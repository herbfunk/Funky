using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fBaseXtensions.Items;
using fBaseXtensions.Items.Enums;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Cache.External.Debugging.EntryObjects
{
	public class DebugItemDataEntry
	{
		public int SNOID { get; set; }
		public string Name { get; set; }
		public PluginItemTypes Type { get; set; }
		public PluginItemQuality Quality { get; set; }

		public DebugItemDataEntry()
		{
			SNOID = 0;
			Name = String.Empty;
			Type = PluginItemTypes.Unknown;
			Quality = PluginItemQuality.Invalid;
		}
		public DebugItemDataEntry(CacheACDItem item)
		{
			SNOID = item.SNO;
			Name = FormatString(item.ThisRealName);
			Type = item.ItemType;
			Quality = (PluginItemQuality)Enum.Parse(typeof(PluginItemQuality), item.ThisQuality.ToString());
		}
		public DebugItemDataEntry(int snoid, string name, PluginItemTypes type, PluginItemQuality quality)
		{
			SNOID = snoid;
			Name = name;
			Type = type;
			Quality = quality;
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

		public string ReturnCacheEntryString()
		{
			string returnString = "";

			returnString = "new ItemDataEntry(" + SNOID + ", PluginItemTypes." + Type.ToString();
			if (Quality == PluginItemQuality.Legendary)
			{
				try
				{
					var LegendaryItemType = (LegendaryItemTypes)Enum.Parse(typeof(LegendaryItemTypes), Name);
					returnString = returnString + ", LegendaryItemTypes." + LegendaryItemType.ToString() + "),";
				}
				catch (ArgumentException)
				{
					returnString = returnString + ", LegendaryItemTypes.None), //" + Name;

				}
			}
			else
				returnString = returnString + "),";

			return returnString;
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
}
