using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using fBaseXtensions.Helpers;
using fBaseXtensions.Items.Enums;

namespace fBaseXtensions.Cache.Objects
{
	public class ItemStringDataCollection
	{
		public string[] ItemInternalNames { get; set; }

		public ItemStringDataCollection()
		{
			ItemInternalNames = new[] 
			{
				"ruby_","emerald_","topaz_","amethyst","diamond_",
				"horadric",
				"infernalmachine","demonkey_","demontrebuchetkey",
				"craftingreagent",
				"lootrunkey","crafting_","craftingmaterials_",
				"flail","axe_","dagger_","bow_","fistweapon_",
				"mace_","spear_","mightyweapon","sword_",
				"wand_","staff_","polearm_","mojo_","orb_",
				"quiver_","shield_","amulet_","ring_","boots_",
				"bracers_","cloak_","gloves_","pants_","belt_",
				"chestarmor_","helm_","helmcloth_",
				"shoulderpads_","spiritstone_","voodoomask_",
				"wizardhat_",
				"lore_book_","page_of_","blacksmithstome",
				"healthpotion","followeritem_","craftingplan_",
				"dye_", "healthglobe","jewelbox_","gold","_powerglobe",
			};
		}

		private static readonly string DefaultFilePath = Path.Combine(FolderPaths.PluginPath, "Cache", "Dictionaries", "String_Cache_Items.xml");
		internal static ItemStringDataCollection DeserializeFromXML()
		{
			var deserializer = new XmlSerializer(typeof(ItemStringDataCollection));
			TextReader textReader = new StreamReader(DefaultFilePath);
			var settings = (ItemStringDataCollection)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
		internal static void SerializeToXML(ItemStringDataCollection settings)
		{
			var serializer = new XmlSerializer(typeof(ItemStringDataCollection));
			var textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}


	}
}
