using System;
using Zeta;
using Zeta.Internals.Actors;
using System.Collections.Generic;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  // Score weighting for armours and jewelry - feel free to change these values!
		  public static int iWeightPrimary=50;
		  public static int iWeightVitality=30;
		  public static int iWeightArmor=3;
		  public static int iWeightMovementSpeed=500;
		  public static int iWeightResistAll=220;
		  public static int iWeightCritPercent=800;
		  public static int iWeightCritDamagePercent=30;
		  public static int iWeightMinDamage=40;
		  public static int iWeightMaxDamage=40;
		  public static int iWeightSocket=100;
		  public static int iWeightLifeOnHit=10;
		  // Score weighting for weapons - feel free to change these values!
		  public static int iWeightWeaponDPS=50;
		  public static int iWeightWeaponVitality=10;
		  public static int iWeightWeaponPrimary=20;
		  public static int iWeightWeaponSocket=200;
		  public static int iWeightWeaponLifeOnHit=20;

		  private static HashSet<int> _alreadyLookedAtBlacklist=new HashSet<int>();
		  private static DateTime _lastLooked=DateTime.Now;
		  private static DateTime _lastFullEvaluation=DateTime.Now;
		  public static bool bIgnoreWeapons=false;
		  public static bool bIgnoreJewelry=false;
		  public static bool bIgnoreArmor=false;
		  public static bool bIgnoreHelm=false;
		  public static bool bDisable60=true;
		  public static float iMyRightFingerPoints=0;
		  public static float iMyLeftFingerPoints=0;
		  public static float iMyBracersPoints=0;
		  public static float iMyFeetPoints=0;
		  public static float iMyHandsPoints=0;
		  public static float iMyHeadPoints=0;
		  public static float iMyLeftHandPoints=0;
		  public static float iMyLegsPoints=0;
		  public static float iMyNeckPoints=0;
		  public static float iMyRightHandPoints=0;
		  public static float iMyShouldersPoints=0;
		  public static float iMyTorsoPoints=0;
		  public static float iMyWaistPoints=0;
		  public static Boolean bMyUsingTwoHandedMelee=false;
		  public static Boolean bNeedFullItemUpdate=true;

		  private static List<ItemType> RestrictedTypes
		  {
				get
				{
					 ItemType[] range;
					 List<ItemType> returningList=new List<ItemType>() { ItemType.CraftingPage, ItemType.CraftingPlan, ItemType.CraftingReagent, ItemType.Gem, ItemType.Potion, ItemType.Unknown };
					 switch (Bot.Class.AC)
					 {
						  case ActorClass.Barbarian:
								range=new ItemType[] { ItemType.Bow, ItemType.CeremonialDagger, ItemType.Cloak, ItemType.Crossbow, ItemType.Daibo, ItemType.FistWeapon, ItemType.HandCrossbow, ItemType.Mojo, ItemType.Orb, ItemType.Quiver, ItemType.SpiritStone, ItemType.Staff, ItemType.VoodooMask, ItemType.Wand, ItemType.WizardHat };
								returningList.AddRange(range);
								break;
						  case ActorClass.DemonHunter:
								range=new ItemType[] { ItemType.Axe, ItemType.CeremonialDagger, ItemType.Dagger, ItemType.Daibo, ItemType.FistWeapon, ItemType.Mace, ItemType.MightyBelt, ItemType.MightyWeapon, ItemType.Mojo, ItemType.Orb, ItemType.Polearm, ItemType.Spear, ItemType.SpiritStone, ItemType.Staff, ItemType.Sword, ItemType.VoodooMask, ItemType.Wand, ItemType.WizardHat };
								returningList.AddRange(range);
								break;
						  case ActorClass.Monk:
								range=new ItemType[] { ItemType.Bow, ItemType.CeremonialDagger, ItemType.Cloak, ItemType.Crossbow, ItemType.HandCrossbow, ItemType.MightyBelt, ItemType.MightyWeapon, ItemType.Mojo, ItemType.Orb, ItemType.Quiver, ItemType.VoodooMask, ItemType.Wand, ItemType.WizardHat };
								returningList.AddRange(range);
								break;
						  case ActorClass.WitchDoctor:
								range=new ItemType[] { ItemType.Cloak, ItemType.Daibo, ItemType.FistWeapon, ItemType.HandCrossbow, ItemType.MightyBelt, ItemType.MightyWeapon, ItemType.Orb, ItemType.Polearm, ItemType.Quiver, ItemType.SpiritStone, ItemType.Wand, ItemType.WizardHat };
								returningList.AddRange(range);
								break;
						  case ActorClass.Wizard:
								range=new ItemType[] { ItemType.CeremonialDagger, ItemType.Cloak, ItemType.Daibo, ItemType.FistWeapon, ItemType.MightyBelt, ItemType.MightyWeapon, ItemType.Mojo, ItemType.Polearm, ItemType.Quiver, ItemType.SpiritStone, ItemType.VoodooMask };
								returningList.AddRange(range);
								break;
					 }
					 return returningList;
				}

		  }

		  private static float ValueThisItem(ACDItem thisitem, bool bIsEquipped)
		  {
				float iTempPoints=0;
				float iThisPrimaryStat=0;
				// Deal with armor and jewelry together
				GilesItemType thisGilesType=DetermineItemType(thisitem.InternalName, thisitem.ItemType, thisitem.FollowerSpecialType);
				GilesBaseItemType thisGilesBaseType=DetermineBaseType(thisGilesType);
				if (thisGilesBaseType==GilesBaseItemType.Armor||thisGilesBaseType==GilesBaseItemType.Jewelry||thisGilesBaseType==GilesBaseItemType.Offhand)
				{
					 // Work out the primary stat based on your class
					 switch (ZetaDia.Me.ActorClass)
					 {
						  case ActorClass.Barbarian:
								iThisPrimaryStat=thisitem.Stats.Strength;
								break;
						  case ActorClass.Monk:
						  case ActorClass.DemonHunter:
								iThisPrimaryStat=thisitem.Stats.Dexterity;
								break;
						  case ActorClass.Wizard:
						  case ActorClass.WitchDoctor:
								iThisPrimaryStat=thisitem.Stats.Intelligence;
								break;

					 } // Switch on your actorclass
					 // Give 5 points free - so it values something without any supported stats over an empty inventory slot
					 iTempPoints=5;
					 iTempPoints+=(iThisPrimaryStat*iWeightPrimary); // primary stat
					 iTempPoints+=(thisitem.Stats.Vitality*iWeightVitality); // vitality 
					 iTempPoints+=((float)thisitem.Stats.ArmorTotal*iWeightArmor); // total armor of item
					 iTempPoints+=(thisitem.Stats.MovementSpeed*iWeightMovementSpeed); // movement speed %
					 iTempPoints+=(thisitem.Stats.ResistAll*iWeightResistAll); // resist all
					 iTempPoints+=(thisitem.Stats.CritPercent*iWeightCritPercent); // Crit chance %
					 iTempPoints+=(thisitem.Stats.CritDamagePercent*iWeightCritDamagePercent); // crit damage bonus %
					 iTempPoints+=(thisitem.Stats.MinDamage*iWeightMinDamage); // Min damage bonus (currently broken in DB)
					 iTempPoints+=(thisitem.Stats.MaxDamage*iWeightMaxDamage); // Max damage bonus (currently broken in DB)
					 iTempPoints+=(thisitem.Stats.Sockets*iWeightSocket); // Sockets
					 iTempPoints+=(thisitem.Stats.LifeOnHit*iWeightLifeOnHit); // Lifeonhit
				}
				// Now deal with weapons
				else if (thisGilesBaseType==GilesBaseItemType.WeaponOneHand||thisGilesBaseType==GilesBaseItemType.WeaponTwoHand||thisGilesBaseType==GilesBaseItemType.WeaponRange)
				{
					 // Work out the primary stat based on your class
					 switch (ZetaDia.Me.ActorClass)
					 {
						  case ActorClass.Barbarian:
								iThisPrimaryStat=thisitem.Stats.Strength;
								break;
						  case ActorClass.Monk:
						  case ActorClass.DemonHunter:
								iThisPrimaryStat=thisitem.Stats.Dexterity;
								break;
						  case ActorClass.Wizard:
						  case ActorClass.WitchDoctor:
								iThisPrimaryStat=thisitem.Stats.Intelligence;
								break;

					 } // Switch on your actorclass
					 // If it's an already-equipped weapon, don't check for appropriateness etc. just weight it on it's stats
					 if (bIsEquipped)
					 {
						  iTempPoints+=(thisitem.Stats.WeaponDamagePerSecond*iWeightWeaponDPS); // DPS
						  iTempPoints+=(iThisPrimaryStat*iWeightWeaponPrimary); // Primary stat
						  iTempPoints+=(thisitem.Stats.Vitality*iWeightWeaponVitality); // Vitality
						  iTempPoints+=(thisitem.Stats.Sockets*iWeightWeaponSocket); // Sockets
						  iTempPoints+=(thisitem.Stats.LifeOnHit*iWeightWeaponLifeOnHit); // LifeOnHit
						  // LifeOnHit needed
						  // Check if it's 2-handed melee (player's own weapon), in which case flag it so we don't try using an off-hander
						  if (thisGilesBaseType==GilesBaseItemType.WeaponTwoHand)
						  {
								bMyUsingTwoHandedMelee=true;
						  }
					 }
					 // Complex checks if it's not an already-equipped weapon, as we limit to class-appropriate one-handers (or bows/xbows for DH's)
					 else if (!bIsEquipped)
					 {
						  if (thisGilesBaseType==GilesBaseItemType.WeaponOneHand||thisGilesBaseType==GilesBaseItemType.WeaponRange)
						  {
								switch (ZetaDia.Me.ActorClass)
								{
									 case ActorClass.Barbarian:
										  if (thisGilesType==GilesItemType.Axe||thisGilesType==GilesItemType.Dagger||thisGilesType==GilesItemType.Mace||
												thisGilesType==GilesItemType.Spear||thisGilesType==GilesItemType.Sword||thisGilesType==GilesItemType.MightyWeapon)
										  {
												iTempPoints+=(thisitem.Stats.WeaponDamagePerSecond*iWeightWeaponDPS); // DPS
												iTempPoints+=(iThisPrimaryStat*iWeightWeaponPrimary); // Primary stat
												iTempPoints+=(thisitem.Stats.Vitality*iWeightWeaponVitality); // Vitality
												iTempPoints+=(thisitem.Stats.Sockets*iWeightWeaponSocket); // Sockets
												iTempPoints+=(thisitem.Stats.LifeOnHit*iWeightWeaponLifeOnHit); // LifeOnHit
										  }
										  break;
									 case ActorClass.Monk:
										  if (thisGilesType==GilesItemType.Axe||thisGilesType==GilesItemType.Dagger||thisGilesType==GilesItemType.FistWeapon||
												thisGilesType==GilesItemType.Mace||thisGilesType==GilesItemType.Spear||thisGilesType==GilesItemType.Sword)
										  {
												iTempPoints+=(thisitem.Stats.WeaponDamagePerSecond*iWeightWeaponDPS); // DPS
												iTempPoints+=(iThisPrimaryStat*iWeightWeaponPrimary); // Primary stat
												iTempPoints+=(thisitem.Stats.Vitality*iWeightWeaponVitality); // Vitality
												iTempPoints+=(thisitem.Stats.Sockets*iWeightWeaponSocket); // Sockets
												iTempPoints+=(thisitem.Stats.LifeOnHit*iWeightWeaponLifeOnHit); // LifeOnHit
										  }
										  break;
									 case ActorClass.DemonHunter:
										  if (thisGilesType==GilesItemType.TwoHandCrossbow||thisGilesType==GilesItemType.TwoHandBow||thisGilesType==GilesItemType.HandCrossbow)
										  {
												iTempPoints+=(thisitem.Stats.WeaponDamagePerSecond*iWeightWeaponDPS); // DPS
												iTempPoints+=(iThisPrimaryStat*iWeightWeaponPrimary); // Primary stat
												iTempPoints+=(thisitem.Stats.Vitality*iWeightWeaponVitality); // Vitality
												iTempPoints+=(thisitem.Stats.Sockets*iWeightWeaponSocket); // Sockets
												iTempPoints+=(thisitem.Stats.LifeOnHit*iWeightWeaponLifeOnHit); // LifeOnHit
										  }
										  break;
									 case ActorClass.Wizard:
										  if (thisGilesType==GilesItemType.Axe||thisGilesType==GilesItemType.Dagger||thisGilesType==GilesItemType.Mace||
												thisGilesType==GilesItemType.Spear||thisGilesType==GilesItemType.Sword||thisGilesType==GilesItemType.Wand)
										  {
												iTempPoints+=(thisitem.Stats.WeaponDamagePerSecond*iWeightWeaponDPS); // DPS
												iTempPoints+=(iThisPrimaryStat*iWeightWeaponPrimary); // Primary stat
												iTempPoints+=(thisitem.Stats.Vitality*iWeightWeaponVitality); // Vitality
												iTempPoints+=(thisitem.Stats.Sockets*iWeightWeaponSocket); // Sockets
												iTempPoints+=(thisitem.Stats.LifeOnHit*iWeightWeaponLifeOnHit); // LifeOnHit
										  }
										  break;
									 case ActorClass.WitchDoctor:
										  if (thisGilesType==GilesItemType.Axe||thisGilesType==GilesItemType.Dagger||thisGilesType==GilesItemType.Mace||
												thisGilesType==GilesItemType.Spear||thisGilesType==GilesItemType.Sword||thisGilesType==GilesItemType.CeremonialKnife)
										  {
												iTempPoints+=(thisitem.Stats.WeaponDamagePerSecond*iWeightWeaponDPS); // DPS
												iTempPoints+=(iThisPrimaryStat*iWeightWeaponPrimary); // Primary stat
												iTempPoints+=(thisitem.Stats.Vitality*iWeightWeaponVitality); // Vitality
												iTempPoints+=(thisitem.Stats.Sockets*iWeightWeaponSocket); // Sockets
												iTempPoints+=(thisitem.Stats.LifeOnHit*iWeightWeaponLifeOnHit); // LifeOnHit
										  }
										  break;

								} // Character class check
						  } // Is one-hander, or a bow or crossbow
					 } // Not an equipped item
				} // Is a base item type of weapon
				return iTempPoints;
		  } // ValueThisItem() function

		  private static void CheckBackpack()
		  {
				float iThisItemPoints=0;

				// Update our last check time
				_lastLooked=DateTime.Now;

				Bot.Character.BackPack.Update();

				// See if we need to refresh all our equipment points
				if (bNeedFullItemUpdate)
				{
					 _alreadyLookedAtBlacklist=new HashSet<int>(); // Clear blacklisted items so we re-check everything
					 bNeedFullItemUpdate=false;
					 bMyUsingTwoHandedMelee=false;
					 _lastFullEvaluation=DateTime.Now;
					 iMyRightFingerPoints=0;
					 iMyLeftFingerPoints=0;
					 iMyBracersPoints=0;
					 iMyFeetPoints=0;
					 iMyHandsPoints=0;
					 iMyHeadPoints=0;
					 iMyLeftHandPoints=0;
					 iMyLegsPoints=0;
					 iMyNeckPoints=0;
					 iMyRightHandPoints=0;
					 iMyShouldersPoints=0;
					 iMyTorsoPoints=0;
					 iMyWaistPoints=0;
					 foreach (var myitem in ZetaDia.Actors.Me.Inventory.Equipped)
					 {
						  _alreadyLookedAtBlacklist.Add(myitem.DynamicId);
						  switch (myitem.InventorySlot)
						  {
								case InventorySlot.PlayerBracers:
									 iMyBracersPoints=ValueThisItem(myitem, true);
									 break;
								case InventorySlot.PlayerFeet:
									 iMyFeetPoints=ValueThisItem(myitem, true);
									 break;
								case InventorySlot.PlayerHands:
									 iMyHandsPoints=ValueThisItem(myitem, true);
									 break;
								case InventorySlot.PlayerHead:
									 iMyHeadPoints=ValueThisItem(myitem, true);
									 break;
								case InventorySlot.PlayerLeftFinger:
									 iMyLeftFingerPoints=ValueThisItem(myitem, true);
									 break;
								case InventorySlot.PlayerLeftHand:
									 iMyLeftHandPoints=ValueThisItem(myitem, true);
									 break;
								case InventorySlot.PlayerLegs:
									 iMyLegsPoints=ValueThisItem(myitem, true);
									 break;
								case InventorySlot.PlayerNeck:
									 iMyNeckPoints=ValueThisItem(myitem, true);
									 break;
								case InventorySlot.PlayerRightFinger:
									 iMyRightFingerPoints=ValueThisItem(myitem, true);
									 break;
								case InventorySlot.PlayerRightHand:
									 iMyRightHandPoints=ValueThisItem(myitem, true);
									 break;
								case InventorySlot.PlayerShoulders:
									 iMyShouldersPoints=ValueThisItem(myitem, true);
									 break;
								case InventorySlot.PlayerTorso:
									 iMyTorsoPoints=ValueThisItem(myitem, true);
									 break;
								case InventorySlot.PlayerWaist:
									 iMyWaistPoints=ValueThisItem(myitem, true);
									 break;
						  } // End switch inventory slot
					 } // Loop through all my equipped items
				} // Do a full equipped items update?

				// Loop through anything in the backpack that we haven't already checked
				foreach (var thisitem in ZetaDia.Me.Inventory.Backpack)
				{
					 if (thisitem.BaseAddress==IntPtr.Zero)
					 {
						  return;
					 }
					 // Check this item is of the necessary item level (if not don't blacklist it until it is!)
					 if (thisitem.RequiredLevel<=ZetaDia.Me.Level&&!thisitem.IsUnidentified)
					 {
						  // Make sure we haven't already analysed this item previously
						  if (!_alreadyLookedAtBlacklist.Contains(thisitem.DynamicId))
						  {
								// Prevent this item ever being looked at again
								_alreadyLookedAtBlacklist.Add(thisitem.DynamicId);

								GilesItemType thisGilesType=DetermineItemType(thisitem.InternalName, thisitem.ItemType, thisitem.FollowerSpecialType);

								// Do things based on the type of item it is now
								switch (thisGilesType)
								{
									 case GilesItemType.TwoHandBow:
									 case GilesItemType.TwoHandCrossbow:
									 case GilesItemType.HandCrossbow:
										  // DH only
										  if (ZetaDia.Me.ActorClass==ActorClass.DemonHunter&&!bIgnoreWeapons)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if (iThisItemPoints>iMyLeftHandPoints)
												{
													 Log("Equipping weapon '"+thisitem.Name+"' (old points="+iMyLeftHandPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerLeftHand);
													 iMyLeftHandPoints=iThisItemPoints;
												}
										  }
										  break;
									 case GilesItemType.CeremonialKnife:
										  // WD only
										  if (ZetaDia.Me.ActorClass==ActorClass.WitchDoctor&&!bIgnoreWeapons)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if (iThisItemPoints>iMyLeftHandPoints)
												{
													 Log("Equipping weapon '"+thisitem.Name+"' (old points="+iMyLeftHandPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerLeftHand);
													 iMyLeftHandPoints=iThisItemPoints;
													 // We've upgraded a two-hander with a one-hander, better allow the checking of off-handers again!
													 if (bMyUsingTwoHandedMelee)
													 {
														  bNeedFullItemUpdate=true;
														  bMyUsingTwoHandedMelee=false;
													 } // Was previously using a two-hander?
												}
										  }
										  break;
									 case GilesItemType.FistWeapon:
										  // Monk only
										  if (ZetaDia.Me.ActorClass==ActorClass.Monk&&!bIgnoreWeapons)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if (iThisItemPoints>iMyLeftHandPoints)
												{
													 Log("Equipping weapon '"+thisitem.Name+"' (old points="+iMyLeftHandPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerLeftHand);
													 iMyLeftHandPoints=iThisItemPoints;
													 // We've upgraded a two-hander with a one-hander, better allow the checking of off-handers again!
													 if (bMyUsingTwoHandedMelee)
													 {
														  bNeedFullItemUpdate=true;
														  bMyUsingTwoHandedMelee=false;
													 } // Was previously using a two-hander?
												}
										  }
										  break;
									 case GilesItemType.Wand:
										  // Monk only
										  if (ZetaDia.Me.ActorClass==ActorClass.Wizard&&!bIgnoreWeapons)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if (iThisItemPoints>iMyLeftHandPoints)
												{
													 Log("Equipping weapon '"+thisitem.Name+"' (old points="+iMyLeftHandPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerLeftHand);
													 iMyLeftHandPoints=iThisItemPoints;
													 // We've upgraded a two-hander with a one-hander, better allow the checking of off-handers again!
													 if (bMyUsingTwoHandedMelee)
													 {
														  bNeedFullItemUpdate=true;
														  bMyUsingTwoHandedMelee=false;
													 } // Was previously using a two-hander?
												}
										  }
										  break;
									 case GilesItemType.MightyWeapon:
										  // Monk only
										  if (ZetaDia.Me.ActorClass==ActorClass.Barbarian&&!bIgnoreWeapons)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if (iThisItemPoints>iMyLeftHandPoints)
												{
													 Log("Equipping weapon '"+thisitem.Name+"' (old points="+iMyLeftHandPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerLeftHand);
													 iMyLeftHandPoints=iThisItemPoints;
													 // We've upgraded a two-hander with a one-hander, better allow the checking of off-handers again!
													 if (bMyUsingTwoHandedMelee)
													 {
														  bNeedFullItemUpdate=true;
														  bMyUsingTwoHandedMelee=false;
													 } // Was previously using a two-hander?
												}
										  }
										  break;
									 case GilesItemType.Axe:
									 case GilesItemType.Dagger:
									 case GilesItemType.Mace:
									 case GilesItemType.Sword:
									 case GilesItemType.Spear:
										  // Not DH
										  if (ZetaDia.Me.ActorClass!=ActorClass.DemonHunter&&!bIgnoreWeapons)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if (iThisItemPoints>iMyLeftHandPoints)
												{
													 Log("Equipping weapon '"+thisitem.Name+"' (old points="+iMyLeftHandPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerLeftHand);
													 iMyLeftHandPoints=iThisItemPoints;
													 // We've upgraded a two-hander with a one-hander, better allow the checking of off-handers again!
													 if (bMyUsingTwoHandedMelee)
													 {
														  bNeedFullItemUpdate=true;
														  bMyUsingTwoHandedMelee=false;
													 } // Was previously using a two-hander?
												}
										  }
										  break;
									 case GilesItemType.Amulet:
										  // Any class
										  if (!bIgnoreJewelry)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if (iThisItemPoints>iMyNeckPoints)
												{
													 Log("Equipping amulet '"+thisitem.Name+"' (old points="+iMyNeckPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerNeck);
													 iMyNeckPoints=iThisItemPoints;
												}
										  }
										  break;
									 case GilesItemType.Ring:
										  // Any class
										  if (!bIgnoreJewelry)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if ((iThisItemPoints>iMyLeftFingerPoints&&iThisItemPoints<=iMyRightFingerPoints)||
													 (iThisItemPoints>iMyLeftFingerPoints&&((iThisItemPoints-iMyLeftFingerPoints)>(iThisItemPoints-iMyRightFingerPoints))))
												{
													 Log("Equipping left ring '"+thisitem.Name+"' (old points="+iMyLeftFingerPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerLeftFinger);
													 iMyLeftFingerPoints=iThisItemPoints;
												}
												else if ((iThisItemPoints>iMyRightFingerPoints&&iThisItemPoints<=iMyLeftFingerPoints)||
															(iThisItemPoints>iMyRightFingerPoints&&((iThisItemPoints-iMyRightFingerPoints)>=(iThisItemPoints-iMyLeftFingerPoints))))
												{
													 Log("Equipping right ring '"+thisitem.Name+"' (old points="+iMyRightFingerPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerRightFinger);
													 iMyRightFingerPoints=iThisItemPoints;
												}
										  }
										  break;
									 case GilesItemType.Belt:
										  // Any class
										  if (!bIgnoreArmor)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if (iThisItemPoints>iMyWaistPoints)
												{
													 Log("Equipping belt '"+thisitem.Name+"' (old points="+iMyWaistPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerWaist);
													 iMyWaistPoints=iThisItemPoints;
												}
										  }
										  break;
									 case GilesItemType.MightyBelt:
										  // Wizard only
										  if (ZetaDia.Me.ActorClass==ActorClass.Barbarian&&!bIgnoreArmor)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if (iThisItemPoints>iMyWaistPoints)
												{
													 Log("Equipping barbarian mighty belt '"+thisitem.Name+"' (old points="+iMyWaistPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerWaist);
													 iMyWaistPoints=iThisItemPoints;
												}
										  }
										  break;
									 case GilesItemType.Boots:
										  // Any class
										  if (!bIgnoreArmor)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if (iThisItemPoints>iMyFeetPoints)
												{
													 Log("Equipping boots '"+thisitem.Name+"' (old points="+iMyFeetPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerFeet);
													 iMyFeetPoints=iThisItemPoints;
												}
										  }
										  break;
									 case GilesItemType.Bracers:
										  // Any class
										  if (!bIgnoreArmor)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if (iThisItemPoints>iMyBracersPoints)
												{
													 Log("Equipping bracer '"+thisitem.Name+"' (old points="+iMyBracersPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerBracers);
													 iMyBracersPoints=iThisItemPoints;
												}
										  }
										  break;
									 case GilesItemType.Chest:
										  // Any class
										  if (!bIgnoreArmor)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if (iThisItemPoints>iMyTorsoPoints)
												{
													 Log("Equipping chest '"+thisitem.Name+"' (old points="+iMyTorsoPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerTorso);
													 iMyTorsoPoints=iThisItemPoints;
												}
										  }
										  break;
									 case GilesItemType.Cloak:
										  // Wizard only
										  if (ZetaDia.Me.ActorClass==ActorClass.DemonHunter&&!bIgnoreArmor)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if (iThisItemPoints>iMyTorsoPoints)
												{
													 Log("Equipping demon hunter cloak '"+thisitem.Name+"' (old points="+iMyTorsoPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerTorso);
													 iMyTorsoPoints=iThisItemPoints;
												}
										  }
										  break;
									 case GilesItemType.Gloves:
										  // Any class
										  if (!bIgnoreArmor)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if (iThisItemPoints>iMyHandsPoints)
												{
													 Log("Equipping gloves '"+thisitem.Name+"' (old points="+iMyHandsPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerHands);
													 iMyHandsPoints=iThisItemPoints;
												}
										  }
										  break;
									 case GilesItemType.Helm:
										  // Any class
										  if (!bIgnoreArmor&&!bIgnoreHelm)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if (iThisItemPoints>iMyHeadPoints)
												{
													 Log("Equipping helm '"+thisitem.Name+"' (old points="+iMyHeadPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerHead);
													 iMyHeadPoints=iThisItemPoints;
												}
										  }
										  break;
									 case GilesItemType.WizardHat:
										  // Wizard only
										  if (ZetaDia.Me.ActorClass==ActorClass.Wizard&&!bIgnoreArmor&&!bIgnoreHelm)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if (iThisItemPoints>iMyHeadPoints)
												{
													 Log("Equipping wizard hat '"+thisitem.Name+"' (old points="+iMyHeadPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerHead);
													 iMyHeadPoints=iThisItemPoints;
												}
										  }
										  break;
									 case GilesItemType.SpiritStone:
										  // Wizard only
										  if (ZetaDia.Me.ActorClass==ActorClass.Monk&&!bIgnoreArmor&&!bIgnoreHelm)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if (iThisItemPoints>iMyHeadPoints)
												{
													 Log("Equipping monk spirit stone '"+thisitem.Name+"' (old points="+iMyHeadPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerHead);
													 iMyHeadPoints=iThisItemPoints;
												}
										  }
										  break;
									 case GilesItemType.VoodooMask:
										  // Wizard only
										  if (ZetaDia.Me.ActorClass==ActorClass.WitchDoctor&&!bIgnoreArmor&&!bIgnoreHelm)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if (iThisItemPoints>iMyHeadPoints)
												{
													 Log("Equipping witchdoctor voodoo mask '"+thisitem.Name+"' (old points="+iMyHeadPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerHead);
													 iMyHeadPoints=iThisItemPoints;
												}
										  }
										  break;
									 case GilesItemType.Pants:
										  // Any class
										  if (!bIgnoreArmor)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if (iThisItemPoints>iMyLegsPoints)
												{
													 Log("Equipping legs '"+thisitem.Name+"' (old points="+iMyLegsPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerLegs);
													 iMyLegsPoints=iThisItemPoints;
												}
										  }
										  break;
									 case GilesItemType.Shoulders:
										  // Any class
										  if (!bIgnoreArmor)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if (iThisItemPoints>iMyShouldersPoints)
												{
													 Log("Equipping shoulders '"+thisitem.Name+"' (old points="+iMyShouldersPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerShoulders);
													 iMyShouldersPoints=iThisItemPoints;
												}
										  }
										  break;
									 case GilesItemType.Mojo:
										  // Witch Doctor only and not if already using a two-hander
										  if (ZetaDia.Me.ActorClass==ActorClass.WitchDoctor&&!bMyUsingTwoHandedMelee&&!bIgnoreWeapons)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if (iThisItemPoints>iMyRightHandPoints)
												{
													 Log("Equipping mojo '"+thisitem.Name+"' (old points="+iMyRightHandPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerRightHand);
													 iMyRightHandPoints=iThisItemPoints;
												}
										  }
										  break;
									 case GilesItemType.Source:
										  // Wizard only and not if already using a two-hander
										  if (ZetaDia.Me.ActorClass==ActorClass.Wizard&&!bMyUsingTwoHandedMelee&&!bIgnoreWeapons)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if (iThisItemPoints>iMyRightHandPoints)
												{
													 Log("Equipping source '"+thisitem.Name+"' (old points="+iMyRightHandPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerRightHand);
													 iMyRightHandPoints=iThisItemPoints;
												}
										  }
										  break;
									 case GilesItemType.Quiver:
										  // DH only
										  if (ZetaDia.Me.ActorClass==ActorClass.DemonHunter&&!bIgnoreWeapons)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if (iThisItemPoints>iMyRightHandPoints)
												{
													 Log("Equipping quiver '"+thisitem.Name+"' (old points="+iMyRightHandPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerRightHand);
													 iMyRightHandPoints=iThisItemPoints;
												}
										  }
										  break;
									 case GilesItemType.Shield:
										  // Barbarian and Monk only and not if already using a two-hander
										  if ((ZetaDia.Me.ActorClass==ActorClass.Barbarian||ZetaDia.Me.ActorClass==ActorClass.Monk)&&!bMyUsingTwoHandedMelee&&!bIgnoreWeapons)
										  {
												iThisItemPoints=ValueThisItem(thisitem, false);
												Log("Evaluated '"+thisitem.Name+"'. Points="+iThisItemPoints.ToString());
												if (iThisItemPoints>iMyRightHandPoints)
												{
													 Log("Equipping shield '"+thisitem.Name+"' (old points="+iMyRightHandPoints.ToString()+")");
													 ZetaDia.Me.Inventory.EquipItem(thisitem.DynamicId, InventorySlot.PlayerRightHand);
													 iMyRightHandPoints=iThisItemPoints;
												}
										  }
										  break;
								} // Switch thisitem.itemtype
						  } // Not already in blacklist?
					 } // Not above current level?
				} // Foreach through backpack items
		  } // CheckBackpack

		  private static bool ItemTypeIsRestricted(ItemType type)
		  {
				/*
				if ((type==ItemType.CeremonialDagger||type==ItemType.VoodooMask||type==ItemType.Mojo)&&Bot.Class.AC==Zeta.Internals.Actors.ActorClass.WitchDoctor)
					 return false;

				if ((type==ItemType.MightyBelt||type==ItemType.MightyWeapon)&&Bot.Class.AC==Zeta.Internals.Actors.ActorClass.Barbarian)
					 return false;

				if ((type==ItemType.WizardHat||type==ItemType.Wand||type==ItemType.Orb)&&Bot.Class.AC==Zeta.Internals.Actors.ActorClass.Wizard)
					 return false;

				if ((type==ItemType.SpiritStone||type==ItemType.FistWeapon||type==ItemType.Daibo)&&Bot.Class.AC==Zeta.Internals.Actors.ActorClass.Monk)
					 return false;

				if ((type==ItemType.HandCrossbow||type==ItemType.Quiver||type==ItemType.Cloak)&&Bot.Class.AC==Zeta.Internals.Actors.ActorClass.DemonHunter)
					 return false;
				*/

				return (RestrictedTypes.Contains(type));
		  }

	 }
}