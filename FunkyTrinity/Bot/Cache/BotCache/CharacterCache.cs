using System;
using System.Linq;
using Zeta;
using Zeta.Internals.Actors;
using Zeta.Internals;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Internals.SNO;
using Zeta.CommonBot;
using FunkyTrinity.Enums;
using FunkyTrinity.Cache;
using FunkyTrinity.Movement;

namespace FunkyTrinity
{


		  public static partial class Bot
		  {
				///<summary>
				///Cache of all values Character related and variable.
				///</summary>
				public class CharacterCache
				{
					 public CharacterCache()
					 {
						  lastUpdatedPlayer=DateTime.Today;
						  lastPreformedNonCombatAction=DateTime.Today;
						  bIsIncapacitated=false;
						  bIsRooted=false;
						  bIsInTown=false;
						  dcurrentHealthPct=0d;
						  dCurrentEnergy=0d;
						  dCurrentEnergyPct=0d;
						  dDiscipline=0d;
						  dDisciplinePct=0d;
						  //EnergyRegenerationRate=0;
						  bWaitingForReserveEnergy=false;
						  iMyDynamicID=0;
						  iMyLevel=1;
						  iMyParagonLevel=0;
						  iSceneID=-1;
						  iCurrentWorldID=-1;
						  BackPack=new Backpack();
						  PetData=new Pets();
						  PickupRadius=1;
						  coinage=0;
							LastCachedTarget=Funky.FakeCacheObject;
						  fCharacterRadius=0f;
						  //iCurrentGameDifficulty=GameDifficulty.Invalid;
						  IsRunningOOCBehavior=false;
						  ShouldBackTrack=false;
						  BackTrackVector=Vector3.Zero;
					 }

					 private DateTime lastUpdatedPlayer { get; set; }
					 internal DateTime lastPreformedNonCombatAction { get; set; }

					 public bool bIsIncapacitated { get; set; }
					 public bool bIsRooted { get; set; }
					 public bool bIsInTown { get; set; }

					 private double dcurrentHealthPct;
					 public double dCurrentHealthPct
					 {
						  get { return dcurrentHealthPct; }
						  set 
						  { 
								dcurrentHealthPct=value;
								if (Bot.SettingsFunky.StopGameOnBotLowHealth&&value<=Bot.SettingsFunky.StopGameOnBotHealthPercent)
								{
									 Bot.ShuttingDownBot=true;
								}
						  }
					 }
					 public double dCurrentEnergy { get; set; }
					 public double dCurrentEnergyPct { get; set; }
					 public double dDiscipline { get; set; }
					 public double dDisciplinePct { get; set; }

					 private int coinage;
					 public int Coinage 
					 { 
						  get
						  {
								return coinage;
						  }
						  set
						  {
								coinage=value;
								UpdateCoinage=false;
						  }
					 }
					 internal bool UpdateCoinage { get; set; }

					 public bool ShouldFlee
					 {
						  get
						  {
								bool flee=Bot.SettingsFunky.Fleeing.EnableFleeingBehavior&&Bot.Character.dCurrentHealthPct<=Bot.SettingsFunky.Fleeing.FleeBotMinimumHealthPercent;
								return flee;
						  }
					 }

					 //internal int EnergyRegenerationRate { get; set; }
					 
					 public int iCurrentWorldID { get; set; }
					 //public GameDifficulty iCurrentGameDifficulty { get; set; }
					 internal bool IsRunningOOCBehavior { get; set; }

					 internal bool IsRunningInteractiveBehavior { get; set; }
					 internal bool ShouldBackTrack { get; set; }
					 internal Vector3 BackTrackVector { get; set; }

					 //Returns Live Data
					 private DateTime lastPositionUpdate=DateTime.Today;
					 private Vector3 lastPosition=Vector3.Zero;
					 public Vector3 Position
					 {
						  get
						  {
								//Because we don't want to update this X amount of times in a single loop!
								if (DateTime.Now.Subtract(lastPositionUpdate).TotalMilliseconds>250)
								{
									 try
									 {
										  lastPosition=ZetaDia.Me.Position;
										  lastPositionUpdate=DateTime.Now;
									 } catch (NullReferenceException)
									 {
										  lastPosition=Vector3.Zero;
									 }


								}

								return lastPosition;
						  }
					 }
					 internal GridPoint PointPosition
					 {
						  get
						  {
								return Position;
						  }
					 }

					 //Male Wizard: Radius 5.633342
					 //Female Demonhunter: Radius 6.437767
					 internal float fCharacterRadius { get; set; }


					 internal Sphere CharacterSphere
					 {
						  get
						  {
								return new Sphere(Position, fCharacterRadius);
						  }
					 }


					 internal bool bWaitingForReserveEnergy { get; set; }
					 internal int iMyDynamicID { get; set; }
					 internal int iMyLevel { get; set; }
					 internal int iMyParagonLevel { get; set; }
					 internal int iTotalPotions { get; set; }
					 internal int iSceneID { get; set; }
					 internal Pets PetData { get; set; }
					 internal Backpack BackPack { get; set; }
					 internal float PickupRadius { get; set; }
					 internal int FreeBackpackSlots { get; set; }

					 internal CacheObject LastCachedTarget { get; set; }
					 internal AnimationState CurrentAnimationState { get; set; }
					 internal SNOAnim CurrentSNOAnim { get; set; }



					 internal DateTime lastUpdateNonEssentialData=DateTime.Today;
					 internal DateTime lastCheckedSceneID=DateTime.Today;


					 internal void Update(bool combat=false, bool force=false)
					 {
						  // If we aren't in the game of a world is loading, don't do anything yet
						  if (!ZetaDia.IsInGame||ZetaDia.IsLoadingWorld)
								return;

						  var me=ZetaDia.Me;
						  if (me==null)
								return;

						  double lastUpdate=DateTime.Now.Subtract(lastUpdatedPlayer).TotalMilliseconds;
						  //Update only every 100ms, unless in combat than 25ms..
						  if (!force&&
								(combat&&lastUpdate<50||lastUpdate<150)) return;

						  using (ZetaDia.Memory.AcquireFrame())
						  {
								try
								{
									 if (Bot.Class.AC==ActorClass.DemonHunter)
									 {
										  dDiscipline=me.CurrentSecondaryResource;
										  dDisciplinePct=Bot.Character.dDiscipline/me.MaxSecondaryResource;
									 }

									 //vCurrentPosition=me.Position;
									 dCurrentHealthPct=me.HitpointsCurrentPct;
									 if (this.ShouldFlee) Bot.Combat.RequiresAvoidance=true;

									 dCurrentEnergy=me.CurrentPrimaryResource;
									 dCurrentEnergyPct=dCurrentEnergy/me.MaxPrimaryResource;
									 //EnergyRegenerationRate=me.CommonData.GetAttribute<int>(ActorAttributeType.ResourceRegenPerSecond);

									 if (dCurrentEnergy>=Bot.Class.iWaitingReservedAmount)
										  bWaitingForReserveEnergy=false;
									 if (dCurrentEnergy<20)
										  bWaitingForReserveEnergy=true;

									 //Critical Avoidance (when no avoidance is set!)
									 if (dCurrentHealthPct<0.50d&&!Bot.SettingsFunky.Avoidance.AttemptAvoidanceMovements&&
										  !Zeta.CommonBot.PowerManager.CanCast(SNOPower.DrinkHealthPotion))
										  Bot.Combat.CriticalAvoidance=true;
									 else if (Bot.Combat.CriticalAvoidance&&!Funky.shouldPreformOOCItemIDing&&!Funky.FunkyTPBehaviorFlag&&dCurrentHealthPct>0.5)
										  //Disable it when not OOC/TP/Low health still..
										  Bot.Combat.CriticalAvoidance=false;

									 bIsInTown=me.IsInTown;
									 bIsRooted=me.IsRooted;
									
									 if (me.IsFeared||me.IsStunned||me.IsFrozen||me.IsBlind)
										  bIsIncapacitated=true;
									 else
									 {
										  bool bIsInKnockBack=(me.CommonData.GetAttribute<int>(ActorAttributeType.InKnockback)!=0);
										  if (bIsInKnockBack)
												bIsIncapacitated=true;
										  else
												bIsIncapacitated=false;
									 }
									
									 //Update vars that are not essential to combat (survival).
									 if (DateTime.Now.Subtract(lastUpdateNonEssentialData).TotalSeconds>30)
									 {
										  lastUpdateNonEssentialData=DateTime.Now;

										  //update level if not 60 else update paragonlevel
										  if (iMyLevel<60) iMyLevel=me.Level;
										  else iMyParagonLevel=me.ParagonLevel;

										  iMyDynamicID=me.CommonData.DynamicId;
										  FreeBackpackSlots=me.Inventory.NumFreeBackpackSlots;
										  PickupRadius=me.GoldPickupRadius;
										  Coinage=me.Inventory.Coinage;
										  //Clear our BPItems list..
										  BackPack.BPItems.Clear();
									 }

									 if (UpdateCoinage)
										  Coinage=me.Inventory.Coinage;

									 //Check current scence every 1.5 seconds
									 if (!bIsInTown&&DateTime.Now.Subtract(lastCheckedSceneID).TotalSeconds>1.50)
									 {
										  //Get the current guid, compare/update.
										  int CurrentSceneID=me.CurrentScene.SceneGuid;
										  if (CurrentSceneID!=Bot.Character.iSceneID)
										  {
												Bot.Character.iSceneID=CurrentSceneID;
												//UpdateSearchGridProvider(true);
										  }
										  lastCheckedSceneID=DateTime.Now;
									 }
								} catch (AccessViolationException)
								{

								}
						  }
						  lastUpdatedPlayer=DateTime.Now;
					 }



					 //private DateTime LastUpdatedAnimationData=DateTime.Today;
					 internal void UpdateAnimationState(bool animState=true, bool snoAnim=true)
					 {
						  // If we aren't in the game of a world is loading, don't do anything yet
						  if (!ZetaDia.IsInGame||ZetaDia.IsLoadingWorld)//||DateTime.Now.Subtract(LastUpdatedAnimationData).TotalMilliseconds<150)
								return;

						  //LastUpdatedAnimationData=DateTime.Now;
						  using (ZetaDia.Memory.AcquireFrame())
						  {
								try
								{
									 if (animState)
										  CurrentAnimationState=ZetaDia.Me.CommonData.AnimationState;

									 if (snoAnim)
										  CurrentSNOAnim=ZetaDia.Me.CommonData.CurrentAnimation;
								} catch (Exception)
								{
								}

						  }
					 }

					 // **********************************************************************************************
					 // *****      Quick and Dirty routine just to force a wait until the character is "free"    *****
					 // **********************************************************************************************
					 public void WaitWhileAnimating(int iMaxSafetyLoops=10, bool bWaitForAttacking=false)
					 {
						  bool bKeepLooping=true;
						  int iSafetyLoops=0;
						  while (bKeepLooping)
						  {
								iSafetyLoops++;
								if (iSafetyLoops>iMaxSafetyLoops)
									 bKeepLooping=false;
								bool bIsAnimating=false;
								try
								{
									 UpdateAnimationState();
									 if (CurrentAnimationState.HasFlag(AnimationState.Casting|AnimationState.Channeling))
										  bIsAnimating=true;
									 if (bWaitForAttacking&&(CurrentAnimationState.HasFlag(AnimationState.Attacking)))
										  bIsAnimating=true;
								} catch (NullReferenceException)
								{
									 bIsAnimating=true;
								}
								if (!bIsAnimating)
									 bKeepLooping=false;
						  }
					 }



					 //Subclass: Holds data on current player pet counts
					 internal class Pets
					 {
						  public Dictionary<PetTypes, int> dictPetCounter=new Dictionary<PetTypes, int>();

						  public Pets()
						  {
								Reset();
						  }

						  // A count for player mystic ally, gargantuans, and zombie dogs
						  public int MysticAlly
						  {
								get
								{
									 if (!dictPetCounter.ContainsKey(PetTypes.MONK_MysticAlly))
										  dictPetCounter.Add(PetTypes.MONK_MysticAlly, 0);

									 return dictPetCounter[PetTypes.MONK_MysticAlly];
								}
								set
								{
									 if (!dictPetCounter.ContainsKey(PetTypes.MONK_MysticAlly))
										  dictPetCounter.Add(PetTypes.MONK_MysticAlly, value);
									 else
										  dictPetCounter[PetTypes.MONK_MysticAlly]=value;
								}
						  }
						  public int Gargantuan
						  {
								get
								{
									 if (!dictPetCounter.ContainsKey(PetTypes.WITCHDOCTOR_Gargantuan))
										  dictPetCounter.Add(PetTypes.WITCHDOCTOR_Gargantuan, 0);

									 return dictPetCounter[PetTypes.WITCHDOCTOR_Gargantuan];
								}
								set
								{
									 if (!dictPetCounter.ContainsKey(PetTypes.WITCHDOCTOR_Gargantuan))
										  dictPetCounter.Add(PetTypes.WITCHDOCTOR_Gargantuan, value);
									 else
										  dictPetCounter[PetTypes.WITCHDOCTOR_Gargantuan]=value;
								}
						  }
						  public int ZombieDogs
						  {
								get
								{
									 if (!dictPetCounter.ContainsKey(PetTypes.WITCHDOCTOR_ZombieDogs))
										  dictPetCounter.Add(PetTypes.WITCHDOCTOR_ZombieDogs, 0);

									 return dictPetCounter[PetTypes.WITCHDOCTOR_ZombieDogs];
								}
								set
								{
									 if (!dictPetCounter.ContainsKey(PetTypes.WITCHDOCTOR_ZombieDogs))
										  dictPetCounter.Add(PetTypes.WITCHDOCTOR_ZombieDogs, value);
									 else
										  dictPetCounter[PetTypes.WITCHDOCTOR_ZombieDogs]=value;
								}
						  }
						  public int DemonHunterPet
						  {
								get
								{
									 if (!dictPetCounter.ContainsKey(PetTypes.DEMONHUNTER_Pet))
										  dictPetCounter.Add(PetTypes.DEMONHUNTER_Pet, 0);

									 return dictPetCounter[PetTypes.DEMONHUNTER_Pet];
								}
								set
								{
									 if (!dictPetCounter.ContainsKey(PetTypes.DEMONHUNTER_Pet))
										  dictPetCounter.Add(PetTypes.DEMONHUNTER_Pet, value);
									 else
										  dictPetCounter[PetTypes.DEMONHUNTER_Pet]=value;
								}
						  }
						  public int WizardHydra
						  {
								get
								{
									 if (!dictPetCounter.ContainsKey(PetTypes.WIZARD_Hydra))
										  dictPetCounter.Add(PetTypes.WIZARD_Hydra, 0);

									 return dictPetCounter[PetTypes.WIZARD_Hydra];
								}
								set
								{
									 if (!dictPetCounter.ContainsKey(PetTypes.WIZARD_Hydra))
										  dictPetCounter.Add(PetTypes.WIZARD_Hydra, value);
									 else
										  dictPetCounter[PetTypes.WIZARD_Hydra]=value;
								}
						  }

						  public void Reset()
						  {
								MysticAlly=0;
								Gargantuan=0;
								ZombieDogs=0;
								DemonHunterPet=0;
								WizardHydra=0;
						  }

					 }

					 //Subclass: Holds data of backpack and multiple subclasses that contain related data
					 internal class Backpack
					 {
						  public Backpack()
						  {
								oocItemCache=new OOCItemCache();
								townRunCache=new TownRunCache();
								BPItems=new List<CacheBPItem>();
								CacheItemList=new Dictionary<int, CacheACDItem>();
						  }
						  public List<CacheBPItem> BPItems { get; set; }
						  public Dictionary<int,CacheACDItem> CacheItemList { get; set; }

						  public ACDItem BestPotionToUse { get; set; }

						  public int CurrentPotionACDGUID=-1;

						  public TownRunCache townRunCache { get; set; }

						  public OOCItemCache oocItemCache { get; set; }


						  //Sets List to current backpack contents
						  public void Update()
						  {

								int CurrentItemCount=ZetaDia.Me.Inventory.Backpack.Count();
								if (CurrentItemCount!=CacheItemList.Count||ZetaDia.Me.Inventory.Backpack.Any(i => !CacheItemList.ContainsKey(i.ACDGuid)))
								{
									 CacheItemList=new Dictionary<int, CacheACDItem>();
									 foreach (var thisitem in ZetaDia.Me.Inventory.Backpack)
									 {
										  using (ZetaDia.Memory.AcquireFrame())
										  {
												//CacheACDItem thiscacheditem=new CacheACDItem(thisitem.InternalName, thisitem.Name, thisitem.Level, thisitem.ItemQualityLevel, thisitem.Gold, thisitem.GameBalanceId,
												//            thisitem.DynamicId, thisitem.Stats.WeaponDamagePerSecond, thisitem.IsOneHand, thisitem.DyeType, thisitem.ItemType, thisitem.FollowerSpecialType,
												//            thisitem.IsUnidentified, thisitem.ItemStackQuantity, thisitem.Stats, thisitem, thisitem.InventoryRow, thisitem.InventoryColumn, thisitem.IsPotion, thisitem.ACDGuid);
												CacheACDItem thiscacheditem=new CacheACDItem(thisitem);
												CacheItemList.Add(thiscacheditem.ACDGUID,thiscacheditem);
										  }
									 }
								}



								//We refresh our BPItem Cache whenever we are checking for looted items!
								if (Bot.Combat.ShouldCheckItemLooted)
								{
									 //Get a list of current BP Cached ACDItems
									 List<ACDItem> BPItemsACDItemList=(from backpackItems in BPItems
																				  select backpackItems.Ref_ACDItem).ToList();
									 var NewItems=ZetaDia.Me.Inventory.Backpack.Where<ACDItem>(I => !BPItemsACDItemList.Contains(I));
									 if (NewItems.Count()==0) return;

									 //Now get items that are not currently in the BPItems List.
									 using (ZetaDia.Memory.AcquireFrame())
									 {
										  foreach (var item in NewItems)
										  {
												BPItems.Add(new CacheBPItem(item.ACDGuid, item));
										  }
									 }
								}
						  }

						  //Used to check if backpack is visible
						  public bool InventoryBackpackVisible()
						  {
								bool InvVisible=false;
								try
								{
									 InvVisible=UIElements.InventoryWindow.IsVisible;
								} catch
								{
								}

								return InvVisible;
						  }

						  //Used to toggle current backpack
						  public void InventoryBackPackToggle(bool show)
						  {
								bool InvVisible=InventoryBackpackVisible();

								if (InvVisible&&!show)
									 UIElements.BackgroundScreenPCButtonInventory.Click();
								else if (!InvVisible&&show)
									 UIElements.BackgroundScreenPCButtonInventory.Click();
						  }

						  public List<ACDItem> ReturnCurrentPotions()
						  {
								//Always update!
								Update();

								using (ZetaDia.Memory.AcquireFrame())
								{
									 var Potions=ZetaDia.Me.Inventory.Backpack.Where<ACDItem>(i => i.IsPotion);
									 if (Potions.Count()<=0) return null;
									 Potions=Potions.OrderByDescending(i => i.HitpointsGranted).ThenByDescending(i => i.ItemStackQuantity);
									 //Set Best Potion to use..
									 CurrentPotionACDGUID=Potions.FirstOrDefault().ACDGuid;
									 int balanceID=Potions.FirstOrDefault().GameBalanceId;
									 //Find best potion to use based upon stack
									 BestPotionToUse=Potions.Where<ACDItem>(i => i.GameBalanceId==balanceID).OrderBy(i => i.ItemStackQuantity).FirstOrDefault();
									 return Potions.ToList();

								}
						  }

						  public Queue<ACDItem> ReturnUnidenifiedItems()
						  {
								Queue<ACDItem> returnQueue=new Queue<ACDItem>();

								Update();

								using (ZetaDia.Memory.AcquireFrame())
								{
									 var filteredItems=ZetaDia.Me.Inventory.Backpack.Where<ACDItem>(i =>
										  i.IsValid&&!i.IsMiscItem);

									 if (filteredItems.Count()>0)
									 {
										  foreach (ACDItem item in filteredItems)
										  {
												try
												{
													 if (item.IsUnidentified)
														  returnQueue.Enqueue(item);
												} catch
												{
													 Logging.WriteDiagnostic("[Funky] Safetly Handled Exception: occured checking of item unidentified flag");
												}
										  }
									 }
								}

								return returnQueue;
						  }

						  private Queue<ACDItem> ReturnUnidenifiedItemsSorted(bool backwards=false)
						  {
								Queue<ACDItem> returnQueue=new Queue<ACDItem>();
								foreach (var item in GetUnidenifiedItemsSorted(backwards))
								{
									 returnQueue.Enqueue(item);
								}
								return returnQueue;
						  }

						  //Combines inventory rows into 3 groupings
						  private int InventoryRowCombine(int i)
						  {
								if ((i&1)==0)
									 return i;
								else
									 return i-1;
						  }
						  private List<ACDItem> GetUnidenifiedItemsSorted(bool Backwards=false)
						  {
								List<ACDItem> returnList=new List<ACDItem>();

								Update();
								using (ZetaDia.Memory.AcquireFrame())
								{
									 var filteredItems=ZetaDia.Me.Inventory.Backpack.Where<ACDItem>(i =>
										  i.IsValid&&!i.IsMiscItem&&i.IsUnidentified);
									 if (Backwards)
										  return filteredItems.OrderByDescending(o => InventoryRowCombine(o.InventoryRow)).ThenByDescending(o => o.InventoryColumn).ToList();
									 else
										  return filteredItems.OrderBy(o => InventoryRowCombine(o.InventoryRow)).ThenBy(o => o.InventoryColumn).ToList();
								}
						  }
						  private int ItemBaseTypePriorty(ItemBaseType type)
						  {
								switch (type)
								{
									 case ItemBaseType.Jewelry:
										  return 0;
									 case ItemBaseType.Weapon:
										  return 1;
									 case ItemBaseType.Armor:
										  return 2;
								}
								return 3;
						  }
						  private Queue<ACDItem> ReturnUnidenifiedItemsSortedByType()
						  {
								//Get sorted items, iterate and add into seperate collections, combine according to importance.
								bool backwards=((int)MathEx.Random(0, 1)==0);
								Queue<ACDItem> returnQueue=new Queue<ACDItem>();
								List<ACDItem> SortedItems=GetUnidenifiedItemsSorted(backwards);

								//Jewelery, Weapons, Armor
								foreach (var item in SortedItems.OrderBy(I => ItemBaseTypePriorty(I.ItemBaseType)))
								{
									 returnQueue.Enqueue(item);
								}

								return returnQueue;
						  }


						  public Queue<ACDItem> ReturnUnidifiedItemsRandomizedSorted()
						  {
								switch ((int)MathEx.Random(0, 1))
								{
									 case 0:
										  return ReturnUnidenifiedItemsSorted(false);
									 case 1:
										  return ReturnUnidenifiedItemsSorted(true);
									 case 2:
										  return ReturnUnidenifiedItemsSortedByType();
								}
								return ReturnUnidenifiedItems();
						  }

						  public bool ShouldRepairItems()
						  {
								try
								{
									 float repairVar=Zeta.CommonBot.Settings.CharacterSettings.Instance.RepairWhenDurabilityBelow;
									 bool ShouldRepair=false;
									 bool intown=ZetaDia.Me.IsInTown;
									 List<float> repairPct=ZetaDia.Me.Inventory.Equipped.Select(o => o.DurabilityPercent).ToList();
									 using (ZetaDia.Memory.AcquireFrame())
									 {
										  //Already in town? Have gear with 50% or less durability?
										  ShouldRepair=(repairPct.Any(o => o<=repairVar)||intown&&repairPct.Any(o => o<=50));
									 }

									 return ShouldRepair;
								} catch
								{
									 return false;
								}
						  }

						  public bool ContainsItem(int ACDGUID)
						  {
								//Update Item List
								Update();
								bool found=(from backpackItems in BPItems
												where backpackItems.ACDGUID==ACDGUID
												select backpackItems).Any();
								return found;
						  }

						  //Used to hold OOC ID behavior data
						  public class OOCItemCache
						  {
								public OOCItemCache()
								{
								}

								//Vars used in actual town runs so we don't have to recheck.
								public List<CacheItem> HerbfunkOOCKeepItems=new List<CacheItem>();
								public HashSet<int> HerbfunkOOCcheckedItemDynamicIDs=new HashSet<int>();

						  }

						  //Used to hold Town Run Data
						  public class TownRunCache
						  {
								private int InventoryRowCombine(int i)
								{
									 if ((i&1)==0)
										  return i;
									 else
										  return i-1;
								}

								public TownRunCache()
								{
								}

								// These three lists are used to cache item data from the backpack when handling sales, salvaging and stashing
								// It completely minimized D3 <-> DB memory access, to reduce any random bugs/crashes etc.
								public HashSet<CacheACDItem> hashGilesCachedKeepItems=new HashSet<CacheACDItem>();
								public HashSet<CacheACDItem> hashGilesCachedSalvageItems=new HashSet<CacheACDItem>();
								public HashSet<CacheACDItem> hashGilesCachedSellItems=new HashSet<CacheACDItem>();
								public HashSet<CacheACDItem> hashGilesCachedUnidStashItems=new HashSet<CacheACDItem>();

								public void sortSellList()
								{
									 List<CacheACDItem> sortedList=hashGilesCachedSellItems.OrderBy(o => InventoryRowCombine(o.invRow)).ThenBy(o => o.invCol).ToList();
									 HashSet<CacheACDItem> newSortedHashSet=new HashSet<CacheACDItem>();
									 foreach (CacheACDItem item in sortedList)
									 {
										  newSortedHashSet.Add(item);
									 }

									 hashGilesCachedSellItems=newSortedHashSet;

								}

								public void sortSalvagelist()
								{
									 List<CacheACDItem> sortedList=hashGilesCachedSalvageItems.OrderBy(o => InventoryRowCombine(o.invRow)).ThenBy(o => o.invCol).ToList();
									 HashSet<CacheACDItem> newSortedHashSet=new HashSet<CacheACDItem>();
									 foreach (CacheACDItem item in sortedList)
									 {
										  newSortedHashSet.Add(item);
									 }

									 hashGilesCachedSalvageItems=newSortedHashSet;

								}
						  }

					 }
				}
		  }
	 
}