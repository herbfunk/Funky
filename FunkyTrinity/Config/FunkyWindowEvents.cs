using System;
using System.Linq;
using Zeta;
using System.Windows.Controls;
using Zeta.Common;
using System.Globalization;
using System.Collections.ObjectModel;
using Zeta.Internals.Actors;
using FunkyTrinity.Enums;
using FunkyTrinity.Cache;
using FunkyTrinity.Movement;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  internal partial class FunkyWindow
         {
				 private void FunkyLogLevelChanged(object sender, EventArgs e)
				 {
					  CheckBox cbSender=(CheckBox)sender;
					  LogLevel LogLevelValue= (LogLevel)Enum.Parse(typeof(LogLevel),cbSender.Name);

					  if (Bot.SettingsFunky.FunkyLogFlags.HasFlag(LogLevelValue))
							Bot.SettingsFunky.FunkyLogFlags&=LogLevelValue;
					  else
							Bot.SettingsFunky.FunkyLogFlags|=LogLevelValue;
			    }

             private void DebugButtonClicked(object sender, EventArgs e)
             {
                 LBDebug.Items.Clear();

                 Button btnsender = (Button)sender;
                 if (btnsender.Name == "Objects")
                 {
							string OutPut=ObjectCache.Objects.DumpDebugInfo();

							LBDebug.Items.Add(OutPut);

							

							#region ColorIndex
							//Create Color Map ID
							StackPanel spColorIndexs=new StackPanel
							{
								 Orientation=Orientation.Horizontal,
							};
							CheckBox cbMonsterID=new CheckBox
							{
								 Background=System.Windows.Media.Brushes.MediumSeaGreen,
								 FontSize=11,
								 Content="Unit",
								 Foreground=System.Windows.Media.Brushes.GhostWhite,
								 Margin=new System.Windows.Thickness(5),
							};
							CheckBox cbItemID=new CheckBox
							{
								 Background=System.Windows.Media.Brushes.Gold,
								 FontSize=11,
								 Content="Item",
								 Foreground=System.Windows.Media.Brushes.GhostWhite,
								 Margin=new System.Windows.Thickness(5),
							};
							CheckBox cbDestructibleID=new CheckBox
							{
								 Background=System.Windows.Media.Brushes.DarkSlateGray,
								 FontSize=11,
								 Content="Destructible",
								 Foreground=System.Windows.Media.Brushes.GhostWhite,
								 Margin=new System.Windows.Thickness(5),
							};
							CheckBox cbInteractableID=new CheckBox
							{
								 Background=System.Windows.Media.Brushes.DimGray,
								 FontSize=11,
								 Content="Interactable",
								 Foreground=System.Windows.Media.Brushes.GhostWhite,
								 Margin=new System.Windows.Thickness(5),
							};
							spColorIndexs.Children.Add(cbMonsterID);
							spColorIndexs.Children.Add(cbItemID);
							spColorIndexs.Children.Add(cbDestructibleID);
							spColorIndexs.Children.Add(cbInteractableID);
							LBDebug.Items.Add(spColorIndexs);
							
							#endregion

                     Zeta.Common.Logging.WriteVerbose("Dumping Object Cache");

							OutPut+="\r\n";
                     try
                     {
								 var SortedValues=ObjectCache.Objects.Values.OrderBy(obj => obj.targetType.Value).ThenBy(obj=>obj.CentreDistance);
								 foreach (var item in SortedValues)
                         {
									  string objDebugStr=item.DebugString;
									  OutPut+=objDebugStr+"\r\n";

									  TextBlock objDebug=new TextBlock
									  {
											Text=objDebugStr,
											TextAlignment= System.Windows.TextAlignment.Left,
											FontSize=11,
											Foreground=(item is CacheItem)?System.Windows.Media.Brushes.Black:System.Windows.Media.Brushes.GhostWhite,
											Background=(item is CacheDestructable)?System.Windows.Media.Brushes.DarkSlateGray
											:(item is CacheUnit)?System.Windows.Media.Brushes.MediumSeaGreen
											:(item is CacheItem)?System.Windows.Media.Brushes.Gold
											:(item is CacheInteractable)?System.Windows.Media.Brushes.DimGray
											:System.Windows.Media.Brushes.Gray,
									  };
									  LBDebug.Items.Add(objDebug);
                         }
                     }
                     catch
                     {
                         LBDebug.Items.Add("End of Output due to Modification Exception");
								 return;
                     }

							Logging.WriteDiagnostic(OutPut);

                 }
                 else if (btnsender.Name == "Obstacles")
                 {
                     LBDebug.Items.Add(ObjectCache.Obstacles.DumpDebugInfo());

                     Zeta.Common.Logging.WriteVerbose("Dumping Obstacle Cache");

                     try
                     {
								 var SortedValues=ObjectCache.Obstacles.Values.OrderBy(obj => obj.Obstacletype.Value).ThenBy(obj => obj.CentreDistance);
                         foreach (var item in ObjectCache.Obstacles)
                         {
                             LBDebug.Items.Add(item.Value.DebugString);
                         }
                     }
                     catch
                     {

                         LBDebug.Items.Add("End of Output due to Modification Exception");
                     }

                 }
                 else if (btnsender.Name == "SNO")
                 {

                     LBDebug.Items.Add(ObjectCache.cacheSnoCollection.DumpDebugInfo());

                     Zeta.Common.Logging.WriteVerbose("Dumping SNO Cache");
                     try
                     {
								 var SortedValues=ObjectCache.cacheSnoCollection.Values.OrderBy(obj => obj.SNOID);
                         foreach (var item in ObjectCache.cacheSnoCollection)
                         {
                             LBDebug.Items.Add(item.Value.DebugString);
                         }
                     }
                     catch
                     {

                         LBDebug.Items.Add("End of Output due to Modification Exception");
                     }

                 }
					  else if (btnsender.Name=="CHARACTER")
                 {
							try
							{
								 Zeta.Common.Logging.WriteVerbose("Dumping Character Cache");

								 string charString=String.Format("Character Info \r\n"+
																			"DynamicID={0} -- WorldID={1} \r\n"+
																			"SNOAnim={2} AnimState={3}\r\n",
																			Bot.Character.iMyDynamicID.ToString(), Bot.Character.iCurrentWorldID.ToString(),
																			Bot.Character.CurrentSNOAnim.ToString(), Bot.Character.CurrentAnimationState.ToString());


								 LBDebug.Items.Add(charString);

							} catch (Exception ex)
							{
								 Logging.WriteVerbose("Safely Handled Exception {0}", ex.Message);
							}
                 }
					  else if (btnsender.Name=="TargetMove")
                 {
							try
							{
								 Logging.Write("TargetMovement: BlockedCounter{0} -- NonMovementCounter{1}", TargetMovement.BlockedMovementCounter, TargetMovement.NonMovementCounter);
							} catch 
							{
								 
							}
                 }
					  else if (btnsender.Name=="CombatCache")
					  {
							try
							{
								 string debugString=Bot.Combat.DebugString;
								 Logging.WriteDiagnostic(debugString);
								 LBDebug.Items.Add(debugString);
							} catch
							{

							}
					  }
					  else if (btnsender.Name=="Ability")
					  {
							try
							{
								 if (Bot.Class==null)
									  return;

								 foreach (var item in Bot.Class.Abilities.Values)
								 {
									  try
									  {
											LBDebug.Items.Add(item.DebugString());
									  } catch (Exception ex)
									  {
											 Logging.WriteVerbose("Safely Handled Exception {0}", ex.Message);
									  }
								 }


							} catch (Exception ex)
							{
								 Logging.WriteVerbose("Safely Handled Exception {0}", ex.Message);
							}

					  }
					  else if (btnsender.Name=="TEST")
					  {
							try
							{
								 Logger.Write(LogLevel.User, "Testing! {0}", "Test?");
							} catch
							{

							}
					  }
                 LBDebug.Items.Refresh();
             }

             private void WeaponItemLevelSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 if (slider_sender.Name == "Magic")
                 {
                     Bot.SettingsFunky.MinimumWeaponItemLevel[0] = Value;
                     TBMinimumWeaponLevel[0].Text = Value.ToString();
                 }
                 else
                 {
                     Bot.SettingsFunky.MinimumWeaponItemLevel[1] = Value;
                     TBMinimumWeaponLevel[1].Text = Value.ToString();
                 }
             }
             private void ArmorItemLevelSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 if (slider_sender.Name == "Magic")
                 {
                     Bot.SettingsFunky.MinimumArmorItemLevel[0] = Value;
                     TBMinimumArmorLevel[0].Text = Value.ToString();
                 }
                 else
                 {
                     Bot.SettingsFunky.MinimumArmorItemLevel[1] = Value;
                     TBMinimumArmorLevel[1].Text = Value.ToString();
                 }
             }
             private void JeweleryItemLevelSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 if (slider_sender.Name == "Magic")
                 {
                     Bot.SettingsFunky.MinimumJeweleryItemLevel[0] = Value;
                     TBMinimumJeweleryLevel[0].Text = Value.ToString();
                 }
                 else
                 {
                     Bot.SettingsFunky.MinimumJeweleryItemLevel[1] = Value;
                     TBMinimumJeweleryLevel[1].Text = Value.ToString();
                 }
             }
             private void GilesWeaponScoreSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.GilesMinimumWeaponScore = Value;
                 TBGilesWeaponScore.Text = Value.ToString();
             }
             private void GilesArmorScoreSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.GilesMinimumArmorScore = Value;
                 TBGilesArmorScore.Text = Value.ToString();
             }
             private void GilesJeweleryScoreSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.GilesMinimumJeweleryScore = Value;
                 TBGilesJeweleryScore.Text = Value.ToString();
             }

             private void LegendaryItemLevelSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.MinimumLegendaryItemLevel = Value;
                 TBMinLegendaryLevel.Text = Value.ToString();
             }
             private void HealthPotionSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.MaximumHealthPotions = Value;
                 TBMaxHealthPots.Text = Value.ToString();
             }
             private void GoldAmountSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.MinimumGoldPile = Value;
                 TBMinGoldPile.Text = Value.ToString();
             }
             private void ClusterDistanceSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.ClusterDistance = Value;
                 TBClusterDistance.Text = Value.ToString();
             }
             private void ClusterMinUnitSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.ClusterMinimumUnitCount = Value;
                 TBClusterMinUnitCount.Text = Value.ToString();
             }
             private void ClusterLowHPValueSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
                 Bot.SettingsFunky.IgnoreClusterLowHPValue = Value;
                 TBClusterLowHPValue.Text = Value.ToString();
             }
				 private void BotStopHPValueSliderChanged(object sender, EventArgs e)
				 {
					  Slider slider_sender=(Slider)sender;
					  double Value=Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
					  Bot.SettingsFunky.StopGameOnBotHealthPercent=Value;
					  TBBotStopHealthPercent.Text=Value.ToString();
				 }
				 private void UseShrineChecked(object sender, EventArgs e)
				 {
					  CheckBox cbSender=(CheckBox)sender;
					  int index=(int)Enum.Parse(typeof(ShrineTypes), cbSender.Name);
					  Bot.SettingsFunky.UseShrineTypes[index]=!(Bot.SettingsFunky.UseShrineTypes[index]);
				 }
				 private void MissileDampeningChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.MissleDampeningEnforceCloseRange=!Bot.SettingsFunky.MissleDampeningEnforceCloseRange;
				 }
             private void EnableClusteringTargetLogicChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.EnableClusteringTargetLogic = !Bot.SettingsFunky.EnableClusteringTargetLogic;
             }
             private void IgnoreClusteringBotLowHPisChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.IgnoreClusteringWhenLowHP = !Bot.SettingsFunky.IgnoreClusteringWhenLowHP;
             }
             private void ClusteringKillLowHPChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.ClusterKillLowHPUnits = !Bot.SettingsFunky.ClusterKillLowHPUnits;
             }
				 private void ClusteringAllowRangedUnitsChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.ClusteringAllowRangedUnits=!Bot.SettingsFunky.ClusteringAllowRangedUnits;
				 }
				 private void ClusteringAllowSpawnerUnitsChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.ClusteringAllowSpawnerUnits=!Bot.SettingsFunky.ClusteringAllowSpawnerUnits;
				 }

             private void PickupCraftTomesChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.PickupCraftTomes = !Bot.SettingsFunky.PickupCraftTomes;
             }
             private void PickupCraftPlansChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.PickupCraftPlans = !Bot.SettingsFunky.PickupCraftPlans;
					  spBlacksmithPlans.IsEnabled=Bot.SettingsFunky.PickupCraftPlans;
					  spJewelerPlans.IsEnabled=Bot.SettingsFunky.PickupCraftPlans;
             }

				 private void PickupBlacksmithPlanSixChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.PickupBlacksmithPlanSix=!Bot.SettingsFunky.PickupBlacksmithPlanSix;
				 }
				 private void PickupBlacksmithPlanFiveChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.PickupBlacksmithPlanFive=!Bot.SettingsFunky.PickupBlacksmithPlanFive;
				 }
				 private void PickupBlacksmithPlanFourChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.PickupBlacksmithPlanFour=!Bot.SettingsFunky.PickupBlacksmithPlanFour;
				 }

				 private void PickupBlacksmithPlanArchonGauntletsChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.PickupBlacksmithPlanArchonGauntlets=!Bot.SettingsFunky.PickupBlacksmithPlanArchonGauntlets;
				 }
				 private void PickupBlacksmithPlanArchonSpauldersChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.PickupBlacksmithPlanArchonSpaulders=!Bot.SettingsFunky.PickupBlacksmithPlanArchonSpaulders;
				 }
				 private void PickupBlacksmithPlanRazorspikesChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.PickupBlacksmithPlanRazorspikes=!Bot.SettingsFunky.PickupBlacksmithPlanRazorspikes;
				 }

				 private void PickupJewelerDesignFlawlessStarChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.PickupJewelerDesignFlawlessStar=!Bot.SettingsFunky.PickupJewelerDesignFlawlessStar;
				 }
				 private void PickupJewelerDesignPerfectStarChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.PickupJewelerDesignPerfectStar=!Bot.SettingsFunky.PickupJewelerDesignPerfectStar;
				 }
				 private void PickupJewelerDesignRadiantStarChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.PickupJewelerDesignRadiantStar=!Bot.SettingsFunky.PickupJewelerDesignRadiantStar;
				 }
				 private void PickupJewelerDesignMarquiseChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.PickupJewelerDesignMarquise=!Bot.SettingsFunky.PickupJewelerDesignMarquise;
				 }
				 private void PickupJewelerDesignAmuletChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.PickupJewelerDesignAmulet=!Bot.SettingsFunky.PickupJewelerDesignAmulet;
				 }
				 private void PickupInfernalKeysChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.PickupInfernalKeys=!Bot.SettingsFunky.PickupInfernalKeys;
				 }
				 private void PickupDemonicEssenceChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.PickupDemonicEssence=!Bot.SettingsFunky.PickupDemonicEssence;
				 }

             private void PickupFollowerItemsChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.PickupFollowerItems = !Bot.SettingsFunky.PickupFollowerItems;
             }
             private void GemsChecked(object sender, EventArgs e)
             {
                 CheckBox sender_ = (CheckBox)sender;
                 if (sender_.Name == "red") Bot.SettingsFunky.PickupGems[0] = !Bot.SettingsFunky.PickupGems[0];
                 if (sender_.Name == "green") Bot.SettingsFunky.PickupGems[1] = !Bot.SettingsFunky.PickupGems[1];
                 if (sender_.Name == "purple") Bot.SettingsFunky.PickupGems[2] = !Bot.SettingsFunky.PickupGems[2];
                 if (sender_.Name == "yellow") Bot.SettingsFunky.PickupGems[3] = !Bot.SettingsFunky.PickupGems[3];
             }
             private void MiscItemLevelSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.MiscItemLevel = Value;
                 TBMiscItemLevel.Text = Value.ToString();
             }
             private void IgnoreCombatRangeChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.IgnoreCombatRange = !Bot.SettingsFunky.IgnoreCombatRange;
             }
             private void IgnoreLootRangeChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.IgnoreLootRange = !Bot.SettingsFunky.IgnoreLootRange;
             }

             private void EliteRangeSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.EliteCombatRange = Value;
                 TBEliteRange.Text = Value.ToString();
             }
             private void GoldRangeSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.GoldRange = Value;
                 TBGoldRange.Text = Value.ToString();
             }
				 private void GlobeRangeSliderChanged(object sender, EventArgs e)
				 {
					  Slider slider_sender=(Slider)sender;
					  int Value=(int)slider_sender.Value;
					  Bot.SettingsFunky.GlobeRange=Value;
					  TBGlobeRange.Text=Value.ToString();
				 }
             private void ItemRangeSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.ItemRange = Value;
                 TBItemRange.Text = Value.ToString();
             }
             private void ShrineRangeSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.ShrineRange = Value;
                 TBShrineRange.Text = Value.ToString();
             }
             private void GlobeHealthSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
                 Bot.SettingsFunky.GlobeHealthPercent = Value;
                 TBGlobeHealth.Text = Value.ToString();
             }
             private void PotionHealthSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
                 Bot.SettingsFunky.PotionHealthPercent = Value;
                 TBPotionHealth.Text = Value.ToString();
             }
             private void ContainerRangeSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.ContainerOpenRange = Value;
                 TBContainerRange.Text = Value.ToString();
             }
             private void NonEliteRangeSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.NonEliteCombatRange = Value;
                 TBNonEliteRange.Text = Value.ToString();
             }
             private void TreasureGoblinRangeSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.TreasureGoblinRange = Value;
                 TBGoblinRange.Text = Value.ToString();
             }
             private void TreasureGoblinMinimumRangeSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.Class.GoblinMinimumRange = Value;
                 TBGoblinMinRange.Text = Value.ToString();
             }
             private void DestructibleSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.DestructibleRange = Value;
                 TBDestructibleRange.Text = Value.ToString();
             }
				 //private void ExtendCombatRangeSliderChanged(object sender, EventArgs e)
				 //{
				 //	 Slider slider_sender = (Slider)sender;
				 //	 int Value = (int)slider_sender.Value;
				 //	 Bot.SettingsFunky.ExtendedCombatRange = Value;
				 //	 TBExtendedCombatRange.Text = Value.ToString();
				 //}
             private void IgnoreCorpsesChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.IgnoreCorpses = !Bot.SettingsFunky.IgnoreCorpses;
             }
             private void IgnoreEliteMonstersChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.IgnoreAboveAverageMobs = !Bot.SettingsFunky.IgnoreAboveAverageMobs;
             }
             private void OutOfCombatMovementChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.OutOfCombatMovement = !Bot.SettingsFunky.OutOfCombatMovement;
             }
				 private void AllowBuffingInTownChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.AllowBuffingInTown=!Bot.SettingsFunky.AllowBuffingInTown;
				 }
             private void KiteSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.KiteDistance = Value;
                 TBKiteDistance.Text = Value.ToString();
             }
				 private void GroupMinimumUnitDistanceSliderChanged(object sender, EventArgs e)
				 {
					  Slider slider_sender=(Slider)sender;
					  int Value=(int)slider_sender.Value;
					  Bot.SettingsFunky.GroupingMinimumUnitDistance=Value;
					  TBGroupingMinUnitDistance.Text=Value.ToString();
				 }
				 private void GroupMaxDistanceSliderChanged(object sender, EventArgs e)
				 {
					  Slider slider_sender=(Slider)sender;
					  int Value=(int)slider_sender.Value;
					  Bot.SettingsFunky.GroupingMaximumDistanceAllowed=Value;
					  TBGroupingMaxDistance.Text=Value.ToString();
				 }
				 private void GroupMinimumClusterCountSliderChanged(object sender, EventArgs e)
				 {
					  Slider slider_sender=(Slider)sender;
					  int Value=(int)slider_sender.Value;
					  Bot.SettingsFunky.GroupingMinimumClusterCount=Value;
					  TBGroupingMinimumClusterCount.Text=Value.ToString();
				 }
				 private void GroupMinimumUnitsInClusterSliderChanged(object sender, EventArgs e)
				 {
					  Slider slider_sender=(Slider)sender;
					  int Value=(int)slider_sender.Value;
					  Bot.SettingsFunky.GroupingMinimumUnitsInCluster=Value;
					  TBGroupingMinimumUnitsInCluster.Text=Value.ToString();
				 }
				 private void FleeMonsterDistanceSliderChanged(object sender, EventArgs e)
				 {
					  Slider slider_sender=(Slider)sender;
					  int Value=(int)slider_sender.Value;
					  Bot.SettingsFunky.FleeMaxMonsterDistance=Value;
					  TBFleemonsterDistance.Text=Value.ToString();
				 }
				 private void FleeMinimumHealthSliderChanged(object sender, EventArgs e)
				 {
					  Slider slider_sender=(Slider)sender;
					  double Value=Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
					  Bot.SettingsFunky.FleeBotMinimumHealthPercent=Value;
					  TBFleeMinimumHealth.Text=Value.ToString();
				 }
             private void AfterCombatDelaySliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.AfterCombatDelay = Value;
                 TBAfterCombatDelay.Text = Value.ToString();
             }
             private void AvoidanceMinimumRetrySliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.AvoidanceRecheckMinimumRate = Value;
                 TBAvoidanceTimeLimits[0].Text = Value.ToString();
             }
             private void AvoidanceMaximumRetrySliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.AvoidanceRecheckMaximumRate = Value;
                 TBAvoidanceTimeLimits[1].Text = Value.ToString();
             }
             private void KitingMaximumRetrySliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.KitingRecheckMaximumRate = Value;
                 TBKiteTimeLimits[1].Text = Value.ToString();
             }
             private void KitingMinimumRetrySliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.KitingRecheckMinimumRate = Value;
                 TBKiteTimeLimits[0].Text = Value.ToString();
             }
             //UseAdvancedProjectileTesting
             private void UseAdvancedProjectileTestingChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.UseAdvancedProjectileTesting = !Bot.SettingsFunky.UseAdvancedProjectileTesting;
             }
             private void AvoidanceAttemptMovementChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.AttemptAvoidanceMovements = !Bot.SettingsFunky.AttemptAvoidanceMovements;
             }
             private void AvoidanceRadiusSliderValueChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 string[] slider_info = slider_sender.Name.Split("_".ToCharArray());
                 int tb_index = Convert.ToInt16(slider_info[2]);
                 float currentValue = (int)slider_sender.Value;

                 TBavoidanceRadius[tb_index].Text = currentValue.ToString();
                 AvoidanceType avoidancetype = (AvoidanceType)Enum.Parse(typeof(AvoidanceType), slider_info[0]);
                 dictAvoidanceRadius[avoidancetype] = currentValue;
             }

             private void AvoidanceHealthSliderValueChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 string[] slider_info = slider_sender.Name.Split("_".ToCharArray());
                 double currentValue = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
                 int tb_index = Convert.ToInt16(slider_info[2]);

                 TBavoidanceHealth[tb_index].Text = currentValue.ToString();
                 AvoidanceType avoidancetype = (AvoidanceType)Enum.Parse(typeof(AvoidanceType), slider_info[0]);


                 switch (Bot.ActorClass)
                 {
                     case Zeta.Internals.Actors.ActorClass.Barbarian:
                         dictAvoidanceHealthBarb[avoidancetype] = currentValue;
                         break;
                     case Zeta.Internals.Actors.ActorClass.DemonHunter:
                         dictAvoidanceHealthDemon[avoidancetype] = currentValue;
                         break;
                     case Zeta.Internals.Actors.ActorClass.Monk:
                         dictAvoidanceHealthMonk[avoidancetype] = currentValue;
                         break;
                     case Zeta.Internals.Actors.ActorClass.WitchDoctor:
                         dictAvoidanceHealthWitch[avoidancetype] = currentValue;
                         break;
                     case Zeta.Internals.Actors.ActorClass.Wizard:
                         dictAvoidanceHealthWizard[avoidancetype] = currentValue;
                         break;
                 }
             }
				 private void FleeingAttemptMovementChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.EnableFleeingBehavior=!Bot.SettingsFunky.EnableFleeingBehavior;
					  bool enabled=Bot.SettingsFunky.EnableFleeingBehavior;
					  spFleeingOptions.IsEnabled=enabled;
				 }

             #region ClassSettings

				 //private void bEnableCriticalMassChecked(object sender, EventArgs e)
				 //{
				 //    Bot.SettingsFunky.Class.bEnableCriticalMass = !Bot.SettingsFunky.Class.bEnableCriticalMass;
				 //}

             private void bWaitForArchonChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.Class.bWaitForArchon = !Bot.SettingsFunky.Class.bWaitForArchon;
             }
             private void bKiteOnlyArchonChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.Class.bKiteOnlyArchon = !Bot.SettingsFunky.Class.bKiteOnlyArchon;
             }
				 private void bCancelArchonRebuffChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.Class.bCancelArchonRebuff=!Bot.SettingsFunky.Class.bCancelArchonRebuff;
				 }
				 private void bTeleportFleeWhenLowHPChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.Class.bTeleportFleeWhenLowHP=!Bot.SettingsFunky.Class.bTeleportFleeWhenLowHP;
				 }
				 private void bTeleportIntoGroupingChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.Class.bTeleportIntoGrouping=!Bot.SettingsFunky.Class.bTeleportIntoGrouping;
				 }
             private void bSelectiveWhirlwindChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.Class.bSelectiveWhirlwind = !Bot.SettingsFunky.Class.bSelectiveWhirlwind;
             }
             private void bWaitForWrathChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.Class.bWaitForWrath = !Bot.SettingsFunky.Class.bWaitForWrath;
             }
             private void bGoblinWrathChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.Class.bGoblinWrath = !Bot.SettingsFunky.Class.bGoblinWrath;
             }
				 private void bBarbUseWOTBAlwaysChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.Class.bBarbUseWOTBAlways=!Bot.SettingsFunky.Class.bBarbUseWOTBAlways;
				 }
             private void bFuryDumpWrathChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.Class.bFuryDumpWrath = !Bot.SettingsFunky.Class.bFuryDumpWrath;
             }
             private void bFuryDumpAlwaysChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.Class.bFuryDumpAlways = !Bot.SettingsFunky.Class.bFuryDumpAlways;
             }
             private void bMonkInnaSetChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.Class.bMonkInnaSet = !Bot.SettingsFunky.Class.bMonkInnaSet;
             }
				 private void bMonkSpamMantraChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.Class.bMonkSpamMantra=!Bot.SettingsFunky.Class.bMonkSpamMantra;
				 }
             private void iDHVaultMovementDelaySliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.Class.iDHVaultMovementDelay = Value;
                 TBiDHVaultMovementDelay.Text = Value.ToString();
             }
             #endregion

             private void OOCIDChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.OOCIdentifyItems = !Bot.SettingsFunky.OOCIdentifyItems;
             }
             private void BuyPotionsDuringTownRunChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.BuyPotionsDuringTownRun = !Bot.SettingsFunky.BuyPotionsDuringTownRun;
             }
             private void EnableWaitAfterContainersChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.EnableWaitAfterContainers = !Bot.SettingsFunky.EnableWaitAfterContainers;
             }

             private void GemQualityLevelChanged(object sender, EventArgs e)
             {
					  ComboBox cbSender=(ComboBox)sender;

					  Bot.SettingsFunky.MinimumGemItemLevel=(int)Enum.Parse(typeof(GemQuality), cbSender.Items[cbSender.SelectedIndex].ToString());
             }
             class GemQualityTypes : ObservableCollection<string>
             {
                 public GemQualityTypes()
                 {
                     string[] GemNames = Enum.GetNames(typeof(GemQuality));
                     foreach (var item in GemNames)
                     {
                         Add(item);
                     }
                 }
             }
             class ItemRuleTypes : ObservableCollection<string>
             {
                 public ItemRuleTypes()
                 {

                     Add("Custom");
                     Add("Soft");
                     Add("Hard");
                 }
             }
             class ItemRuleQuality : ObservableCollection<string>
             {
                 public ItemRuleQuality()
                 {
                     Add("Common");
                     Add("Normal");
                     Add("Magic");
                     Add("Rare");
                     Add("Legendary");
                 }
             }
             class GoblinPriority : ObservableCollection<string>
             {
                 public GoblinPriority()
                 {

                     Add("None");
                     Add("Normal");
                     Add("Important");
                     Add("Ridiculousness");
                 }
             }
             private void GoblinPriorityChanged(object sender, EventArgs e)
             {
                 ComboBox senderCB = (ComboBox)sender;
                 Bot.SettingsFunky.GoblinPriority = senderCB.SelectedIndex;
             }
             private void ItemRulesTypeChanged(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.ItemRuleType = ItemRuleType.Items[ItemRuleType.SelectedIndex].ToString();
             }
				 private void ItemRulesBrowse_Click(object sender, EventArgs e)
				 {
					  System.Windows.Forms.FolderBrowserDialog OFD=new System.Windows.Forms.FolderBrowserDialog
					  {
							
					  };
					  System.Windows.Forms.DialogResult OFD_Result=OFD.ShowDialog();
					  
					  if (OFD_Result==System.Windows.Forms.DialogResult.OK)
					  {
							try
							{
								 Bot.SettingsFunky.ItemRuleCustomPath=OFD.SelectedPath;
								 tbCustomItemRulePath.Text=Bot.SettingsFunky.ItemRuleCustomPath;
							} catch
							{

							}
					  }
				 }
             private void ItemRulesScoringChanged(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.ItemRuleGilesScoring = ItemRuleGilesScoring.IsChecked.Value;
             }
             private void ItemRulesLogPickupChanged(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.ItemRuleLogPickup = ItemRuleLogPickup.Items[ItemRuleLogPickup.SelectedIndex].ToString();
             }
             private void ItemRulesLogKeepChanged(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.ItemRuleLogKeep = ItemRuleLogKeep.Items[ItemRuleLogKeep.SelectedIndex].ToString();
             }

             private void ItemRulesChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.UseItemRules = !Bot.SettingsFunky.UseItemRules;
					  ItemRuleGilesScoring.IsEnabled=!Bot.SettingsFunky.UseItemRules;
					  ItemRuleDBScoring.IsEnabled=!Bot.SettingsFunky.UseItemRules;
             }
             private void ItemRulesPickupChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.UseItemRulesPickup = !Bot.SettingsFunky.UseItemRulesPickup;
             }
             private void ItemRulesItemIDsChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.ItemRuleUseItemIDs = !Bot.SettingsFunky.ItemRuleUseItemIDs;
             }
             private void ItemRulesDebugChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.ItemRuleDebug = !Bot.SettingsFunky.ItemRuleDebug;
             }
             private void ItemLevelingLogicChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.UseLevelingLogic = !Bot.SettingsFunky.UseLevelingLogic;
             }
             private void ItemRulesSalvagingChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.ItemRulesSalvaging = !Bot.SettingsFunky.ItemRulesSalvaging;
             }
				 private void ItemRulesUnidStashingChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.ItemRulesUnidStashing=!Bot.SettingsFunky.ItemRulesUnidStashing;
				 }
             //UseLevelingLogic
				 private void ItemRulesOpenFolder_Click(object sender, EventArgs e)
				 {
					  System.Diagnostics.Process.Start(System.IO.Path.Combine(FolderPaths.sTrinityPluginPath, "ItemRules", "Rules"));
				 }
             private void ItemRulesReload_Click(object sender, EventArgs e)
             {
					  if (Funky.ItemRulesEval==null)
					  {
							Log("Cannot reload rules until bot has started", true);
							return;
					  }

                 try
                 {
                     Funky.ItemRulesEval.reloadFromUI();
                 }
                 catch (Exception ex)
                 {
                     Log(ex.Message + "\r\n" + ex.StackTrace);
                 }

             }
             private void DebugStatusBarChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.DebugStatusBar = !Bot.SettingsFunky.DebugStatusBar;
             }
				 private void SkipAheadChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.SkipAhead=!Bot.SettingsFunky.SkipAhead;
				 }
				 private void StopGameOnBotLowHealthChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.StopGameOnBotLowHealth=!Bot.SettingsFunky.StopGameOnBotLowHealth;
					  bool enabled=Bot.SettingsFunky.StopGameOnBotLowHealth;
					  spBotStop.IsEnabled=enabled;
				 }
				 private void StopGameOnBotEnableScreenShotChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.StopGameOnBotEnableScreenShot=!Bot.SettingsFunky.StopGameOnBotEnableScreenShot;
				 }
				 private void GroupingBehaviorChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.AttemptGroupingMovements=!Bot.SettingsFunky.AttemptGroupingMovements;
						bool enabled=Bot.SettingsFunky.AttemptGroupingMovements;
						spGroupingOptions.IsEnabled=enabled;
				 }
             private void ExtendRangeRepChestChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.UseExtendedRangeRepChest = !Bot.SettingsFunky.UseExtendedRangeRepChest;
             }
             private void EnableCoffeeBreaksChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.EnableCoffeeBreaks = !Bot.SettingsFunky.EnableCoffeeBreaks;
             }
             private void BreakMinMinutesSliderChange(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.MinBreakTime = Value;
                 tbMinBreakTime.Text = Value.ToString();
             }
             private void BreakMaxMinutesSliderChange(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.SettingsFunky.MaxBreakTime = Value;
                 tbMaxBreakTime.Text = Value.ToString();
             }

             #region OOCIDItemTextBox
             private void OOCIdentifyItemsMinValueChanged(object sender, EventArgs e)
             {
                 string lastText = OOCIdentfyItemsMinCount.Text;
                 if (isStringFullyNumerical(lastText))
                 {
                     Bot.SettingsFunky.OOCIdentifyItemsMinimumRequired = Convert.ToInt32(lastText);
                 }
             }
             private void OOCMinimumItems_KeyUp(object sender, EventArgs e)
             {
                 if (!Char.IsNumber(OOCIdentfyItemsMinCount.Text.Last()))
                 {
                     OOCIdentfyItemsMinCount.Text = OOCIdentfyItemsMinCount.Text.Substring(0, OOCIdentfyItemsMinCount.Text.Length - 1);
                 }
             }
             #endregion

             private void BreakTimeHourSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
                 Bot.SettingsFunky.breakTimeHour = Value;
                 TBBreakTimeHour.Text = Value.ToString();
             }

             private bool isStringFullyNumerical(String S, bool isDouble = false)
             {
                 if (!isDouble)
                 {
                     return !S.Any<Char>(c => !Char.IsNumber(c));
                 }
                 else
                 {
                     System.Text.RegularExpressions.Regex isnumber = new System.Text.RegularExpressions.Regex(@"^[0-9]+(\.[0-9]+)?$");
                     return isnumber.IsMatch(S);
                 }
             }

             private void OpenPluginSettings_Click(object sender, EventArgs e)
             {
                 var thisplugin = Zeta.Common.Plugins.PluginManager.Plugins.Where(p => p.Plugin.Name == "FunkyTrinity");
                 if (thisplugin.Any())
                     thisplugin.First().Plugin.DisplayWindow.Show();
             }
             private void OpenPluginFolder_Click(object sender, EventArgs e)
             {
                 System.Diagnostics.Process.Start(FolderPaths.sTrinityPluginPath);
             }

             protected override void OnClosed(EventArgs e)
             {
                 Settings_Funky.SerializeToXML(Bot.SettingsFunky);
                 base.OnClosed(e);
             }

         }
    }
}