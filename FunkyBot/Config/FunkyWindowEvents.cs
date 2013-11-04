using System;
using System.IO;
using System.Linq;
using System.Windows;
using FunkyBot.Cache;
using FunkyBot.Movement;
using FunkyBot.Settings;
using Zeta;
using System.Windows.Controls;
using Zeta.Common;
using System.Globalization;
using System.Collections.ObjectModel;
using Zeta.Internals.Actors;

namespace FunkyBot
{

		  internal partial class FunkyWindow
         {
				internal static void buttonFunkySettingDB_Click(object sender, RoutedEventArgs e)
				 {
					 //Update Account Details when bot is not running!
					 if (!Zeta.CommonBot.BotMain.IsRunning)
						 Bot.Game.UpdateCurrentAccountDetails();

					 string settingsFolder = FolderPaths.sDemonBuddyPath + @"\Settings\FunkyBot\" + Bot.Game.CurrentAccountName;
					  if (!Directory.Exists(settingsFolder)) Directory.CreateDirectory(settingsFolder);

					  try
					  {
							funkyConfigWindow=new FunkyWindow();
							funkyConfigWindow.Show();
					  } catch (Exception ex)
					  {
							Logging.WriteVerbose("Failure to initilize Funky Setting Window! \r\n {0} \r\n {1} \r\n {2}", ex.Message, ex.Source, ex.StackTrace);
					  }
				 }

				internal static FunkyWindow funkyConfigWindow;

                private void DefaultOpenSettingsFileClicked(object sender, EventArgs e)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(FolderPaths.sFunkySettingsCurrentPath);
                    }
                    catch
                    {

                    }
                }
				private void DefaultMenuLevelingClicked(object sender, EventArgs e)
				{
					 System.Windows.MessageBoxResult confirm=System.Windows.MessageBox.Show(funkyConfigWindow, 
						  "Are you sure you want to overwrite settings with default settings?", 
						  "Confirm Overwrite", System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Question);
					 if (confirm== System.Windows.MessageBoxResult.Yes)
					 {
						  string DefaultLeveling=Path.Combine(FolderPaths.sTrinityPluginPath, "Config", "Defaults", "LowLevel.xml");
						  Logging.Write("Creating new settings for {0} -- {1} using file {2}", Bot.Game.CurrentAccountName, Bot.Game.CurrentHeroName, DefaultLeveling);
						  Settings_Funky newSettings=Settings_Funky.DeserializeFromXML(DefaultLeveling);
						  Bot.Settings=newSettings;
						  funkyConfigWindow.Close();
					 }
				}
				private void DefaultMenuLoadProfileClicked(object sender, EventArgs e)
				{
					 System.Windows.Forms.OpenFileDialog OFD=new System.Windows.Forms.OpenFileDialog
					 {
						  InitialDirectory=Path.Combine(FolderPaths.sTrinityPluginPath, "Config", "Defaults"),
						  RestoreDirectory=false,
						  Filter="xml files (*.xml)|*.xml|All files (*.*)|*.*",
						  Title="Open Settings",
					 };
					 System.Windows.Forms.DialogResult OFD_Result=OFD.ShowDialog();

					 if (OFD_Result==System.Windows.Forms.DialogResult.OK)
					 {
						  try
						  {
								System.Windows.MessageBoxResult confirm=System.Windows.MessageBox.Show(funkyConfigWindow, "Are you sure you want to overwrite settings with selected profile?", "Confirm Overwrite", System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Question);
								if (confirm==System.Windows.MessageBoxResult.Yes)
								{
									Logging.Write("Creating new settings for {0} -- {1} using file {2}", Bot.Game.CurrentAccountName, Bot.Game.CurrentHeroName, OFD.FileName);
									 Settings_Funky newSettings=Settings_Funky.DeserializeFromXML(OFD.FileName);
									 Bot.Settings=newSettings;
									 funkyConfigWindow.Close();
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

					  if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevelValue))
							Bot.Settings.Debug.FunkyLogFlags&=LogLevelValue;
					  else
							Bot.Settings.Debug.FunkyLogFlags|=LogLevelValue;
			    }
				 private void FunkyLogLevelComboBoxSelected(object sender, EventArgs e)
				 {
					  //LogLevelNone
					  //LogLevelAll
					  RadioButton CBsender=(RadioButton)sender;
					  if (CBsender.Name=="LogLevelNone")
					  {
							CBLogLevels.ForEach(cb => cb.IsChecked=false);
							Bot.Settings.Debug.FunkyLogFlags=LogLevel.None;
					  }
					  else
					  {
							CBLogLevels.ForEach(cb => cb.IsChecked=true);
							Bot.Settings.Debug.FunkyLogFlags=LogLevel.All;
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

								 LBDebug.Items.Add(Bot.Character.DebugString());

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
								 foreach (var item in Bot.Class.HotBar.CachedPowers)
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
                                LBDebug.Items.Add(String.Format("Last Avoidance: {0}",Bot.Targeting.LastAvoidanceMovement.ToString()));
                                LBDebug.Items.Add(String.Format("Avoided Last Target=={0}",Bot.Targeting.AvoidanceLastTarget.ToString()));

							} catch
							{

							}
					  }
                 LBDebug.Items.Refresh();
             }

             #region ClassSettings

             private void bWaitForArchonChecked(object sender, EventArgs e)
             {
                 Bot.Settings.Class.bWaitForArchon = !Bot.Settings.Class.bWaitForArchon;
             }
             private void bKiteOnlyArchonChecked(object sender, EventArgs e)
             {
                 Bot.Settings.Class.bKiteOnlyArchon = !Bot.Settings.Class.bKiteOnlyArchon;
             }
				 private void bCancelArchonRebuffChecked(object sender, EventArgs e)
				 {
					  Bot.Settings.Class.bCancelArchonRebuff=!Bot.Settings.Class.bCancelArchonRebuff;
				 }
				 private void bTeleportFleeWhenLowHPChecked(object sender, EventArgs e)
				 {
					  Bot.Settings.Class.bTeleportFleeWhenLowHP=!Bot.Settings.Class.bTeleportFleeWhenLowHP;
				 }
				 private void bTeleportIntoGroupingChecked(object sender, EventArgs e)
				 {
					  Bot.Settings.Class.bTeleportIntoGrouping=!Bot.Settings.Class.bTeleportIntoGrouping;
				 }
             private void bSelectiveWhirlwindChecked(object sender, EventArgs e)
             {
                 Bot.Settings.Class.bSelectiveWhirlwind = !Bot.Settings.Class.bSelectiveWhirlwind;
             }
             private void bWaitForWrathChecked(object sender, EventArgs e)
             {
                 Bot.Settings.Class.bWaitForWrath = !Bot.Settings.Class.bWaitForWrath;
             }
             private void bGoblinWrathChecked(object sender, EventArgs e)
             {
                 Bot.Settings.Class.bGoblinWrath = !Bot.Settings.Class.bGoblinWrath;
             }
				 private void bBarbUseWOTBAlwaysChecked(object sender, EventArgs e)
				 {
					  Bot.Settings.Class.bBarbUseWOTBAlways=!Bot.Settings.Class.bBarbUseWOTBAlways;
				 }
             private void bFuryDumpWrathChecked(object sender, EventArgs e)
             {
                 Bot.Settings.Class.bFuryDumpWrath = !Bot.Settings.Class.bFuryDumpWrath;
             }
             private void bFuryDumpAlwaysChecked(object sender, EventArgs e)
             {
                 Bot.Settings.Class.bFuryDumpAlways = !Bot.Settings.Class.bFuryDumpAlways;
             }
             private void bMonkMaintainSweepingWindChecked(object sender, EventArgs e)
             {
                 Bot.Settings.Class.bMonkMaintainSweepingWind = !Bot.Settings.Class.bMonkMaintainSweepingWind;
             }
             private void bMonkSpamMantraChecked(object sender, EventArgs e)
             {
                 Bot.Settings.Class.bMonkSpamMantra = !Bot.Settings.Class.bMonkSpamMantra;
             }
             private void iDHVaultMovementDelaySliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 Bot.Settings.Class.iDHVaultMovementDelay = Value;
                 TBiDHVaultMovementDelay.Text = Value.ToString();
             }
             #endregion

    
             private void DebugStatusBarChecked(object sender, EventArgs e)
             {
                 Bot.Settings.Debug.DebugStatusBar = !Bot.Settings.Debug.DebugStatusBar;
             }
				 private void SkipAheadChecked(object sender, EventArgs e)
				 {
					  Bot.Settings.Debug.SkipAhead=!Bot.Settings.Debug.SkipAhead;
				 }


             protected override void OnClosed(EventArgs e)
             {
					  Settings_Funky.SerializeToXML(Bot.Settings);
                 base.OnClosed(e);
             }

         }
    
}