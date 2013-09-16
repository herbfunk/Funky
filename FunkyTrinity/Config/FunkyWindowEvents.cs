using System;
using System.IO;
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
using FunkyTrinity.Settings;

namespace FunkyTrinity
{

		  internal partial class FunkyWindow
         {
				private void DefaultMenuLevelingClicked(object sender, EventArgs e)
				{
					 System.Windows.MessageBoxResult confirm=System.Windows.MessageBox.Show(Funky.funkyConfigWindow, 
						  "Are you sure you want to overwrite settings with default settings?", 
						  "Confirm Overwrite", System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Question);
					 if (confirm== System.Windows.MessageBoxResult.Yes)
					 {
						  Bot.SettingsFunky.Grouping.AttemptGroupingMovements=false;
						  Bot.SettingsFunky.Fleeing.EnableFleeingBehavior=false;
						  Bot.SettingsFunky.Cluster.EnableClusteringTargetLogic=false;
						  Bot.SettingsFunky.UseLevelingLogic=true;
						  Settings_Funky.SerializeToXML(Bot.SettingsFunky);
						  Funky.funkyConfigWindow.Close();
					 }
				}
				private void DefaultMenuLoadProfileClicked(object sender, EventArgs e)
				{
					 System.Windows.Forms.OpenFileDialog OFD=new System.Windows.Forms.OpenFileDialog
					 {
						  InitialDirectory=Path.Combine(Funky.FolderPaths.sTrinityPluginPath, "Config", "Defaults"),
						  RestoreDirectory=false,
						  Filter="xml files (*.xml)|*.xml|All files (*.*)|*.*",
						  Title="Fleeing Template",
					 };
					 System.Windows.Forms.DialogResult OFD_Result=OFD.ShowDialog();

					 if (OFD_Result==System.Windows.Forms.DialogResult.OK)
					 {
						  try
						  {
								System.Windows.MessageBoxResult confirm=System.Windows.MessageBox.Show(Funky.funkyConfigWindow, "Are you sure you want to overwrite settings with selected profile?", "Confirm Overwrite", System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Question);
								if (confirm==System.Windows.MessageBoxResult.Yes)
								{
									 Logging.Write("Creating new settings for {0} -- {1} using file {2}", Bot.CurrentAccountName, Bot.CurrentHeroName, OFD.FileName);
									 Settings_Funky newSettings=Settings_Funky.DeserializeFromXML(OFD.FileName);
									 Bot.SettingsFunky=newSettings;
									 Funky.funkyConfigWindow.Close();
								}
						  } catch
						  {

						  }
					 }
				}

				 private void FunkyLogLevelChanged(object sender, EventArgs e)
				 {
					  CheckBox cbSender=(CheckBox)sender;
					  LogLevel LogLevelValue= (LogLevel)Enum.Parse(typeof(LogLevel),cbSender.Name);

					  if (Bot.SettingsFunky.Debug.FunkyLogFlags.HasFlag(LogLevelValue))
							Bot.SettingsFunky.Debug.FunkyLogFlags&=LogLevelValue;
					  else
							Bot.SettingsFunky.Debug.FunkyLogFlags|=LogLevelValue;
			    }
				 private void FunkyLogLevelComboBoxSelected(object sender, EventArgs e)
				 {
					  //LogLevelNone
					  //LogLevelAll
					  RadioButton CBsender=(RadioButton)sender;
					  if (CBsender.Name=="LogLevelNone")
					  {
							CBLogLevels.ForEach(cb => cb.IsChecked=false);
							Bot.SettingsFunky.Debug.FunkyLogFlags=LogLevel.None;
					  }
					  else
					  {
							CBLogLevels.ForEach(cb => cb.IsChecked=true);
							Bot.SettingsFunky.Debug.FunkyLogFlags=LogLevel.All;
					  }
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
								 if (Bot.Class==null) return;

								 LBDebug.Items.Add("==Current HotBar Abilities==");
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

								 LBDebug.Items.Add("==Cached HotBar Abilities==");
								 foreach (var item in Bot.Class.CachedPowers)
								 {
									  try
									  {
											LBDebug.Items.Add(item.ToString());
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
								 LBDebug.Items.Add(Bot.NavigationCache.CurrentPathVector.ToString());
							} catch
							{

							}
					  }
                 LBDebug.Items.Refresh();
             }

             #region ClassSettings

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

    
             private void DebugStatusBarChecked(object sender, EventArgs e)
             {
                 Bot.SettingsFunky.Debug.DebugStatusBar = !Bot.SettingsFunky.Debug.DebugStatusBar;
             }
				 private void SkipAheadChecked(object sender, EventArgs e)
				 {
					  Bot.SettingsFunky.Debug.SkipAhead=!Bot.SettingsFunky.Debug.SkipAhead;
				 }

             protected override void OnClosed(EventArgs e)
             {
					  Settings_Funky.SerializeToXML(Bot.SettingsFunky);
                 base.OnClosed(e);
             }

         }
    
}