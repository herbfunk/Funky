using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Demonbuddy;
using FunkyBot.Settings;
using FunkyBot.Game;

namespace FunkyBot
{

			[ComVisible(false)]
			internal partial class FunkyWindow : Window
			{
				 private TabControl tcItems;
				 private TabControl tcTargeting;
				 private TabControl tcGeneral;
				 private TabControl CombatTabControl;

				 private ListBox lbGeneralContent;

				 private TextBox TBGoblinMinRange, TBiDHVaultMovementDelay;


				 private ListBox LBDebug;

				 private CheckBox[] CBLogLevels;

				 public FunkyWindow()
				 {
						Settings_Funky.LoadFunkyConfiguration();

						Owner=App.Current.MainWindow;
						Title = "Funky Settings -- " + Bot.Character.Account.CurrentHeroName;
						SizeToContent=SizeToContent.WidthAndHeight;
						ResizeMode=ResizeMode.CanMinimize;
						Background=Brushes.Black;
						Foreground=Brushes.PaleGoldenrod;
						//this.Width=600;
						//this.Height=600;

						ListBox LBWindowContent=new ListBox
						{
							 HorizontalAlignment=HorizontalAlignment.Stretch,
							 VerticalAlignment=VerticalAlignment.Stretch,
						};

						Menu Menu_Settings=new Menu
						{
							 HorizontalAlignment= HorizontalAlignment.Stretch,
						};
						MenuItem Menu_Defaults=new MenuItem
						{
							 Header="Settings",
							 Height=25,
							 FontSize=12,
						};
						MenuItem Menu_Default_Open=new MenuItem
						{
							 Header="Open File..",
							 Height=25,
							 FontSize=12,
						};
						Menu_Default_Open.Click+=DefaultMenuLoadProfileClicked;
						Menu_Defaults.Items.Add(Menu_Default_Open);
						MenuItem Menu_Default_Leveling=new MenuItem
						{
							 Header="Use Default Leveling",
							 Height=25,
							 FontSize=12,
						};
						Menu_Default_Leveling.Click+=DefaultMenuLevelingClicked;
						Menu_Defaults.Items.Add(Menu_Default_Leveling);
                        MenuItem Menu_ViewSettingFile = new MenuItem
                        {
                            Header = "Open Settings File",
                            Height = 25,
                            FontSize = 12,
                        };
                        Menu_ViewSettingFile.Click += DefaultOpenSettingsFileClicked;
                        Menu_Defaults.Items.Add(Menu_ViewSettingFile);

						Menu_Settings.Items.Add(Menu_Defaults);
						LBWindowContent.Items.Add(Menu_Settings);

						TabControl tabControl1=new TabControl
						{
							 Width=600,
							 Height=600,
							 HorizontalAlignment=HorizontalAlignment.Stretch,
							 VerticalAlignment=VerticalAlignment.Stretch,
							 VerticalContentAlignment=VerticalAlignment.Stretch,
							 HorizontalContentAlignment=HorizontalAlignment.Stretch,
							 FontSize=12,
						};
						LBWindowContent.Items.Add(tabControl1);

						#region Combat
						//Character
						TabItem CombatSettingsTabItem=new TabItem();
						CombatSettingsTabItem.Header="Combat";

						tabControl1.Items.Add(CombatSettingsTabItem);
						CombatTabControl=new TabControl
						{
							 HorizontalAlignment=HorizontalAlignment.Stretch,
							 VerticalAlignment=VerticalAlignment.Stretch,
						};

					  InitCombatControls();

					  InitClusteringControls();

					  InitGroupingControls();

					  InitAvoidanceControls();

                      InitFleeingControls();

					  InitPlayerClassControls();
						


						CombatSettingsTabItem.Content=CombatTabControl;
						#endregion


						#region Targeting
						TabItem TargetTabItem=new TabItem();
						TargetTabItem.Header="Targeting";
						tabControl1.Items.Add(TargetTabItem);

						tcTargeting=new TabControl
						{
							 Height=600,
							 Width=600,
							 Focusable=false,
						};

						InitTargetingGeneralControls();
						InitTargetRangeControls();
                        InitLOSMovementControls();

						TargetTabItem.Content=tcTargeting;

						#endregion


						#region General
						tcGeneral=new TabControl
						{
							 Width=600,
							 Height=600,
						};

						TabItem GeneralSettingsTabItem=new TabItem();
						GeneralSettingsTabItem.Header="General";
						tabControl1.Items.Add(GeneralSettingsTabItem);

						InitGeneralControls();

						GeneralSettingsTabItem.Content=tcGeneral;
						#endregion


						#region Items


						TabItem CustomSettingsTabItem=new TabItem();
						CustomSettingsTabItem.Header="Items";
						tabControl1.Items.Add(CustomSettingsTabItem);

						tcItems=new TabControl
						{
							 Width=600,
							 Height=600
						};

						InitItemRulesControls();
						InitLootPickUpControls();
						InitItemScoringControls();

						CustomSettingsTabItem.Content=tcItems;
						#endregion


						TabItem AdvancedTabItem=new TabItem();
						AdvancedTabItem.Header="Advanced";
						tabControl1.Items.Add(AdvancedTabItem);
						ListBox lbAdvancedContent=new ListBox();

						#region Debug Logging
						StackPanel SPLoggingOptions=new StackPanel();
						TextBlock Logging_Text_Header=new TextBlock
						{
							 Text="Debug Output Options",
							 FontSize=12,
							 Background=Brushes.LightSeaGreen,
							 TextAlignment=TextAlignment.Center,
							 HorizontalAlignment=HorizontalAlignment.Stretch,
						};
						SPLoggingOptions.Children.Add(Logging_Text_Header);

						CheckBox CBDebugStatusBar=new CheckBox
						{
							 Content="Enable Debug Status Bar",
							 Width=300,
							 Height=20,
							 IsChecked=Bot.Settings.Debug.DebugStatusBar,
						};
						CBDebugStatusBar.Checked+=DebugStatusBarChecked;
						CBDebugStatusBar.Unchecked+=DebugStatusBarChecked;
						SPLoggingOptions.Children.Add(CBDebugStatusBar);


						TextBlock Logging_FunkyLogLevels_Header=new TextBlock
						{
							 Text="Funky Logging Options",
							 FontSize=12,
							 Background=Brushes.MediumSeaGreen,
							 TextAlignment=TextAlignment.Center,
							 HorizontalAlignment=HorizontalAlignment.Stretch,
						};

						StackPanel panelFunkyLogFlags=new StackPanel
						{
							 Orientation= Orientation.Vertical,
							 VerticalAlignment= VerticalAlignment.Stretch,
						};
						panelFunkyLogFlags.Children.Add(Logging_FunkyLogLevels_Header);

						var LogLevels=Enum.GetValues(typeof(LogLevel));
					   Logger.GetLogLevelName fRetrieveNames=s=> Enum.GetName(typeof(LogLevel), s);

						CBLogLevels=new CheckBox[LogLevels.Length-2];
					  int counter=0;
					  bool noFlags=Bot.Settings.Debug.FunkyLogFlags.Equals(LogLevel.None);
						foreach (var logLevel in LogLevels)
						{
							 LogLevel thisloglevel=(LogLevel)logLevel;
							 if (thisloglevel.Equals(LogLevel.None)||thisloglevel.Equals(LogLevel.All)) continue;

							 string loglevelName=fRetrieveNames(logLevel);
							 CBLogLevels[counter]=new CheckBox
							 {
								  Name=loglevelName,
								  Content=loglevelName,
								  IsChecked=!noFlags?Bot.Settings.Debug.FunkyLogFlags.HasFlag(thisloglevel):false,
							 };
							 CBLogLevels[counter].Checked+=FunkyLogLevelChanged;
							 CBLogLevels[counter].Unchecked+=FunkyLogLevelChanged;

							 panelFunkyLogFlags.Children.Add(CBLogLevels[counter]);
							 counter++;
						}

						StackPanel StackPanelLogLevelComboBoxes=new StackPanel();
						RadioButton cbLogLevelNone=new RadioButton
						{
							 Name="LogLevelNone",
							 Content="None",
						};
						cbLogLevelNone.Checked+=FunkyLogLevelComboBoxSelected;
						RadioButton cbLogLevelAll=new RadioButton
						{
							 Name="LogLevelAll",
							 Content="All",
						};
						cbLogLevelAll.Checked+=FunkyLogLevelComboBoxSelected;

						StackPanelLogLevelComboBoxes.Children.Add(cbLogLevelNone);
						StackPanelLogLevelComboBoxes.Children.Add(cbLogLevelAll);
						panelFunkyLogFlags.Children.Add(StackPanelLogLevelComboBoxes);

						SPLoggingOptions.Children.Add(panelFunkyLogFlags);

						lbAdvancedContent.Items.Add(SPLoggingOptions);
						#endregion

						


						CheckBox CBSkipAhead=new CheckBox
						{
							 Content="Skip Ahead Feature (TrinityMoveTo/Explore)",
							 Width=300,
							 Height=20,
							 IsChecked=Bot.Settings.Debug.SkipAhead,
						};
						CBSkipAhead.Checked+=SkipAheadChecked;
						CBSkipAhead.Unchecked+=SkipAheadChecked;
						lbAdvancedContent.Items.Add(CBSkipAhead);


                        //CheckBox CBLineOfSightBehavior = new CheckBox
                        //{
                        //    Content = "Enable Line-Of-Sight Behavior",
                        //    Width = 300,
                        //    Height = 20,
                        //    IsChecked = Bot.Settings.Plugin.EnableLineOfSightBehavior,
                        //};
                        //CBLineOfSightBehavior.Checked += LineOfSightBehaviorChecked;
                        //CBLineOfSightBehavior.Unchecked += LineOfSightBehaviorChecked;
                        //lbAdvancedContent.Items.Add(CBLineOfSightBehavior);

						AdvancedTabItem.Content=lbAdvancedContent;

						TabItem MiscTabItem=new TabItem();
						MiscTabItem.Header="Misc";
						tabControl1.Items.Add(MiscTabItem);
						ListBox lbMiscContent=new ListBox();
						try
						{



							GameStats cur = Bot.Game.CurrentGameStats;
							lbMiscContent.Items.Add("\r\n== CURRENT GAME SUMMARY ==");
							lbMiscContent.Items.Add(String.Format("Total Profiles:{0}\r\nDeaths:{1} TotalTime:{2} TotalGold:{3} TotalXP:{4}\r\n{5}",
																	cur.Profiles.Count, cur.TotalDeaths, cur.TotalTimeRunning.ToString(@"hh\ \h\ mm\ \m\ ss\ \s"), cur.TotalGold, cur.TotalXP, cur.TotalLootTracker.ToString()));

                            if (Bot.Game.CurrentGameStats.Profiles.Count > 0)
                            {
                                lbMiscContent.Items.Add("\r\n== PROFILES ==");
								foreach (var item in Bot.Game.CurrentGameStats.Profiles)
                                {
									lbMiscContent.Items.Add(String.Format("{0}\r\nDeaths:{1} TotalTime:{2} TotalGold:{3} TotalXP:{4}\r\n{5}",
										item.ProfileName, item.DeathCount, item.TotalTimeSpan.ToString(@"hh\ \h\ mm\ \m\ ss\ \s"), item.TotalGold, item.TotalXP, item.LootTracker.ToString()));
								}
                            }


							TotalStats all = Bot.Game.TrackingStats;
							lbMiscContent.Items.Add("\r\n== CURRENT GAME SUMMARY ==");
							lbMiscContent.Items.Add(String.Format("Total Games:{0} -- Total Unique Profiles:{1}\r\nDeaths:{2} TotalTime:{3} TotalGold:{4} TotalXP:{5}\r\n{6}",
																	all.GameCount, all.Profiles.Count, all.TotalDeaths, all.TotalTimeRunning.ToString(@"hh\ \h\ mm\ \m\ ss\ \s"), all.TotalGold, all.TotalXP, all.TotalLootTracker.ToString()));

							 
						}
						catch
						{
							 lbMiscContent.Items.Add("Exception Handled");
						}

						MiscTabItem.Content=lbMiscContent;



						TabItem DebuggingTabItem=new TabItem
						{
							 Header="Debug",
							 VerticalContentAlignment=VerticalAlignment.Stretch,
							 VerticalAlignment=VerticalAlignment.Stretch,
						};
						tabControl1.Items.Add(DebuggingTabItem);
						DockPanel DockPanel_Debug=new DockPanel
						{
							 LastChildFill=true,
							 FlowDirection=FlowDirection.LeftToRight,

						};


						Button btnObjects_Debug=new Button
						{
							 Content="Object Cache",
							 FontSize=10,
							 Width=80,
							 Height=25,
							 Name="Objects",
						};
						btnObjects_Debug.Click+=DebugButtonClicked;
						Button btnObstacles_Debug=new Button
						{
							 Content="Obstacle Cache",
							 Width=80,
							 FontSize=10,
							 Height=25,
							 Name="Obstacles",
						};
						btnObstacles_Debug.Click+=DebugButtonClicked;
						Button btnSNO_Debug=new Button
						{
							 Content="SNO Cache",
							 FontSize=10,
							 Width=80,
							 Height=25,
							 Name="SNO",
						};
						btnSNO_Debug.Click+=DebugButtonClicked;
						Button btnAbility_Debug=new Button
						{
							 Content="Ability Cache",
							 FontSize=10,
							 Width=80,
							 Height=25,
							 Name="Ability",
						};
						btnAbility_Debug.Click+=DebugButtonClicked;
						Button btnMGP_Debug=new Button
						{
							 Content="MGP Details",
							 Width=150,
							 Height=25,
							 Name="MGP",
						};
						btnMGP_Debug.Click+=DebugButtonClicked;
						Button btnCharacterCache_Debug=new Button
						{
							 Content="Character",
							 FontSize=10,
							 Width=80,
							 Height=25,
							 Name="CHARACTER",
						};
						btnCharacterCache_Debug.Click+=DebugButtonClicked;
						Button btnTargetMovement_Debug=new Button
						{
							 Content="TargetMove",
							 FontSize=10,
							 Width=80,
							 Height=25,
							 Name="TargetMove",
						};
						btnTargetMovement_Debug.Click+=DebugButtonClicked;
						Button btnCombatCache_Debug=new Button
						{
							 Content="CombatCache",
							 FontSize=10,
							 Width=80,
							 Height=25,
							 Name="CombatCache",
						};
						btnCombatCache_Debug.Click+=DebugButtonClicked;
						Button btnTEST_Debug=new Button
						{
							 Content="Test",
							 FontSize=10,
							 Width=80,
							 Height=25,
							 Name="TEST",
						};
						btnTEST_Debug.Click+=DebugButtonClicked;
						StackPanel StackPanel_DebugButtons=new StackPanel
						{
							 Height=40,
							 HorizontalAlignment=HorizontalAlignment.Stretch,
							 FlowDirection=FlowDirection.LeftToRight,
							 Orientation=Orientation.Horizontal,
							 VerticalAlignment=VerticalAlignment.Top,

						};
						StackPanel_DebugButtons.Children.Add(btnObjects_Debug);
						StackPanel_DebugButtons.Children.Add(btnObstacles_Debug);
						StackPanel_DebugButtons.Children.Add(btnSNO_Debug);
						StackPanel_DebugButtons.Children.Add(btnAbility_Debug);
						StackPanel_DebugButtons.Children.Add(btnCharacterCache_Debug);
						StackPanel_DebugButtons.Children.Add(btnCombatCache_Debug);
						StackPanel_DebugButtons.Children.Add(btnTEST_Debug);

						//StackPanel_DebugButtons.Children.Add(btnGPC_Debug);

						DockPanel.SetDock(StackPanel_DebugButtons, Dock.Top);
						DockPanel_Debug.Children.Add(StackPanel_DebugButtons);

						LBDebug=new ListBox
						{
							 SelectionMode=SelectionMode.Extended,
							 HorizontalAlignment=HorizontalAlignment.Stretch,
							 VerticalAlignment=VerticalAlignment.Stretch,
							 VerticalContentAlignment=VerticalAlignment.Stretch,
							 HorizontalContentAlignment=HorizontalAlignment.Stretch,
							 FontSize=10,
							 FontStretch=FontStretches.SemiCondensed,
							 Background=Brushes.Black,
							 Foreground=Brushes.GhostWhite,
						};

						DockPanel_Debug.Children.Add(LBDebug);

						DebuggingTabItem.Content=DockPanel_Debug;


						AddChild(LBWindowContent);
				 }

			}


	 
}