using System;
using Zeta;
using System.Collections.Generic;
using Zeta.Internals.Actors;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  public static class CacheMovementTracking
		  {
				// A list of small areas covering zones we move through while fighting to help our custom move-handler skip ahead waypoints
				public class SkipAheadNavigation
				{
					 public Vector3 Position { get; set; }
					 public float Radius { get; set; }

					 public SkipAheadNavigation(Vector3 pos, float radius)
					 {
						  this.Position=pos;
						  this.Radius=radius;
					 }
				}


				private static List<SkipAheadNavigation> SkipAheadAreaCache=new List<SkipAheadNavigation>();
				public static bool bSkipAheadAGo=false;

				private static DateTime lastRecordedSkipAheadCache=DateTime.Today;
				internal static void RecordSkipAheadCachePoint()
				{
					 double millisecondsLastRecord=DateTime.Now.Subtract(lastRecordedSkipAheadCache).TotalMilliseconds;

					 if (millisecondsLastRecord<100)
						  return;
					// else if (millisecondsLastRecord>10000) //10 seconds.. clear cache!
						 // SkipAheadAreaCache.Clear();

					 if (SkipAheadAreaCache.Any(p => p.Position.Distance2D(ZetaDia.Me.Position)<=20f))
						  return;

					 SkipAheadAreaCache.Add(new SkipAheadNavigation(ZetaDia.Me.Position, 20f));

					 lastRecordedSkipAheadCache=DateTime.Now;
				}

				private static List<SkipAheadNavigation> UsedSkipAheadAreaCache=new List<SkipAheadNavigation>();
				internal static void ClearCache()
				{
					 SkipAheadAreaCache.Clear();
					 UsedSkipAheadAreaCache.Clear();
					 lastRecordedSkipAheadCache=DateTime.Now;
				}
				public static bool CheckPositionForSkipping(Vector3 Position)
				{
					 foreach (var v in UsedSkipAheadAreaCache)
					 {
						  if (Position.Distance(v.Position)<=v.Radius)
								return true;
					 }

					 bool valid=false;
					 if (SkipAheadAreaCache.Count>0)
					 {
						  int validIndex=-1;
						  for (int i=0; i<SkipAheadAreaCache.Count-1; i++)
						  {
								SkipAheadNavigation v = SkipAheadAreaCache[i];
								if (Position.Distance(v.Position)<=v.Radius)
								{
									 validIndex=i;
									 valid=true;
									 break;
								}
						  }
						  if (valid&&validIndex>0)
						  {
								UsedSkipAheadAreaCache.Add(SkipAheadAreaCache[validIndex]);
								SkipAheadAreaCache.RemoveRange(0, validIndex-1);
								SkipAheadAreaCache.TrimExcess();
						  }
					 }
					 return valid;
					 
				}

		
		  }


		  private static Dictionary<int, CacheBalance> dictGameBalanceCache=new Dictionary<int, CacheBalance>
       
		  #region GameBalanceIDCache
        {
{-1106917318, new CacheBalance(1, ItemType.CraftingPage, false, FollowerType.None)}, //Lore_Book_Flippy-10246    - Could be an error item!?!?
{181033993, new CacheBalance(62, ItemType.Crossbow, false, FollowerType.None)}, //XBow_norm_base_flippy_07-946
{-970366835, new CacheBalance(61, ItemType.CraftingPage, false, FollowerType.None)}, //Lore_Book_Flippy-898
{126259833, new CacheBalance(1, ItemType.Unknown, false, FollowerType.None)}, //GoldSmall-914
{126259831, new CacheBalance(1, ItemType.Unknown, false, FollowerType.None)}, //GoldCoin-1881
{126259832, new CacheBalance(1, ItemType.Unknown, false, FollowerType.None)}, //GoldCoins-1007
{126259834, new CacheBalance(1, ItemType.Unknown, false, FollowerType.None)}, //GoldMedium-657
{126259835, new CacheBalance(1, ItemType.Unknown, false, FollowerType.None)}, //GoldLarge-7981
{-1483610851, new CacheBalance(60, ItemType.Potion, false, FollowerType.None)}, //healthPotion_Mythic-776
{-1962741247, new CacheBalance(1, ItemType.Unknown, false, FollowerType.None)}, //HealthGlobe-580
{1661414572, new CacheBalance(61, ItemType.Axe, true, FollowerType.None)}, //Axe_norm_base_flippy_05-495
{-1533912123, new CacheBalance(61, ItemType.Gloves, false, FollowerType.None)}, //Gloves_norm_base_flippy-1995
{-330720411, new CacheBalance(63, ItemType.Shield, false, FollowerType.None)}, //Shield_norm_base_flippy_07-1580
{-733829188, new CacheBalance(61, ItemType.Belt, false, FollowerType.None)}, //Belt_norm_base_flippy-1696
{2140882331, new CacheBalance(60, ItemType.Boots, false, FollowerType.None)}, //Boots_norm_base_flippy-1701
{-1616888606, new CacheBalance(55, ItemType.Mace, false, FollowerType.None)}, //twoHandedMace_norm_base_flippy_02-1697
{1565456762, new CacheBalance(60, ItemType.Helm, false, FollowerType.None)}, //Helm_norm_base_flippy-3182
{2140882332, new CacheBalance(61, ItemType.Boots, false, FollowerType.None)}, //Boots_norm_base_flippy-3189
{-2115689173, new CacheBalance(62, ItemType.Staff, false, FollowerType.None)}, //Staff_norm_base_flippy_06-3325
{-1962741209, new CacheBalance(1, ItemType.Unknown, false, FollowerType.None)}, //HealthGlobe_02-3320
{2140882334, new CacheBalance(63, ItemType.Boots, false, FollowerType.None)}, //Boots_norm_base_flippy-3495
{40857596, new CacheBalance(56, ItemType.Cloak, false, FollowerType.None)}, //chestArmor_norm_base_flippy-3571
{2058771892, new CacheBalance(54, ItemType.Gem, false, FollowerType.None)}, //Topaz_07-3299
{-1303413119, new CacheBalance(62, ItemType.Dagger, true, FollowerType.None)}, //Dagger_norm_base_flippy_06-3766
{-2115689174, new CacheBalance(61, ItemType.Staff, false, FollowerType.None)}, //Staff_norm_base_flippy_05-3744
{1565456763, new CacheBalance(61, ItemType.Helm, false, FollowerType.None)}, //Helm_norm_base_flippy-3738
{290068679, new CacheBalance(57, ItemType.MightyWeapon, true, FollowerType.None)}, //mightyWeapon_1H_norm_base_flippy_01-3850
{-1533912124, new CacheBalance(60, ItemType.Gloves, false, FollowerType.None)}, //Gloves_norm_base_flippy-3917
{88667232, new CacheBalance(61, ItemType.Wand, true, FollowerType.None)}, //Wand_norm_base_flippy_05-4365
{1146967348, new CacheBalance(62, ItemType.Ring, false, FollowerType.None)}, //Ring_flippy-4767
{-1512729955, new CacheBalance(63, ItemType.Legs, false, FollowerType.None)}, //pants_norm_base_flippy-4731
{-1303413123, new CacheBalance(55, ItemType.Dagger, true, FollowerType.None)}, //Dagger_norm_base_flippy_02-4517
{1700549963, new CacheBalance(61, ItemType.Axe, false, FollowerType.None)}, //twoHandedAxe_norm_base_flippy_03-4584
{-2115689176, new CacheBalance(57, ItemType.Staff, false, FollowerType.None)}, //Staff_norm_base_flippy_03-4601
{-335464095, new CacheBalance(59, ItemType.Shield, false, FollowerType.None)}, //Shield_norm_base_flippy_03-4602
{-733830276, new CacheBalance(52, ItemType.Belt, false, FollowerType.None)}, //Belt_norm_base_flippy-4600
{620036246, new CacheBalance(61, ItemType.VoodooMask, false, FollowerType.None)}, //Helm_norm_base_flippy-4950
{-1616888603, new CacheBalance(62, ItemType.Mace, false, FollowerType.None)}, //twoHandedMace_norm_base_flippy_05-4944
{1682228653, new CacheBalance(62, ItemType.Amulet, false, FollowerType.None)}, //Amulet_norm_base_flippy-5681
{-733829189, new CacheBalance(60, ItemType.Belt, false, FollowerType.None)}, //Belt_norm_base_flippy-6048
{-1303413121, new CacheBalance(59, ItemType.Dagger, true, FollowerType.None)}, //Dagger_norm_base_flippy_04-1383
{1815809035, new CacheBalance(55, ItemType.Shield, false, FollowerType.None)}, //Shield_norm_base_flippy_01-2564
{1565456765, new CacheBalance(63, ItemType.Helm, false, FollowerType.None)}, //Helm_norm_base_flippy-3402
{1603007817, new CacheBalance(60, ItemType.Gem, false, FollowerType.None)}, //Ruby_08-3158
{1236607149, new CacheBalance(61, ItemType.FistWeapon, true, FollowerType.None)}, //fistWeapon_norm_base_flippy_02-3659
{1146967346, new CacheBalance(60, ItemType.Ring, false, FollowerType.None)}, //Ring_flippy-3918
{-1337761336, new CacheBalance(62, ItemType.Polearm, false, FollowerType.None)}, //Polearm_norm_base_flippy_07-4052
{181033988, new CacheBalance(53, ItemType.Crossbow, false, FollowerType.None)}, //XBow_norm_base_flippy_02-4078
{-1337761340, new CacheBalance(56, ItemType.Polearm, false, FollowerType.None)}, //Polearm_norm_base_flippy_03-5687
{2140882330, new CacheBalance(58, ItemType.Boots, false, FollowerType.None)}, //Boots_norm_base_flippy-5685
{1565456761, new CacheBalance(58, ItemType.Helm, false, FollowerType.None)}, //Helm_norm_base_flippy-5684
{-1533912125, new CacheBalance(58, ItemType.Gloves, false, FollowerType.None)}, //Gloves_norm_base_flippy-6368
{-270936739, new CacheBalance(59, ItemType.Sword, true, FollowerType.None)}, //Sword_norm_base_flippy_03-6486
{-1337761339, new CacheBalance(58, ItemType.Polearm, false, FollowerType.None)}, //Polearm_norm_base_flippy_04-7960
{-2091501889, new CacheBalance(63, ItemType.Bow, false, FollowerType.None)}, //Bow_norm_base_flippy_06-7966
{-101310578, new CacheBalance(58, ItemType.Spear, true, FollowerType.None)}, //Spear_norm_base_flippy_02-7962
{1755623811, new CacheBalance(62, ItemType.WizardHat, false, FollowerType.None)}, //HelmCloth_norm_base_flippy-8781
{1236607148, new CacheBalance(60, ItemType.FistWeapon, true, FollowerType.None)}, //fistWeapon_norm_base_flippy_03-8789
{-1733388799, new CacheBalance(60, ItemType.Gem, false, FollowerType.None)}, //Emerald_08-10718
{1682228651, new CacheBalance(60, ItemType.Amulet, false, FollowerType.None)}, //Amulet_norm_base_flippy-10727
{-136815383, new CacheBalance(53, ItemType.Mojo, false, FollowerType.None)}, //Mojo_norm_base_flippy_04-10810
{1700549962, new CacheBalance(59, ItemType.Axe, false, FollowerType.None)}, //twoHandedAxe_norm_base_flippy_02-10812
{2112157586, new CacheBalance(61, ItemType.MightyBelt, false, FollowerType.None)}, //Belt_norm_base_flippy-11311
{-1533912121, new CacheBalance(63, ItemType.Gloves, false, FollowerType.None)}, //Gloves_norm_base_flippy-11314
{181033989, new CacheBalance(54, ItemType.Crossbow, false, FollowerType.None)}, //XBow_norm_base_flippy_03-12195
{2140882329, new CacheBalance(55, ItemType.Boots, false, FollowerType.None)}, //Boots_norm_base_flippy-12217
{-1303413122, new CacheBalance(57, ItemType.Dagger, true, FollowerType.None)}, //Dagger_norm_base_flippy_03-13817
{-1303413120, new CacheBalance(61, ItemType.Dagger, true, FollowerType.None)}, //Dagger_norm_base_flippy_05-14074
{-270936738, new CacheBalance(61, ItemType.Sword, true, FollowerType.None)}, //Sword_norm_base_flippy_04-14072
{-1337761337, new CacheBalance(61, ItemType.Polearm, false, FollowerType.None)}, //Polearm_norm_base_flippy_06-14964
{365492431, new CacheBalance(62, ItemType.Shoulder, false, FollowerType.None)}, //shoulderPads_norm_base_flippy-1437
{1565456764, new CacheBalance(62, ItemType.Helm, false, FollowerType.None)}, //Helm_norm_base_flippy-1315
{-101310577, new CacheBalance(61, ItemType.Spear, true, FollowerType.None)}, //Spear_norm_base_flippy_03-3318
{-1512729959, new CacheBalance(58, ItemType.Legs, false, FollowerType.None)}, //pants_norm_base_flippy-7736
{-875942695, new CacheBalance(63, ItemType.Bracer, false, FollowerType.None)}, //Bracers_norm_base_06-8312
{-2015049108, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-11075
{-733829186, new CacheBalance(63, ItemType.Belt, false, FollowerType.None)}, //Belt_norm_base_flippy-11066
{2140882333, new CacheBalance(62, ItemType.Boots, false, FollowerType.None)}, //Boots_norm_base_flippy-11993
{1539238478, new CacheBalance(56, ItemType.Quiver, false, FollowerType.None)}, //Quiver_norm_base_flippy_01-12027
{-733829187, new CacheBalance(62, ItemType.Belt, false, FollowerType.None)}, //Belt_norm_base_flippy-13327
{-231801347, new CacheBalance(61, ItemType.Sword, false, FollowerType.None)}, //twoHandedSword_norm_base_flippy_05-14009
{-331906332, new CacheBalance(62, ItemType.Shield, false, FollowerType.None)}, //Shield_norm_base_flippy_06-15167
{-1616888604, new CacheBalance(61, ItemType.Mace, false, FollowerType.None)}, //twoHandedMace_norm_base_flippy_04-16886
{-875942700, new CacheBalance(55, ItemType.Bracer, false, FollowerType.None)}, //Bracers_norm_base_01-668
{-1337761335, new CacheBalance(63, ItemType.Polearm, false, FollowerType.None)}, //Polearm_norm_base_flippy_08-1354
{1700549964, new CacheBalance(62, ItemType.Axe, false, FollowerType.None)}, //twoHandedAxe_norm_base_flippy_04-1346
{-363389486, new CacheBalance(61, ItemType.HandCrossbow, true, FollowerType.None)}, //handXBow_norm_base_flippy_06-1802
{-1512729958, new CacheBalance(60, ItemType.Legs, false, FollowerType.None)}, //pants_norm_base_flippy-3342
{-2115689175, new CacheBalance(59, ItemType.Staff, false, FollowerType.None)}, //Staff_norm_base_flippy_04-3662
{-1411866890, new CacheBalance(60, ItemType.Gem, false, FollowerType.None)}, //Amethyst_08-3681
{2058771893, new CacheBalance(60, ItemType.Gem, false, FollowerType.None)}, //Topaz_08-3683
{-875942698, new CacheBalance(60, ItemType.Bracer, false, FollowerType.None)}, //Bracers_norm_base_03-5860
{1565456760, new CacheBalance(55, ItemType.Helm, false, FollowerType.None)}, //Helm_norm_base_flippy-7297
{-1512729957, new CacheBalance(61, ItemType.Legs, false, FollowerType.None)}, //pants_norm_base_flippy-8321
{-875942699, new CacheBalance(58, ItemType.Bracer, false, FollowerType.None)}, //Bracers_norm_base_02-9464
{-1337761341, new CacheBalance(54, ItemType.Polearm, false, FollowerType.None)}, //Polearm_norm_base_flippy_02-9531
{-363389487, new CacheBalance(60, ItemType.HandCrossbow, true, FollowerType.None)}, //handXBow_norm_base_flippy_05-9530
{-229899869, new CacheBalance(57, ItemType.FollowerSpecial, false, FollowerType.Scoundrel)}, //JewelBox_Flippy-10528
{-733829191, new CacheBalance(55, ItemType.Belt, false, FollowerType.None)}, //Belt_norm_base_flippy-10549
{1146967347, new CacheBalance(61, ItemType.Ring, false, FollowerType.None)}, //Ring_flippy-10553
{1612259883, new CacheBalance(58, ItemType.Chest, false, FollowerType.None)}, //chestArmor_norm_base_flippy-13213
{1682228652, new CacheBalance(61, ItemType.Amulet, false, FollowerType.None)}, //Amulet_norm_base_flippy-13904
{-101310576, new CacheBalance(62, ItemType.Spear, true, FollowerType.None)}, //Spear_norm_base_flippy_04-14801
{-333092253, new CacheBalance(61, ItemType.Shield, false, FollowerType.None)}, //Shield_norm_base_flippy_05-15478
{-875942696, new CacheBalance(62, ItemType.Bracer, false, FollowerType.None)}, //Bracers_norm_base_05-2766
{1661414569, new CacheBalance(56, ItemType.Axe, true, FollowerType.None)}, //Axe_norm_base_flippy_02-4779
{-363389485, new CacheBalance(62, ItemType.HandCrossbow, true, FollowerType.None)}, //handXBow_norm_base_flippy_07-6008
{2140881244, new CacheBalance(52, ItemType.Boots, false, FollowerType.None)}, //Boots_norm_base_flippy-6020
{290068680, new CacheBalance(61, ItemType.MightyWeapon, true, FollowerType.None)}, //mightyWeapon_1H_norm_base_flippy_02-6907
{365492432, new CacheBalance(63, ItemType.Shoulder, false, FollowerType.None)}, //shoulderPads_norm_base_flippy-7287
{-1656025083, new CacheBalance(51, ItemType.Mace, true, FollowerType.None)}, //Mace_norm_base_flippy_07-14357
{-635267403, new CacheBalance(60, ItemType.CeremonialDagger, true, FollowerType.None)}, //ceremonialDagger_norm_base_flippy_03-14369
{-242893289, new CacheBalance(60, ItemType.SpiritStone, false, FollowerType.None)}, //Helm_norm_base_flippy-17237
{1539238479, new CacheBalance(59, ItemType.Quiver, false, FollowerType.None)}, //Quiver_norm_base_flippy_01-17846
{1612259885, new CacheBalance(61, ItemType.Chest, false, FollowerType.None)}, //chestArmor_norm_base_flippy-4464
{1612259886, new CacheBalance(62, ItemType.Chest, false, FollowerType.None)}, //chestArmor_norm_base_flippy-7114
{290068681, new CacheBalance(62, ItemType.MightyWeapon, true, FollowerType.None)}, //mightyWeapon_1H_norm_base_flippy_03-7147
{-1616888605, new CacheBalance(58, ItemType.Mace, false, FollowerType.None)}, //twoHandedMace_norm_base_flippy_03-7560
{1146967345, new CacheBalance(57, ItemType.Ring, false, FollowerType.None)}, //Ring_flippy-7571
{-2091501894, new CacheBalance(52, ItemType.Bow, false, FollowerType.None)}, //Bow_norm_base_flippy_01-7570
{1612259882, new CacheBalance(55, ItemType.Chest, false, FollowerType.None)}, //chestArmor_norm_base_flippy-7569
{1661414570, new CacheBalance(58, ItemType.Axe, true, FollowerType.None)}, //Axe_norm_base_flippy_03-7578
{-363389488, new CacheBalance(58, ItemType.HandCrossbow, true, FollowerType.None)}, //handXBow_norm_base_flippy_04-9381
{1147341802, new CacheBalance(60, ItemType.FollowerSpecial, false, FollowerType.Templar)}, //JewelBox_Flippy-10391
{-1512729960, new CacheBalance(55, ItemType.Legs, false, FollowerType.None)}, //pants_norm_base_flippy-13909
{365492429, new CacheBalance(60, ItemType.Shoulder, false, FollowerType.None)}, //shoulderPads_norm_base_flippy-13913
{-101310575, new CacheBalance(63, ItemType.Spear, true, FollowerType.None)}, //Spear_norm_base_flippy_05-15926
{88667230, new CacheBalance(58, ItemType.Wand, true, FollowerType.None)}, //Wand_norm_base_flippy_03-17386
{-1656023995, new CacheBalance(62, ItemType.Mace, true, FollowerType.None)}, //Mace_norm_base_flippy_06-17434
{-363389490, new CacheBalance(54, ItemType.HandCrossbow, true, FollowerType.None)}, //handXbow_norm_base_flippy_02-19180
{365491342, new CacheBalance(52, ItemType.Shoulder, false, FollowerType.None)}, //shoulderPads_norm_base_flippy-19272
{365492430, new CacheBalance(61, ItemType.Shoulder, false, FollowerType.None)}, //shoulderPads_norm_base_flippy-20609
{-136814295, new CacheBalance(61, ItemType.Mojo, false, FollowerType.None)}, //Mojo_norm_base_flippy_03-24992
{-1512729956, new CacheBalance(62, ItemType.Legs, false, FollowerType.None)}, //pants_norm_base_flippy-3581
{181033991, new CacheBalance(59, ItemType.Crossbow, false, FollowerType.None)}, //XBow_norm_base_flippy_05-5325
{-2091501892, new CacheBalance(58, ItemType.Bow, false, FollowerType.None)}, //Bow_norm_base_flippy_03-6306
{1565455675, new CacheBalance(52, ItemType.Helm, false, FollowerType.None)}, //Helm_norm_base_flippy-2176
{181033992, new CacheBalance(61, ItemType.Crossbow, false, FollowerType.None)}, //XBow_norm_base_flippy_06-3659
{1661413485, new CacheBalance(52, ItemType.Axe, true, FollowerType.None)}, //Axe_norm_base_flippy_07-3991
{-1533912122, new CacheBalance(62, ItemType.Gloves, false, FollowerType.None)}, //Gloves_norm_base_flippy-3996
{2112157587, new CacheBalance(62, ItemType.MightyBelt, false, FollowerType.None)}, //Belt_norm_base_flippy-6378
{-1303413118, new CacheBalance(63, ItemType.Dagger, true, FollowerType.None)}, //Dagger_norm_base_flippy_07-8259
{-1733388800, new CacheBalance(54, ItemType.Gem, false, FollowerType.None)}, //Emerald_07-8261
{1612258797, new CacheBalance(52, ItemType.Chest, false, FollowerType.None)}, //chestArmor_norm_base_flippy-8260
{1682228650, new CacheBalance(55, ItemType.Amulet, false, FollowerType.None)}, //Amulet_norm_base_flippy-9071
{-1411866891, new CacheBalance(54, ItemType.Gem, false, FollowerType.None)}, //Amethyst_07-2037
{-1512731045, new CacheBalance(52, ItemType.Legs, false, FollowerType.None)}, //pants_norm_base_flippy-2899
{88667231, new CacheBalance(60, ItemType.Wand, true, FollowerType.None)}, //Wand_norm_base_flippy_04-3797
{-2115689177, new CacheBalance(55, ItemType.Staff, false, FollowerType.None)}, //Staff_norm_base_flippy_02-5676
{-1656023997, new CacheBalance(59, ItemType.Mace, true, FollowerType.None)}, //Mace_norm_base_flippy_04-11365
{-136814294, new CacheBalance(62, ItemType.Mojo, false, FollowerType.None)}, //Mojo_norm_base_flippy_04-11892
{329204073, new CacheBalance(61, ItemType.MightyWeapon, false, FollowerType.None)}, //mightyWeapon_2H_norm_base_flippy_02-1331
{-270936736, new CacheBalance(63, ItemType.Sword, true, FollowerType.None)}, //Sword_norm_base_flippy_07-2509
{-1616888602, new CacheBalance(63, ItemType.Mace, false, FollowerType.None)}, //twoHandedMace_norm_base_flippy_06-3566
{-1656023999, new CacheBalance(55, ItemType.Mace, true, FollowerType.None)}, //Mace_norm_base_flippy_02-6299
{329204072, new CacheBalance(56, ItemType.MightyWeapon, false, FollowerType.None)}, //mightyWeapon_2H_norm_base_flippy_01-6293
{-733829190, new CacheBalance(58, ItemType.Belt, false, FollowerType.None)}, //Belt_norm_base_flippy-7011
{1661414573, new CacheBalance(62, ItemType.Axe, true, FollowerType.None)}, //Axe_norm_base_flippy_06-8628
{181033994, new CacheBalance(63, ItemType.Crossbow, false, FollowerType.None)}, //XBow_norm_base_flippy_08-9102
{1603007816, new CacheBalance(54, ItemType.Gem, false, FollowerType.None)}, //Ruby_07-11231
{-1962741148, new CacheBalance(1, ItemType.Unknown, false, FollowerType.None)}, //HealthGlobe_02-15599
{-334278174, new CacheBalance(60, ItemType.Shield, false, FollowerType.None)}, //Shield_norm_base_flippy_04-11811
{1539238480, new CacheBalance(60, ItemType.Quiver, false, FollowerType.None)}, //Quiver_norm_base_flippy_01-13479
{-1656023996, new CacheBalance(61, ItemType.Mace, true, FollowerType.None)}, //Mace_norm_base_flippy_05-16080
{1815809036, new CacheBalance(57, ItemType.Shield, false, FollowerType.None)}, //Shield_norm_base_flippy_02-16556
{40857598, new CacheBalance(61, ItemType.Cloak, false, FollowerType.None)}, //chestArmor_norm_base_flippy-18151
{329204074, new CacheBalance(62, ItemType.MightyWeapon, false, FollowerType.None)}, //mightyWeapon_2H_norm_base_flippy_03-18904
{-1656023998, new CacheBalance(57, ItemType.Mace, true, FollowerType.None)}, //Mace_norm_base_flippy_03-19117
{329202986, new CacheBalance(51, ItemType.MightyWeapon, false, FollowerType.None)}, //mightyWeapon_2H_norm_base_flippy_04-21596
{1755622722, new CacheBalance(53, ItemType.WizardHat, false, FollowerType.None)}, //HelmCloth_norm_base_flippy-1940
{1236607150, new CacheBalance(62, ItemType.FistWeapon, true, FollowerType.None)}, //fistWeapon_norm_base_flippy_03-3679
{-231801345, new CacheBalance(63, ItemType.Sword, false, FollowerType.None)}, //twoHandedSword_norm_base_flippy_06-7909
{365492428, new CacheBalance(58, ItemType.Shoulder, false, FollowerType.None)}, //shoulderPads_norm_base_flippy-8662
{620036244, new CacheBalance(56, ItemType.VoodooMask, false, FollowerType.None)}, //Helm_norm_base_flippy-11733
{88667228, new CacheBalance(54, ItemType.Wand, true, FollowerType.None)}, //Wand_norm_base_flippy_01-19036
{-875942697, new CacheBalance(61, ItemType.Bracer, false, FollowerType.None)}, //Bracers_norm_base_04-21441
{-363389484, new CacheBalance(63, ItemType.HandCrossbow, true, FollowerType.None)}, //handXBow_norm_base_flippy_08-21449
{-270936737, new CacheBalance(62, ItemType.Sword, true, FollowerType.None)}, //Sword_norm_base_flippy_08-3422
{1661414568, new CacheBalance(54, ItemType.Axe, true, FollowerType.None)}, //Axe_norm_base_flippy_01-4034
{2112156498, new CacheBalance(53, ItemType.MightyBelt, false, FollowerType.None)}, //Belt_norm_base_flippy-4089
{-2091501890, new CacheBalance(62, ItemType.Bow, false, FollowerType.None)}, //Bow_norm_base_flippy_05-5750
{1539238481, new CacheBalance(61, ItemType.Quiver, false, FollowerType.None)}, //Quiver_norm_base_flippy_01-7305
{-363389489, new CacheBalance(56, ItemType.HandCrossbow, true, FollowerType.None)}, //handXBow_norm_base_flippy_03-7443
{-1533912126, new CacheBalance(55, ItemType.Gloves, false, FollowerType.None)}, //Gloves_norm_base_flippy-7516
{-2091501893, new CacheBalance(55, ItemType.Bow, false, FollowerType.None)}, //Bow_norm_base_flippy_02-10478
{-231801349, new CacheBalance(55, ItemType.Sword, false, FollowerType.None)}, //twoHandedSword_norm_base_flippy_03-10866
{-231801348, new CacheBalance(58, ItemType.Sword, false, FollowerType.None)}, //twoHandedSword_norm_base_flippy_02-15614
{-1337761342, new CacheBalance(52, ItemType.Polearm, false, FollowerType.None)}, //Polearm_norm_base_flippy_01-15747
{620036245, new CacheBalance(60, ItemType.VoodooMask, false, FollowerType.None)}, //Helm_norm_base_flippy-16062
{1755623809, new CacheBalance(60, ItemType.WizardHat, false, FollowerType.None)}, //HelmCloth_norm_base_flippy-18920
{-242893288, new CacheBalance(61, ItemType.SpiritStone, false, FollowerType.None)}, //Helm_norm_base_flippy-18926
{1612259884, new CacheBalance(60, ItemType.Chest, false, FollowerType.None)}, //chestArmor_norm_base_flippy-22557
{181033990, new CacheBalance(56, ItemType.Crossbow, false, FollowerType.None)}, //XBow_norm_base_flippy_04-22861
{1661414571, new CacheBalance(60, ItemType.Axe, true, FollowerType.None)}, //Axe_norm_base_flippy_04-2058
{1146967320, new CacheBalance(51, ItemType.Ring, false, FollowerType.None)}, //Ring_flippy-5049
{-229899868, new CacheBalance(60, ItemType.FollowerSpecial, false, FollowerType.Scoundrel)}, //JewelBox_Flippy-7661
{1146967321, new CacheBalance(54, ItemType.Ring, false, FollowerType.None)}, //Ring_flippy-8882
{-242893290, new CacheBalance(56, ItemType.SpiritStone, false, FollowerType.None)}, //Helm_norm_base_flippy-8875
{761439027, new CacheBalance(60, ItemType.FollowerSpecial, false, FollowerType.Enchantress)}, //JewelBox_Flippy-12503
{-231801346, new CacheBalance(62, ItemType.Sword, false, FollowerType.None)}, //twoHandedSword_norm_base_flippy_04-18285
{-1656024000, new CacheBalance(53, ItemType.Mace, true, FollowerType.None)}, //Mace_norm_base_flippy_01-18617
{1755623808, new CacheBalance(56, ItemType.WizardHat, false, FollowerType.None)}, //HelmCloth_norm_base_flippy-1483
{40857597, new CacheBalance(60, ItemType.Cloak, false, FollowerType.None)}, //chestArmor_norm_base_flippy-8723
{-875943785, new CacheBalance(52, ItemType.Bracer, false, FollowerType.None)}, //Bracers_norm_base_05-11841
{1700549961, new CacheBalance(55, ItemType.Axe, false, FollowerType.None)}, //twoHandedAxe_norm_base_flippy_01-14697
{-1656023994, new CacheBalance(63, ItemType.Mace, true, FollowerType.None)}, //Mace_norm_base_flippy_07-23400
{-2115690261, new CacheBalance(51, ItemType.Staff, false, FollowerType.None)}, //Staff_norm_base_flippy_07-440
{1771751032, new CacheBalance(61, ItemType.Daibo, false, FollowerType.None)}, //combatStaff_norm_base_flippy_02-9926
{-270936742, new CacheBalance(53, ItemType.Sword, true, FollowerType.None)}, //Sword_norm_base_flippy_02-11499
{-2091501891, new CacheBalance(61, ItemType.Bow, false, FollowerType.None)}, //Bow_norm_base_flippy_04-15879
{1612259887, new CacheBalance(63, ItemType.Chest, false, FollowerType.None)}, //chestArmor_norm_base_flippy-16971
{88667229, new CacheBalance(56, ItemType.Wand, true, FollowerType.None)}, //Wand_norm_base_flippy_02-17893
{-242893287, new CacheBalance(62, ItemType.SpiritStone, false, FollowerType.None)}, //Helm_norm_base_flippy-21766
{-635267401, new CacheBalance(62, ItemType.CeremonialDagger, true, FollowerType.None)}, //ceremonialDagger_norm_base_flippy_03-5833
{1661414574, new CacheBalance(63, ItemType.Axe, true, FollowerType.None)}, //Axe_norm_base_flippy_07-7199
{1771751034, new CacheBalance(63, ItemType.Daibo, false, FollowerType.None)}, //combatStaff_norm_base_flippy_04-11249
{2112157584, new CacheBalance(56, ItemType.MightyBelt, false, FollowerType.None)}, //Belt_norm_base_flippy-11552
{40857599, new CacheBalance(62, ItemType.Cloak, false, FollowerType.None)}, //chestArmor_norm_base_flippy-14809
{-363389491, new CacheBalance(52, ItemType.HandCrossbow, true, FollowerType.None)}, //handXbow_norm_base_flippy_01-15178
{-2115689172, new CacheBalance(63, ItemType.Staff, false, FollowerType.None)}, //Staff_norm_base_flippy_07-17487
{1755623810, new CacheBalance(61, ItemType.WizardHat, false, FollowerType.None)}, //HelmCloth_norm_base_flippy-21671
{1539237393, new CacheBalance(53, ItemType.Quiver, false, FollowerType.None)}, //Quiver_norm_base_flippy_01-23717
{1236607147, new CacheBalance(56, ItemType.FistWeapon, true, FollowerType.None)}, //fistWeapon_norm_base_flippy_02-8621
{1905181656, new CacheBalance(62, ItemType.Orb, false, FollowerType.None)}, //orb_norm_base_flippy_04-5332
{620035158, new CacheBalance(53, ItemType.VoodooMask, false, FollowerType.None)}, //Helm_norm_base_flippy-490
{1288600121, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-8240
{1147341801, new CacheBalance(57, ItemType.FollowerSpecial, false, FollowerType.Templar)}, //JewelBox_Flippy-6201
{-635267400, new CacheBalance(63, ItemType.CeremonialDagger, true, FollowerType.None)}, //ceremonialDagger_norm_base_flippy_04-24457
{1700549965, new CacheBalance(63, ItemType.Axe, false, FollowerType.None)}, //twoHandedAxe_norm_base_flippy_05-7304
{-2115689178, new CacheBalance(53, ItemType.Staff, false, FollowerType.None)}, //Staff_norm_base_flippy_01-20183
{365492427, new CacheBalance(55, ItemType.Shoulder, false, FollowerType.None)}, //shoulderPads_norm_base_flippy-1374
{290067593, new CacheBalance(52, ItemType.MightyWeapon, true, FollowerType.None)}, //mightyWeapon_1H_norm_base_flippy_04-6474
{-136814296, new CacheBalance(60, ItemType.Mojo, false, FollowerType.None)}, //Mojo_norm_base_flippy_02-9438
{-1533913211, new CacheBalance(52, ItemType.Gloves, false, FollowerType.None)}, //Gloves_norm_base_flippy-15515
{-270936741, new CacheBalance(55, ItemType.Sword, true, FollowerType.None)}, //Sword_norm_base_flippy_05-23963
{-101310579, new CacheBalance(55, ItemType.Spear, true, FollowerType.None)}, //Spear_norm_base_flippy_01-3836
{88667233, new CacheBalance(62, ItemType.Wand, true, FollowerType.None)}, //Wand_norm_base_flippy_06-5434
{1905181655, new CacheBalance(61, ItemType.Orb, false, FollowerType.None)}, //orb_norm_base_flippy_03-18857
{1236607151, new CacheBalance(63, ItemType.FistWeapon, true, FollowerType.None)}, //fistWeapon_norm_base_flippy_04-4638
{2112157585, new CacheBalance(60, ItemType.MightyBelt, false, FollowerType.None)}, //Belt_norm_base_flippy-688
{88667234, new CacheBalance(63, ItemType.Wand, true, FollowerType.None)}, //Wand_norm_base_flippy_07-2315
{761439026, new CacheBalance(57, ItemType.FollowerSpecial, false, FollowerType.Enchantress)}, //JewelBox_Flippy-5680
{-1337761338, new CacheBalance(60, ItemType.Polearm, false, FollowerType.None)}, //Polearm_norm_base_flippy_05-1977
{1236607146, new CacheBalance(52, ItemType.FistWeapon, true, FollowerType.None)}, //fistWeapon_norm_base_flippy_01-6741
{-635267402, new CacheBalance(61, ItemType.CeremonialDagger, true, FollowerType.None)}, //ceremonialDagger_norm_base_flippy_02-682
{1771751033, new CacheBalance(62, ItemType.Daibo, false, FollowerType.None)}, //combatStaff_norm_base_flippy_03-1186
{290068682, new CacheBalance(63, ItemType.MightyWeapon, true, FollowerType.None)}, //mightyWeapon_1H_norm_base_flippy_04-1455
{1815807951, new CacheBalance(51, ItemType.Shield, false, FollowerType.None)}, //Shield_norm_base_flippy_06-7216
{-242894376, new CacheBalance(53, ItemType.SpiritStone, false, FollowerType.None)}, //Helm_norm_base_flippy-7932
{-1303413124, new CacheBalance(53, ItemType.Dagger, true, FollowerType.None)}, //Dagger_norm_base_flippy_01-7929
{-136814297, new CacheBalance(56, ItemType.Mojo, false, FollowerType.None)}, //Mojo_norm_base_flippy_01-10670
{-1303414207, new CacheBalance(51, ItemType.Dagger, true, FollowerType.None)}, //Dagger_norm_base_flippy_07-15282
{-270936740, new CacheBalance(57, ItemType.Sword, true, FollowerType.None)}, //Sword_norm_base_flippy_06-22060
{88666145, new CacheBalance(52, ItemType.Wand, true, FollowerType.None)}, //Wand_norm_base_flippy_07-25299
{-275669100, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-6906
{1815807952, new CacheBalance(53, ItemType.Shield, false, FollowerType.None)}, //Shield_norm_base_flippy_07-45607
{1905181654, new CacheBalance(60, ItemType.Orb, false, FollowerType.None)}, //orb_norm_base_flippy_02-3074
{-231801350, new CacheBalance(52, ItemType.Sword, false, FollowerType.None)}, //twoHandedSword_norm_base_flippy_01-10063
{620036247, new CacheBalance(62, ItemType.VoodooMask, false, FollowerType.None)}, //Helm_norm_base_flippy-17813
{-229899870, new CacheBalance(54, ItemType.FollowerSpecial, false, FollowerType.Scoundrel)}, //JewelBox_Flippy-66005
{-270936743, new CacheBalance(51, ItemType.Sword, true, FollowerType.None)}, //Sword_norm_base_flippy_01-9813
{1771751031, new CacheBalance(58, ItemType.Daibo, false, FollowerType.None)}, //combatStaff_norm_base_flippy_03-996
{-635267404, new CacheBalance(56, ItemType.CeremonialDagger, true, FollowerType.None)}, //ceremonialDagger_norm_base_flippy_02-20442
{-1616888607, new CacheBalance(52, ItemType.Mace, false, FollowerType.None)}, //twoHandedMace_norm_base_flippy_01-12628
{1147341800, new CacheBalance(54, ItemType.FollowerSpecial, false, FollowerType.Templar)}, //JewelBox_Flippy-4753
{1539238482, new CacheBalance(62, ItemType.Quiver, false, FollowerType.None)}, //Quiver_norm_base_flippy_01-16011
{761439025, new CacheBalance(54, ItemType.FollowerSpecial, false, FollowerType.Enchantress)}, //JewelBox_Flippy-17285
{1771751030, new CacheBalance(55, ItemType.Daibo, false, FollowerType.None)}, //combatStaff_norm_base_flippy_02-19316
{1771751029, new CacheBalance(52, ItemType.Daibo, false, FollowerType.None)}, //combatStaff_norm_base_flippy_01-10268
{1905180567, new CacheBalance(53, ItemType.Orb, false, FollowerType.None)}, //orb_norm_base_flippy_04-10666
{181032905, new CacheBalance(52, ItemType.Crossbow, false, FollowerType.None)}, //XBow_norm_base_flippy_08-15728
{-246124382, new CacheBalance(63, ItemType.Shoulder, false, FollowerType.None)}, //shoulderPads_norm_base_flippy-9313
{1809242064, new CacheBalance(56, ItemType.Gloves, false, FollowerType.None)}, //Gloves_norm_base_flippy-33597
{-1137443897, new CacheBalance(61, ItemType.Amulet, false, FollowerType.None)}, //Amulet_norm_base_flippy-10421
{1905181653, new CacheBalance(56, ItemType.Orb, false, FollowerType.None)}, //orb_norm_base_flippy_01-18233
{329204075, new CacheBalance(63, ItemType.MightyWeapon, false, FollowerType.None)}, //mightyWeapon_2H_norm_base_flippy_04-14928
{386201988, new CacheBalance(62, ItemType.Sword, false, FollowerType.None)}, //twoHandedSword_norm_base_flippy_07-59260
{-1849339001, new CacheBalance(63, ItemType.Unknown, false, FollowerType.None)}, //Helm_norm_base_flippy-14437
{1700548876, new CacheBalance(51, ItemType.Axe, false, FollowerType.None)}, //twoHandedAxe_norm_base_flippy_05-10421
{40856510, new CacheBalance(53, ItemType.Cloak, false, FollowerType.None)}, //chestArmor_norm_base_flippy-9233
{-635267405, new CacheBalance(52, ItemType.CeremonialDagger, true, FollowerType.None)}, //ceremonialDagger_norm_base_flippy_01-10948
{-101311664, new CacheBalance(52, ItemType.Spear, true, FollowerType.None)}, //Spear_norm_base_flippy_05-2331
{255305004, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-12352
{402571149, new CacheBalance(63, ItemType.Bracer, false, FollowerType.None)}, //Bracers_norm_base_01-7789
{-247310303, new CacheBalance(58, ItemType.Shoulder, false, FollowerType.None)}, //shoulderPads_norm_base_flippy-21253
{-327168932, new CacheBalance(63, ItemType.Quiver, false, FollowerType.None)}, //Quiver_norm_base_flippy_01-49247
{1810427985, new CacheBalance(63, ItemType.Gloves, false, FollowerType.None)}, //Gloves_norm_base_flippy-57737
{399013386, new CacheBalance(60, ItemType.Bracer, false, FollowerType.None)}, //Bracers_norm_base_01-16344
{-494657717, new CacheBalance(63, ItemType.WizardHat, false, FollowerType.None)}, //HelmCloth_norm_base_flippy-19691
{-578170868, new CacheBalance(61, ItemType.Ring, false, FollowerType.None)}, //Ring_flippy-17344
{-1953228509, new CacheBalance(63, ItemType.VoodooMask, false, FollowerType.None)}, //Helm_norm_base_flippy-17168
{-1960344035, new CacheBalance(58, ItemType.VoodooMask, false, FollowerType.None)}, //Helm_norm_base_flippy-18639
{253088131, new CacheBalance(63, ItemType.Legs, false, FollowerType.None)}, //pants_norm_base_flippy-67142
{-1961529956, new CacheBalance(62, ItemType.VoodooMask, false, FollowerType.None)}, //Helm_norm_base_flippy-26460
{1841261931, new CacheBalance(61, ItemType.Gloves, false, FollowerType.None)}, //Gloves_norm_base_flippy-24850
{-1855268606, new CacheBalance(62, ItemType.Unknown, false, FollowerType.None)}, //Helm_norm_base_flippy-69134
{469402902, new CacheBalance(60, ItemType.Chest, false, FollowerType.None)}, //chestArmor_norm_base_flippy-54263
{-1271477944, new CacheBalance(54, ItemType.Cloak, false, FollowerType.None)}, //chestArmor_norm_base_flippy-72607
{761573024, new CacheBalance(1, ItemType.Unknown, false, FollowerType.None)}, //BlackRockLedger01-7009
{761573025, new CacheBalance(1, ItemType.Unknown, false, FollowerType.None)}, //BlackRockLedger02-7561
{761573026, new CacheBalance(1, ItemType.Unknown, false, FollowerType.None)}, //BlackRockLedger03-8774
{761573027, new CacheBalance(1, ItemType.Unknown, false, FollowerType.None)}, //BlackRockLedger04-9277
{761573028, new CacheBalance(1, ItemType.Unknown, false, FollowerType.None)}, //BlackRockLedger05-7156
{761573029, new CacheBalance(1, ItemType.Unknown, false, FollowerType.None)}, //BlackRockLedger06-7390
{193631, new CacheBalance(0, ItemType.Unknown, false, FollowerType.None)}, //A2C2AlcarnusPrisoner2-5586
{-1385743629, new CacheBalance(1, ItemType.Unknown, false, FollowerType.None)}, //Lore_AzmodansOrders6-41895
{543691114, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-13741
{368302887, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-11107
{-636820188, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-18831
{-576445432, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-20698
{-275669102, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-1847
{435695962, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-10675
{-636820187, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-14144
{-115137849, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-31858
{1108898771, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-15883
{1134806015, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-17495
{-1689047028, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-12708
{-1051150314, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-18380
{623242820, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-5655
{-1660666895, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-18640
{-1661852814, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-36893
{368302886, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-52051
{-1205502140, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-52452
{-1162323497, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-43350
{-576445431, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-7553
{-807237754, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-22338
{-807237753, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-8329
{82340209, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-28639
{398631475, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-65157
{543691115, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-19549
{-1690232950, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-50444
{1717766203, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-12732
{844895417, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-1926
{1288600122, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-4913
{-1661852816, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-9603
{972140825, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-21742
{2129978459, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-30273
{844895416, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-68468
{-1690232948, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-31138
{255305005, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-6242
{364927530, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-7631
{623242821, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-11172
{-1690232949, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-64675
{1110084692, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-9210
{-638006108, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-59824
{368302888, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-11092
{82340210, new CacheBalance(46, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Smith_Drop-29649
{521743063, new CacheBalance(61, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Jeweler_Drop-7137
{-1171649812, new CacheBalance(61, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Jeweler_Drop-60398
{872611723, new CacheBalance(61, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Jeweler_Drop-3119
{2147165121, new CacheBalance(62, ItemType.CraftingPlan, false, FollowerType.None)}, //CraftingPlan_Jeweler_Drop-51842
{626121463, new CacheBalance(63, ItemType.HandCrossbow, true, FollowerType.None)}, //handXbow_norm_unique_flippy_08-28023
{1738057815, new CacheBalance(63, ItemType.Spear, true, FollowerType.None)}, //Spear_norm_unique_flippy_02-46594
{-1864479819, new CacheBalance(62, ItemType.FistWeapon, true, FollowerType.None)}, //fistWeapon_norm_unique_flippy_04-52011
{1880318728, new CacheBalance(62, ItemType.Mace, true, FollowerType.None)}, //Mace_norm_unique_flippy_07-42689
{1845044989, new CacheBalance(63, ItemType.Daibo, false, FollowerType.None)}, //combatStaff_norm_unique_flippy_08-45075
{1025903124, new CacheBalance(61, ItemType.Shield, false, FollowerType.None)}, //Shield_norm_unique_flippy_08-10113
{140743477, new CacheBalance(60, ItemType.Axe, true, FollowerType.None)}, //Axe_norm_unique_flippy_06-8301
{589357912, new CacheBalance(60, ItemType.HandCrossbow, true, FollowerType.None)}, //handXbow_norm_unique_flippy_02-9863
{-1665099598, new CacheBalance(57, ItemType.Mojo, false, FollowerType.None)}, //Mojo_norm_unique_flippy_04-9022
{-1269640592, new CacheBalance(60, ItemType.Staff, false, FollowerType.None)}, //Staff_norm_unique_flippy_07-4037
{-1178676596, new CacheBalance(60, ItemType.Polearm, false, FollowerType.None)}, //Polearm_norm_unique_flippy_02-24734
{421779618, new CacheBalance(63, ItemType.Sword, false, FollowerType.None)}, //twoHandedSword_norm_unique_flippy_04-45679
{-1451977669, new CacheBalance(61, ItemType.Crossbow, false, FollowerType.None)}, //XBow_norm_unique_flippy_02-36957
{1034204571, new CacheBalance(63, ItemType.Shield, false, FollowerType.None)}, //Shield_norm_unique_flippy_06-26363
{1033018650, new CacheBalance(56, ItemType.Shield, false, FollowerType.None)}, //Shield_norm_unique_flippy_07-69093
{1949306026, new CacheBalance(62, ItemType.Mace, false, FollowerType.None)}, //twoHandedMace_norm_unique_flippy_04-35520
{-1241813500, new CacheBalance(61, ItemType.CeremonialDagger, true, FollowerType.None)}, //ceremonialDagger_norm_unique_flippy_09-12122
{-1264896908, new CacheBalance(61, ItemType.Staff, false, FollowerType.None)}, //Staff_norm_unique_flippy_05-15262
{-1450791748, new CacheBalance(63, ItemType.Crossbow, false, FollowerType.None)}, //XBow_norm_unique_flippy_06-41422
{-2078257915, new CacheBalance(56, ItemType.Dagger, true, FollowerType.None)}, //Dagger_norm_unique_flippy_02-1460
{-2041494364, new CacheBalance(63, ItemType.Dagger, true, FollowerType.None)}, //Dagger_norm_unique_flippy_05-46093
{1888620175, new CacheBalance(60, ItemType.Mace, true, FollowerType.None)}, //Mace_norm_unique_flippy_02-15158
{-1262525066, new CacheBalance(63, ItemType.Staff, false, FollowerType.None)}, //Staff_norm_unique_flippy_04-17468
{1664512741, new CacheBalance(63, ItemType.MightyWeapon, true, FollowerType.None)}, //mightyWeapon_1H_norm_unique_flippy_06-26377
{1279577735, new CacheBalance(63, ItemType.Wand, true, FollowerType.None)}, //Wand_norm_unique_flippy_01-21404
{139557556, new CacheBalance(63, ItemType.Axe, true, FollowerType.None)}, //Axe_norm_unique_flippy_04-39065
        };
		  #endregion

    }
}