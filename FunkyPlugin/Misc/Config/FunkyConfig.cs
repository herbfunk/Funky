using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Markup;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using Zeta;
using Zeta.Common;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  private static Zeta.Internals.Actors.ActorClass ActorClass;
		  private static string CurrentAccountName;
		  private static string CurrentHeroName;

		  private static void UpdateCurrentAccountDetails()
		  {
				ActorClass=Zeta.ZetaDia.Service.CurrentHero.Class;
				CurrentAccountName=Zeta.ZetaDia.Service.CurrentHero.BattleTagName;
				CurrentHeroName=Zeta.ZetaDia.Service.CurrentHero.Name;
		  }

		  private static void buttonFunkySettingDB_Click(object sender, RoutedEventArgs e)
		  {
				ActorClass=Zeta.ZetaDia.Service.CurrentHero.Class;
				CurrentAccountName=Zeta.ZetaDia.Service.CurrentHero.BattleTagName;
				CurrentHeroName=Zeta.ZetaDia.Service.CurrentHero.Name;

				string settingsFolder=FolderPaths.sDemonBuddyPath+@"\Settings\FunkyTrinity\"+CurrentAccountName;
				if (!Directory.Exists(settingsFolder))
					 Directory.CreateDirectory(settingsFolder);
				try
				{
					 funkyConfigWindow=new LootWindow();
					 funkyConfigWindow.Show();
				} catch (Exception ex)
				{
					 Logging.WriteVerbose("Failure to initilize Funky Setting Window! \r\n {0} \r\n {1} \r\n {2}", ex.Message, ex.Source, ex.StackTrace);
				}
		  }

		  internal static LootWindow funkyConfigWindow;

		  [System.Runtime.InteropServices.ComVisible(false)]
		  public partial class LootWindow : Window
		  {
				private ListBox lbGeneralContent;

				private CheckBox OOCIdentifyItems;
				private TextBox OOCIdentfyItemsMinCount;

				private CheckBox BuyPotionsDuringTownRunCB;
				private CheckBox EnableWaitAfterContainersCB;
				private CheckBox UseExtendedRangeRepChestCB;

				private CheckBox ItemRules;
				private CheckBox ItemRulesPickup;
				private Button ItemRulesReload;

				private CheckBox ItemRuleUseItemIDs;
				private CheckBox ItemRuleDebug;
				private ComboBox ItemRuleLogKeep;
				private ComboBox ItemRuleLogPickup;
				private ComboBox ItemRuleType;

				private RadioButton ItemRuleGilesScoring, ItemRuleDBScoring;

				private CheckBox CoffeeBreaks;
				private TextBox tbMinBreakTime, tbMaxBreakTime;

				private Button OpenPluginFolder;

				private TextBox[] TBavoidanceRadius;
				private TextBox[] TBavoidanceHealth;

				private TextBox[] TBMinimumWeaponLevel;
				private TextBox[] TBMinimumArmorLevel;
				private TextBox[] TBMinimumJeweleryLevel;
				private CheckBox[] CBGems;
				private ComboBox CBGemQualityLevel;

				private TextBox TBBreakTimeHour, TBKiteDistance, TBGlobeHealth, TBPotionHealth, TBContainerRange, TBNonEliteRange, TBDestructibleRange, TBAfterCombatDelay, TBiDHVaultMovementDelay, TBShrineRange, TBEliteRange, TBExtendedCombatRange, TBGoldRange, TBMinLegendaryLevel, TBMaxHealthPots, TBMinGoldPile, TBMiscItemLevel, TBGilesWeaponScore, TBGilesArmorScore, TBGilesJeweleryScore, TBClusterDistance, TBClusterMinUnitCount, TBItemRange, TBGoblinRange, TBGoblinMinRange, TBClusterLowHPValue;
				private TextBox[] TBKiteTimeLimits;
				private TextBox[] TBAvoidanceTimeLimits;

				private ListBox LBDebug;


				public LootWindow()
				{
					 LoadFunkyConfiguration();

					 this.Owner=Demonbuddy.App.Current.MainWindow;
					 this.Title="Funky Window";
					 this.SizeToContent=System.Windows.SizeToContent.WidthAndHeight;
					 this.ResizeMode=System.Windows.ResizeMode.CanMinimize;
					 this.Background=System.Windows.Media.Brushes.Black;
					 this.Foreground=System.Windows.Media.Brushes.PaleGoldenrod;
					 //this.Width=600;
					 //this.Height=600;

					 ListBox LBWindowContent=new ListBox
					 {
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
						  VerticalAlignment=System.Windows.VerticalAlignment.Stretch,
					 };

					 StackPanel StackPanelTopWindow=new StackPanel
					 {
						  Height=40,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
						  FlowDirection=System.Windows.FlowDirection.LeftToRight,
						  Orientation=Orientation.Horizontal,
					 };
					 Button OpenPluginSettings=new Button
					 {
						  Content="Open Plugin Settings",
						  Width=150,
						  Height=25,
					 };
					 OpenPluginSettings.Click+=OpenPluginSettings_Click;
					 StackPanelTopWindow.Children.Add(OpenPluginSettings);


					 OpenPluginFolder=new Button
					 {
						  Content="Open Trinity Folder",
						  Width=150,
						  Height=25,
					 };
					 OpenPluginFolder.Click+=OpenPluginFolder_Click;
					 StackPanelTopWindow.Children.Add(OpenPluginFolder);

					 LBWindowContent.Items.Add(StackPanelTopWindow);

					 TabControl tabControl1=new TabControl
					 {
						  Width=600,
						  Height=600,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
						  VerticalAlignment=System.Windows.VerticalAlignment.Stretch,
						  VerticalContentAlignment=System.Windows.VerticalAlignment.Stretch,
						  HorizontalContentAlignment=System.Windows.HorizontalAlignment.Stretch,
						  FontSize=12,
					 };
					 LBWindowContent.Items.Add(tabControl1);

					 #region Character
					 //Character
					 TabItem CharacterSettingsTabItem=new TabItem();
					 CharacterSettingsTabItem.Header="Character";

					 tabControl1.Items.Add(CharacterSettingsTabItem);
					 TabControl tcCharacter=new TabControl
					 {
						  Width=600,
						  Height=600,

					 };

					 #region Avoidances
					 TabItem AvoidanceTabItem=new TabItem();
					 AvoidanceTabItem.Header="Avoidances";
					 tcCharacter.Items.Add(AvoidanceTabItem);
					 ListBox LBcharacterAvoidance=new ListBox();




					 Grid AvoidanceLayoutGrid=new Grid
					 {
						  UseLayoutRounding=true,
						  ShowGridLines=false,
						  VerticalAlignment=System.Windows.VerticalAlignment.Stretch,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
						  FlowDirection=System.Windows.FlowDirection.LeftToRight,
						  Focusable=false,
					 };

					 ColumnDefinition colDef1=new ColumnDefinition();
					 ColumnDefinition colDef2=new ColumnDefinition();
					 ColumnDefinition colDef3=new ColumnDefinition();
					 AvoidanceLayoutGrid.ColumnDefinitions.Add(colDef1);
					 AvoidanceLayoutGrid.ColumnDefinitions.Add(colDef2);
					 AvoidanceLayoutGrid.ColumnDefinitions.Add(colDef3);
					 RowDefinition rowDef1=new RowDefinition();
					 AvoidanceLayoutGrid.RowDefinitions.Add(rowDef1);

					 TextBlock ColumnHeader1=new TextBlock
					 {
						  Text="Type",
						  FontSize=12,
						  TextAlignment=System.Windows.TextAlignment.Center,
						  Background=System.Windows.Media.Brushes.WhiteSmoke,
					 };
					 TextBlock ColumnHeader2=new TextBlock
					 {
						  Text="Radius",
						  FontSize=12,
						  TextAlignment=System.Windows.TextAlignment.Center,
						  Background=System.Windows.Media.Brushes.Goldenrod,
					 };
					 TextBlock ColumnHeader3=new TextBlock
					 {
						  Text="Health",
						  FontSize=12,
						  TextAlignment=System.Windows.TextAlignment.Center,
						  Background=System.Windows.Media.Brushes.DarkRed,
					 };
					 Grid.SetColumn(ColumnHeader1, 0);
					 Grid.SetColumn(ColumnHeader2, 1);
					 Grid.SetColumn(ColumnHeader3, 2);
					 Grid.SetRow(ColumnHeader1, 0);
					 Grid.SetRow(ColumnHeader2, 0);
					 Grid.SetRow(ColumnHeader3, 0);
					 AvoidanceLayoutGrid.Children.Add(ColumnHeader1);
					 AvoidanceLayoutGrid.Children.Add(ColumnHeader2);
					 AvoidanceLayoutGrid.Children.Add(ColumnHeader3);

					 Dictionary<AvoidanceType, double> currentDictionaryAvoidance=ReturnDictionaryUsingActorClass(ActorClass);
					 AvoidanceType[] avoidanceTypes=currentDictionaryAvoidance.Keys.ToArray();
					 TBavoidanceHealth=new TextBox[avoidanceTypes.Length-1];
					 TBavoidanceRadius=new TextBox[avoidanceTypes.Length-1];
					 for (int i=0; i<avoidanceTypes.Length-1; i++)
					 {
						  string avoidanceString=avoidanceTypes[i].ToString();

						  float defaultRadius=0f;
						  dictAvoidanceRadius.TryGetValue(avoidanceTypes[i], out defaultRadius);
						  Slider avoidanceRadius=new Slider
						  {
								Width=125,
								Name=avoidanceString+"_radius_"+i.ToString(),
								Maximum=25,
								Minimum=0,
								TickFrequency=5,
								LargeChange=5,
								SmallChange=1,
								Value=defaultRadius,
								HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
								VerticalAlignment=System.Windows.VerticalAlignment.Center,
								//Padding=new Thickness(2),
								Margin=new Thickness(5),
						  };
						  avoidanceRadius.ValueChanged+=AvoidanceRadiusSliderValueChanged;
						  TBavoidanceRadius[i]=new TextBox
						  {
								Text=defaultRadius.ToString(),
								IsReadOnly=true,
								VerticalAlignment=System.Windows.VerticalAlignment.Top,
								HorizontalAlignment=System.Windows.HorizontalAlignment.Right,
						  };

						  double defaultHealth=0d;
						  ReturnDictionaryUsingActorClass(ActorClass).TryGetValue(avoidanceTypes[i], out defaultHealth);
						  Slider avoidanceHealth=new Slider
						  {
								Name=avoidanceString+"_health_"+i.ToString(),
								Width=125,
								Maximum=1,
								Minimum=0,
								TickFrequency=0.10,
								LargeChange=0.10,
								SmallChange=0.05,
								Value=defaultHealth,
								HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
								VerticalAlignment=System.Windows.VerticalAlignment.Center,
								Margin=new Thickness(5),
						  };
						  avoidanceHealth.ValueChanged+=AvoidanceHealthSliderValueChanged;
						  TBavoidanceHealth[i]=new TextBox
						  {
								Text=defaultHealth.ToString("F2", CultureInfo.InvariantCulture),
								IsReadOnly=true,
								VerticalAlignment=System.Windows.VerticalAlignment.Top,
								HorizontalAlignment=System.Windows.HorizontalAlignment.Right,
						  };

						  RowDefinition newRow=new RowDefinition();
						  AvoidanceLayoutGrid.RowDefinitions.Add(newRow);


						  TextBlock txt1=new TextBlock();
						  txt1.Text=avoidanceString;
						  txt1.FontSize=14;
						  txt1.Background=System.Windows.Media.Brushes.Moccasin;

						  StackPanel avoidRadiusStackPanel=new StackPanel
						  {
								Width=175,
								Height=30,
								Orientation=Orientation.Horizontal,
						  };
						  avoidRadiusStackPanel.Children.Add(avoidanceRadius);
						  avoidRadiusStackPanel.Children.Add(TBavoidanceRadius[i]);

						  StackPanel avoidHealthStackPanel=new StackPanel
						  {
								Width=175,
								Height=30,
								Orientation=Orientation.Horizontal,
						  };
						  avoidHealthStackPanel.Children.Add(avoidanceHealth);
						  avoidHealthStackPanel.Children.Add(TBavoidanceHealth[i]);

						  Grid.SetColumn(txt1, 0);
						  Grid.SetColumn(avoidRadiusStackPanel, 1);
						  Grid.SetColumn(avoidHealthStackPanel, 2);

						  int currentIndex=AvoidanceLayoutGrid.RowDefinitions.Count-1;
						  Grid.SetRow(avoidRadiusStackPanel, currentIndex);
						  Grid.SetRow(avoidHealthStackPanel, currentIndex);
						  Grid.SetRow(txt1, currentIndex);

						  AvoidanceLayoutGrid.Children.Add(txt1);
						  AvoidanceLayoutGrid.Children.Add(avoidRadiusStackPanel);
						  AvoidanceLayoutGrid.Children.Add(avoidHealthStackPanel);
					 }

					 LBcharacterAvoidance.Items.Add(AvoidanceLayoutGrid);


					 AvoidanceTabItem.Content=LBcharacterAvoidance;
					 #endregion

					 #region CharacterCombat
					 //Combat
					 TabItem CombatMiscTabItem=new TabItem();
					 CombatMiscTabItem.Header="Combat";
					 tcCharacter.Items.Add(CombatMiscTabItem);
					 ListBox LBcharacterCombat=new ListBox();

					 CheckBox CBAttemptAvoidanceMovements=new CheckBox
					 {
						  Content="Enable Avoidance",
						  Width=300,
						  Height=30,
						  IsChecked=SettingsFunky.AttemptAvoidanceMovements,
					 };
					 CBAttemptAvoidanceMovements.Checked+=AvoidanceAttemptMovementChecked;
					 CBAttemptAvoidanceMovements.Unchecked+=AvoidanceAttemptMovementChecked;

					 CheckBox CBAdvancedProjectileTesting=new CheckBox
					 {
						  Content="Use Advanced Avoidance Projectile Test",
						  Width=300,
						  Height=30,
						  IsChecked=SettingsFunky.UseAdvancedProjectileTesting,
					 };
					 CBAdvancedProjectileTesting.Checked+=UseAdvancedProjectileTestingChecked;
					 CBAdvancedProjectileTesting.Unchecked+=UseAdvancedProjectileTestingChecked;

					 LBcharacterCombat.Items.Add(CBAttemptAvoidanceMovements);
					 LBcharacterCombat.Items.Add(CBAdvancedProjectileTesting);

					 #region AvoidanceRetry
					 TextBlock Avoid_Retry_Text=new TextBlock
					 {
						  Text="Avoidance Search Delay",
						  FontSize=15,
						  Background=System.Windows.Media.Brushes.Orchid,
						  TextAlignment=TextAlignment.Center,
					 };
					 LBcharacterCombat.Items.Add(Avoid_Retry_Text);

					 TextBlock Avoid_Retry_Min_Text=new TextBlock
					 {
						  Text="Minimum",
						  FontSize=13,
						  Background=System.Windows.Media.Brushes.DarkRed,
						  TextAlignment=TextAlignment.Center,
					 };

					 Slider sliderAvoidMinimumRetry=new Slider
					 {
						  Width=120,
						  Maximum=10000,
						  Minimum=0,
						  TickFrequency=500,
						  LargeChange=1000,
						  SmallChange=50,
						  Value=SettingsFunky.AvoidanceRecheckMinimumRate,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						  Margin=new Thickness(6),
					 };
					 sliderAvoidMinimumRetry.ValueChanged+=AvoidanceMinimumRetrySliderChanged;
					 TBAvoidanceTimeLimits=new TextBox[2];
					 TBAvoidanceTimeLimits[0]=new TextBox
					 {
						  Text=SettingsFunky.AvoidanceRecheckMinimumRate.ToString(),
						  IsReadOnly=true,
					 };

					 TextBlock Avoid_Retry_Max_Text=new TextBlock
					 {
						  Text="Maximum",
						  FontSize=13,
						  Background=System.Windows.Media.Brushes.DarkGreen,
						  TextAlignment=TextAlignment.Center,
					 };

					 Slider sliderAvoidMaximumRetry=new Slider
					 {
						  Width=120,
						  Maximum=10000,
						  Minimum=0,
						  TickFrequency=500,
						  LargeChange=1000,
						  SmallChange=50,
						  Value=SettingsFunky.AvoidanceRecheckMaximumRate,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						  Margin=new Thickness(6),
					 };
					 sliderAvoidMaximumRetry.ValueChanged+=AvoidanceMaximumRetrySliderChanged;
					 TBAvoidanceTimeLimits[1]=new TextBox
					 {
						  Text=SettingsFunky.AvoidanceRecheckMaximumRate.ToString(),
						  IsReadOnly=true,
					 };

					 StackPanel AvoidRetryTimeStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=30,
						  Orientation=Orientation.Horizontal,
					 };
					 AvoidRetryTimeStackPanel.Children.Add(Avoid_Retry_Min_Text);
					 AvoidRetryTimeStackPanel.Children.Add(sliderAvoidMinimumRetry);
					 AvoidRetryTimeStackPanel.Children.Add(TBAvoidanceTimeLimits[0]);
					 AvoidRetryTimeStackPanel.Children.Add(Avoid_Retry_Max_Text);
					 AvoidRetryTimeStackPanel.Children.Add(sliderAvoidMaximumRetry);
					 AvoidRetryTimeStackPanel.Children.Add(TBAvoidanceTimeLimits[1]);
					 LBcharacterCombat.Items.Add(AvoidRetryTimeStackPanel);
					 #endregion


					 #region KitingRetry
					 TextBlock Kite_retry_Text=new TextBlock
					 {
						  Text="Kiting Search Delay",
						  FontSize=15,
						  Background=System.Windows.Media.Brushes.NavajoWhite,
						  TextAlignment=TextAlignment.Center,
					 };
					 LBcharacterCombat.Items.Add(Kite_retry_Text);
					 Slider sliderKiteMinimumRetry=new Slider
					 {
						  Width=120,
						  Maximum=10000,
						  Minimum=0,
						  TickFrequency=500,
						  LargeChange=1000,
						  SmallChange=50,
						  Value=SettingsFunky.KitingRecheckMinimumRate,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						  Margin=new Thickness(6),

					 };
					 sliderKiteMinimumRetry.ValueChanged+=KitingMinimumRetrySliderChanged;
					 TBKiteTimeLimits=new TextBox[2];
					 TBKiteTimeLimits[0]=new TextBox
					 {
						  Text=SettingsFunky.KitingRecheckMinimumRate.ToString(),
						  IsReadOnly=true,
					 };

					 TextBlock Kite_Retry_Min_Text=new TextBlock
					 {
						  Text="Minimum",
						  FontSize=13,
						  Background=System.Windows.Media.Brushes.DarkRed,
						  TextAlignment=TextAlignment.Center,
					 };


					 Slider sliderKiteMaximumRetry=new Slider
					 {
						  Width=120,
						  Maximum=10000,
						  Minimum=0,
						  TickFrequency=500,
						  LargeChange=1000,
						  SmallChange=50,
						  Value=SettingsFunky.KitingRecheckMaximumRate,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						  Margin=new Thickness(6),
					 };
					 sliderKiteMaximumRetry.ValueChanged+=KitingMaximumRetrySliderChanged;
					 TBKiteTimeLimits[1]=new TextBox
					 {
						  Text=SettingsFunky.KitingRecheckMaximumRate.ToString(),
						  IsReadOnly=true,
					 };
					 TextBlock Kite_Retry_Max_Text=new TextBlock
					 {
						  Text="Maximum",
						  FontSize=13,
						  Background=System.Windows.Media.Brushes.DarkGreen,
						  TextAlignment=TextAlignment.Center,
					 };
					 StackPanel KiteRetryTimeStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=30,
						  Orientation=Orientation.Horizontal,
					 };
					 //
					 KiteRetryTimeStackPanel.Children.Add(Kite_Retry_Min_Text);
					 KiteRetryTimeStackPanel.Children.Add(sliderKiteMinimumRetry);
					 KiteRetryTimeStackPanel.Children.Add(TBKiteTimeLimits[0]);
					 KiteRetryTimeStackPanel.Children.Add(Kite_Retry_Max_Text);
					 KiteRetryTimeStackPanel.Children.Add(sliderKiteMaximumRetry);
					 KiteRetryTimeStackPanel.Children.Add(TBKiteTimeLimits[1]);
					 LBcharacterCombat.Items.Add(KiteRetryTimeStackPanel);
					 #endregion


					 #region Kiting
					 TextBlock Kite_Text=new TextBlock
					 {
						  Text="Minimum Kiting Distance",
						  FontSize=15,
						  Background=System.Windows.Media.Brushes.NavajoWhite,
						  TextAlignment=TextAlignment.Center,
					 };
					 LBcharacterCombat.Items.Add(Kite_Text);

					 Slider sliderKiteDistance=new Slider
					 {
						  Width=100,
						  Maximum=20,
						  Minimum=0,
						  TickFrequency=5,
						  LargeChange=5,
						  SmallChange=1,
						  Value=SettingsFunky.KiteDistance,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 sliderKiteDistance.ValueChanged+=KiteSliderChanged;
					 TBKiteDistance=new TextBox
					 {
						  Text=SettingsFunky.KiteDistance.ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel KiteStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 KiteStackPanel.Children.Add(sliderKiteDistance);
					 KiteStackPanel.Children.Add(TBKiteDistance);
					 LBcharacterCombat.Items.Add(KiteStackPanel);
					 #endregion

					 #region GlobeHealthPercent
					 LBcharacterCombat.Items.Add("Globe Health Percent");
					 Slider sliderGlobeHealth=new Slider
					 {
						  Width=100,
						  Maximum=1,
						  Minimum=0,
						  TickFrequency=0.25,
						  LargeChange=0.20,
						  SmallChange=0.10,
						  Value=SettingsFunky.GlobeHealthPercent,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 sliderGlobeHealth.ValueChanged+=GlobeHealthSliderChanged;
					 TBGlobeHealth=new TextBox
					 {
						  Text=SettingsFunky.GlobeHealthPercent.ToString("F2", CultureInfo.InvariantCulture),
						  IsReadOnly=true,
					 };
					 StackPanel GlobeHealthStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 GlobeHealthStackPanel.Children.Add(sliderGlobeHealth);
					 GlobeHealthStackPanel.Children.Add(TBGlobeHealth);
					 LBcharacterCombat.Items.Add(GlobeHealthStackPanel);
					 #endregion

					 #region PotionHealthPercent
					 LBcharacterCombat.Items.Add("Potion Health Percent");
					 Slider sliderPotionHealth=new Slider
					 {
						  Width=100,
						  Maximum=1,
						  Minimum=0,
						  TickFrequency=0.25,
						  LargeChange=0.20,
						  SmallChange=0.10,
						  Value=SettingsFunky.PotionHealthPercent,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 sliderPotionHealth.ValueChanged+=PotionHealthSliderChanged;
					 TBPotionHealth=new TextBox
					 {
						  Text=SettingsFunky.PotionHealthPercent.ToString("F2", CultureInfo.InvariantCulture),
						  IsReadOnly=true,
					 };
					 StackPanel PotionHealthStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 PotionHealthStackPanel.Children.Add(sliderPotionHealth);
					 PotionHealthStackPanel.Children.Add(TBPotionHealth);
					 LBcharacterCombat.Items.Add(PotionHealthStackPanel);
					 #endregion

					 CombatMiscTabItem.Content=LBcharacterCombat;
					 #endregion

					 #region ClassSettings
					 //Class Specific
					 TabItem ClassTabItem=new TabItem();
					 ClassTabItem.Header="Class";
					 tcCharacter.Items.Add(ClassTabItem);
					 ListBox LBClass=new ListBox();

					 switch (ActorClass)
					 {
						  case Zeta.Internals.Actors.ActorClass.Barbarian:
								CheckBox cbbSelectiveWhirlwind=new CheckBox
								{
									 Content="Selective Whirlwind Targeting",
									 Width=300,
									 Height=30,
									 IsChecked=(SettingsFunky.Class.bSelectiveWhirlwind)
								};
								cbbSelectiveWhirlwind.Checked+=bSelectiveWhirlwindChecked;
								cbbSelectiveWhirlwind.Unchecked+=bSelectiveWhirlwindChecked;
								LBClass.Items.Add(cbbSelectiveWhirlwind);

								CheckBox cbbWaitForWrath=new CheckBox
								{
									 Content="Wait for Wrath",
									 Width=300,
									 Height=30,
									 IsChecked=(SettingsFunky.Class.bWaitForWrath)
								};
								cbbWaitForWrath.Checked+=bWaitForWrathChecked;
								cbbWaitForWrath.Unchecked+=bWaitForWrathChecked;
								LBClass.Items.Add(cbbWaitForWrath);

								CheckBox cbbGoblinWrath=new CheckBox
								{
									 Content="Use Wrath on Goblins",
									 Width=300,
									 Height=30,
									 IsChecked=(SettingsFunky.Class.bGoblinWrath)
								};
								cbbGoblinWrath.Checked+=bGoblinWrathChecked;
								cbbGoblinWrath.Unchecked+=bGoblinWrathChecked;
								LBClass.Items.Add(cbbGoblinWrath);

								CheckBox cbbFuryDumpWrath=new CheckBox
								{
									 Content="Fury Dump during Wrath",
									 Width=300,
									 Height=30,
									 IsChecked=(SettingsFunky.Class.bFuryDumpWrath)
								};
								cbbFuryDumpWrath.Checked+=bFuryDumpWrathChecked;
								cbbFuryDumpWrath.Unchecked+=bFuryDumpWrathChecked;
								LBClass.Items.Add(cbbFuryDumpWrath);

								CheckBox cbbFuryDumpAlways=new CheckBox
								{
									 Content="Fury Dump Always",
									 Width=300,
									 Height=30,
									 IsChecked=(SettingsFunky.Class.bFuryDumpAlways)
								};
								cbbFuryDumpAlways.Checked+=bFuryDumpAlwaysChecked;
								cbbFuryDumpAlways.Unchecked+=bFuryDumpAlwaysChecked;
								LBClass.Items.Add(cbbFuryDumpAlways);

								break;
						  case Zeta.Internals.Actors.ActorClass.DemonHunter:
								LBClass.Items.Add("Reuse Vault Delay");
								Slider iDHVaultMovementDelayslider=new Slider
								{
									 Width=200,
									 Maximum=4000,
									 Minimum=400,
									 TickFrequency=5,
									 LargeChange=5,
									 SmallChange=1,
									 Value=SettingsFunky.Class.iDHVaultMovementDelay,
									 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
								};
								iDHVaultMovementDelayslider.ValueChanged+=iDHVaultMovementDelaySliderChanged;
								TBiDHVaultMovementDelay=new TextBox
								{
									 Text=SettingsFunky.Class.iDHVaultMovementDelay.ToString(),
									 IsReadOnly=true,
								};
								StackPanel DhVaultPanel=new StackPanel
								{
									 Width=600,
									 Height=30,
									 Orientation=Orientation.Horizontal,
								};
								DhVaultPanel.Children.Add(iDHVaultMovementDelayslider);
								DhVaultPanel.Children.Add(TBiDHVaultMovementDelay);
								LBClass.Items.Add(DhVaultPanel);

								break;
						  case Zeta.Internals.Actors.ActorClass.Monk:
								CheckBox cbbMonkInnaSet=new CheckBox
								{
									 Content="Full Inna Set Bonus",
									 Width=300,
									 Height=30,
									 IsChecked=(SettingsFunky.Class.bMonkInnaSet)
								};
								cbbMonkInnaSet.Checked+=bMonkInnaSetChecked;
								cbbMonkInnaSet.Unchecked+=bMonkInnaSetChecked;
								LBClass.Items.Add(cbbMonkInnaSet);

								break;
						  case Zeta.Internals.Actors.ActorClass.WitchDoctor:
						  case Zeta.Internals.Actors.ActorClass.Wizard:
								CheckBox cbbEnableCriticalMass=new CheckBox
								{
									 Content="Critical Mass",
									 Width=300,
									 Height=30,
									 IsChecked=(SettingsFunky.Class.bEnableCriticalMass)
								};
								cbbEnableCriticalMass.Checked+=bEnableCriticalMassChecked;
								cbbEnableCriticalMass.Unchecked+=bEnableCriticalMassChecked;
								LBClass.Items.Add(cbbEnableCriticalMass);

								if (ActorClass==Zeta.Internals.Actors.ActorClass.Wizard)
								{
									 CheckBox cbbWaitForArchon=new CheckBox
									 {
										  Content="Wait for Archon",
										  Width=300,
										  Height=30,
										  IsChecked=(SettingsFunky.Class.bWaitForArchon)
									 };
									 cbbWaitForArchon.Checked+=bWaitForArchonChecked;
									 cbbWaitForArchon.Unchecked+=bWaitForArchonChecked;
									 LBClass.Items.Add(cbbWaitForArchon);

									 CheckBox cbbKiteOnlyArchon=new CheckBox
									 {
										  Content="Kite Only During Archon",
										  Width=300,
										  Height=30,
										  IsChecked=(SettingsFunky.Class.bKiteOnlyArchon)
									 };
									 cbbKiteOnlyArchon.Checked+=bKiteOnlyArchonChecked;
									 cbbKiteOnlyArchon.Unchecked+=bKiteOnlyArchonChecked;
									 LBClass.Items.Add(cbbKiteOnlyArchon);

								}

								break;
					 }
					 if (ActorClass==Zeta.Internals.Actors.ActorClass.DemonHunter||ActorClass==Zeta.Internals.Actors.ActorClass.WitchDoctor||ActorClass==Zeta.Internals.Actors.ActorClass.Wizard)
					 {

						  #region GoblinMinimumRange
						  LBClass.Items.Add("Treasure Goblin Minimum Range");
						  Slider sliderGoblinMinRange=new Slider
						  {
								Width=200,
								Maximum=75,
								Minimum=0,
								TickFrequency=5,
								LargeChange=5,
								SmallChange=1,
								Value=SettingsFunky.Class.GoblinMinimumRange,
								HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						  };
						  sliderGoblinMinRange.ValueChanged+=TreasureGoblinMinimumRangeSliderChanged;
						  TBGoblinMinRange=new TextBox
						  {
								Text=SettingsFunky.Class.GoblinMinimumRange.ToString(),
								IsReadOnly=true,
						  };
						  StackPanel GoblinMinRangeStackPanel=new StackPanel
						  {
								Width=600,
								Height=30,
								Orientation=Orientation.Horizontal,
						  };
						  GoblinMinRangeStackPanel.Children.Add(sliderGoblinMinRange);
						  GoblinMinRangeStackPanel.Children.Add(TBGoblinMinRange);
						  LBClass.Items.Add(GoblinMinRangeStackPanel);
						  #endregion
					 }
					 ClassTabItem.Content=LBClass;
					 #endregion


					 CharacterSettingsTabItem.Content=tcCharacter;
					 #endregion

					 #region General
					 TabControl tcGeneral=new TabControl
					 {
						  Width=600,
						  Height=600,
					 };

					 TabItem GeneralSettingsTabItem=new TabItem();
					 GeneralSettingsTabItem.Header="General";
					 tabControl1.Items.Add(GeneralSettingsTabItem);

					 TabItem GeneralTab=new TabItem();
					 GeneralTab.Header="General";
					 tcGeneral.Items.Add(GeneralTab);
					 lbGeneralContent=new ListBox();


					 #region OOC_ID_Items
					 OOCIdentifyItems=new CheckBox
					 {
						  Content="Enable Out Of Combat Idenification Behavior",
						  Width=300,
						  Height=30,
						  IsChecked=(SettingsFunky.OOCIdentifyItems)

					 };
					 OOCIdentifyItems.Checked+=OOCIDChecked;
					 OOCIdentifyItems.Unchecked+=OOCIDChecked;
					 lbGeneralContent.Items.Add(OOCIdentifyItems);
					 #endregion

					 #region OOC_Min_Item_Count
					 lbGeneralContent.Items.Add("OOC Minimum Unid Items before Behavior Starts.");
					 OOCIdentfyItemsMinCount=new TextBox
					 {
						  Text=SettingsFunky.OOCIdentifyItemsMinimumRequired.ToString(),
						  Width=100,
						  Height=25
					 };
					 OOCIdentfyItemsMinCount.KeyUp+=OOCMinimumItems_KeyUp;
					 OOCIdentfyItemsMinCount.TextChanged+=OOCIdentifyItemsMinValueChanged;
					 lbGeneralContent.Items.Add(OOCIdentfyItemsMinCount);
					 #endregion

					 #region PotionsDuringTownRun
					 BuyPotionsDuringTownRunCB=new CheckBox
					 {
						  Content="Buy Potions During Town Run (Uses Maximum Potion Count Setting)",
						  Width=500,
						  Height=30,
						  IsChecked=(SettingsFunky.BuyPotionsDuringTownRun)
					 };
					 BuyPotionsDuringTownRunCB.Checked+=BuyPotionsDuringTownRunChecked;
					 BuyPotionsDuringTownRunCB.Unchecked+=BuyPotionsDuringTownRunChecked;
					 lbGeneralContent.Items.Add(BuyPotionsDuringTownRunCB);
					 #endregion

					 #region OutOfCombatMovement
					 CheckBox cbOutOfCombatMovement=new CheckBox
					 {
						  Content="Use Out Of Combat Ability Movements",
						  Width=300,
						  Height=30,
						  IsChecked=(SettingsFunky.OutOfCombatMovement)
					 };
					 cbOutOfCombatMovement.Checked+=OutOfCombatMovementChecked;
					 cbOutOfCombatMovement.Unchecked+=OutOfCombatMovementChecked;
					 lbGeneralContent.Items.Add(cbOutOfCombatMovement);
					 #endregion

					 GeneralTab.Content=lbGeneralContent;



					 #region CoffeeBreaks
					 TabItem CoffeeBreakTab=new TabItem();
					 CoffeeBreakTab.Header="Coffee Breaks";
					 tcGeneral.Items.Add(CoffeeBreakTab);
					 ListBox LBCoffeebreak=new ListBox();

					 CoffeeBreaks=new CheckBox
					 {
						  Content="Enable Coffee Breaks",
						  Width=300,
						  Height=30,
						  IsChecked=(SettingsFunky.EnableCoffeeBreaks)

					 };
					 CoffeeBreaks.Checked+=EnableCoffeeBreaksChecked;
					 CoffeeBreaks.Unchecked+=EnableCoffeeBreaksChecked;
					 LBCoffeebreak.Items.Add(CoffeeBreaks);

					 #region BreakTimeMinMinutes
					 LBCoffeebreak.Items.Add("Break Minimum Minutes");
					 Slider sliderBreakMinMinutes=new Slider
					 {
						  Width=200,
						  Maximum=20,
						  Minimum=0,
						  TickFrequency=5,
						  LargeChange=2,
						  SmallChange=1,
						  Value=SettingsFunky.MinBreakTime,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 sliderBreakMinMinutes.ValueChanged+=BreakMinMinutesSliderChange;
					 tbMinBreakTime=new TextBox
					 {
						  Text=sliderBreakMinMinutes.Value.ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel BreakTimeMinMinutestackPanel=new StackPanel
					 {
						  Width=600,
						  Height=30,
						  Orientation=Orientation.Horizontal,
					 };
					 BreakTimeMinMinutestackPanel.Children.Add(sliderBreakMinMinutes);
					 BreakTimeMinMinutestackPanel.Children.Add(tbMinBreakTime);
					 LBCoffeebreak.Items.Add(BreakTimeMinMinutestackPanel);
					 #endregion

					 #region BreakTimeMaxMinutes
					 LBCoffeebreak.Items.Add("Break Time Minutes (Added to Minimum)");
					 Slider sliderBreakMaxMinutes=new Slider
					 {
						  Width=200,
						  Maximum=20,
						  Minimum=0,
						  TickFrequency=5,
						  LargeChange=2,
						  SmallChange=1,
						  Value=SettingsFunky.MaxBreakTime,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 sliderBreakMaxMinutes.ValueChanged+=BreakMaxMinutesSliderChange;
					 tbMaxBreakTime=new TextBox
					 {
						  Text=sliderBreakMaxMinutes.Value.ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel BreakTimeMaxMinutestackPanel=new StackPanel
					 {
						  Width=600,
						  Height=30,
						  Orientation=Orientation.Horizontal,
					 };
					 BreakTimeMaxMinutestackPanel.Children.Add(sliderBreakMaxMinutes);
					 BreakTimeMaxMinutestackPanel.Children.Add(tbMaxBreakTime);
					 LBCoffeebreak.Items.Add(BreakTimeMaxMinutestackPanel);
					 #endregion

					 #region BreakTimeIntervalHour
					 LBCoffeebreak.Items.Add("Break Hour Interval (1 Equals One Hour)");
					 Slider sliderBreakTimeHour=new Slider
					 {
						  Width=200,
						  Maximum=10,
						  Minimum=0,
						  TickFrequency=1,
						  LargeChange=0.50,
						  SmallChange=0.05,
						  Value=SettingsFunky.breakTimeHour,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 sliderBreakTimeHour.ValueChanged+=BreakTimeHourSliderChanged;
					 TBBreakTimeHour=new TextBox
					 {
						  Text=sliderBreakTimeHour.Value.ToString("F2", CultureInfo.InvariantCulture),
						  IsReadOnly=true,
					 };
					 StackPanel BreakTimeHourStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=30,
						  Orientation=Orientation.Horizontal,
					 };
					 BreakTimeHourStackPanel.Children.Add(sliderBreakTimeHour);
					 BreakTimeHourStackPanel.Children.Add(TBBreakTimeHour);
					 LBCoffeebreak.Items.Add(BreakTimeHourStackPanel);
					 #endregion


					 CoffeeBreakTab.Content=LBCoffeebreak;
					 #endregion



					 GeneralSettingsTabItem.Content=tcGeneral;
					 #endregion

					 #region Items


					 TabItem CustomSettingsTabItem=new TabItem();
					 CustomSettingsTabItem.Header="Items";
					 tabControl1.Items.Add(CustomSettingsTabItem);

					 TabControl tcItems=new TabControl
					 {
						  Width=600,
						  Height=600
					 };

					 #region ItemGeneral
					 TabItem ItemGeneralTabItem=new TabItem();
					 ItemGeneralTabItem.Header="General";
					 tcItems.Items.Add(ItemGeneralTabItem);
					 ListBox lbLootContent=new ListBox();

					 lbLootContent.Items.Add("Default Scoring Option");
					 ItemRuleGilesScoring=new RadioButton
					 {
						  GroupName="Scoring",
						  Content="Giles Item Scoring",
						  Width=300,
						  Height=30,
						  IsChecked=SettingsFunky.ItemRuleGilesScoring
					 };
					 ItemRuleDBScoring=new RadioButton
					 {
						  GroupName="Scoring",
						  Content="DB Weight Scoring",
						  Width=300,
						  Height=30,
						  IsChecked=!SettingsFunky.ItemRuleGilesScoring
					 };

					 ItemRuleGilesScoring.Checked+=ItemRulesScoringChanged;
					 ItemRuleDBScoring.Checked+=ItemRulesScoringChanged;
					 lbLootContent.Items.Add(ItemRuleGilesScoring);
					 lbLootContent.Items.Add(ItemRuleDBScoring);

					 CheckBox LevelingLogic=new CheckBox
					 {
						  Content="Leveling Item Logic",
						  Width=300,
						  Height=30,
						  IsChecked=(SettingsFunky.UseLevelingLogic),
					 };
					 LevelingLogic.Checked+=ItemLevelingLogicChecked;
					 LevelingLogic.Unchecked+=ItemLevelingLogicChecked;
					 lbLootContent.Items.Add(LevelingLogic);

					 ItemGeneralTabItem.Content=lbLootContent;
					 #endregion

					 #region ItemRules
					 TabItem ItemRulesTabItem=new TabItem();
					 ItemRulesTabItem.Header="Item Rules";
					 tcItems.Items.Add(ItemRulesTabItem);
					 ListBox lbItemRulesContent=new ListBox();

					 ItemRules=new CheckBox
					 {
						  Content="Use Item Rules",
						  Width=300,
						  Height=30,
						  IsChecked=(SettingsFunky.UseItemRules)

					 };
					 ItemRules.Checked+=ItemRulesChecked;
					 ItemRules.Unchecked+=ItemRulesChecked;
					 lbItemRulesContent.Items.Add(ItemRules);

					 ItemRulesPickup=new CheckBox
					 {
						  Content="Use Item Rules Pickup",
						  Width=300,
						  Height=30,
						  IsChecked=(SettingsFunky.UseItemRulesPickup)

					 };
					 ItemRulesPickup.Checked+=ItemRulesPickupChecked;
					 ItemRulesPickup.Unchecked+=ItemRulesPickupChecked;
					 lbItemRulesContent.Items.Add(ItemRulesPickup);

					 CheckBox CBItemRulesSalvaging=new CheckBox
					 {
						  Content="Item Rules Salvaging",
						  Width=300,
						  Height=30,
						  IsChecked=(SettingsFunky.ItemRulesSalvaging),
					 };
					 CBItemRulesSalvaging.Checked+=ItemRulesSalvagingChecked;
					 CBItemRulesSalvaging.Unchecked+=ItemRulesSalvagingChecked;
					 lbItemRulesContent.Items.Add(CBItemRulesSalvaging);

					 lbItemRulesContent.Items.Add("Rule Set");
					 ItemRuleType=new ComboBox
					 {
						  Height=30,
						  Width=300,
						  ItemsSource=new ItemRuleTypes(),
						  Text=SettingsFunky.ItemRuleType
					 };
					 ItemRuleType.SelectionChanged+=ItemRulesTypeChanged;
					 lbItemRulesContent.Items.Add(ItemRuleType);


					 lbItemRulesContent.Items.Add("Log Items Keep");
					 ItemRuleLogKeep=new ComboBox
					 {
						  Height=30,
						  Width=300,
						  ItemsSource=new ItemRuleQuality(),
						  Text=SettingsFunky.ItemRuleLogKeep
					 };
					 ItemRuleLogKeep.SelectionChanged+=ItemRulesLogKeepChanged;
					 lbItemRulesContent.Items.Add(ItemRuleLogKeep);

					 lbItemRulesContent.Items.Add("Log Items Pickup");
					 ItemRuleLogPickup=new ComboBox
					 {
						  Height=30,
						  Width=300,
						  ItemsSource=new ItemRuleQuality(),
						  Text=SettingsFunky.ItemRuleLogPickup
					 };
					 ItemRuleLogPickup.SelectionChanged+=ItemRulesLogPickupChanged;
					 lbItemRulesContent.Items.Add(ItemRuleLogPickup);

					 ItemRuleUseItemIDs=new CheckBox
					 {
						  Content="Use Item IDs",
						  Width=300,
						  Height=30,
						  IsChecked=(SettingsFunky.ItemRuleUseItemIDs)

					 };
					 ItemRuleUseItemIDs.Checked+=ItemRulesItemIDsChecked;
					 ItemRuleUseItemIDs.Unchecked+=ItemRulesItemIDsChecked;
					 lbItemRulesContent.Items.Add(ItemRuleUseItemIDs);

					 ItemRuleDebug=new CheckBox
					 {
						  Content="Debug",
						  Width=300,
						  Height=30,
						  IsChecked=(SettingsFunky.ItemRuleDebug)

					 };
					 ItemRuleDebug.Checked+=ItemRulesDebugChecked;
					 ItemRuleDebug.Unchecked+=ItemRulesDebugChecked;
					 lbItemRulesContent.Items.Add(ItemRuleDebug);


					 ItemRulesReload=new Button
					 {
						  Content="Reload rules",
						  Width=300,
						  Height=30
					 };
					 ItemRulesReload.Click+=ItemRulesReload_Click;
					 lbItemRulesContent.Items.Add(ItemRulesReload);

					 ItemRulesTabItem.Content=lbItemRulesContent;
					 #endregion

					 #region GilesPickup
					 TabItem ItemGilesTabItem=new TabItem();
					 ItemGilesTabItem.Header="Giles Pickup";
					 tcItems.Items.Add(ItemGilesTabItem);
					 ListBox lbGilesContent=new ListBox();

					 lbGilesContent.Items.Add("Item Level Pickup");
					 #region minimumWeaponILevel
					 TextBlock txt_weaponIlvl=new TextBlock
					 {
						  Text="Weapons",
						  FontSize=12,
						  Background=System.Windows.Media.Brushes.DarkSlateGray,
					 };
					 lbGilesContent.Items.Add(txt_weaponIlvl);

					 TextBlock txt_weaponMagical=new TextBlock
					 {
						  Text="Magic",
						  FontSize=12,
						  Background=System.Windows.Media.Brushes.LightSteelBlue,
					 };
					 TBMinimumWeaponLevel=new TextBox[2];
					 Slider weaponMagicLevelSlider=new Slider
					 {
						  Name="Magic",
						  Width=120,
						  Maximum=63,
						  Minimum=0,
						  TickFrequency=5,
						  LargeChange=5,
						  SmallChange=1,
						  Value=SettingsFunky.MinimumWeaponItemLevel[0],
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 weaponMagicLevelSlider.ValueChanged+=WeaponItemLevelSliderChanged;
					 TBMinimumWeaponLevel[0]=new TextBox
					 {
						  Text=SettingsFunky.MinimumWeaponItemLevel[0].ToString(),
						  IsReadOnly=true,
					 };

					 TextBlock txt_weaponRare=new TextBlock
					 {
						  Text="Rare",
						  FontSize=12,
						  Background=System.Windows.Media.Brushes.LightGoldenrodYellow,
					 };
					 Slider weaponRareLevelSlider=new Slider
					 {
						  Name="Rare",
						  Width=120,
						  Maximum=63,
						  Minimum=0,
						  TickFrequency=5,
						  LargeChange=5,
						  SmallChange=1,
						  Value=SettingsFunky.MinimumWeaponItemLevel[1],
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Right,
					 };
					 weaponRareLevelSlider.ValueChanged+=WeaponItemLevelSliderChanged;
					 TBMinimumWeaponLevel[1]=new TextBox
					 {
						  Text=SettingsFunky.MinimumWeaponItemLevel[1].ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel weaponLevelSPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 weaponLevelSPanel.Children.Add(txt_weaponMagical);
					 weaponLevelSPanel.Children.Add(weaponMagicLevelSlider);
					 weaponLevelSPanel.Children.Add(TBMinimumWeaponLevel[0]);
					 weaponLevelSPanel.Children.Add(txt_weaponRare);
					 weaponLevelSPanel.Children.Add(weaponRareLevelSlider);
					 weaponLevelSPanel.Children.Add(TBMinimumWeaponLevel[1]);
					 lbGilesContent.Items.Add(weaponLevelSPanel);
					 #endregion
					 #region minimumArmorILevel
					 TBMinimumArmorLevel=new TextBox[2];
					 TextBlock txt_armorIlvl=new TextBlock
					 {
						  Text="Armor",
						  FontSize=12,
						  Background=System.Windows.Media.Brushes.DarkSlateGray,
					 };
					 lbGilesContent.Items.Add(txt_armorIlvl);

					 TextBlock txt_armorMagic=new TextBlock
					 {
						  Text="Magic",
						  FontSize=12,
						  Background=System.Windows.Media.Brushes.LightSteelBlue,
					 };
					 Slider armorMagicLevelSlider=new Slider
					 {
						  Name="Magic",
						  Width=120,
						  Maximum=63,
						  Minimum=0,
						  TickFrequency=5,
						  LargeChange=5,
						  SmallChange=1,
						  Value=SettingsFunky.MinimumArmorItemLevel[0],
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 armorMagicLevelSlider.ValueChanged+=ArmorItemLevelSliderChanged;
					 TBMinimumArmorLevel[0]=new TextBox
					 {
						  Text=SettingsFunky.MinimumArmorItemLevel[0].ToString(),
						  IsReadOnly=true,
					 };

					 TextBlock txt_armorRare=new TextBlock
					 {
						  Text="Rare",
						  FontSize=12,
						  Background=System.Windows.Media.Brushes.LightGoldenrodYellow,
					 };
					 Slider armorRareLevelSlider=new Slider
					 {
						  Name="Rare",
						  Maximum=63,
						  Minimum=0,
						  TickFrequency=5,
						  LargeChange=5,
						  SmallChange=1,
						  Width=120,
						  Value=SettingsFunky.MinimumArmorItemLevel[1],
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Right,
					 };
					 armorRareLevelSlider.ValueChanged+=ArmorItemLevelSliderChanged;
					 TBMinimumArmorLevel[1]=new TextBox
					 {
						  Text=SettingsFunky.MinimumArmorItemLevel[1].ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel armorLevelSPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 armorLevelSPanel.Children.Add(txt_armorMagic);
					 armorLevelSPanel.Children.Add(armorMagicLevelSlider);
					 armorLevelSPanel.Children.Add(TBMinimumArmorLevel[0]);
					 armorLevelSPanel.Children.Add(txt_armorRare);
					 armorLevelSPanel.Children.Add(armorRareLevelSlider);
					 armorLevelSPanel.Children.Add(TBMinimumArmorLevel[1]);
					 lbGilesContent.Items.Add(armorLevelSPanel);
					 #endregion
					 #region minimumJeweleryILevel
					 TBMinimumJeweleryLevel=new TextBox[2];
					 TextBlock txt_jeweleryIlvl=new TextBlock
					 {
						  Text="Jewelery",
						  FontSize=12,
						  Background=System.Windows.Media.Brushes.DarkSlateGray,
					 };
					 lbGilesContent.Items.Add(txt_jeweleryIlvl);

					 TextBlock txt_jeweleryMagic=new TextBlock
					 {
						  Text="Magic",
						  FontSize=12,
						  Background=System.Windows.Media.Brushes.LightSteelBlue,
					 };
					 Slider jeweleryMagicLevelSlider=new Slider
					 {
						  Name="Magic",
						  Width=120,
						  Maximum=63,
						  Minimum=0,
						  TickFrequency=5,
						  LargeChange=5,
						  SmallChange=1,
						  Value=SettingsFunky.MinimumJeweleryItemLevel[0],
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 jeweleryMagicLevelSlider.ValueChanged+=JeweleryItemLevelSliderChanged;
					 TBMinimumJeweleryLevel[0]=new TextBox
					 {
						  Text=SettingsFunky.MinimumJeweleryItemLevel[0].ToString(),
						  IsReadOnly=true,
					 };
					 TextBlock txt_jeweleryRare=new TextBlock
					 {
						  Text="Rare",
						  FontSize=12,
						  Background=System.Windows.Media.Brushes.LightGoldenrodYellow,
					 };
					 Slider jeweleryRareLevelSlider=new Slider
					 {
						  Name="Rare",
						  Maximum=63,
						  Minimum=0,
						  TickFrequency=5,
						  LargeChange=5,
						  SmallChange=1,
						  Width=120,
						  Value=SettingsFunky.MinimumJeweleryItemLevel[1],
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Right,
					 };
					 jeweleryRareLevelSlider.ValueChanged+=JeweleryItemLevelSliderChanged;
					 TBMinimumJeweleryLevel[1]=new TextBox
					 {
						  Text=SettingsFunky.MinimumJeweleryItemLevel[1].ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel jeweleryLevelSPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 jeweleryLevelSPanel.Children.Add(txt_jeweleryMagic);
					 jeweleryLevelSPanel.Children.Add(jeweleryMagicLevelSlider);
					 jeweleryLevelSPanel.Children.Add(TBMinimumJeweleryLevel[0]);
					 jeweleryLevelSPanel.Children.Add(txt_jeweleryRare);
					 jeweleryLevelSPanel.Children.Add(jeweleryRareLevelSlider);
					 jeweleryLevelSPanel.Children.Add(TBMinimumJeweleryLevel[1]);
					 lbGilesContent.Items.Add(jeweleryLevelSPanel);
					 #endregion


					 #region LegendaryLevel
					 lbGilesContent.Items.Add("Minimum Legendary Item Level");
					 Slider sliderLegendaryILevel=new Slider
					 {
						  Width=120,
						  Maximum=63,
						  Minimum=0,
						  TickFrequency=5,
						  LargeChange=5,
						  SmallChange=1,
						  Value=SettingsFunky.MinimumLegendaryItemLevel,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 sliderLegendaryILevel.ValueChanged+=LegendaryItemLevelSliderChanged;
					 TBMinLegendaryLevel=new TextBox
					 {
						  Text=SettingsFunky.MinimumLegendaryItemLevel.ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel LegendaryILvlStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 LegendaryILvlStackPanel.Children.Add(sliderLegendaryILevel);
					 LegendaryILvlStackPanel.Children.Add(TBMinLegendaryLevel);
					 lbGilesContent.Items.Add(LegendaryILvlStackPanel);
					 #endregion

					 #region MaxHealthPotions
					 lbGilesContent.Items.Add("Maximum Health Potions");
					 Slider sliderMaxHealthPots=new Slider
					 {
						  Width=100,
						  Maximum=100,
						  Minimum=0,
						  TickFrequency=25,
						  LargeChange=20,
						  SmallChange=5,
						  Value=SettingsFunky.MaximumHealthPotions,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 sliderMaxHealthPots.ValueChanged+=HealthPotionSliderChanged;
					 TBMaxHealthPots=new TextBox
					 {
						  Text=SettingsFunky.MaximumHealthPotions.ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel MaxHealthPotsStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 MaxHealthPotsStackPanel.Children.Add(sliderMaxHealthPots);
					 MaxHealthPotsStackPanel.Children.Add(TBMaxHealthPots);
					 lbGilesContent.Items.Add(MaxHealthPotsStackPanel);
					 #endregion

					 #region MinimumGoldPile
					 lbGilesContent.Items.Add("Minimum Gold Pile");
					 Slider slideMinGoldPile=new Slider
					 {
						  Width=120,
						  Maximum=7500,
						  Minimum=0,
						  TickFrequency=1000,
						  LargeChange=1000,
						  SmallChange=1,
						  Value=SettingsFunky.MinimumGoldPile,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 slideMinGoldPile.ValueChanged+=GoldAmountSliderChanged;
					 TBMinGoldPile=new TextBox
					 {
						  Text=SettingsFunky.MinimumGoldPile.ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel MinGoldPileStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 MinGoldPileStackPanel.Children.Add(slideMinGoldPile);
					 MinGoldPileStackPanel.Children.Add(TBMinGoldPile);
					 lbGilesContent.Items.Add(MinGoldPileStackPanel);
					 #endregion


					 #region PickupCraftTomes
					 CheckBox cbPickupCraftTomes=new CheckBox
					 {
						  Content="Pickup Craft Tomes",
						  Width=300,
						  Height=20,
						  IsChecked=(SettingsFunky.PickupCraftTomes)
					 };
					 cbPickupCraftTomes.Checked+=PickupCraftTomesChecked;
					 cbPickupCraftTomes.Unchecked+=PickupCraftTomesChecked;
					 lbGilesContent.Items.Add(cbPickupCraftTomes);
					 #endregion
					 #region PickupCraftPlans
					 CheckBox cbPickupCraftPlans=new CheckBox
					 {
						  Content="Pickup Craft Plans",
						  Width=300,
						  Height=20,
						  IsChecked=(SettingsFunky.PickupCraftPlans)
					 };
					 cbPickupCraftPlans.Checked+=PickupCraftPlansChecked;
					 cbPickupCraftPlans.Unchecked+=PickupCraftPlansChecked;
					 lbGilesContent.Items.Add(cbPickupCraftPlans);
					 #endregion
					 #region PickupFollowerItems
					 CheckBox cbPickupFollowerItems=new CheckBox
					 {
						  Content="Pickup Follower Items",
						  Width=300,
						  Height=20,
						  IsChecked=(SettingsFunky.PickupFollowerItems)
					 };
					 cbPickupFollowerItems.Checked+=PickupFollowerItemsChecked;
					 cbPickupFollowerItems.Unchecked+=PickupFollowerItemsChecked;
					 lbGilesContent.Items.Add(cbPickupFollowerItems);
					 #endregion
					 #region MinMiscItemLevel
					 lbGilesContent.Items.Add("Misc Item Level");
					 Slider slideMinMiscItemLevel=new Slider
					 {
						  Width=100,
						  Maximum=63,
						  Minimum=0,
						  TickFrequency=5,
						  LargeChange=5,
						  SmallChange=1,
						  Value=SettingsFunky.MiscItemLevel,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 slideMinMiscItemLevel.ValueChanged+=MiscItemLevelSliderChanged;
					 TBMiscItemLevel=new TextBox
					 {
						  Text=SettingsFunky.MiscItemLevel.ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel MinMiscItemLevelStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 MinMiscItemLevelStackPanel.Children.Add(slideMinMiscItemLevel);
					 MinMiscItemLevelStackPanel.Children.Add(TBMiscItemLevel);
					 lbGilesContent.Items.Add(MinMiscItemLevelStackPanel);
					 #endregion


					 #region GemQuality
					 lbGilesContent.Items.Add("Minimum Gem Quality");
					 CBGemQualityLevel=new ComboBox
					 {
						  Height=20,
						  Width=200,
						  ItemsSource=new GemQualityTypes(),
						  Text=Enum.GetName(typeof(GemQuality), SettingsFunky.MinimumGemItemLevel).ToString(),
					 };
					 CBGemQualityLevel.SelectionChanged+=GemQualityLevelChanged;
					 lbGilesContent.Items.Add(CBGemQualityLevel);
					 #endregion

					 CBGems=new CheckBox[4];

					 #region PickupGemsRed
					 CBGems[0]=new CheckBox
					 {
						  Content="Pickup Gem Red",
						  Name="red",
						  Width=300,
						  Height=20,
						  IsChecked=(SettingsFunky.PickupGems[0])
					 };
					 CBGems[0].Checked+=GemsChecked;
					 CBGems[0].Unchecked+=GemsChecked;
					 lbGilesContent.Items.Add(CBGems[0]);
					 #endregion
					 #region PickupGemsGreen
					 CBGems[1]=new CheckBox
					 {
						  Content="Pickup Gem Green",
						  Name="green",
						  Width=300,
						  Height=20,
						  IsChecked=(SettingsFunky.PickupGems[1])
					 };
					 CBGems[1].Checked+=GemsChecked;
					 CBGems[1].Unchecked+=GemsChecked;
					 lbGilesContent.Items.Add(CBGems[1]);
					 #endregion
					 #region PickupGemsPurple
					 CBGems[2]=new CheckBox
					 {
						  Content="Pickup Gem Purple",
						  Name="purple",
						  Width=300,
						  Height=20,
						  IsChecked=(SettingsFunky.PickupGems[2])
					 };
					 CBGems[2].Checked+=GemsChecked;
					 CBGems[2].Unchecked+=GemsChecked;
					 lbGilesContent.Items.Add(CBGems[2]);
					 #endregion
					 #region PickupGemsYellow
					 CBGems[3]=new CheckBox
					 {
						  Content="Pickup Gem Yellow",
						  Name="yellow",
						  Width=300,
						  Height=20,
						  IsChecked=(SettingsFunky.PickupGems[3])
					 };
					 CBGems[3].Checked+=GemsChecked;
					 CBGems[3].Unchecked+=GemsChecked;
					 lbGilesContent.Items.Add(CBGems[3]);
					 #endregion

					 ItemGilesTabItem.Content=lbGilesContent;
					 #endregion

					 #region GilesScoring
					 TabItem ItemGilesScoringTabItem=new TabItem();
					 ItemGilesScoringTabItem.Header="Giles Scoring";
					 tcItems.Items.Add(ItemGilesScoringTabItem);
					 ListBox lbGilesScoringContent=new ListBox();

					 #region WeaponScore
					 lbGilesScoringContent.Items.Add("Minimum Weapon Score to Stash");
					 Slider sliderGilesWeaponScore=new Slider
					 {
						  Width=100,
						  Maximum=100000,
						  Minimum=1,
						  TickFrequency=1000,
						  LargeChange=5000,
						  SmallChange=1000,
						  Value=SettingsFunky.GilesMinimumWeaponScore,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 sliderGilesWeaponScore.ValueChanged+=GilesWeaponScoreSliderChanged;
					 TBGilesWeaponScore=new TextBox
					 {
						  Text=SettingsFunky.GilesMinimumWeaponScore.ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel GilesWeaponScoreStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 GilesWeaponScoreStackPanel.Children.Add(sliderGilesWeaponScore);
					 GilesWeaponScoreStackPanel.Children.Add(TBGilesWeaponScore);
					 lbGilesScoringContent.Items.Add(GilesWeaponScoreStackPanel);
					 #endregion

					 #region ArmorScore
					 lbGilesScoringContent.Items.Add("Minimum Armor Score to Stash");
					 Slider sliderGilesArmorScore=new Slider
					 {
						  Width=100,
						  Maximum=100000,
						  Minimum=1,
						  TickFrequency=1000,
						  LargeChange=5000,
						  SmallChange=1000,
						  Value=SettingsFunky.GilesMinimumArmorScore,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 sliderGilesArmorScore.ValueChanged+=GilesArmorScoreSliderChanged;
					 TBGilesArmorScore=new TextBox
					 {
						  Text=SettingsFunky.GilesMinimumArmorScore.ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel GilesArmorScoreStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 GilesArmorScoreStackPanel.Children.Add(sliderGilesArmorScore);
					 GilesArmorScoreStackPanel.Children.Add(TBGilesArmorScore);
					 lbGilesScoringContent.Items.Add(GilesArmorScoreStackPanel);
					 #endregion

					 #region JeweleryScore
					 lbGilesScoringContent.Items.Add("Minimum Jewelery Score to Stash");
					 Slider sliderGilesJeweleryScore=new Slider
					 {
						  Width=100,
						  Maximum=100000,
						  Minimum=1,
						  TickFrequency=1000,
						  LargeChange=5000,
						  SmallChange=1000,
						  Value=SettingsFunky.GilesMinimumJeweleryScore,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 sliderGilesJeweleryScore.ValueChanged+=GilesJeweleryScoreSliderChanged;
					 TBGilesJeweleryScore=new TextBox
					 {
						  Text=SettingsFunky.GilesMinimumJeweleryScore.ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel GilesJeweleryScoreStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 GilesJeweleryScoreStackPanel.Children.Add(sliderGilesJeweleryScore);
					 GilesJeweleryScoreStackPanel.Children.Add(TBGilesJeweleryScore);
					 lbGilesScoringContent.Items.Add(GilesJeweleryScoreStackPanel);
					 #endregion

					 ItemGilesScoringTabItem.Content=lbGilesScoringContent;
					 #endregion


					 CustomSettingsTabItem.Content=tcItems;
					 #endregion

					 #region Targeting
					 TabItem TargetTabItem=new TabItem();
					 TargetTabItem.Header="Targeting";
					 tabControl1.Items.Add(TargetTabItem);

					 TabControl tcTargeting=new TabControl
					 {
						  Height=600,
						  Width=600,
					 };

					 #region Targeting_General
					 TabItem TargetingMiscTabItem=new TabItem();
					 TargetingMiscTabItem.Header="General";
					 tcTargeting.Items.Add(TargetingMiscTabItem);
					 ListBox lbTargetGeneral=new ListBox();


					 #region KillLOWHPUnits
					 CheckBox cbClusterKillLowHPUnits=new CheckBox
					 {
						  Content="Finish Units with 25% or less HP",
						  Width=300,
						  Height=30,
						  IsChecked=(SettingsFunky.ClusterKillLowHPUnits)
					 };
					 cbClusterKillLowHPUnits.Checked+=ClusteringKillLowHPChecked;
					 cbClusterKillLowHPUnits.Unchecked+=ClusteringKillLowHPChecked;
					 lbTargetGeneral.Items.Add(cbClusterKillLowHPUnits);
					 #endregion

					 #region IgnoreElites
					 CheckBox cbIgnoreElites=new CheckBox
					 {
						  Content="Ignore Rare/Elite/Unique Monsters",
						  Width=300,
						  Height=30,
						  IsChecked=(SettingsFunky.IgnoreAboveAverageMobs)
					 };
					 cbIgnoreElites.Checked+=IgnoreEliteMonstersChecked;
					 cbIgnoreElites.Unchecked+=IgnoreEliteMonstersChecked;
					 lbTargetGeneral.Items.Add(cbIgnoreElites);
					 #endregion

					 #region IgnoreCorpses
					 CheckBox cbIgnoreCorpses=new CheckBox
					 {
						  Content="Ignore Looting Corpses",
						  Width=300,
						  Height=30,
						  IsChecked=(SettingsFunky.IgnoreCorpses)
					 };
					 cbIgnoreCorpses.Checked+=IgnoreCorpsesChecked;
					 cbIgnoreCorpses.Unchecked+=IgnoreCorpsesChecked;
					 lbTargetGeneral.Items.Add(cbIgnoreCorpses);
					 #endregion

					 #region WaitTimerAfterContainers
					 EnableWaitAfterContainersCB=new CheckBox
					 {
						  Content="Enable Wait After Opening Containers",
						  Width=300,
						  Height=30,
						  IsChecked=(SettingsFunky.EnableWaitAfterContainers)
					 };
					 EnableWaitAfterContainersCB.Checked+=EnableWaitAfterContainersChecked;
					 EnableWaitAfterContainersCB.Unchecked+=EnableWaitAfterContainersChecked;
					 lbTargetGeneral.Items.Add(EnableWaitAfterContainersCB);
					 #endregion

					 #region ExtendedRepChestRange
					 UseExtendedRangeRepChestCB=new CheckBox
					 {
						  Content="Use Extended Target Range for Rep Chest (Rare)",
						  Width=300,
						  Height=30,
						  IsChecked=(SettingsFunky.UseExtendedRangeRepChest)

					 };
					 UseExtendedRangeRepChestCB.Checked+=ExtendRangeRepChestChecked;
					 UseExtendedRangeRepChestCB.Unchecked+=ExtendRangeRepChestChecked;
					 lbTargetGeneral.Items.Add(UseExtendedRangeRepChestCB);
					 #endregion

					 #region GoblinPriority
					 lbTargetGeneral.Items.Add("Goblin Priority");
					 ComboBox CBGoblinPriority=new ComboBox
					 {
						  Height=30,
						  Width=300,
						  ItemsSource=new GoblinPriority(),
						  SelectedIndex=SettingsFunky.GoblinPriority,
					 };
					 CBGoblinPriority.SelectionChanged+=GoblinPriorityChanged;
					 lbTargetGeneral.Items.Add(CBGoblinPriority);
					 #endregion

					 #region AfterCombatDelay
					 lbTargetGeneral.Items.Add("Delay after combat for loot drops");
					 Slider sliderAfterCombatDelay=new Slider
					 {
						  Width=200,
						  Maximum=2000,
						  Minimum=0,
						  TickFrequency=200,
						  LargeChange=100,
						  SmallChange=50,
						  Value=SettingsFunky.AfterCombatDelay,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 sliderAfterCombatDelay.ValueChanged+=AfterCombatDelaySliderChanged;
					 TBAfterCombatDelay=new TextBox
					 {
						  Text=SettingsFunky.AfterCombatDelay.ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel AfterCombatStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=30,
						  Orientation=Orientation.Horizontal,
					 };
					 AfterCombatStackPanel.Children.Add(sliderAfterCombatDelay);
					 AfterCombatStackPanel.Children.Add(TBAfterCombatDelay);
					 lbTargetGeneral.Items.Add(AfterCombatStackPanel);
					 #endregion

					 TargetingMiscTabItem.Content=lbTargetGeneral;
					 #endregion

					 #region Targeting_Ranges
					 TabItem RangeTabItem=new TabItem();
					 RangeTabItem.Header="Range";
					 tcTargeting.Items.Add(RangeTabItem);
					 ListBox lbTargetRange=new ListBox();

					 CheckBox cbIgnoreCombatRange=new CheckBox
					 {
						  Content="Ignore Combat Range (Set by Profile)",
						  Width=300,
						  Height=30,
						  IsChecked=(SettingsFunky.IgnoreCombatRange)
					 };
					 cbIgnoreCombatRange.Checked+=IgnoreCombatRangeChecked;
					 cbIgnoreCombatRange.Unchecked+=IgnoreCombatRangeChecked;
					 lbTargetRange.Items.Add(cbIgnoreCombatRange);

					 CheckBox cbIgnoreLootRange=new CheckBox
					 {
						  Content="Ignore Loot Range (Set by Profile)",
						  Width=300,
						  Height=30,
						  IsChecked=(SettingsFunky.IgnoreLootRange)
					 };
					 cbIgnoreLootRange.Checked+=IgnoreLootRangeChecked;
					 cbIgnoreLootRange.Unchecked+=IgnoreLootRangeChecked;
					 lbTargetRange.Items.Add(cbIgnoreLootRange);

					 #region EliteRange
					 lbTargetRange.Items.Add("Elite Combat Range");
					 Slider sliderEliteRange=new Slider
					 {
						  Width=100,
						  Maximum=75,
						  Minimum=0,
						  TickFrequency=5,
						  LargeChange=5,
						  SmallChange=1,
						  Value=SettingsFunky.EliteCombatRange,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 sliderEliteRange.ValueChanged+=EliteRangeSliderChanged;
					 TBEliteRange=new TextBox
					 {
						  Text=SettingsFunky.EliteCombatRange.ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel EliteStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 EliteStackPanel.Children.Add(sliderEliteRange);
					 EliteStackPanel.Children.Add(TBEliteRange);
					 lbTargetRange.Items.Add(EliteStackPanel);
					 #endregion

					 #region NonEliteRange
					 lbTargetRange.Items.Add("Non-Elite Combat Range");
					 Slider sliderNonEliteRange=new Slider
					 {
						  Width=100,
						  Maximum=75,
						  Minimum=0,
						  TickFrequency=5,
						  LargeChange=5,
						  SmallChange=1,
						  Value=SettingsFunky.NonEliteCombatRange,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 sliderNonEliteRange.ValueChanged+=NonEliteRangeSliderChanged;
					 TBNonEliteRange=new TextBox
					 {
						  Text=SettingsFunky.NonEliteCombatRange.ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel NonEliteStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 NonEliteStackPanel.Children.Add(sliderNonEliteRange);
					 NonEliteStackPanel.Children.Add(TBNonEliteRange);
					 lbTargetRange.Items.Add(NonEliteStackPanel);
					 #endregion

					 #region ExtendedCombatRange
					 lbTargetRange.Items.Add("Extended Combat Range");
					 Slider sliderExtendedCombatRange=new Slider
					 {
						  Width=100,
						  Maximum=50,
						  Minimum=0,
						  TickFrequency=5,
						  LargeChange=5,
						  SmallChange=1,
						  Value=SettingsFunky.ExtendedCombatRange,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 sliderExtendedCombatRange.ValueChanged+=ExtendCombatRangeSliderChanged;
					 TBExtendedCombatRange=new TextBox
					 {
						  Text=SettingsFunky.ExtendedCombatRange.ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel ExtendedRangeStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 ExtendedRangeStackPanel.Children.Add(sliderExtendedCombatRange);
					 ExtendedRangeStackPanel.Children.Add(TBExtendedCombatRange);
					 lbTargetRange.Items.Add(ExtendedRangeStackPanel);
					 #endregion

					 #region ShrineRange
					 lbTargetRange.Items.Add("Shrine Range");
					 Slider sliderShrineRange=new Slider
					 {
						  Width=100,
						  Maximum=70,
						  Minimum=0,
						  TickFrequency=5,
						  LargeChange=5,
						  SmallChange=1,
						  Value=SettingsFunky.ShrineRange,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 sliderShrineRange.ValueChanged+=ShrineRangeSliderChanged;
					 TBShrineRange=new TextBox
					 {
						  Text=SettingsFunky.ShrineRange.ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel ShrineStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 ShrineStackPanel.Children.Add(sliderShrineRange);
					 ShrineStackPanel.Children.Add(TBShrineRange);
					 lbTargetRange.Items.Add(ShrineStackPanel);
					 #endregion

					 #region ContainerRange
					 lbTargetRange.Items.Add("Container Range");
					 Slider sliderContainerRange=new Slider
					 {
						  Width=100,
						  Maximum=70,
						  Minimum=0,
						  TickFrequency=5,
						  LargeChange=5,
						  SmallChange=1,
						  Value=SettingsFunky.ContainerOpenRange,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 sliderContainerRange.ValueChanged+=ContainerRangeSliderChanged;
					 TBContainerRange=new TextBox
					 {
						  Text=SettingsFunky.ContainerOpenRange.ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel ContainerStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 ContainerStackPanel.Children.Add(sliderContainerRange);
					 ContainerStackPanel.Children.Add(TBContainerRange);
					 lbTargetRange.Items.Add(ContainerStackPanel);
					 #endregion

					 #region DestructibleRange
					 lbTargetRange.Items.Add("Destuctible Range");
					 Slider sliderDestructibleRange=new Slider
					 {
						  Width=100,
						  Maximum=70,
						  Minimum=0,
						  TickFrequency=5,
						  LargeChange=5,
						  SmallChange=1,
						  Value=SettingsFunky.DestructibleRange,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 sliderDestructibleRange.ValueChanged+=DestructibleSliderChanged;
					 TBDestructibleRange=new TextBox
					 {
						  Text=SettingsFunky.DestructibleRange.ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel DestructibleStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 DestructibleStackPanel.Children.Add(sliderDestructibleRange);
					 DestructibleStackPanel.Children.Add(TBDestructibleRange);
					 lbTargetRange.Items.Add(DestructibleStackPanel);
					 #endregion

					 #region GoldRange
					 lbTargetRange.Items.Add("Gold Range");
					 Slider sliderGoldRange=new Slider
					 {
						  Width=100,
						  Maximum=75,
						  Minimum=0,
						  TickFrequency=5,
						  LargeChange=5,
						  SmallChange=1,
						  Value=SettingsFunky.GoldRange,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 sliderGoldRange.ValueChanged+=GoldRangeSliderChanged;
					 TBGoldRange=new TextBox
					 {
						  Text=SettingsFunky.GoldRange.ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel GoldRangeStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 GoldRangeStackPanel.Children.Add(sliderGoldRange);
					 GoldRangeStackPanel.Children.Add(TBGoldRange);
					 lbTargetRange.Items.Add(GoldRangeStackPanel);
					 #endregion

					 #region ItemRange
					 lbTargetRange.Items.Add("Item Loot Range");
					 Slider sliderItemRange=new Slider
					 {
						  Width=100,
						  Maximum=75,
						  Minimum=0,
						  TickFrequency=5,
						  LargeChange=5,
						  SmallChange=1,
						  Value=SettingsFunky.ItemRange,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 sliderItemRange.ValueChanged+=ItemRangeSliderChanged;
					 TBItemRange=new TextBox
					 {
						  Text=SettingsFunky.ItemRange.ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel ItemRangeStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 ItemRangeStackPanel.Children.Add(sliderItemRange);
					 ItemRangeStackPanel.Children.Add(TBItemRange);
					 lbTargetRange.Items.Add(ItemRangeStackPanel);
					 #endregion

					 #region GoblinRange
					 lbTargetRange.Items.Add("Treasure Goblin Range");
					 Slider sliderGoblinRange=new Slider
					 {
						  Width=100,
						  Maximum=75,
						  Minimum=0,
						  TickFrequency=5,
						  LargeChange=5,
						  SmallChange=1,
						  Value=SettingsFunky.TreasureGoblinRange,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 sliderGoblinRange.ValueChanged+=TreasureGoblinRangeSliderChanged;
					 TBGoblinRange=new TextBox
					 {
						  Text=SettingsFunky.TreasureGoblinRange.ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel GoblinRangeStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 GoblinRangeStackPanel.Children.Add(sliderGoblinRange);
					 GoblinRangeStackPanel.Children.Add(TBGoblinRange);
					 lbTargetRange.Items.Add(GoblinRangeStackPanel);
					 #endregion

					 RangeTabItem.Content=lbTargetRange;
					 #endregion

					 TabItem TargetingClusterItem=new TabItem();
					 TargetingClusterItem.Header="Clustering";
					 tcTargeting.Items.Add(TargetingClusterItem);
					 ListBox lbTargetCluster=new ListBox();

					 CheckBox cbClusterEnabled=new CheckBox
					 {
						  Content="Enable Clustering Target Logic",
						  Width=300,
						  Height=20,
						  IsChecked=(SettingsFunky.EnableClusteringTargetLogic)
					 };
					 cbClusterEnabled.Checked+=EnableClusteringTargetLogicChecked;
					 cbClusterEnabled.Unchecked+=EnableClusteringTargetLogicChecked;
					 lbTargetCluster.Items.Add(cbClusterEnabled);

					 #region LowHP
					 CheckBox cbClusterIgnoreBotLowHP=new CheckBox
				 {
					  Content="Disable Logic on Bot HP",
					  Width=300,
					  Height=20,
					  IsChecked=(SettingsFunky.IgnoreClusteringWhenLowHP)
				 };
					 cbClusterIgnoreBotLowHP.Checked+=IgnoreClusteringBotLowHPisChecked;
					 cbClusterIgnoreBotLowHP.Unchecked+=IgnoreClusteringBotLowHPisChecked;
					 // lbTargetCluster.Items.Add(cbClusterIgnoreBotLowHP);

					 #region ClusterLowHPSliderValue
					 lbTargetCluster.Items.Add("HP Value");
					 Slider sliderClusterLowHPValue=new Slider
					 {
						  Width=100,
						  Maximum=1,
						  Minimum=0,
						  TickFrequency=0.25,
						  LargeChange=0.25,
						  SmallChange=0.10,
						  Value=SettingsFunky.IgnoreClusterLowHPValue,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 sliderClusterLowHPValue.ValueChanged+=ClusterLowHPValueSliderChanged;
					 TBClusterLowHPValue=new TextBox
					 {
						  Text=SettingsFunky.IgnoreClusterLowHPValue.ToString("F2", CultureInfo.InvariantCulture),
						  IsReadOnly=true,
					 };
					 StackPanel ClusterLowHPValueStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 ClusterLowHPValueStackPanel.Children.Add(cbClusterIgnoreBotLowHP);
					 ClusterLowHPValueStackPanel.Children.Add(sliderClusterLowHPValue);
					 ClusterLowHPValueStackPanel.Children.Add(TBClusterLowHPValue);
					 lbTargetCluster.Items.Add(ClusterLowHPValueStackPanel);
					 #endregion
					 #endregion


					 #region ClusterDistance
					 lbTargetCluster.Items.Add("Cluster Distance");
					 Slider sliderClusterDistance=new Slider
					 {
						  Width=100,
						  Maximum=20,
						  Minimum=0,
						  TickFrequency=4,
						  LargeChange=5,
						  SmallChange=1,
						  Value=SettingsFunky.ClusterDistance,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 sliderClusterDistance.ValueChanged+=ClusterDistanceSliderChanged;
					 TBClusterDistance=new TextBox
					 {
						  Text=SettingsFunky.ClusterDistance.ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel ClusterDistanceStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 ClusterDistanceStackPanel.Children.Add(sliderClusterDistance);
					 ClusterDistanceStackPanel.Children.Add(TBClusterDistance);
					 lbTargetCluster.Items.Add(ClusterDistanceStackPanel);
					 #endregion

					 #region ClusterMinUnitCount
					 lbTargetCluster.Items.Add("Cluster Minimum Unit Count");
					 Slider sliderClusterMinUnitCount=new Slider
					 {
						  Width=100,
						  Maximum=10,
						  Minimum=1,
						  TickFrequency=2,
						  LargeChange=2,
						  SmallChange=1,
						  Value=SettingsFunky.ClusterMinimumUnitCount,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 };
					 sliderClusterMinUnitCount.ValueChanged+=ClusterMinUnitSliderChanged;
					 TBClusterMinUnitCount=new TextBox
					 {
						  Text=SettingsFunky.ClusterMinimumUnitCount.ToString(),
						  IsReadOnly=true,
					 };
					 StackPanel ClusterMinUnitCountStackPanel=new StackPanel
					 {
						  Width=600,
						  Height=20,
						  Orientation=Orientation.Horizontal,
					 };
					 ClusterMinUnitCountStackPanel.Children.Add(sliderClusterMinUnitCount);
					 ClusterMinUnitCountStackPanel.Children.Add(TBClusterMinUnitCount);
					 lbTargetCluster.Items.Add(ClusterMinUnitCountStackPanel);
					 #endregion


					 TargetingClusterItem.Content=lbTargetCluster;


					 TargetTabItem.Content=tcTargeting;

					 #endregion


					 TabItem AdvancedTabItem=new TabItem();
					 AdvancedTabItem.Header="Advanced";
					 tabControl1.Items.Add(AdvancedTabItem);
					 ListBox lbAdvancedContent=new ListBox();

					 CheckBox CBDebugStatusBar=new CheckBox
					 {
						  Content="Enable Debug Status Bar",
						  Width=300,
						  Height=20,
						  IsChecked=SettingsFunky.DebugStatusBar,
					 };
					 CBDebugStatusBar.Checked+=DebugStatusBarChecked;
					 CBDebugStatusBar.Unchecked+=DebugStatusBarChecked;
					 lbAdvancedContent.Items.Add(CBDebugStatusBar);

					 CheckBox CBLogSafeMovementOutput=new CheckBox
					 {
						  Content="Enable Logging For Safe Movement",
						  Width=300,
						  Height=20,
						  IsChecked=SettingsFunky.LogSafeMovementOutput,
					 };
					 CBLogSafeMovementOutput.Checked+=LogSafeMovementOutputChecked;
					 CBLogSafeMovementOutput.Unchecked+=LogSafeMovementOutputChecked;
					 lbAdvancedContent.Items.Add(CBLogSafeMovementOutput);

					 AdvancedTabItem.Content=lbAdvancedContent;

					 TabItem MiscTabItem=new TabItem();
					 MiscTabItem.Header="Misc";
					 tabControl1.Items.Add(MiscTabItem);
					 ListBox lbMiscContent=new ListBox();
					 try
					 {
						  lbMiscContent.Items.Add(ReturnLogOutputString());
					 } catch
					 {
						  lbMiscContent.Items.Add("Exception Handled");
					 }

					 MiscTabItem.Content=lbMiscContent;



					 TabItem DebuggingTabItem=new TabItem
					 {
						  Header="Debug",
						  VerticalContentAlignment=System.Windows.VerticalAlignment.Stretch,
						  VerticalAlignment=System.Windows.VerticalAlignment.Stretch,
						  //Width=600,
						  //Height=600,
						  //VerticalContentAlignment=System.Windows.VerticalAlignment.Stretch,
						  //HorizontalContentAlignment=System.Windows.HorizontalAlignment.Stretch,
					 };
					 tabControl1.Items.Add(DebuggingTabItem);
					 DockPanel DockPanel_Debug=new DockPanel
					 {
						  LastChildFill=true,
						  FlowDirection=System.Windows.FlowDirection.LeftToRight,

					 };

					 //ListBox lbDebugContent=new ListBox();
					 //lbDebugContent.HorizontalContentAlignment=System.Windows.HorizontalAlignment.Stretch;
					 //lbDebugContent.VerticalContentAlignment=System.Windows.VerticalAlignment.Stretch;

					 Button btnObjects_Debug=new Button
					 {
						  Content="Dump Object Cache",
						  Width=150,
						  Height=25,
						  Name="Objects",
					 };
					 btnObjects_Debug.Click+=DebugButtonClicked;
					 Button btnObstacles_Debug=new Button
					 {
						  Content="Dump Obstacle Cache",
						  Width=150,
						  Height=25,
						  Name="Obstacles",
					 };
					 btnObstacles_Debug.Click+=DebugButtonClicked;
					 Button btnSNO_Debug=new Button
					 {
						  Content="Dump SNO Cache",
						  Width=150,
						  Height=25,
						  Name="SNO",
					 };
					 btnSNO_Debug.Click+=DebugButtonClicked;
					 Button btnGPC_Debug=new Button
					 {
						  Content="Reset Bot Cache",
						  Width=150,
						  Height=25,
						  Name="RESET",
					 };
					 btnGPC_Debug.Click+=DebugButtonClicked;
					 Button btnMGP_Debug=new Button
					 {
						  Content="MGP Details",
						  Width=150,
						  Height=25,
						  Name="MGP",
					 };
					 btnMGP_Debug.Click+=DebugButtonClicked;
					 Button btnTEST_Debug=new Button
					 {
						  Content="Test",
						  Width=150,
						  Height=25,
						  Name="TEST",
					 };
					 btnTEST_Debug.Click+=DebugButtonClicked;
					 StackPanel StackPanel_DebugButtons=new StackPanel
					 {
						  Height=40,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
						  FlowDirection=System.Windows.FlowDirection.LeftToRight,
						  Orientation=Orientation.Horizontal,
						  VerticalAlignment=System.Windows.VerticalAlignment.Top,

					 };
					 StackPanel_DebugButtons.Children.Add(btnObjects_Debug);
					 StackPanel_DebugButtons.Children.Add(btnObstacles_Debug);
					 StackPanel_DebugButtons.Children.Add(btnSNO_Debug);
					 StackPanel_DebugButtons.Children.Add(btnTEST_Debug);
					 //StackPanel_DebugButtons.Children.Add(btnMGP_Debug);
					 //StackPanel_DebugButtons.Children.Add(btnGPC_Debug);

					 DockPanel.SetDock(StackPanel_DebugButtons, Dock.Top);
					 DockPanel_Debug.Children.Add(StackPanel_DebugButtons);

					 LBDebug=new ListBox
					 {
						  SelectionMode=SelectionMode.Extended,
						  HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
						  VerticalAlignment=System.Windows.VerticalAlignment.Stretch,
						  VerticalContentAlignment=System.Windows.VerticalAlignment.Stretch,
						  HorizontalContentAlignment=System.Windows.HorizontalAlignment.Stretch,
					 };

					 DockPanel_Debug.Children.Add(LBDebug);

					 DebuggingTabItem.Content=DockPanel_Debug;


					 this.AddChild(LBWindowContent);
				}

				private void DebugButtonClicked(object sender, EventArgs e)
				{
					 LBDebug.Items.Clear();

					 Button btnsender=(Button)sender;
					 if (btnsender.Name=="Objects")
					 {
						  LBDebug.Items.Add(ObjectCache.Objects.DumpDebugInfo());

						  Zeta.Common.Logging.WriteVerbose("Dumping Object Cache");
						  try
						  {

								foreach (var item in ObjectCache.Objects.Values)
								{
									 LBDebug.Items.Add(item.DebugString);
								}
						  } catch (InvalidOperationException)
						  {
								LBDebug.Items.Add("End of Output due to Modification Exception");
						  }

					 }
					 else if (btnsender.Name=="Obstacles")
					 {
						  LBDebug.Items.Add(ObjectCache.Obstacles.DumpDebugInfo());

						  Zeta.Common.Logging.WriteVerbose("Dumping Obstacle Cache");
						  try
						  {
								foreach (var item in ObjectCache.Obstacles)
								{
									 LBDebug.Items.Add(item.Value.DebugString);
								}
						  } catch (InvalidOperationException)
						  {

								LBDebug.Items.Add("End of Output due to Modification Exception");
						  }

					 }
					 else if (btnsender.Name=="SNO")
					 {

						  LBDebug.Items.Add(ObjectCache.cacheSnoCollection.DumpDebugInfo());

						  Zeta.Common.Logging.WriteVerbose("Dumping SNO Cache");
						  try
						  {
								foreach (var item in ObjectCache.cacheSnoCollection)
								{
									 LBDebug.Items.Add(item.Value.DebugString);
								}
						  } catch (InvalidOperationException)
						  {

								LBDebug.Items.Add("End of Output due to Modification Exception");
						  }

					 }
					 else if (btnsender.Name=="RESET")
					 {

					 }
					 else if (btnsender.Name=="MGP")
					 {
						  UpdateSearchGridProvider(true);
					 }
					 else if (btnsender.Name=="TEST")
					 {
						  try
						  {
								if (Bot.Class==null)
									 return;

								Logging.Write("Character Information");
								Logging.Write("Hotbar Abilities");
								foreach (Zeta.Internals.Actors.SNOPower item in Bot.Class.HotbarAbilities)
								{
									 Logging.Write("{0} with current rune index {1}", item.ToString(), Bot.Class.RuneIndexCache.ContainsKey(item)?Bot.Class.RuneIndexCache[item].ToString():"none");
								}
								Bot.Character.UpdateAnimationState();
								Logging.Write("State: {0} -- SNOAnim {1}", Bot.Character.CurrentAnimationState.ToString(), Bot.Character.CurrentSNOAnim.ToString());

						  } catch (Exception ex)
						  {
								Logging.WriteVerbose("Safely Handled Exception {0}", ex.Message);
						  }

					 }

					 LBDebug.Items.Refresh();
				}

				private void WeaponItemLevelSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 if (slider_sender.Name=="Magic")
					 {
						  SettingsFunky.MinimumWeaponItemLevel[0]=Value;
						  TBMinimumWeaponLevel[0].Text=Value.ToString();
					 }
					 else
					 {
						  SettingsFunky.MinimumWeaponItemLevel[1]=Value;
						  TBMinimumWeaponLevel[1].Text=Value.ToString();
					 }
				}
				private void ArmorItemLevelSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 if (slider_sender.Name=="Magic")
					 {
						  SettingsFunky.MinimumArmorItemLevel[0]=Value;
						  TBMinimumArmorLevel[0].Text=Value.ToString();
					 }
					 else
					 {
						  SettingsFunky.MinimumArmorItemLevel[1]=Value;
						  TBMinimumArmorLevel[1].Text=Value.ToString();
					 }
				}
				private void JeweleryItemLevelSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 if (slider_sender.Name=="Magic")
					 {
						  SettingsFunky.MinimumJeweleryItemLevel[0]=Value;
						  TBMinimumJeweleryLevel[0].Text=Value.ToString();
					 }
					 else
					 {
						  SettingsFunky.MinimumJeweleryItemLevel[1]=Value;
						  TBMinimumJeweleryLevel[1].Text=Value.ToString();
					 }
				}
				private void GilesWeaponScoreSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.GilesMinimumWeaponScore=Value;
					 TBGilesWeaponScore.Text=Value.ToString();
				}
				private void GilesArmorScoreSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.GilesMinimumArmorScore=Value;
					 TBGilesArmorScore.Text=Value.ToString();
				}
				private void GilesJeweleryScoreSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.GilesMinimumJeweleryScore=Value;
					 TBGilesJeweleryScore.Text=Value.ToString();
				}

				private void LegendaryItemLevelSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.MinimumLegendaryItemLevel=Value;
					 TBMinLegendaryLevel.Text=Value.ToString();
				}
				private void HealthPotionSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.MaximumHealthPotions=Value;
					 TBMaxHealthPots.Text=Value.ToString();
				}
				private void GoldAmountSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.MinimumGoldPile=Value;
					 TBMinGoldPile.Text=Value.ToString();
				}
				private void ClusterDistanceSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.ClusterDistance=Value;
					 TBClusterDistance.Text=Value.ToString();
				}
				private void ClusterMinUnitSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.ClusterMinimumUnitCount=Value;
					 TBClusterMinUnitCount.Text=Value.ToString();
				}
				private void ClusterLowHPValueSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 double Value=Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
					 SettingsFunky.IgnoreClusterLowHPValue=Value;
					 TBClusterLowHPValue.Text=Value.ToString();
				}
				private void EnableClusteringTargetLogicChecked(object sender, EventArgs e)
				{
					 SettingsFunky.EnableClusteringTargetLogic=!SettingsFunky.EnableClusteringTargetLogic;
				}
				private void IgnoreClusteringBotLowHPisChecked(object sender, EventArgs e)
				{
					 SettingsFunky.IgnoreClusteringWhenLowHP=!SettingsFunky.IgnoreClusteringWhenLowHP;
				}
				private void ClusteringKillLowHPChecked(object sender, EventArgs e)
				{
					 SettingsFunky.ClusterKillLowHPUnits=!SettingsFunky.ClusterKillLowHPUnits;
				}
				private void PickupCraftTomesChecked(object sender, EventArgs e)
				{
					 SettingsFunky.PickupCraftTomes=!SettingsFunky.PickupCraftTomes;
				}
				private void PickupCraftPlansChecked(object sender, EventArgs e)
				{
					 SettingsFunky.PickupCraftPlans=!SettingsFunky.PickupCraftPlans;
				}
				private void PickupFollowerItemsChecked(object sender, EventArgs e)
				{
					 SettingsFunky.PickupFollowerItems=!SettingsFunky.PickupFollowerItems;
				}
				private void GemsChecked(object sender, EventArgs e)
				{
					 CheckBox sender_=(CheckBox)sender;
					 if (sender_.Name=="red") SettingsFunky.PickupGems[0]=!SettingsFunky.PickupGems[0];
					 if (sender_.Name=="green") SettingsFunky.PickupGems[1]=!SettingsFunky.PickupGems[1];
					 if (sender_.Name=="purple") SettingsFunky.PickupGems[2]=!SettingsFunky.PickupGems[2];
					 if (sender_.Name=="yellow") SettingsFunky.PickupGems[3]=!SettingsFunky.PickupGems[3];
				}
				private void MiscItemLevelSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.MiscItemLevel=Value;
					 TBMiscItemLevel.Text=Value.ToString();
				}
				private void IgnoreCombatRangeChecked(object sender, EventArgs e)
				{
					 SettingsFunky.IgnoreCombatRange=!SettingsFunky.IgnoreCombatRange;
				}
				private void IgnoreLootRangeChecked(object sender, EventArgs e)
				{
					 SettingsFunky.IgnoreLootRange=!SettingsFunky.IgnoreLootRange;
				}

				private void EliteRangeSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.EliteCombatRange=Value;
					 TBEliteRange.Text=Value.ToString();
				}
				private void GoldRangeSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.GoldRange=Value;
					 TBGoldRange.Text=Value.ToString();
				}
				private void ItemRangeSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.ItemRange=Value;
					 TBItemRange.Text=Value.ToString();
				}
				private void ShrineRangeSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.ShrineRange=Value;
					 TBShrineRange.Text=Value.ToString();
				}
				private void GlobeHealthSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 double Value=Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
					 SettingsFunky.GlobeHealthPercent=Value;
					 TBGlobeHealth.Text=Value.ToString();
				}
				private void PotionHealthSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 double Value=Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
					 SettingsFunky.PotionHealthPercent=Value;
					 TBPotionHealth.Text=Value.ToString();
				}
				private void ContainerRangeSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.ContainerOpenRange=Value;
					 TBContainerRange.Text=Value.ToString();
				}
				private void NonEliteRangeSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.NonEliteCombatRange=Value;
					 TBNonEliteRange.Text=Value.ToString();
				}
				private void TreasureGoblinRangeSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.TreasureGoblinRange=Value;
					 TBGoblinRange.Text=Value.ToString();
				}
				private void TreasureGoblinMinimumRangeSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.Class.GoblinMinimumRange=Value;
					 TBGoblinMinRange.Text=Value.ToString();
				}
				private void DestructibleSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.DestructibleRange=Value;
					 TBDestructibleRange.Text=Value.ToString();
				}
				private void ExtendCombatRangeSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.ExtendedCombatRange=Value;
					 TBExtendedCombatRange.Text=Value.ToString();
				}
				private void IgnoreCorpsesChecked(object sender, EventArgs e)
				{
					 SettingsFunky.IgnoreCorpses=!SettingsFunky.IgnoreCorpses;
				}
				private void IgnoreEliteMonstersChecked(object sender, EventArgs e)
				{
					 SettingsFunky.IgnoreAboveAverageMobs=!SettingsFunky.IgnoreAboveAverageMobs;
				}
				private void OutOfCombatMovementChecked(object sender, EventArgs e)
				{
					 SettingsFunky.OutOfCombatMovement=!SettingsFunky.OutOfCombatMovement;
				}
				private void KiteSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.KiteDistance=Value;
					 TBKiteDistance.Text=Value.ToString();
				}
				private void AfterCombatDelaySliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.AfterCombatDelay=Value;
					 TBAfterCombatDelay.Text=Value.ToString();
				}
				private void AvoidanceMinimumRetrySliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.AvoidanceRecheckMinimumRate=Value;
					 TBAvoidanceTimeLimits[0].Text=Value.ToString();
				}
				private void AvoidanceMaximumRetrySliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.AvoidanceRecheckMaximumRate=Value;
					 TBAvoidanceTimeLimits[1].Text=Value.ToString();
				}
				private void KitingMaximumRetrySliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.KitingRecheckMaximumRate=Value;
					 TBKiteTimeLimits[1].Text=Value.ToString();
				}
				private void KitingMinimumRetrySliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.KitingRecheckMinimumRate=Value;
					 TBKiteTimeLimits[0].Text=Value.ToString();
				}
				//UseAdvancedProjectileTesting
				private void UseAdvancedProjectileTestingChecked(object sender, EventArgs e)
				{
					 SettingsFunky.UseAdvancedProjectileTesting=!SettingsFunky.UseAdvancedProjectileTesting;
				}
				private void AvoidanceAttemptMovementChecked(object sender, EventArgs e)
				{
					 SettingsFunky.AttemptAvoidanceMovements=!SettingsFunky.AttemptAvoidanceMovements;
				}
				private void AvoidanceRadiusSliderValueChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 string[] slider_info=slider_sender.Name.Split("_".ToCharArray());
					 int tb_index=Convert.ToInt16(slider_info[2]);
					 float currentValue=(int)slider_sender.Value;

					 TBavoidanceRadius[tb_index].Text=currentValue.ToString();
					 AvoidanceType avoidancetype=(AvoidanceType)Enum.Parse(typeof(AvoidanceType), slider_info[0]);
					 dictAvoidanceRadius[avoidancetype]=currentValue;
				}

				private void AvoidanceHealthSliderValueChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 string[] slider_info=slider_sender.Name.Split("_".ToCharArray());
					 double currentValue=Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
					 int tb_index=Convert.ToInt16(slider_info[2]);

					 TBavoidanceHealth[tb_index].Text=currentValue.ToString();
					 AvoidanceType avoidancetype=(AvoidanceType)Enum.Parse(typeof(AvoidanceType), slider_info[0]);


					 switch (ActorClass)
					 {
						  case Zeta.Internals.Actors.ActorClass.Barbarian:
								dictAvoidanceHealthBarb[avoidancetype]=currentValue;
								break;
						  case Zeta.Internals.Actors.ActorClass.DemonHunter:
								dictAvoidanceHealthDemon[avoidancetype]=currentValue;
								break;
						  case Zeta.Internals.Actors.ActorClass.Monk:
								dictAvoidanceHealthMonk[avoidancetype]=currentValue;
								break;
						  case Zeta.Internals.Actors.ActorClass.WitchDoctor:
								dictAvoidanceHealthWitch[avoidancetype]=currentValue;
								break;
						  case Zeta.Internals.Actors.ActorClass.Wizard:
								dictAvoidanceHealthWizard[avoidancetype]=currentValue;
								break;
					 }
				}

				#region ClassSettings

				private void bEnableCriticalMassChecked(object sender, EventArgs e)
				{
					 SettingsFunky.Class.bEnableCriticalMass=!SettingsFunky.Class.bEnableCriticalMass;
				}

				private void bWaitForArchonChecked(object sender, EventArgs e)
				{
					 SettingsFunky.Class.bWaitForArchon=!SettingsFunky.Class.bWaitForArchon;
				}
				private void bKiteOnlyArchonChecked(object sender, EventArgs e)
				{
					 SettingsFunky.Class.bKiteOnlyArchon=!SettingsFunky.Class.bKiteOnlyArchon;
				}

				private void bSelectiveWhirlwindChecked(object sender, EventArgs e)
				{
					 SettingsFunky.Class.bSelectiveWhirlwind=!SettingsFunky.Class.bSelectiveWhirlwind;
				}
				private void bWaitForWrathChecked(object sender, EventArgs e)
				{
					 SettingsFunky.Class.bWaitForWrath=!SettingsFunky.Class.bWaitForWrath;
				}
				private void bGoblinWrathChecked(object sender, EventArgs e)
				{
					 SettingsFunky.Class.bGoblinWrath=!SettingsFunky.Class.bGoblinWrath;
				}
				private void bFuryDumpWrathChecked(object sender, EventArgs e)
				{
					 SettingsFunky.Class.bFuryDumpWrath=!SettingsFunky.Class.bFuryDumpWrath;
				}
				private void bFuryDumpAlwaysChecked(object sender, EventArgs e)
				{
					 SettingsFunky.Class.bFuryDumpAlways=!SettingsFunky.Class.bFuryDumpAlways;
				}
				private void bMonkInnaSetChecked(object sender, EventArgs e)
				{
					 SettingsFunky.Class.bMonkInnaSet=!SettingsFunky.Class.bMonkInnaSet;
				}
				private void iDHVaultMovementDelaySliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.Class.iDHVaultMovementDelay=Value;
					 TBiDHVaultMovementDelay.Text=Value.ToString();
				}
				#endregion

				private void OOCIDChecked(object sender, EventArgs e)
				{
					 SettingsFunky.OOCIdentifyItems=!SettingsFunky.OOCIdentifyItems;
				}
				private void BuyPotionsDuringTownRunChecked(object sender, EventArgs e)
				{
					 SettingsFunky.BuyPotionsDuringTownRun=!SettingsFunky.BuyPotionsDuringTownRun;
				}
				private void EnableWaitAfterContainersChecked(object sender, EventArgs e)
				{
					 SettingsFunky.EnableWaitAfterContainers=!SettingsFunky.EnableWaitAfterContainers;
				}

				private void GemQualityLevelChanged(object sender, EventArgs e)
				{
					 SettingsFunky.MinimumGemItemLevel=(int)Enum.Parse(typeof(GemQuality), CBGemQualityLevel.Items[CBGemQualityLevel.SelectedIndex].ToString());
				}
				class GemQualityTypes : ObservableCollection<string>
				{
					 public GemQualityTypes()
					 {
						  string[] GemNames=Enum.GetNames(typeof(GemQuality));
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
					 ComboBox senderCB=(ComboBox)sender;
					 SettingsFunky.GoblinPriority=senderCB.SelectedIndex;
				}
				private void ItemRulesTypeChanged(object sender, EventArgs e)
				{
					 SettingsFunky.ItemRuleType=ItemRuleType.Items[ItemRuleType.SelectedIndex].ToString();
				}
				private void ItemRulesScoringChanged(object sender, EventArgs e)
				{
					 SettingsFunky.ItemRuleGilesScoring=ItemRuleGilesScoring.IsChecked.Value;
				}
				private void ItemRulesLogPickupChanged(object sender, EventArgs e)
				{
					 SettingsFunky.ItemRuleLogPickup=ItemRuleLogPickup.Items[ItemRuleLogPickup.SelectedIndex].ToString();
				}
				private void ItemRulesLogKeepChanged(object sender, EventArgs e)
				{
					 SettingsFunky.ItemRuleLogKeep=ItemRuleLogKeep.Items[ItemRuleLogKeep.SelectedIndex].ToString();
				}

				private void ItemRulesChecked(object sender, EventArgs e)
				{
					 SettingsFunky.UseItemRules=!SettingsFunky.UseItemRules;
				}
				private void ItemRulesPickupChecked(object sender, EventArgs e)
				{
					 SettingsFunky.UseItemRulesPickup=!SettingsFunky.UseItemRulesPickup;
				}
				private void ItemRulesItemIDsChecked(object sender, EventArgs e)
				{
					 SettingsFunky.ItemRuleUseItemIDs=!SettingsFunky.ItemRuleUseItemIDs;
				}
				private void ItemRulesDebugChecked(object sender, EventArgs e)
				{
					 SettingsFunky.ItemRuleDebug=!SettingsFunky.ItemRuleDebug;
				}
				private void ItemLevelingLogicChecked(object sender, EventArgs e)
				{
					 SettingsFunky.UseLevelingLogic=!SettingsFunky.UseLevelingLogic;
				}
				private void ItemRulesSalvagingChecked(object sender, EventArgs e)
				{
					 SettingsFunky.ItemRulesSalvaging=!SettingsFunky.ItemRulesSalvaging;
				}
				//UseLevelingLogic
				private void ItemRulesReload_Click(object sender, EventArgs e)
				{
					 try
					 {
						  Funky.ItemRulesEval.reloadFromUI();
					 } catch (Exception ex)
					 {
						  Log(ex.Message+"\r\n"+ex.StackTrace);
					 }

				}
				private void DebugStatusBarChecked(object sender, EventArgs e)
				{
					 SettingsFunky.DebugStatusBar=!SettingsFunky.DebugStatusBar;
				}
				private void LogSafeMovementOutputChecked(object sender, EventArgs e)
				{
					 SettingsFunky.LogSafeMovementOutput=!SettingsFunky.LogSafeMovementOutput;
				}

				private void ExtendRangeRepChestChecked(object sender, EventArgs e)
				{
					 SettingsFunky.UseExtendedRangeRepChest=!SettingsFunky.UseExtendedRangeRepChest;
				}
				private void EnableCoffeeBreaksChecked(object sender, EventArgs e)
				{
					 SettingsFunky.EnableCoffeeBreaks=!SettingsFunky.EnableCoffeeBreaks;
				}
				private void BreakMinMinutesSliderChange(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.MinBreakTime=Value;
					 tbMinBreakTime.Text=Value.ToString();
				}
				private void BreakMaxMinutesSliderChange(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 int Value=(int)slider_sender.Value;
					 SettingsFunky.MaxBreakTime=Value;
					 tbMaxBreakTime.Text=Value.ToString();
				}

				#region OOCIDItemTextBox
				private void OOCIdentifyItemsMinValueChanged(object sender, EventArgs e)
				{
					 string lastText=OOCIdentfyItemsMinCount.Text;
					 if (isStringFullyNumerical(lastText))
					 {
						  SettingsFunky.OOCIdentifyItemsMinimumRequired=Convert.ToInt32(lastText);
					 }
				}
				private void OOCMinimumItems_KeyUp(object sender, EventArgs e)
				{
					 if (!Char.IsNumber(OOCIdentfyItemsMinCount.Text.Last()))
					 {
						  OOCIdentfyItemsMinCount.Text=OOCIdentfyItemsMinCount.Text.Substring(0, OOCIdentfyItemsMinCount.Text.Length-1);
					 }
				}
				#endregion

				private void BreakTimeHourSliderChanged(object sender, EventArgs e)
				{
					 Slider slider_sender=(Slider)sender;
					 double Value=Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
					 SettingsFunky.breakTimeHour=Value;
					 TBBreakTimeHour.Text=Value.ToString();
				}

				private bool isStringFullyNumerical(String S, bool isDouble=false)
				{
					 if (!isDouble)
					 {
						  return !S.Any<Char>(c => !Char.IsNumber(c));
					 }
					 else
					 {
						  Regex isnumber=new Regex(@"^[0-9]+(\.[0-9]+)?$");
						  return isnumber.IsMatch(S);
					 }
				}

				private void OpenPluginSettings_Click(object sender, EventArgs e)
				{
					 var thisplugin=Zeta.Common.Plugins.PluginManager.Plugins.Where(p => p.Plugin.Name=="FunkyTrinity");
					 if (thisplugin.Any())
						  thisplugin.First().Plugin.DisplayWindow.Show();
				}
				private void OpenPluginFolder_Click(object sender, EventArgs e)
				{
					 Process.Start(FolderPaths.sTrinityPluginPath);
				}

				protected override void OnClosed(EventArgs e)
				{
					 SaveFunkyConfiguration();
					 base.OnClosed(e);
				}

		  }

		  private static void SaveFunkyConfiguration()
		  {

				string sFunkyCharacterConfigFile=Path.Combine(FolderPaths.sDemonBuddyPath, "Settings", "FunkyTrinity", CurrentAccountName, CurrentHeroName+".cfg");

				FileStream configStream=File.Open(sFunkyCharacterConfigFile, FileMode.Create, FileAccess.Write, FileShare.Read);
				using (StreamWriter configWriter=new StreamWriter(configStream))
				{
					 configWriter.WriteLine("OOCIdentifyItems="+SettingsFunky.OOCIdentifyItems.ToString());
					 configWriter.WriteLine("BuyPotionsDuringTownRun="+SettingsFunky.BuyPotionsDuringTownRun.ToString());
					 configWriter.WriteLine("EnableWaitAfterContainers="+SettingsFunky.EnableWaitAfterContainers.ToString());
					 configWriter.WriteLine("UseExtendedRangeRepChest="+SettingsFunky.UseExtendedRangeRepChest.ToString());
					 configWriter.WriteLine("UseItemRules="+SettingsFunky.UseItemRules.ToString());
					 configWriter.WriteLine("UseItemRulesPickup="+SettingsFunky.UseItemRulesPickup.ToString());
					 configWriter.WriteLine("OOCIdentifyItemsMinimumRequired="+SettingsFunky.OOCIdentifyItemsMinimumRequired.ToString());

					 configWriter.WriteLine("EnableCoffeeBreaks="+SettingsFunky.EnableCoffeeBreaks.ToString());
					 configWriter.WriteLine("MinBreakTime="+SettingsFunky.MinBreakTime.ToString());
					 configWriter.WriteLine("MaxBreakTime="+SettingsFunky.MaxBreakTime.ToString());
					 configWriter.WriteLine("breakTimeHour="+SettingsFunky.breakTimeHour.ToString());
					 configWriter.WriteLine("ShrineRange="+SettingsFunky.ShrineRange.ToString());

					 configWriter.WriteLine("ItemRuleUseItemIDs="+SettingsFunky.ItemRuleUseItemIDs.ToString());
					 configWriter.WriteLine("ItemRuleDebug="+SettingsFunky.ItemRuleDebug.ToString());
					 configWriter.WriteLine("ItemRuleType="+SettingsFunky.ItemRuleType.ToString());
					 configWriter.WriteLine("ItemRuleLogPickup="+SettingsFunky.ItemRuleLogPickup);
					 configWriter.WriteLine("ItemRuleLogKeep="+SettingsFunky.ItemRuleLogKeep);
					 configWriter.WriteLine("ItemRuleGilesScoring="+SettingsFunky.ItemRuleGilesScoring.ToString());
					 configWriter.WriteLine("UseLevelingLogic="+SettingsFunky.UseLevelingLogic.ToString());

					 configWriter.WriteLine("AttemptAvoidanceMovements="+SettingsFunky.AttemptAvoidanceMovements.ToString());
					 configWriter.WriteLine("UseAdvancedProjectileTesting="+SettingsFunky.UseAdvancedProjectileTesting.ToString());

					 configWriter.WriteLine("KiteDistance="+SettingsFunky.KiteDistance.ToString());
					 configWriter.WriteLine("DestructibleRange="+SettingsFunky.DestructibleRange.ToString());

					 configWriter.WriteLine("GlobeHealthPercent="+SettingsFunky.GlobeHealthPercent.ToString());
					 configWriter.WriteLine("PotionHealthPercent="+SettingsFunky.PotionHealthPercent.ToString());

					 configWriter.WriteLine("IgnoreCombatRange="+SettingsFunky.IgnoreCombatRange.ToString());
					 configWriter.WriteLine("IgnoreLootRange="+SettingsFunky.IgnoreLootRange.ToString());
					 configWriter.WriteLine("ItemRange="+SettingsFunky.ItemRange.ToString());
					 configWriter.WriteLine("ContainerOpenRange="+SettingsFunky.ContainerOpenRange.ToString());
					 configWriter.WriteLine("NonEliteCombatRange="+SettingsFunky.NonEliteCombatRange.ToString());
					 configWriter.WriteLine("IgnoreCorpses="+SettingsFunky.IgnoreCorpses.ToString());
					 configWriter.WriteLine("IgnoreAboveAverageMobs="+SettingsFunky.IgnoreAboveAverageMobs.ToString());
					 configWriter.WriteLine("GoblinPriority="+SettingsFunky.GoblinPriority.ToString());
					 configWriter.WriteLine("AfterCombatDelay="+SettingsFunky.AfterCombatDelay.ToString());
					 configWriter.WriteLine("OutOfCombatMovement="+SettingsFunky.OutOfCombatMovement.ToString());
					 configWriter.WriteLine("EliteCombatRange="+SettingsFunky.EliteCombatRange.ToString());
					 configWriter.WriteLine("ExtendedCombatRange="+SettingsFunky.ExtendedCombatRange.ToString());
					 configWriter.WriteLine("GoldRange="+SettingsFunky.GoldRange.ToString());
					 configWriter.WriteLine("MinimumWeaponItemLevel="+SettingsFunky.MinimumWeaponItemLevel[0].ToString()+","+SettingsFunky.MinimumWeaponItemLevel[1].ToString());
					 configWriter.WriteLine("MinimumArmorItemLevel="+SettingsFunky.MinimumArmorItemLevel[0].ToString()+","+SettingsFunky.MinimumArmorItemLevel[1].ToString());
					 configWriter.WriteLine("MinimumJeweleryItemLevel="+SettingsFunky.MinimumJeweleryItemLevel[0].ToString()+","+SettingsFunky.MinimumJeweleryItemLevel[1].ToString());
					 configWriter.WriteLine("MinimumLegendaryItemLevel="+SettingsFunky.MinimumLegendaryItemLevel.ToString());
					 configWriter.WriteLine("MaximumHealthPotions="+SettingsFunky.MaximumHealthPotions.ToString());
					 configWriter.WriteLine("MinimumGoldPile="+SettingsFunky.MinimumGoldPile.ToString());
					 configWriter.WriteLine("MinimumGemItemLevel="+SettingsFunky.MinimumGemItemLevel.ToString());
					 configWriter.WriteLine("PickupGems="+SettingsFunky.PickupGems[0].ToString()+","+SettingsFunky.PickupGems[1].ToString()+","+SettingsFunky.PickupGems[2].ToString()+","+SettingsFunky.PickupGems[3].ToString());
					 configWriter.WriteLine("PickupCraftTomes="+SettingsFunky.PickupCraftTomes.ToString());
					 configWriter.WriteLine("PickupCraftPlans="+SettingsFunky.PickupCraftPlans.ToString());
					 configWriter.WriteLine("PickupFollowerItems="+SettingsFunky.PickupFollowerItems.ToString());
					 configWriter.WriteLine("MiscItemLevel="+SettingsFunky.MiscItemLevel.ToString());
					 configWriter.WriteLine("GilesMinimumWeaponScore="+SettingsFunky.GilesMinimumWeaponScore.ToString());
					 configWriter.WriteLine("GilesMinimumArmorScore="+SettingsFunky.GilesMinimumArmorScore.ToString());
					 configWriter.WriteLine("GilesMinimumJeweleryScore="+SettingsFunky.GilesMinimumJeweleryScore.ToString());

					 configWriter.WriteLine("AvoidanceRetryMin="+SettingsFunky.AvoidanceRecheckMinimumRate);
					 configWriter.WriteLine("AvoidanceRetryMax="+SettingsFunky.AvoidanceRecheckMaximumRate);
					 configWriter.WriteLine("KiteRetryMin="+SettingsFunky.KitingRecheckMinimumRate);
					 configWriter.WriteLine("KiteRetryMax="+SettingsFunky.KitingRecheckMaximumRate);
					 configWriter.WriteLine("ItemRulesSalvaging="+SettingsFunky.ItemRulesSalvaging);
					 configWriter.WriteLine("DebugStatusBar="+SettingsFunky.DebugStatusBar);
					 configWriter.WriteLine("LogSafeMovementOutput="+SettingsFunky.LogSafeMovementOutput);

					 configWriter.WriteLine("EnableClusteringTargetLogic="+SettingsFunky.EnableClusteringTargetLogic.ToString());
					 configWriter.WriteLine("ClusterDistance="+SettingsFunky.ClusterDistance.ToString());
					 configWriter.WriteLine("ClusterMinimumUnitCount="+SettingsFunky.ClusterMinimumUnitCount.ToString());
					 configWriter.WriteLine("ClusterKillLowHPUnits="+SettingsFunky.ClusterKillLowHPUnits.ToString());
					 configWriter.WriteLine("IgnoreClusteringWhenLowHP="+SettingsFunky.IgnoreClusteringWhenLowHP.ToString());
					 configWriter.WriteLine("IgnoreClusterLowHPValue="+SettingsFunky.IgnoreClusterLowHPValue.ToString());
					 configWriter.WriteLine("TreasureGoblinRange="+SettingsFunky.TreasureGoblinRange.ToString());


					 //GoldRange
					 switch (ActorClass)
					 {
						  case Zeta.Internals.Actors.ActorClass.Barbarian:
								configWriter.WriteLine("bSelectiveWhirlwind="+SettingsFunky.Class.bSelectiveWhirlwind.ToString());
								configWriter.WriteLine("bWaitForWrath="+SettingsFunky.Class.bWaitForWrath.ToString());
								configWriter.WriteLine("bGoblinWrath="+SettingsFunky.Class.bGoblinWrath.ToString());
								configWriter.WriteLine("bFuryDumpWrath="+SettingsFunky.Class.bFuryDumpWrath.ToString());
								configWriter.WriteLine("bFuryDumpWrath="+SettingsFunky.Class.bFuryDumpAlways.ToString());
								break;
						  case Zeta.Internals.Actors.ActorClass.DemonHunter:
								configWriter.WriteLine("iDHVaultMovementDelay="+SettingsFunky.Class.iDHVaultMovementDelay.ToString());
								configWriter.WriteLine("GoblinMinimumRange="+SettingsFunky.Class.GoblinMinimumRange.ToString());
								break;
						  case Zeta.Internals.Actors.ActorClass.Monk:
								configWriter.WriteLine("bMonkInnaSet="+SettingsFunky.Class.bMonkInnaSet.ToString());
								break;
						  case Zeta.Internals.Actors.ActorClass.WitchDoctor:
								configWriter.WriteLine("bEnableCriticalMass="+SettingsFunky.Class.bEnableCriticalMass.ToString());
								configWriter.WriteLine("GoblinMinimumRange="+SettingsFunky.Class.GoblinMinimumRange.ToString());
								break;
						  case Zeta.Internals.Actors.ActorClass.Wizard:
								configWriter.WriteLine("bEnableCriticalMass="+SettingsFunky.Class.bEnableCriticalMass.ToString());
								configWriter.WriteLine("bWaitForArchon="+SettingsFunky.Class.bWaitForArchon.ToString());
								configWriter.WriteLine("bKiteOnlyArchon="+SettingsFunky.Class.bKiteOnlyArchon.ToString());
								configWriter.WriteLine("GoblinMinimumRange="+SettingsFunky.Class.GoblinMinimumRange.ToString());
								break;
					 }

					 Dictionary<AvoidanceType, double> currentDictionaryAvoidance=ReturnDictionaryUsingActorClass(ActorClass);
					 //Avoidances..
					 foreach (AvoidanceType item in currentDictionaryAvoidance.Keys)
					 {
						  configWriter.WriteLine(item.ToString()+"_radius="+dictAvoidanceRadius[item].ToString());
						  configWriter.WriteLine(item.ToString()+"_health="+currentDictionaryAvoidance[item].ToString());
					 }

				}
				//configStream.Close();
		  }

		  private static void LoadFunkyConfiguration()
		  {

				if (CurrentAccountName==null)
				{
					 UpdateCurrentAccountDetails();
				}

				string sFunkyCharacterFolder=Path.Combine(FolderPaths.sDemonBuddyPath, "Settings", "FunkyTrinity", CurrentAccountName);
				if (!System.IO.Directory.Exists(sFunkyCharacterFolder))
					 System.IO.Directory.CreateDirectory(sFunkyCharacterFolder);

				string sFunkyCharacterConfigFile=Path.Combine(sFunkyCharacterFolder, CurrentHeroName+".cfg");

				//Check for Config file
				if (!File.Exists(sFunkyCharacterConfigFile))
				{
					 Log("No config file found, now creating a new config from defaults at: "+sFunkyCharacterConfigFile);
					 SettingsFunky=new Settings_Funky(false, false, false, false, false, false, false, 4, 8, 3, 1.5d, true, 20, false, false, "hard", "Rare", "Rare", true, false, 0, 5, 15, 20, 0.5d, 0.5d, false, 2, 500, false, 50, 30, 40, new int[1], new int[1], new int[1], 1, 100, 300, new bool[3], 60, true, true, true, 59, false, 70000, 16000, 15000, false, false);
					 SaveFunkyConfiguration();
				}

				string[] splitValue;

				//Load File
				using (StreamReader configReader=new StreamReader(sFunkyCharacterConfigFile))
				{
					 while (!configReader.EndOfStream)
					 {
						  string[] config=configReader.ReadLine().Split('=');
						  if (config!=null)
						  {
								//Check if its an avoidance..
								if (config[0].Contains("_"))
								{
									 string[] avoidstr=config[0].Split('_');
									 AvoidanceType avoidType=(AvoidanceType)Enum.Parse(typeof(AvoidanceType), avoidstr[0]);
									 if (avoidstr[1].Contains("health"))
									 {
										  double health=0d;
										  try
										  {
												health=Convert.ToDouble(String.Format(config[1], "F2", CultureInfo.InvariantCulture));
										  } catch
										  {
												Logging.Write("Exception converting to Double At Avoidance Health {0}", avoidType.ToString());
										  }

										  switch (ActorClass)
										  {
												case Zeta.Internals.Actors.ActorClass.Barbarian:
													 dictAvoidanceHealthBarb[avoidType]=health;
													 break;
												case Zeta.Internals.Actors.ActorClass.DemonHunter:
													 dictAvoidanceHealthDemon[avoidType]=health;
													 break;
												case Zeta.Internals.Actors.ActorClass.Monk:
													 dictAvoidanceHealthMonk[avoidType]=health;
													 break;
												case Zeta.Internals.Actors.ActorClass.WitchDoctor:
													 dictAvoidanceHealthWitch[avoidType]=health;
													 break;
												case Zeta.Internals.Actors.ActorClass.Wizard:
													 dictAvoidanceHealthWizard[avoidType]=health;
													 break;
										  }
									 }
									 else
									 {
										  dictAvoidanceRadius[avoidType]=Convert.ToSingle(config[1]);
									 }
								}
								else
								{
									 switch (config[0])
									 {
										  case "OOCIdentifyItems":
												SettingsFunky.OOCIdentifyItems=Convert.ToBoolean(config[1]);
												break;
										  case "BuyPotionsDuringTownRun":
												SettingsFunky.BuyPotionsDuringTownRun=Convert.ToBoolean(config[1]);
												break;
										  case "EnableWaitAfterContainers":
												SettingsFunky.EnableWaitAfterContainers=Convert.ToBoolean(config[1]);
												break;
										  case "UseItemRules":
												SettingsFunky.UseItemRules=Convert.ToBoolean(config[1]);
												break;
										  case "UseItemRulesPickup":
												SettingsFunky.UseItemRulesPickup=Convert.ToBoolean(config[1]);
												break;
										  case "UseExtendedRangeRepChest":
												SettingsFunky.UseExtendedRangeRepChest=Convert.ToBoolean(config[1]);
												break;
										  case "EnableCoffeeBreaks":
												SettingsFunky.EnableCoffeeBreaks=Convert.ToBoolean(config[1]);
												break;
										  case "MinBreakTime":
												SettingsFunky.MinBreakTime=Convert.ToInt32(config[1]);
												break;
										  case "MaxBreakTime":
												SettingsFunky.MaxBreakTime=Convert.ToInt32(config[1]);
												break;
										  case "OOCIdentifyItemsMinimumRequired":
												SettingsFunky.OOCIdentifyItemsMinimumRequired=Convert.ToInt32(config[1]);
												break;
										  case "breakTimeHour":
												try
												{
													 SettingsFunky.breakTimeHour=Convert.ToDouble(String.Format(config[1], "F2", CultureInfo.InvariantCulture));
												} catch
												{
													 Logging.Write("Exception converting breakTimeHour to Double");
												}
												break;
										  case "ShrineRange":
												SettingsFunky.ShrineRange=Convert.ToInt16(config[1]);
												break;
										  case "ItemRuleUseItemIDs":
												SettingsFunky.ItemRuleUseItemIDs=Convert.ToBoolean(config[1]);
												break;
										  case "ItemRuleDebug":
												SettingsFunky.ItemRuleDebug=Convert.ToBoolean(config[1]);
												break;
										  case "ItemRuleType":
												SettingsFunky.ItemRuleType=Convert.ToString(config[1]);
												break;
										  case "ItemRuleLogKeep":
												SettingsFunky.ItemRuleLogKeep=Convert.ToString(config[1]);
												break;
										  case "ItemRuleLogPickup":
												SettingsFunky.ItemRuleLogPickup=Convert.ToString(config[1]);
												break;
										  case "ItemRuleGilesScoring":
												SettingsFunky.ItemRuleGilesScoring=Convert.ToBoolean(config[1]);
												break;
										  case "AttemptAvoidanceMovements":
												SettingsFunky.AttemptAvoidanceMovements=Convert.ToBoolean(config[1]);
												break;
										  case "KiteDistance":
												SettingsFunky.KiteDistance=Convert.ToInt32(config[1]);
												break;
										  case "DestructibleRange":
												SettingsFunky.DestructibleRange=Convert.ToInt32(config[1]);
												break;
										  case "GlobeHealthPercent":
												try
												{
													 SettingsFunky.GlobeHealthPercent=Convert.ToDouble(String.Format(config[1], "F2", CultureInfo.InvariantCulture));
												} catch
												{
													 Logging.Write("Exception converting GlobeHealthPercent to Double");
												}

												break;
										  case "PotionHealthPercent":
												try
												{
													 SettingsFunky.PotionHealthPercent=Convert.ToDouble(String.Format(config[1], "F2", CultureInfo.InvariantCulture));

												} catch
												{
													 Logging.Write("Exception converting PotionHealthPercent to Double");
												}

												break;
										  case "ContainerOpenRange":
												SettingsFunky.ContainerOpenRange=Convert.ToInt32(config[1]);
												break;
										  case "IgnoreCombatRange":
												SettingsFunky.IgnoreCombatRange=Convert.ToBoolean(config[1]);
												break;
										  case "IgnoreLootRange":
												SettingsFunky.IgnoreLootRange=Convert.ToBoolean(config[1]);
												break;
										  case "NonEliteCombatRange":
												SettingsFunky.NonEliteCombatRange=Convert.ToInt32(config[1]);
												break;
										  case "IgnoreCorpses":
												SettingsFunky.IgnoreCorpses=Convert.ToBoolean(config[1]);
												break;
										  case "GoblinPriority":
												SettingsFunky.GoblinPriority=Convert.ToInt32(config[1]);
												break;
										  case "TreasureGoblinRange":
												SettingsFunky.TreasureGoblinRange=Convert.ToInt32(config[1]);
												break;
										  case "AfterCombatDelay":
												SettingsFunky.AfterCombatDelay=Convert.ToInt32(config[1]);
												break;
										  case "OutOfCombatMovement":
												SettingsFunky.OutOfCombatMovement=Convert.ToBoolean(config[1]);
												break;
										  case "bEnableCriticalMass":
												SettingsFunky.Class.bEnableCriticalMass=Convert.ToBoolean(config[1]);
												break;
										  case "bSelectiveWhirlwind":
												SettingsFunky.Class.bSelectiveWhirlwind=Convert.ToBoolean(config[1]);
												break;
										  case "bWaitForWrath":
												SettingsFunky.Class.bWaitForWrath=Convert.ToBoolean(config[1]);
												break;
										  case "bGoblinWrath":
												SettingsFunky.Class.bGoblinWrath=Convert.ToBoolean(config[1]);
												break;
										  case "bFuryDumpWrath":
												SettingsFunky.Class.bFuryDumpWrath=Convert.ToBoolean(config[1]);
												break;
										  case "bFuryDumpAlways":
												SettingsFunky.Class.bFuryDumpAlways=Convert.ToBoolean(config[1]);
												break;
										  case "iDHVaultMovementDelay":
												SettingsFunky.Class.iDHVaultMovementDelay=Convert.ToInt32(config[1]);
												break;
										  case "bMonkInnaSet":
												SettingsFunky.Class.bMonkInnaSet=Convert.ToBoolean(config[1]);
												break;
										  case "bWaitForArchon":
												SettingsFunky.Class.bWaitForArchon=Convert.ToBoolean(config[1]);
												break;
										  case "bKiteOnlyArchon":
												SettingsFunky.Class.bKiteOnlyArchon=Convert.ToBoolean(config[1]);
												break;
										  case "EliteCombatRange":
												SettingsFunky.EliteCombatRange=Convert.ToInt32(config[1]);
												break;
										  case "ExtendedCombatRange":
												SettingsFunky.ExtendedCombatRange=Convert.ToInt32(config[1]);
												break;
										  case "GoldRange":
												SettingsFunky.GoldRange=Convert.ToInt32(config[1]);
												break;
										  case "MinimumWeaponItemLevel":
												splitValue=config[1].Split(',');
												SettingsFunky.MinimumWeaponItemLevel=new int[] { Convert.ToInt16(splitValue[0]), Convert.ToInt16(splitValue[1]) };
												break;
										  case "MinimumArmorItemLevel":
												splitValue=config[1].Split(',');
												SettingsFunky.MinimumArmorItemLevel=new int[] { Convert.ToInt16(splitValue[0]), Convert.ToInt16(splitValue[1]) };
												break;
										  case "MinimumJeweleryItemLevel":
												splitValue=config[1].Split(',');
												SettingsFunky.MinimumJeweleryItemLevel=new int[] { Convert.ToInt16(splitValue[0]), Convert.ToInt16(splitValue[1]) };
												break;
										  case "MinimumLegendaryItemLevel":
												SettingsFunky.MinimumLegendaryItemLevel=Convert.ToInt32(config[1]);
												break;
										  case "MaximumHealthPotions":
												SettingsFunky.MaximumHealthPotions=Convert.ToInt32(config[1]);
												break;
										  case "MinimumGoldPile":
												SettingsFunky.MinimumGoldPile=Convert.ToInt32(config[1]);
												break;
										  case "MinimumGemItemLevel":
												SettingsFunky.MinimumGemItemLevel=Convert.ToInt32(config[1]);
												break;
										  case "PickupCraftTomes":
												SettingsFunky.PickupCraftTomes=Convert.ToBoolean(config[1]);
												break;
										  case "PickupCraftPlans":
												SettingsFunky.PickupCraftPlans=Convert.ToBoolean(config[1]);
												break;
										  case "PickupFollowerItems":
												SettingsFunky.PickupFollowerItems=Convert.ToBoolean(config[1]);
												break;
										  case "MiscItemLevel":
												SettingsFunky.MiscItemLevel=Convert.ToInt32(config[1]);
												break;
										  case "GilesMinimumWeaponScore":
												SettingsFunky.GilesMinimumWeaponScore=Convert.ToInt32(config[1]);
												break;
										  case "GilesMinimumArmorScore":
												SettingsFunky.GilesMinimumArmorScore=Convert.ToInt32(config[1]);
												break;
										  case "GilesMinimumJeweleryScore":
												SettingsFunky.GilesMinimumJeweleryScore=Convert.ToInt32(config[1]);
												break;
										  case "PickupGems":
												splitValue=config[1].Split(',');
												SettingsFunky.PickupGems=new bool[] { Convert.ToBoolean(splitValue[0]), Convert.ToBoolean(splitValue[1]), Convert.ToBoolean(splitValue[2]), Convert.ToBoolean(splitValue[3]) };
												break;
										  case "UseLevelingLogic":
												//UseLevelingLogic
												SettingsFunky.UseLevelingLogic=Convert.ToBoolean(config[1]);
												break;
										  case "UseAdvancedProjectileTesting":
												SettingsFunky.UseAdvancedProjectileTesting=Convert.ToBoolean(config[1]);
												break;
										  case "IgnoreAboveAverageMobs":
												SettingsFunky.IgnoreAboveAverageMobs=Convert.ToBoolean(config[1]);
												break;
										  case "ItemRulesSalvaging":
												SettingsFunky.ItemRulesSalvaging=Convert.ToBoolean(config[1]);
												break;
										  case "AvoidanceRetryMin":
												SettingsFunky.AvoidanceRecheckMinimumRate=Convert.ToInt32(config[1]);
												break;
										  case "AvoidanceRetryMax":
												SettingsFunky.AvoidanceRecheckMaximumRate=Convert.ToInt32(config[1]);
												break;
										  case "KiteRetryMin":
												SettingsFunky.KitingRecheckMinimumRate=Convert.ToInt32(config[1]);
												break;
										  case "KiteRetryMax":
												SettingsFunky.KitingRecheckMaximumRate=Convert.ToInt32(config[1]);
												break;
										  case "DebugStatusBar":
												SettingsFunky.DebugStatusBar=Convert.ToBoolean(config[1]);
												break;
										  case "LogSafeMovementOutput":
												SettingsFunky.LogSafeMovementOutput=Convert.ToBoolean(config[1]);
												break;
										  case "EnableClusteringTargetLogic":
												SettingsFunky.EnableClusteringTargetLogic=Convert.ToBoolean(config[1]);
												break;
										  case "ClusterDistance":
												SettingsFunky.ClusterDistance=Convert.ToInt32(config[1]);
												break;
										  case "ClusterMinimumUnitCount":
												SettingsFunky.ClusterMinimumUnitCount=Convert.ToInt32(config[1]);
												break;
										  case "ClusterKillLowHPUnits":
												SettingsFunky.ClusterKillLowHPUnits=Convert.ToBoolean(config[1]);
												break;
										  case "IgnoreClusteringWhenLowHP":
												SettingsFunky.IgnoreClusteringWhenLowHP=Convert.ToBoolean(config[1]);
												break;
										  case "IgnoreClusterLowHPValue":
												try
												{
													 SettingsFunky.IgnoreClusterLowHPValue=Convert.ToDouble(String.Format(config[1], "F2", CultureInfo.InvariantCulture));
												} catch
												{
													 Logging.Write("Exception converting IgnoreClusterLowHPValue to Double");
												}

												break;
										  case "ItemRange":
												SettingsFunky.ItemRange=Convert.ToInt32(config[1]);
												break;
										  case "GoblinMinimumRange":
												SettingsFunky.Class.GoblinMinimumRange=Convert.ToInt32(config[1]);
												break;
										  //
									 }
								}
						  }

					 }
					 //configReader.Close();
				}

				Zeta.Common.Logging.WriteDiagnostic("[Funky] Character settings loaded!");
		  }

		  public static Settings_Funky SettingsFunky=new Settings_Funky(false, false, false, false, false, false, false, 4, 8, 3, 1.5d, true, 20, false, false, "hard", "Rare", "Rare", true, false, 0, 10, 30, 60, 0.6d, 0.4d, true, 2, 250, false, 60, 30, 40, new int[1], new int[1], new int[1], 1, 100, 300, new bool[3], 60, true, true, true, 59, false, 75000, 25000, 25000, false, false);

		  public class Settings_Funky
		  {
				public bool DebugStatusBar { get; set; }
				public bool LogSafeMovementOutput { get; set; }

				public bool OOCIdentifyItems { get; set; }
				public bool BuyPotionsDuringTownRun { get; set; }
				public bool EnableWaitAfterContainers { get; set; }
				public bool UseItemRulesPickup { get; set; }
				public bool UseItemRules { get; set; }
				public bool UseExtendedRangeRepChest { get; set; }
				public bool EnableCoffeeBreaks { get; set; }
				public int MinBreakTime { get; set; }
				public int MaxBreakTime { get; set; }
				public int OOCIdentifyItemsMinimumRequired { get; set; }
				public double breakTimeHour { get; set; }

				//Character Related
				public bool AttemptAvoidanceMovements { get; set; }
				public bool UseAdvancedProjectileTesting { get; set; }
				public int KiteDistance { get; set; }
				public double GlobeHealthPercent { get; set; }
				public double PotionHealthPercent { get; set; }
				public bool IgnoreCorpses { get; set; }
				public int GoblinPriority { get; set; }
				public int AfterCombatDelay { get; set; }
				public bool IgnoreAboveAverageMobs { get; set; }
				public bool OutOfCombatMovement { get; set; }

				//new additions
				public bool IgnoreCombatRange { get; set; }
				public bool IgnoreLootRange { get; set; }
				public int ItemRange { get; set; }
				public int GoldRange { get; set; }
				public int ShrineRange { get; set; }
				public int DestructibleRange { get; set; }
				public int ContainerOpenRange { get; set; }
				public int NonEliteCombatRange { get; set; }
				public int EliteCombatRange { get; set; }
				public int TreasureGoblinRange { get; set; }
				public int ExtendedCombatRange { get; set; }

				//Item Rules Additions
				public bool ItemRulesSalvaging { get; set; }
				public bool ItemRuleUseItemIDs { get; set; }
				public bool ItemRuleDebug { get; set; }
				public string ItemRuleType { get; set; }
				public string ItemRuleLogPickup { get; set; }
				public string ItemRuleLogKeep { get; set; }
				public bool ItemRuleGilesScoring { get; set; }
				public bool UseLevelingLogic { get; set; }

				//Plugin Item Default Settings
				public int GilesMinimumWeaponScore { get; set; }
				public int GilesMinimumArmorScore { get; set; }
				public int GilesMinimumJeweleryScore { get; set; }

				public int[] MinimumWeaponItemLevel { get; set; }
				public int[] MinimumArmorItemLevel { get; set; }
				public int[] MinimumJeweleryItemLevel { get; set; }
				public int MinimumLegendaryItemLevel { get; set; }

				public int MaximumHealthPotions { get; set; }
				public int MinimumGoldPile { get; set; }

				//red, green, purple, yellow
				public bool[] PickupGems { get; set; }
				public int MinimumGemItemLevel { get; set; }

				public bool PickupCraftTomes { get; set; }
				public bool PickupCraftPlans { get; set; }
				public bool PickupFollowerItems { get; set; }
				public int MiscItemLevel { get; set; }

				public int AvoidanceRecheckMaximumRate { get; set; }//=1250;
				public int AvoidanceRecheckMinimumRate { get; set; }//=500;
				public int KitingRecheckMaximumRate { get; set; }//=4000;
				public int KitingRecheckMinimumRate { get; set; }//=2000;

				public double ClusterDistance { get; set; }
				public int ClusterMinimumUnitCount { get; set; }
				public bool EnableClusteringTargetLogic { get; set; }
				public bool ClusterKillLowHPUnits { get; set; }
				public bool IgnoreClusteringWhenLowHP { get; set; }
				public double IgnoreClusterLowHPValue { get; set; }

				//Class Settings
				public ClassSettings Class { get; set; }

				public Settings_Funky(bool oocIDitems, bool buyPotions, bool WaitForContainers,
					 bool itemRulesPickup, bool itemRules, bool extendRangeRepChest,
					 bool coffeebreak, int minbreak, int maxbreak, int minOOCitems, double breakTime, bool ignoreDestructibles, int ShrineRange
					 , bool itemruleIDs, bool itemruleDebug, string itemruletype, string itemrulekeeplog, string itemrulepickuplog, bool gilesscoring,
					 bool attemptavoidance, int Kite, int destructiblerange, int containerrange, int noneliterange,
					 double globehealth, double potionhealth, bool ignorecorpse, int goblinpriority, int aftercombatdelay, bool outofcombatmovement,
					 int eliterange, int extendedrange, int goldrange, int[] minweaponlevel, int[] minarmorlevel, int[] minjewelerylevel, int minlegendarylevel,
					 int maxhealthpots, int mingoldpile, bool[] gems, int minGemLevel, bool craftTomes, bool craftPlans, bool Followeritems, int miscitemlevel, bool itemlevelinglogic, int gilesWeaponScore, int gilesArmorScore, int gilesJeweleryScore, bool projectiletesting, bool ignoreelites)
				{
					 AvoidanceRecheckMaximumRate=1000;
					 AvoidanceRecheckMinimumRate=250;
					 KitingRecheckMaximumRate=2500;
					 KitingRecheckMinimumRate=750;
					 ItemRulesSalvaging=true;
					 OOCIdentifyItems=oocIDitems;
					 BuyPotionsDuringTownRun=buyPotions;
					 EnableWaitAfterContainers=WaitForContainers;
					 UseItemRulesPickup=itemRulesPickup;
					 UseItemRules=itemRules;
					 UseExtendedRangeRepChest=extendRangeRepChest;
					 EnableCoffeeBreaks=coffeebreak;
					 MaxBreakTime=minbreak;
					 MinBreakTime=maxbreak;
					 OOCIdentifyItemsMinimumRequired=minOOCitems;
					 breakTimeHour=breakTime;
					 ItemRuleDebug=itemruleDebug;
					 ItemRuleUseItemIDs=itemruleIDs;
					 ItemRuleType=itemruletype;
					 ItemRuleLogKeep=itemrulekeeplog;
					 ItemRuleLogPickup=itemrulepickuplog;
					 ItemRuleGilesScoring=gilesscoring;
					 AttemptAvoidanceMovements=attemptavoidance;
					 KiteDistance=Kite;
					 IgnoreCombatRange=false;
					 IgnoreLootRange=false;
					 DestructibleRange=destructiblerange;
					 ContainerOpenRange=containerrange;
					 NonEliteCombatRange=noneliterange;
					 GlobeHealthPercent=globehealth;
					 PotionHealthPercent=potionhealth;
					 IgnoreCorpses=ignorecorpse;
					 GoblinPriority=goblinpriority;
					 AfterCombatDelay=aftercombatdelay;
					 OutOfCombatMovement=outofcombatmovement;
					 EliteCombatRange=eliterange;
					 ExtendedCombatRange=extendedrange;
					 GoldRange=goldrange;
					 ItemRange=50;
					 TreasureGoblinRange=40;
					 ShrineRange=30;
					 MinimumWeaponItemLevel=new int[] { 0, 55 };
					 MinimumArmorItemLevel=new int[] { 0, 55 };
					 MinimumJeweleryItemLevel=new int[] { 0, 55 };
					 GilesMinimumWeaponScore=gilesWeaponScore;
					 GilesMinimumArmorScore=gilesArmorScore;
					 GilesMinimumJeweleryScore=gilesJeweleryScore;

					 MinimumLegendaryItemLevel=minlegendarylevel;
					 MaximumHealthPotions=maxhealthpots;
					 MinimumGoldPile=mingoldpile;

					 PickupGems=new bool[] { true, true, true, true };

					 MinimumGemItemLevel=minGemLevel;
					 PickupCraftTomes=craftTomes;
					 PickupCraftPlans=craftPlans;
					 PickupFollowerItems=Followeritems;
					 MiscItemLevel=miscitemlevel;
					 UseLevelingLogic=itemlevelinglogic;
					 UseAdvancedProjectileTesting=projectiletesting;
					 IgnoreAboveAverageMobs=ignoreelites;
					 DebugStatusBar=false;
					 LogSafeMovementOutput=false;


					 EnableClusteringTargetLogic=true;
					 IgnoreClusteringWhenLowHP=true;
					 ClusterKillLowHPUnits=true;
					 ClusterDistance=7d;
					 ClusterMinimumUnitCount=3;
					 IgnoreClusterLowHPValue=0.65d;

					 Class=new ClassSettings();
				}


				public class ClassSettings
				{
					 //barb
					 public bool bSelectiveWhirlwind { get; set; }
					 public bool bWaitForWrath { get; set; }
					 public bool bGoblinWrath { get; set; }
					 public bool bFuryDumpWrath { get; set; }
					 public bool bFuryDumpAlways { get; set; }

					 //DH
					 public int iDHVaultMovementDelay { get; set; }

					 //Monk
					 public bool bMonkInnaSet { get; set; }

					 //Wiz
					 public bool bWaitForArchon { get; set; }
					 public bool bKiteOnlyArchon { get; set; }

					 //WD+Wiz
					 public bool bEnableCriticalMass { get; set; }

					 //Range Class
					 public int GoblinMinimumRange { get; set; }

					 public ClassSettings()
					 {
						  bEnableCriticalMass=false;
						  bSelectiveWhirlwind=false;
						  bWaitForWrath=false;
						  bGoblinWrath=false;
						  bFuryDumpWrath=false;
						  bFuryDumpAlways=false;
						  iDHVaultMovementDelay=400;
						  bMonkInnaSet=false;
						  bWaitForArchon=false;
						  bKiteOnlyArchon=false;
						  GoblinMinimumRange=30;
					 }
				}
		  }
	 }
}