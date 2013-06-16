using System;
using System.Linq;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using Zeta.Common;
using Zeta.Internals.Actors;
using Zeta.CommonBot;
using Zeta;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  //D3 v1.07 new plans
		  //Amulet
		  //Archon Armor
		  //Archon Gauntlets
		  //Archon Spaulders
		  //Razorspikes


		  // **********************************************************************************************
		  // *****       Pickup Validation - Determines what should or should not be picked up        *****
		  // **********************************************************************************************
		  private static bool GilesPickupItemValidation(CacheItem item)
		  {
				// Calculate giles item types and base types etc.
				GilesItemType thisGilesItemType=DetermineItemType(item.InternalName, item.BalanceData.thisItemType, item.BalanceData.thisFollowerType);
				GilesBaseItemType thisGilesBaseType=DetermineBaseType(thisGilesItemType);

				// If it's legendary, we always want it *IF* it's level is right
				if (item.Itemquality>=ItemQuality.Legendary)
				{
					 //Frost burn
					 // if (thisdbItemType == ItemType.Gloves && templevel == 62)
					 //   return false;
					 //Death mantle Watch
					 //if (thisdbItemType == ItemType.Shoulder && templevel == 58)
					 //  return false;
					 //Heart Slaughter
					 //if (thisdbItemType == ItemType.Polearm && templevel == 60)
					 //  return false;
					 //Wormwood
					 // if (thisdbItemType == ItemType.Staff && templevel == 60)
					 //   return false;
					 //Sultan 2h Sword
					 //if (thisGilesItemType == GilesItemType.TwoHandSword && templevel == 60)
					 //  return false;

					 if (Bot.SettingsFunky.MinimumLegendaryItemLevel>0&&(item.BalanceData.iThisItemLevel>=Bot.SettingsFunky.MinimumLegendaryItemLevel||Bot.SettingsFunky.MinimumLegendaryItemLevel==1))
						  return true;

					 return false;
				}



				// Error logging for DemonBuddy item mis-reading
				ItemType gilesDBItemType=GilesToDBItemType(thisGilesItemType);
				if (gilesDBItemType!=item.BalanceData.thisItemType)
				{
					 Log("GSError: Item type mis-match detected: Item Internal="+item.InternalName+". DemonBuddy ItemType thinks item type is="+item.BalanceData.thisItemBaseType.ToString()+". Giles thinks item type is="+
						  gilesDBItemType.ToString()+". [pickup]", true);
				}

				switch (thisGilesBaseType)
				{
					 case GilesBaseItemType.WeaponTwoHand:
					 case GilesBaseItemType.WeaponOneHand:
					 case GilesBaseItemType.WeaponRange:
						  // Not enough DPS, so analyse for possibility to blacklist
						  if (item.Itemquality<ItemQuality.Magic1)
						  {
								// White item, blacklist
								return false;
						  }
						  if (item.Itemquality>=ItemQuality.Magic1&&item.Itemquality<ItemQuality.Rare4)
						  {
								if (Bot.SettingsFunky.MinimumWeaponItemLevel[0]==0||(Bot.SettingsFunky.MinimumWeaponItemLevel[0]!=0&&item.BalanceData.iThisItemLevel<Bot.SettingsFunky.MinimumWeaponItemLevel[0]))
								{
									 // Between magic and rare, and either we want no blues, or this level is higher than the blue level we want
									 return false;
								}
						  }
						  else
						  {
								if (Bot.SettingsFunky.MinimumWeaponItemLevel[1]==0||(Bot.SettingsFunky.MinimumWeaponItemLevel[1]!=0&&item.BalanceData.iThisItemLevel<Bot.SettingsFunky.MinimumWeaponItemLevel[1]))
								{
									 // Between magic and rare, and either we want no blues, or this level is higher than the blue level we want
									 return false;
								}
						  }
						  break;
					 case GilesBaseItemType.Armor:
					 case GilesBaseItemType.Offhand:
						  if (item.Itemquality<ItemQuality.Magic1)
						  {
								// White item, blacklist
								return false;
						  }
						  if (item.Itemquality>=ItemQuality.Magic1&&item.Itemquality<ItemQuality.Rare4)
						  {
								if (Bot.SettingsFunky.MinimumArmorItemLevel[0]==0||(Bot.SettingsFunky.MinimumArmorItemLevel[0]!=0&&item.BalanceData.iThisItemLevel<Bot.SettingsFunky.MinimumArmorItemLevel[0]))
								{
									 // Between magic and rare, and either we want no blues, or this level is higher than the blue level we want
									 return false;
								}
						  }
						  else
						  {
								if (Bot.SettingsFunky.MinimumArmorItemLevel[1]==0||(Bot.SettingsFunky.MinimumArmorItemLevel[1]!=0&&item.BalanceData.iThisItemLevel<Bot.SettingsFunky.MinimumArmorItemLevel[1]))
								{
									 // Between magic and rare, and either we want no blues, or this level is higher than the blue level we want
									 return false;
								}
						  }
						  break;
					 case GilesBaseItemType.Jewelry:
						  if (item.Itemquality<ItemQuality.Magic1)
						  {
								// White item, blacklist
								return false;
						  }
						  if (item.Itemquality>=ItemQuality.Magic1&&item.Itemquality<ItemQuality.Rare4)
						  {
								if (Bot.SettingsFunky.MinimumJeweleryItemLevel[0]==0||(Bot.SettingsFunky.MinimumJeweleryItemLevel[0]!=0&&item.BalanceData.iThisItemLevel<Bot.SettingsFunky.MinimumJeweleryItemLevel[0]))
								{
									 // Between magic and rare, and either we want no blues, or this level is higher than the blue level we want
									 return false;
								}
						  }
						  else
						  {
								if (Bot.SettingsFunky.MinimumJeweleryItemLevel[1]==0||(Bot.SettingsFunky.MinimumJeweleryItemLevel[1]!=0&&item.BalanceData.iThisItemLevel<Bot.SettingsFunky.MinimumJeweleryItemLevel[1]))
								{
									 // Between magic and rare, and either we want no blues, or this level is higher than the blue level we want
									 return false;
								}
						  }
						  break;
					 case GilesBaseItemType.FollowerItem:
						  if (item.BalanceData.iThisItemLevel<60||!Bot.SettingsFunky.PickupFollowerItems||item.Itemquality<ItemQuality.Rare4)
						  {
								if (!_hashsetItemFollowersIgnored.Contains(item.DynamicID.Value))
								{
									 _hashsetItemFollowersIgnored.Add(item.DynamicID.Value);
									 iTotalFollowerItemsIgnored++;
								}
								return false;
						  }
						  break;
					 case GilesBaseItemType.Gem:
						  if (item.BalanceData.iThisItemLevel<Bot.SettingsFunky.MinimumGemItemLevel||(thisGilesItemType==GilesItemType.Ruby&&!Bot.SettingsFunky.PickupGems[0])||(thisGilesItemType==GilesItemType.Emerald&&!Bot.SettingsFunky.PickupGems[1])||
								(thisGilesItemType==GilesItemType.Amethyst&&!Bot.SettingsFunky.PickupGems[2])||(thisGilesItemType==GilesItemType.Topaz&&!Bot.SettingsFunky.PickupGems[3]))
						  {
								return false;
						  }
						  break;
					 case GilesBaseItemType.Misc:
						  // Note; Infernal keys are misc, so should be picked up here - we aren't filtering them out, so should default to true at the end of this function
						  if (thisGilesItemType==GilesItemType.CraftingMaterial&&item.BalanceData.iThisItemLevel<Bot.SettingsFunky.MiscItemLevel)
						  {
								return false;
						  }
						  if (thisGilesItemType==GilesItemType.CraftTome&&(item.BalanceData.iThisItemLevel<Bot.SettingsFunky.MiscItemLevel||!Bot.SettingsFunky.PickupCraftTomes))
						  {
								return false;
						  }
						  if (thisGilesItemType==GilesItemType.CraftingPlan&&!Bot.SettingsFunky.PickupCraftPlans)
						  {
								return false;
						  }
						  // Potion filtering
						  if (thisGilesItemType==GilesItemType.HealthPotion)
						  {	
								if (Bot.SettingsFunky.MaximumHealthPotions<=0) 
									 return false;

								var Potions = Bot.Character.BackPack.ReturnCurrentPotions();

								if (Potions==null||!Potions.Any()||Bot.Character.BackPack.BestPotionToUse==null)
									 return true;
								else if (Bot.Character.BackPack.BestPotionToUse!=null&&item.BalanceData.iThisItemLevel<Bot.Character.BackPack.BestPotionToUse.Level) 
									 return false;
								else if (Potions.Sum(potions => potions.ItemStackQuantity)>=Bot.SettingsFunky.MaximumHealthPotions)
									 return false;
						  }
						  break;
					 case GilesBaseItemType.HealthGlobe:
						  return false;
					 case GilesBaseItemType.Unknown:
						  return false;
					 default:
						  return false;
				} // Switch giles base item type
				// Didn't cancel it, so default to true!
				return true;
		  }

		  // **********************************************************************************************
		  // *****      Sell Validation - Determines what should or should not be sold to vendor      *****
		  // **********************************************************************************************
		  private static bool GilesSellValidation(string thisinternalname, int thislevel, ItemQuality thisquality, ItemType thisdbitemtype, FollowerType thisfollowertype)
		  {
				GilesItemType thisGilesItemType=DetermineItemType(thisinternalname, thisdbitemtype, thisfollowertype);
				GilesBaseItemType thisGilesBaseType=DetermineBaseType(thisGilesItemType);

				switch (thisGilesBaseType)
				{
					 case GilesBaseItemType.WeaponRange:
					 case GilesBaseItemType.WeaponOneHand:
					 case GilesBaseItemType.WeaponTwoHand:
					 case GilesBaseItemType.Armor:
					 case GilesBaseItemType.Offhand:
					 case GilesBaseItemType.Jewelry:
					 case GilesBaseItemType.FollowerItem:
						  return true;
					 case GilesBaseItemType.Gem:
					 case GilesBaseItemType.Misc:
					 case GilesBaseItemType.Unknown:
						  //Sell any plans not already stashed.
						  if (thisdbitemtype==ItemType.CraftingPlan)
								return true;
						  return false;
				} // Switch giles base item type
				return false;
		  }

		  // **********************************************************************************************
		  // ***** DetermineItemType - Calculates what kind of item it is from D3 internalnames       *****
		  // **********************************************************************************************
		  private static GilesItemType DetermineItemType(string sThisInternalName, ItemType DBItemType, FollowerType dbFollowerType=FollowerType.None)
		  {
				sThisInternalName=sThisInternalName.ToLower();
				if (sThisInternalName.StartsWith("axe_")) return GilesItemType.Axe;
				if (sThisInternalName.StartsWith("ceremonialdagger_")) return GilesItemType.CeremonialKnife;
				if (sThisInternalName.StartsWith("handxbow_")) return GilesItemType.HandCrossbow;
				if (sThisInternalName.StartsWith("dagger_")) return GilesItemType.Dagger;
				if (sThisInternalName.StartsWith("fistweapon_")) return GilesItemType.FistWeapon;
				if (sThisInternalName.StartsWith("mace_")) return GilesItemType.Mace;
				if (sThisInternalName.StartsWith("mightyweapon_1h_")) return GilesItemType.MightyWeapon;
				if (sThisInternalName.StartsWith("spear_")) return GilesItemType.Spear;
				if (sThisInternalName.StartsWith("sword_")) return GilesItemType.Sword;
				if (sThisInternalName.StartsWith("wand_")) return GilesItemType.Wand;
				if (sThisInternalName.StartsWith("twohandedaxe_")) return GilesItemType.TwoHandAxe;
				if (sThisInternalName.StartsWith("bow_")) return GilesItemType.TwoHandBow;
				if (sThisInternalName.StartsWith("combatstaff_")) return GilesItemType.TwoHandDaibo;
				if (sThisInternalName.StartsWith("xbow_")) return GilesItemType.TwoHandCrossbow;
				if (sThisInternalName.StartsWith("twohandedmace_")) return GilesItemType.TwoHandMace;
				if (sThisInternalName.StartsWith("mightyweapon_2h_")) return GilesItemType.TwoHandMighty;
				if (sThisInternalName.StartsWith("polearm_")) return GilesItemType.TwoHandPolearm;
				if (sThisInternalName.StartsWith("staff_")) return GilesItemType.TwoHandStaff;
				if (sThisInternalName.StartsWith("twohandedsword_")) return GilesItemType.TwoHandSword;
				if (sThisInternalName.StartsWith("staffofcow")) return GilesItemType.StaffOfHerding;
				if (sThisInternalName.StartsWith("mojo_")) return GilesItemType.Mojo;
				if (sThisInternalName.StartsWith("orb_")) return GilesItemType.Source;
				if (sThisInternalName.StartsWith("quiver_")) return GilesItemType.Quiver;
				if (sThisInternalName.StartsWith("shield_")) return GilesItemType.Shield;
				if (sThisInternalName.StartsWith("amulet_")) return GilesItemType.Amulet;
				if (sThisInternalName.StartsWith("ring_")) return GilesItemType.Ring;
				if (sThisInternalName.StartsWith("boots_")) return GilesItemType.Boots;
				if (sThisInternalName.StartsWith("bracers_")) return GilesItemType.Bracers;
				if (sThisInternalName.StartsWith("cloak_")) return GilesItemType.Cloak;
				if (sThisInternalName.StartsWith("gloves_")) return GilesItemType.Gloves;
				if (sThisInternalName.StartsWith("pants_")) return GilesItemType.Pants;
				if (sThisInternalName.StartsWith("barbbelt_")) return GilesItemType.MightyBelt;
				if (sThisInternalName.StartsWith("shoulderpads_")) return GilesItemType.Shoulders;
				if (sThisInternalName.StartsWith("spiritstone_")) return GilesItemType.SpiritStone;
				if (sThisInternalName.StartsWith("voodoomask_")) return GilesItemType.VoodooMask;
				if (sThisInternalName.StartsWith("wizardhat_")) return GilesItemType.WizardHat;
				if (sThisInternalName.StartsWith("lore_book_")) return GilesItemType.CraftTome;
				if (sThisInternalName.StartsWith("page_of_")) return GilesItemType.CraftTome;
				if (sThisInternalName.StartsWith("blacksmithstome")) return GilesItemType.CraftTome;
				if (sThisInternalName.StartsWith("ruby_")) return GilesItemType.Ruby;
				if (sThisInternalName.StartsWith("emerald_")) return GilesItemType.Emerald;
				if (sThisInternalName.StartsWith("topaz_")) return GilesItemType.Topaz;
				if (sThisInternalName.StartsWith("amethyst")) return GilesItemType.Amethyst;
				if (sThisInternalName.StartsWith("healthpotion_")) return GilesItemType.HealthPotion;
				if (sThisInternalName.StartsWith("followeritem_enchantress_")) return GilesItemType.FollowerEnchantress;
				if (sThisInternalName.StartsWith("followeritem_scoundrel_")) return GilesItemType.FollowerScoundrel;
				if (sThisInternalName.StartsWith("followeritem_templar_")) return GilesItemType.FollowerTemplar;
				if (sThisInternalName.StartsWith("craftingplan_")) return GilesItemType.CraftingPlan;
				if (sThisInternalName.StartsWith("dye_")) return GilesItemType.Dye;
				if (sThisInternalName.StartsWith("a1_")) return GilesItemType.SpecialItem;
				if (sThisInternalName.StartsWith("healthglobe")) return GilesItemType.HealthGlobe;

				// Follower item types
				if (sThisInternalName.StartsWith("jewelbox_")||DBItemType==ItemType.FollowerSpecial)
				{
					 if (dbFollowerType==FollowerType.Scoundrel)
						  return GilesItemType.FollowerScoundrel;
					 if (dbFollowerType==FollowerType.Templar)
						  return GilesItemType.FollowerTemplar;
					 if (dbFollowerType==FollowerType.Enchantress)
						  return GilesItemType.FollowerEnchantress;
				}

				// Fall back on some partial DB item type checking 
				if (sThisInternalName.StartsWith("crafting_")||sThisInternalName.StartsWith("craftingmaterials_"))
				{
					 if (DBItemType==ItemType.CraftingPage) return GilesItemType.CraftTome;
					 return GilesItemType.CraftingMaterial;
				}

				if (sThisInternalName.StartsWith("chestarmor_"))
				{
					 if (DBItemType==ItemType.Cloak) return GilesItemType.Cloak;
					 return GilesItemType.Chest;
				}
				if (sThisInternalName.StartsWith("helm_"))
				{
					 if (DBItemType==ItemType.SpiritStone) return GilesItemType.SpiritStone;
					 if (DBItemType==ItemType.VoodooMask) return GilesItemType.VoodooMask;
					 if (DBItemType==ItemType.WizardHat) return GilesItemType.WizardHat;
					 return GilesItemType.Helm;
				}
				if (sThisInternalName.StartsWith("helmcloth_"))
				{
					 if (DBItemType==ItemType.SpiritStone) return GilesItemType.SpiritStone;
					 if (DBItemType==ItemType.VoodooMask) return GilesItemType.VoodooMask;
					 if (DBItemType==ItemType.WizardHat) return GilesItemType.WizardHat;
					 return GilesItemType.Helm;
				}
				if (sThisInternalName.StartsWith("belt_"))
				{
					 if (DBItemType==ItemType.MightyBelt) return GilesItemType.MightyBelt;
					 return GilesItemType.Belt;
				}
				if (sThisInternalName.StartsWith("demonkey_")||sThisInternalName.StartsWith("demontrebuchetkey"))
				{
					 return GilesItemType.InfernalKey;
				}

				return GilesItemType.Unknown;
		  }

		  // **********************************************************************************************
		  // *****      DetermineBaseType - Calculates a more generic, "basic" type of item           *****
		  // **********************************************************************************************
		  private static GilesBaseItemType DetermineBaseType(GilesItemType thisGilesItemType)
		  {
				GilesBaseItemType thisGilesBaseType=GilesBaseItemType.Unknown;
				if (thisGilesItemType==GilesItemType.Axe||thisGilesItemType==GilesItemType.CeremonialKnife||thisGilesItemType==GilesItemType.Dagger||
					 thisGilesItemType==GilesItemType.FistWeapon||thisGilesItemType==GilesItemType.Mace||thisGilesItemType==GilesItemType.MightyWeapon||
					 thisGilesItemType==GilesItemType.Spear||thisGilesItemType==GilesItemType.Sword||thisGilesItemType==GilesItemType.Wand)
				{
					 thisGilesBaseType=GilesBaseItemType.WeaponOneHand;
				}
				else if (thisGilesItemType==GilesItemType.TwoHandDaibo||thisGilesItemType==GilesItemType.TwoHandMace||
					 thisGilesItemType==GilesItemType.TwoHandMighty||thisGilesItemType==GilesItemType.TwoHandPolearm||thisGilesItemType==GilesItemType.TwoHandStaff||
					 thisGilesItemType==GilesItemType.TwoHandSword||thisGilesItemType==GilesItemType.TwoHandAxe)
				{
					 thisGilesBaseType=GilesBaseItemType.WeaponTwoHand;
				}
				else if (thisGilesItemType==GilesItemType.TwoHandCrossbow||thisGilesItemType==GilesItemType.HandCrossbow||thisGilesItemType==GilesItemType.TwoHandBow)
				{
					 thisGilesBaseType=GilesBaseItemType.WeaponRange;
				}
				else if (thisGilesItemType==GilesItemType.Mojo||thisGilesItemType==GilesItemType.Source||
					 thisGilesItemType==GilesItemType.Quiver||thisGilesItemType==GilesItemType.Shield)
				{
					 thisGilesBaseType=GilesBaseItemType.Offhand;
				}
				else if (thisGilesItemType==GilesItemType.Boots||thisGilesItemType==GilesItemType.Bracers||thisGilesItemType==GilesItemType.Chest||
					 thisGilesItemType==GilesItemType.Cloak||thisGilesItemType==GilesItemType.Gloves||thisGilesItemType==GilesItemType.Helm||
					 thisGilesItemType==GilesItemType.Pants||thisGilesItemType==GilesItemType.Shoulders||thisGilesItemType==GilesItemType.SpiritStone||
					 thisGilesItemType==GilesItemType.VoodooMask||thisGilesItemType==GilesItemType.WizardHat||thisGilesItemType==GilesItemType.Belt||
					 thisGilesItemType==GilesItemType.MightyBelt)
				{
					 thisGilesBaseType=GilesBaseItemType.Armor;
				}
				else if (thisGilesItemType==GilesItemType.Amulet||thisGilesItemType==GilesItemType.Ring)
				{
					 thisGilesBaseType=GilesBaseItemType.Jewelry;
				}
				else if (thisGilesItemType==GilesItemType.FollowerEnchantress||thisGilesItemType==GilesItemType.FollowerScoundrel||
					 thisGilesItemType==GilesItemType.FollowerTemplar)
				{
					 thisGilesBaseType=GilesBaseItemType.FollowerItem;
				}
				else if (thisGilesItemType==GilesItemType.CraftingMaterial||thisGilesItemType==GilesItemType.CraftTome||
					 thisGilesItemType==GilesItemType.SpecialItem||thisGilesItemType==GilesItemType.CraftingPlan||thisGilesItemType==GilesItemType.HealthPotion||
					 thisGilesItemType==GilesItemType.Dye||thisGilesItemType==GilesItemType.StaffOfHerding||thisGilesItemType==GilesItemType.InfernalKey)
				{
					 thisGilesBaseType=GilesBaseItemType.Misc;
				}
				else if (thisGilesItemType==GilesItemType.Ruby||thisGilesItemType==GilesItemType.Emerald||thisGilesItemType==GilesItemType.Topaz||
					 thisGilesItemType==GilesItemType.Amethyst)
				{
					 thisGilesBaseType=GilesBaseItemType.Gem;
				}
				else if (thisGilesItemType==GilesItemType.HealthGlobe)
				{
					 thisGilesBaseType=GilesBaseItemType.HealthGlobe;
				}
				return thisGilesBaseType;
		  }

		  // **********************************************************************************************
		  // *****          DetermineIsStackable - Calculates what items can be stacked up            *****
		  // **********************************************************************************************
		  private static bool DetermineIsStackable(GilesItemType thisGilesItemType)
		  {
				bool bIsStackable=thisGilesItemType==GilesItemType.CraftingMaterial||thisGilesItemType==GilesItemType.CraftTome||thisGilesItemType==GilesItemType.Ruby||
										  thisGilesItemType==GilesItemType.Emerald||thisGilesItemType==GilesItemType.Topaz||thisGilesItemType==GilesItemType.Amethyst||
										  thisGilesItemType==GilesItemType.HealthPotion||thisGilesItemType==GilesItemType.CraftingPlan||thisGilesItemType==GilesItemType.Dye||
										  thisGilesItemType==GilesItemType.InfernalKey;
				return bIsStackable;
		  }

		  // **********************************************************************************************
		  // *****      DetermineIsTwoSlot - Tries to calculate what items take up 2 slots or 1       *****
		  // **********************************************************************************************
		  private static bool DetermineIsTwoSlot(GilesItemType thisGilesItemType)
		  {
				if (thisGilesItemType==GilesItemType.Axe||thisGilesItemType==GilesItemType.CeremonialKnife||thisGilesItemType==GilesItemType.Dagger||
					 thisGilesItemType==GilesItemType.FistWeapon||thisGilesItemType==GilesItemType.Mace||thisGilesItemType==GilesItemType.MightyWeapon||
					 thisGilesItemType==GilesItemType.Spear||thisGilesItemType==GilesItemType.Sword||thisGilesItemType==GilesItemType.Wand||
					 thisGilesItemType==GilesItemType.TwoHandDaibo||thisGilesItemType==GilesItemType.TwoHandCrossbow||thisGilesItemType==GilesItemType.TwoHandMace||
					 thisGilesItemType==GilesItemType.TwoHandMighty||thisGilesItemType==GilesItemType.TwoHandPolearm||thisGilesItemType==GilesItemType.TwoHandStaff||
					 thisGilesItemType==GilesItemType.TwoHandSword||thisGilesItemType==GilesItemType.TwoHandAxe||thisGilesItemType==GilesItemType.HandCrossbow||
					 thisGilesItemType==GilesItemType.TwoHandBow||thisGilesItemType==GilesItemType.Mojo||thisGilesItemType==GilesItemType.Source||
					 thisGilesItemType==GilesItemType.Quiver||thisGilesItemType==GilesItemType.Shield||thisGilesItemType==GilesItemType.Boots||
					 thisGilesItemType==GilesItemType.Bracers||thisGilesItemType==GilesItemType.Chest||thisGilesItemType==GilesItemType.Cloak||
					 thisGilesItemType==GilesItemType.Gloves||thisGilesItemType==GilesItemType.Helm||thisGilesItemType==GilesItemType.Pants||
					 thisGilesItemType==GilesItemType.Shoulders||thisGilesItemType==GilesItemType.SpiritStone||
					 thisGilesItemType==GilesItemType.VoodooMask||thisGilesItemType==GilesItemType.WizardHat||thisGilesItemType==GilesItemType.StaffOfHerding)
					 return true;
				return false;
		  }

		  // **********************************************************************************************
		  // *****   This is for DemonBuddy error checking - see what sort of item DB THINKS it is    *****
		  // **********************************************************************************************
		  private static ItemType GilesToDBItemType(GilesItemType thisgilesitemtype)
		  {
				switch (thisgilesitemtype)
				{
					 case GilesItemType.Axe: return ItemType.Axe;
					 case GilesItemType.CeremonialKnife: return ItemType.CeremonialDagger;
					 case GilesItemType.HandCrossbow: return ItemType.HandCrossbow;
					 case GilesItemType.Dagger: return ItemType.Dagger;
					 case GilesItemType.FistWeapon: return ItemType.FistWeapon;
					 case GilesItemType.Mace: return ItemType.Mace;
					 case GilesItemType.MightyWeapon: return ItemType.MightyWeapon;
					 case GilesItemType.Spear: return ItemType.Spear;
					 case GilesItemType.Sword: return ItemType.Sword;
					 case GilesItemType.Wand: return ItemType.Wand;
					 case GilesItemType.TwoHandAxe: return ItemType.Axe;
					 case GilesItemType.TwoHandBow: return ItemType.Bow;
					 case GilesItemType.TwoHandDaibo: return ItemType.Daibo;
					 case GilesItemType.TwoHandCrossbow: return ItemType.Crossbow;
					 case GilesItemType.TwoHandMace: return ItemType.Mace;
					 case GilesItemType.TwoHandMighty: return ItemType.MightyWeapon;
					 case GilesItemType.TwoHandPolearm: return ItemType.Polearm;
					 case GilesItemType.TwoHandStaff: return ItemType.Staff;
					 case GilesItemType.TwoHandSword: return ItemType.Sword;
					 case GilesItemType.StaffOfHerding: return ItemType.Staff;
					 case GilesItemType.Mojo: return ItemType.Mojo;
					 case GilesItemType.Source: return ItemType.Orb;
					 case GilesItemType.Quiver: return ItemType.Quiver;
					 case GilesItemType.Shield: return ItemType.Shield;
					 case GilesItemType.Amulet: return ItemType.Amulet;
					 case GilesItemType.Ring: return ItemType.Ring;
					 case GilesItemType.Belt: return ItemType.Belt;
					 case GilesItemType.Boots: return ItemType.Boots;
					 case GilesItemType.Bracers: return ItemType.Bracer;
					 case GilesItemType.Chest: return ItemType.Chest;
					 case GilesItemType.Cloak: return ItemType.Cloak;
					 case GilesItemType.Gloves: return ItemType.Gloves;
					 case GilesItemType.Helm: return ItemType.Helm;
					 case GilesItemType.Pants: return ItemType.Legs;
					 case GilesItemType.MightyBelt: return ItemType.MightyBelt;
					 case GilesItemType.Shoulders: return ItemType.Shoulder;
					 case GilesItemType.SpiritStone: return ItemType.SpiritStone;
					 case GilesItemType.VoodooMask: return ItemType.VoodooMask;
					 case GilesItemType.WizardHat: return ItemType.WizardHat;
					 case GilesItemType.FollowerEnchantress: return ItemType.FollowerSpecial;
					 case GilesItemType.FollowerScoundrel: return ItemType.FollowerSpecial;
					 case GilesItemType.FollowerTemplar: return ItemType.FollowerSpecial;
					 case GilesItemType.CraftingMaterial: return ItemType.CraftingReagent;
					 case GilesItemType.CraftTome: return ItemType.CraftingPage;
					 case GilesItemType.Ruby: return ItemType.Gem;
					 case GilesItemType.Emerald: return ItemType.Gem;
					 case GilesItemType.Topaz: return ItemType.Gem;
					 case GilesItemType.Amethyst: return ItemType.Gem;
					 case GilesItemType.SpecialItem: return ItemType.Unknown;
					 case GilesItemType.CraftingPlan: return ItemType.CraftingPlan;
					 case GilesItemType.HealthPotion: return ItemType.Potion;
					 case GilesItemType.Dye: return ItemType.Unknown;
					 case GilesItemType.InfernalKey: return ItemType.Unknown;
				}
				return ItemType.Unknown;
		  }

		  // **********************************************************************************************
		  // *****          Search backpack to see if we have room for a 2-slot item anywhere         *****
		  // **********************************************************************************************
		  //private static bool[,] GilesBackpackSlotBlocked=new bool[10, 6];
	 }
}