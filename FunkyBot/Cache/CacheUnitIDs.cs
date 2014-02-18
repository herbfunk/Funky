using System.IO;
using System.Xml.Serialization;

namespace FunkyBot.Cache
{
	public class CacheUnitIDs
	{
		public int[] SpawnerUnits { get; set; }
		public int[] FastUnits { get; set; }
		public int[] BurrowableUnits { get; set; }
		public int[] GrotesqueUnits { get; set; }
		public int[] SucideBomberUnits { get; set; }
		public int[] StealthUnits { get; set; }
		public int[] ReflectiveMissleUnits { get; set; }
		public int[] RangedUnits { get; set; }
		public int[] FlyingUnits { get; set; }
		public int[] RevivableUnits { get; set; }
		public int[] GoblinUnits { get; set; }
		public int[] BossUnits { get; set; }
		public UnitPriority[] UnitPriorities { get; set; }
		public CacheUnitIDs()
		{
			SpawnerUnits = new[] { 3037, 204509, 208826, 208825, 208824, 204944, 120652, 171283, 4153, 4154, 6639, 6638, 6640, 5387, 5388, 5389, 182279 };
			FastUnits = new[] { 5212, 5208, 5209, 5210, 4085, 4084, 4083, 4080, 4093, 4094, 4095, 5189, 5188, 5187, 5508, 152679, 5436, 169615, 4104, 4105, 4106, 370, 4196, 4197, 4198, 5235, 5236, 5238, 5239 };
			BurrowableUnits = new[] { 5088, 5090, 144400, 203048, 3384, 3385, 5199, 221402, 156738, 5189, 5188, 5187, 5191, 5192, 5193, 5194 };
			GrotesqueUnits = new[] { 3847, 3848, 3849, 3850, 218308, 218405, 113994, 195639 };
			SucideBomberUnits = new[] { 4093, 4094, 4095, 231356 };
			StealthUnits = new[] { 5432, 5433, 5434, 106714 };
			ReflectiveMissleUnits = new[] { 256000, 3981, 3980, 3982, 5191, 5192, 5193, 5194 };
			RangedUnits = new[] { 365, 4100, 4738, 62736, 130794, 5508, 4409, 4099, 4098, 4290, 4303, 4304, 375, 4286, 4287, 4299, 4300 };
			FlyingUnits = new[] { 3384, 3385, 222011, 222385, 5208, 5209, 5210, 370, 136943, 209553, 218441, 156353, 85971, 4196, 4197, 4198, 133093, 156763, 157006, 165602, 222526, 4799, 5508, 209596, 152679, 152679, 219673, 152535, 5512, 5513, 5514, 5515, 4156, 218314, 218362, 4157, 81954, 368, 218566, 4158, 195747, 217308, 104424, 256015 };
			RevivableUnits = new[] { 4084, 4083, 4080, 4085 };
			GoblinUnits = new[] { 5984, 5985, 5987, 5988 };
			BossUnits = new[] { 96192, 89690, 95250, 193077, 80509, 220160, 3349, 114917, 133562, 218947, 144001, 144003, 143996, 143994, 86624, 256508, 255929, 256187, 256189, 256711, 256709, 256000 };
			UnitPriorities = new[] 
			{
				    new UnitPriority(495, 901),
					new UnitPriority(496, 901), 
					new UnitPriority(6572, 901), 
					new UnitPriority(139454, 901), 
					new UnitPriority(139456, 901), 
					new UnitPriority(170324, 901), 
					new UnitPriority(170325, 901),
					// Fallen Shaman prophets goblin Summoners (365 & 4100)
					new UnitPriority(365, 1901), new UnitPriority(4100, 1901), new UnitPriority(4409, 1901), new UnitPriority(4098, 1901), new UnitPriority(4099,1901),
					// The annoying grunts summoned by the above
					new UnitPriority(4084, -401), new UnitPriority(4083, -401), new UnitPriority(4080, -401), new UnitPriority(4085, -401),
					// Fallen Champions (Big Guys who SMASH!)
					new UnitPriority(4070, 501), new UnitPriority(4071, 501),
					//A2 Foul Conjurer
					new UnitPriority(6038,501),
					//Dervish (Spinning AoE monsters)
					new UnitPriority(3980, 501), new UnitPriority(3981, 501), new UnitPriority(3982,501),
					//Sand Sharks
					new UnitPriority(5199, -401),
					//A2 Birds (Attacking but is still in the air!)
					new UnitPriority(3384, -401), new UnitPriority(3385, -401),
					// Wretched mothers that summon zombies in act 1 (6639)
					new UnitPriority(6639, 951), 
					// Fallen lunatic (4095)
					new UnitPriority(4095, 2999),
					// Pestilence hands (4738)
					new UnitPriority(4738, 1901), 
					 // Maghda and her minions
					new UnitPriority(6031, 801), new UnitPriority(178512, 901),
					// Cydaea boss (95250)
					new UnitPriority(95250, 1501),
					//Cydaea Spiderlings (137139)
					//new UnitPriority(137139, -301),
					// GoatMutantshaman Elite (4304)
					//new UnitPriority(4304, 999),
					// GoatMutantshaman (4300)
					//new UnitPriority(4300, 901),
					// Succubus (5508)
					//new UnitPriority(5508, 801),
					// skeleton summoners (5387, 5388, 5389)
					new UnitPriority(5387, 951), new UnitPriority(5388, 951), new UnitPriority(5389, 951), 
					// Weak skeletons summoned by the above
					new UnitPriority(5395, -401),
					// Wasp/Bees - Act 2 annoying flyers (5212) //5208,5209,5210
					new UnitPriority(5212, 1501), new UnitPriority(5208,1501), new UnitPriority(5209,1501), new UnitPriority(5210,1501),
					// Act 2 Construct Fire Mage
					new UnitPriority(5372, 1501),
					// Act 2 Construct Ice Mage
					new UnitPriority(5368, 501),
					// Dark summoner - summons the helion dogs (6035)
					new UnitPriority(6035, 501), 
					// Dark berserkers - has the huge damaging slow hit (6052)
					new UnitPriority(6052, 501), 
					// The giant undead fat grotesques that explode in act 1 (3847)
					new UnitPriority(3847, 401), 
					// Hive pods that summon flyers in act 1 (4152, 4153, 4154)
					new UnitPriority(4152, 901), new UnitPriority(4153, 901), new UnitPriority(4154, 901), 
					// Totems in act 1 that summon the ranged goatmen (166452)
					new UnitPriority(166452, 901), 
					// Totems in act 1 dungeons that summon skeletons (176907)
					new UnitPriority(176907, 901),
					//A2 Summoning Towers
					//Telsa
					new UnitPriority(208824,501),
					//Construct Summoner (A2 imp respawner)
					new UnitPriority(3037, 901),
					//Weak Skeletons
					new UnitPriority(5397, -201),
					//Weak Archer Skeletons
					new UnitPriority(5349, -101)
			};


		}

		private static string DefaultFilePath = Path.Combine(FolderPaths.sTrinityPluginPath, "Cache", "Dictionaries", "SNOId_Cache_Units.xml");
		public static CacheUnitIDs DeserializeFromXML()
		{
			XmlSerializer deserializer = new XmlSerializer(typeof(CacheUnitIDs));
			TextReader textReader = new StreamReader(DefaultFilePath);
			CacheUnitIDs settings;
			settings = (CacheUnitIDs)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
		public static void SerializeToXML(CacheUnitIDs settings)
		{
			// Type[] Settings=new Type[] {typeof(SettingCluster),typeof(SettingFleeing),typeof(SettingGrouping),typeof(SettingItemRules),typeof(SettingLoot),typeof(SettingRanges) };
			XmlSerializer serializer = new XmlSerializer(typeof(CacheUnitIDs));
			StreamWriter textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
	}
	public class UnitPriority
	{
		public int SNOId { get; set; }
		public int Value { get; set; }

		public UnitPriority()
		{
			SNOId = -1;
			Value = -1;
		}
		public UnitPriority(int snoId, int value)
		{
			SNOId = snoId;
			Value = value;
		}
	}
}
