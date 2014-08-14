using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Helpers;

namespace fBaseXtensions.Cache.External.Objects
{
	public class AvoidanceDataCollection
	{
		public HashSet<AvoidanceEntry> AvoidanceCache { get; set; }

		public AvoidanceDataCollection()
		{
			AvoidanceCache = new HashSet<AvoidanceEntry>
			{
				new AvoidanceEntry(219702, AvoidanceType.ArcaneSentry),
				new AvoidanceEntry(221225, AvoidanceType.ArcaneSentry),
				new AvoidanceEntry(84608, AvoidanceType.Dececrator),
				new AvoidanceEntry(5482, AvoidanceType.TreeSpore),
				new AvoidanceEntry(6578, AvoidanceType.TreeSpore),
				new AvoidanceEntry(4803, AvoidanceType.MoltenCore),
				new AvoidanceEntry(4804, AvoidanceType.MoltenCore),
				new AvoidanceEntry(224225, AvoidanceType.MoltenCore),
				new AvoidanceEntry(247987, AvoidanceType.MoltenCore),
				new AvoidanceEntry(95868, AvoidanceType.MoltenTrail),
				new AvoidanceEntry(108869, AvoidanceType.PlagueCloud),
				new AvoidanceEntry(402, AvoidanceType.Frozen),
				new AvoidanceEntry(223675, AvoidanceType.Frozen),
				new AvoidanceEntry(5212, AvoidanceType.BeeProjectile),
				new AvoidanceEntry(3865, AvoidanceType.PlagueHand),
				new AvoidanceEntry(123124, AvoidanceType.AzmodanPool),
				new AvoidanceEntry(123842, AvoidanceType.AzmodanFireball),
				new AvoidanceEntry(123839, AvoidanceType.AzmodanBodies),
				new AvoidanceEntry(161822, AvoidanceType.BelialGround),
				new AvoidanceEntry(161833, AvoidanceType.BelialGround),
				new AvoidanceEntry(4103, AvoidanceType.ShamanFireBall),
				new AvoidanceEntry(432, AvoidanceType.MageFirePool),
				new AvoidanceEntry(168031, AvoidanceType.DiabloPrison),
				new AvoidanceEntry(214845, AvoidanceType.DiabloMetor),
				new AvoidanceEntry(4176, AvoidanceType.PoisonGas),
				new AvoidanceEntry(4546, AvoidanceType.LacuniBomb),
				new AvoidanceEntry(164829, AvoidanceType.SuccubusProjectile),
				new AvoidanceEntry(343539, AvoidanceType.OrbitProjectile),
				new AvoidanceEntry(343582, AvoidanceType.OrbitFocalPoint),
				new AvoidanceEntry(349774, AvoidanceType.FrozenPulse),
				new AvoidanceEntry(341512, AvoidanceType.Thunderstorm),
				new AvoidanceEntry(185366, AvoidanceType.MeteorImpact),
				new AvoidanceEntry(332924, AvoidanceType.BloodSpringSmall),
				new AvoidanceEntry(332922, AvoidanceType.BloodSpringMedium),
				new AvoidanceEntry(332923, AvoidanceType.BloodSpringLarge),
				new AvoidanceEntry(337109, AvoidanceType.Teleport),
				new AvoidanceEntry(335505, AvoidanceType.MalthaelDrainSoul),
				new AvoidanceEntry(325136, AvoidanceType.MalthaelDeathFog),
				new AvoidanceEntry(340512, AvoidanceType.MalthaelLightning),
				new AvoidanceEntry(360738, AvoidanceType.AdriaArcanePool),
				new AvoidanceEntry(358404, AvoidanceType.AdriaBlood),
				new AvoidanceEntry(360046, AvoidanceType.RiftPoison),
				new AvoidanceEntry(93837, AvoidanceType.GhomGasCloud),
				new AvoidanceEntry(134831, AvoidanceType.DestroyerDrop),
				new AvoidanceEntry(185924, AvoidanceType.ZoltBubble),
				new AvoidanceEntry(139741, AvoidanceType.ZoltTwister),

				//new AvoidanceEntry(000000, AvoidanceType.ArcaneSentry),
			};

			// A list of all the SNO's to avoid - you could add 
			//Avoidances = new HashSet<int>
			//{
			//	  // Arcane        Arcane 2      Desecrator   Poison Tree    Molten Core   Molten Trail   Plague Cloud   Ice Balls     
			//	  219702,          221225,       84608,       5482,6578,     4803, 4804,   95868,         108869,        402, 223675,             
			//	  // Bees-Wasps    Plague-Hands  Azmo Pools   Azmo fireball  Azmo bodies   Belial 1       Belial 2      
			//	  5212,            3865,         123124,      123842,        123839,       161822,        161833, 
			//	  // Sha-Ball      Mol Ball      Mage Fire    Diablo Prison  Diablo Meteor Ice-trail      PoisonGas
			//	  4103,            160154,       432,         168031,        214845,       260377,        4176,
			//	  //lacuni bomb		Succubus Bloodstar	  Halls Of Agony: Inferno Wall
			//	  4546,			   164829,						  89578,
			//	  //Lightning Orbiter Projectile		Lightning Orbiter Focal Point
			//	  343539,								343582,
			//	  //Frozen Pulse
			//	  349774,
			//	  //Thunderstorm
			//	  341512,
			//	  //Meteor Impact
			//	  185366,
			//	  //Bloodspring (small)
			//	  332924,
			//	  //Bloodspring (medium)
			//	  332922,
			//	  //Bloodspring (Large)
			//	  332923,
			//	  //Teleport
			//	  337109,
				  
			//	  335505, // x1_malthael_drainSoul_ghost
			//	  325136, // x1_Malthael_DeathFogMonster
			//	  340512, // x1_Malthael_Mephisto_LightningObject

			//	  360738, // X1_Adria_arcanePool
			//	  358404, // X1_Adria_blood_large

			//	  360046, //X1_Unique_Monster_Generic_AOE_DOT_Poison

			//	  93837, //Ghom's Gluttony_gasCloud

			//	  134831, //A4 Destroyer Drop Location
			//};

		}

		public void ClearCollections()
		{
			AvoidanceCache.Clear();
		}

		private static readonly string DefaultFilePath = Path.Combine(FolderPaths.PluginPath, "Cache", "External", "Dictionaries", "Cache_Avoidance.xml");
		internal static AvoidanceDataCollection DeserializeFromXML()
		{
			var deserializer = new XmlSerializer(typeof(AvoidanceDataCollection));
			TextReader textReader = new StreamReader(DefaultFilePath);
			var settings = (AvoidanceDataCollection)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
		internal static void SerializeToXML(AvoidanceDataCollection settings)
		{
			var serializer = new XmlSerializer(typeof(AvoidanceDataCollection));
			var textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
	}
}
