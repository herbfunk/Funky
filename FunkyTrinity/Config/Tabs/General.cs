using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FunkyTrinity.Cache.Enums;


namespace FunkyTrinity
{
	 internal partial class FunkyWindow : Window
	 {
		  #region EventHandling
		  private void OutOfCombatMovementChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.OutOfCombatMovement=!Bot.SettingsFunky.OutOfCombatMovement;
		  }
		  private void AllowBuffingInTownChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.AllowBuffingInTown=!Bot.SettingsFunky.AllowBuffingInTown;
		  }
		  private void AfterCombatDelaySliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				int Value=(int)slider_sender.Value;
				Bot.SettingsFunky.AfterCombatDelay=Value;
				TBAfterCombatDelay.Text=Value.ToString();
		  }
		  private void BotStopHPValueSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				double Value=Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
				Bot.SettingsFunky.StopGameOnBotHealthPercent=Value;
				TBBotStopHealthPercent.Text=Value.ToString();
		  }
		  private void OOCIDChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.OOCIdentifyItems=!Bot.SettingsFunky.OOCIdentifyItems;
		  }
		  private void BuyPotionsDuringTownRunChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.BuyPotionsDuringTownRun=!Bot.SettingsFunky.BuyPotionsDuringTownRun;
		  }
		  private void EnableWaitAfterContainersChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.EnableWaitAfterContainers=!Bot.SettingsFunky.EnableWaitAfterContainers;
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

		  private void EnableCoffeeBreaksChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.EnableCoffeeBreaks=!Bot.SettingsFunky.EnableCoffeeBreaks;
		  }
		  private void BreakMinMinutesSliderChange(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				int Value=(int)slider_sender.Value;
				Bot.SettingsFunky.MinBreakTime=Value;
				tbMinBreakTime.Text=Value.ToString();
		  }
		  private void BreakMaxMinutesSliderChange(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				int Value=(int)slider_sender.Value;
				Bot.SettingsFunky.MaxBreakTime=Value;
				tbMaxBreakTime.Text=Value.ToString();
		  }

		  #region OOCIDItemTextBox
		  private void OOCIdentifyItemsMinValueChanged(object sender, EventArgs e)
		  {
				string lastText=OOCIdentfyItemsMinCount.Text;
				if (isStringFullyNumerical(lastText))
				{
					 Bot.SettingsFunky.OOCIdentifyItemsMinimumRequired=Convert.ToInt32(lastText);
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
				Bot.SettingsFunky.breakTimeHour=Value;
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
					 System.Text.RegularExpressions.Regex isnumber=new System.Text.RegularExpressions.Regex(@"^[0-9]+(\.[0-9]+)?$");
					 return isnumber.IsMatch(S);
				}
		  }

		  private void EnableDemonBuddySettingsChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Demonbuddy.EnableDemonBuddyCharacterSettings=!Bot.SettingsFunky.Demonbuddy.EnableDemonBuddyCharacterSettings;
		  }
		  private void DemonBuddyMonsterPowerSliderChange(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				int Value=(int)slider_sender.Value;
				Bot.SettingsFunky.Demonbuddy.MonsterPower=Value;
				tbMonsterPower.Text=Value.ToString();
		  }
		  #endregion

		  private CheckBox CoffeeBreaks;
		  private TextBox tbMinBreakTime, tbMaxBreakTime;
		  private TextBox tbMonsterPower;
		  private CheckBox OOCIdentifyItems;
		  private TextBox OOCIdentfyItemsMinCount;
		  private CheckBox BuyPotionsDuringTownRunCB;
		  private CheckBox EnableWaitAfterContainersCB;
		  private StackPanel spBotStop;
		  private TextBox TBBreakTimeHour, TBBotStopHealthPercent, TBAfterCombatDelay;

		  internal void InitGeneralControls()
		  {
				TabItem GeneralTab=new TabItem();
				GeneralTab.Header="General";
				tcGeneral.Items.Add(GeneralTab);
				lbGeneralContent=new ListBox();

				#region OOCItemBehavior
				StackPanel OOCItemBehaviorStackPanel=new StackPanel
				{
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
					 Background=System.Windows.Media.Brushes.DimGray,
				};
				TextBlock OOCItemBehavior_Header_Text=new TextBlock
				{
					 Text="Out-Of-Combat Item Idenification",
					 FontSize=13,
					 Background=System.Windows.Media.Brushes.LightSeaGreen,
					 TextAlignment=TextAlignment.Left,
				};
				TextBlock OOCItemBehavior_Header_Info=new TextBlock
				{
					 Text="Behavior Preforms idenfication of items (Individual IDing) when unid count is surpassed",
					 FontSize=11,
					 FontStyle=FontStyles.Italic,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 TextAlignment=TextAlignment.Left,
				};

				#region OOC_ID_Items
				OOCIdentifyItems=new CheckBox
				{
					 Content="Enable Out Of Combat Idenification Behavior",
					 IsChecked=(Bot.SettingsFunky.OOCIdentifyItems),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,

				};
				OOCIdentifyItems.Checked+=OOCIDChecked;
				OOCIdentifyItems.Unchecked+=OOCIDChecked;
				#endregion
				TextBlock OOCItemBehavior_MinItem_Text=new TextBlock
				{
					 Text="Minimum Unid Items",
					 FontSize=13,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 TextAlignment=TextAlignment.Left,
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
				};

				#region OOC_Min_Item_Count

				OOCIdentfyItemsMinCount=new TextBox
				{
					 Text=Bot.SettingsFunky.OOCIdentifyItemsMinimumRequired.ToString(),
					 Width=100,
					 Height=25
				};
				OOCIdentfyItemsMinCount.KeyUp+=OOCMinimumItems_KeyUp;
				OOCIdentfyItemsMinCount.TextChanged+=OOCIdentifyItemsMinValueChanged;

				#endregion


				OOCItemBehaviorStackPanel.Children.Add(OOCItemBehavior_Header_Text);
				OOCItemBehaviorStackPanel.Children.Add(OOCIdentifyItems);
				OOCItemBehaviorStackPanel.Children.Add(OOCItemBehavior_Header_Info);
				OOCItemBehaviorStackPanel.Children.Add(OOCItemBehavior_MinItem_Text);
				OOCItemBehaviorStackPanel.Children.Add(OOCIdentfyItemsMinCount);
				lbGeneralContent.Items.Add(OOCItemBehaviorStackPanel);
				#endregion

				#region Bot Stop Feature
				StackPanel spBotStopLowHP=new StackPanel
				{
					 Orientation=Orientation.Vertical,
					 Background=System.Windows.Media.Brushes.DimGray,
				};
				TextBlock BotStop_Text_Header=new TextBlock
				{
					 Text="Emergency Bot Health Stopping",
					 FontSize=12,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 Background=System.Windows.Media.Brushes.IndianRed,
				};
				spBotStopLowHP.Children.Add(BotStop_Text_Header);

				#region StopGameOnBotLowHealth
				CheckBox CBStopGameOnBotLowHealth=new CheckBox
				{
					 Content="Enable Bot Stop Behavior",
					 Width=300,
					 Height=30,
					 IsChecked=(Bot.SettingsFunky.StopGameOnBotLowHealth),
				};
				CBStopGameOnBotLowHealth.Checked+=StopGameOnBotLowHealthChecked;
				CBStopGameOnBotLowHealth.Unchecked+=StopGameOnBotLowHealthChecked;
				spBotStopLowHP.Children.Add(CBStopGameOnBotLowHealth);
				#endregion

				#region StopBotOnLowHealth--Slider
				spBotStop=new StackPanel
				{
					 IsEnabled=Bot.SettingsFunky.StopGameOnBotLowHealth,
				};
				TextBlock BotStopLowHP_Text_Header=new TextBlock
				{
					 Text="Bot Stop Health Percent",
					 FontSize=12,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 //Background=System.Windows.Media.Brushes.MediumSeaGreen,
				};
				spBotStop.Children.Add(BotStopLowHP_Text_Header);

				Slider sliderBotStopLowHPValue=new Slider
				{
					 Width=100,
					 Maximum=1,
					 Minimum=0,
					 TickFrequency=0.25,
					 LargeChange=0.25,
					 SmallChange=0.10,
					 Value=Bot.SettingsFunky.StopGameOnBotHealthPercent,
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
				};
				sliderBotStopLowHPValue.ValueChanged+=BotStopHPValueSliderChanged;
				TBBotStopHealthPercent=new TextBox
				{
					 Text=Bot.SettingsFunky.StopGameOnBotHealthPercent.ToString("F2", CultureInfo.InvariantCulture),
					 IsReadOnly=true,
				};
				StackPanel BotStopHPValueStackPanel=new StackPanel
				{
					 Orientation=Orientation.Horizontal,
				};
				BotStopHPValueStackPanel.Children.Add(sliderBotStopLowHPValue);
				BotStopHPValueStackPanel.Children.Add(TBBotStopHealthPercent);
				spBotStop.Children.Add(BotStopHPValueStackPanel);
				#endregion

				#region StopGameOnBotLowHealth-ScreenShot
				CheckBox CBStopGameOnBotEnableScreenShot=new CheckBox
				{
					 Content="Take A Screenshot before stopping",
					 Width=300,
					 Height=30,
					 IsChecked=(Bot.SettingsFunky.StopGameOnBotEnableScreenShot),
				};
				CBStopGameOnBotEnableScreenShot.Checked+=StopGameOnBotEnableScreenShotChecked;
				CBStopGameOnBotEnableScreenShot.Unchecked+=StopGameOnBotEnableScreenShotChecked;
				spBotStop.Children.Add(CBStopGameOnBotEnableScreenShot);
				#endregion

				spBotStopLowHP.Children.Add(spBotStop);
				lbGeneralContent.Items.Add(spBotStopLowHP);
				#endregion

				#region LevelingLogic
				ToolTip TTLevelingLogic=new System.Windows.Controls.ToolTip
				{
					 Content="Enables auto-equipping of items, abilities. This overrides default item loot settings.",
				};
				CheckBox LevelingLogic=new CheckBox
				{
					 Content="Leveling Item Logic",
					 Width=300,
					 Height=30,
					 IsChecked=(Bot.SettingsFunky.UseLevelingLogic),
					 ToolTip=TTLevelingLogic,
				};
				LevelingLogic.Checked+=ItemLevelingLogicChecked;
				LevelingLogic.Unchecked+=ItemLevelingLogicChecked;
				lbGeneralContent.Items.Add(LevelingLogic);
				#endregion

				#region PotionsDuringTownRun
				BuyPotionsDuringTownRunCB=new CheckBox
				{
					 Content="Buy Potions During Town Run (Uses Maximum Potion Count Setting)",
					 Width=500,
					 Height=30,
					 IsChecked=(Bot.SettingsFunky.BuyPotionsDuringTownRun)
				};
				BuyPotionsDuringTownRunCB.Checked+=BuyPotionsDuringTownRunChecked;
				BuyPotionsDuringTownRunCB.Unchecked+=BuyPotionsDuringTownRunChecked;
				lbGeneralContent.Items.Add(BuyPotionsDuringTownRunCB);
				#endregion

				#region OutOfCombatMovement
				CheckBox cbOutOfCombatMovement=new CheckBox
				{
					 Content="Use Out Of Combat ability Movements",
					 Width=300,
					 Height=30,
					 IsChecked=(Bot.SettingsFunky.OutOfCombatMovement)
				};
				cbOutOfCombatMovement.Checked+=OutOfCombatMovementChecked;
				cbOutOfCombatMovement.Unchecked+=OutOfCombatMovementChecked;
				lbGeneralContent.Items.Add(cbOutOfCombatMovement);
				#endregion

				#region AllowBuffingInTown
				CheckBox cbAllowBuffingInTown=new CheckBox
				{
					 Content="Allow Buffing In Town",
					 Width=300,
					 Height=30,
					 IsChecked=(Bot.SettingsFunky.AllowBuffingInTown)
				};
				cbAllowBuffingInTown.Checked+=AllowBuffingInTownChecked;
				cbAllowBuffingInTown.Unchecked+=AllowBuffingInTownChecked;
				lbGeneralContent.Items.Add(cbAllowBuffingInTown);
				#endregion

				#region AfterCombatDelayOptions
				StackPanel AfterCombatDelayStackPanel=new StackPanel();
				#region AfterCombatDelay

				Slider sliderAfterCombatDelay=new Slider
				{
					 Width=100,
					 Maximum=2000,
					 Minimum=0,
					 TickFrequency=200,
					 LargeChange=100,
					 SmallChange=50,
					 Value=Bot.SettingsFunky.AfterCombatDelay,
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
				};
				sliderAfterCombatDelay.ValueChanged+=AfterCombatDelaySliderChanged;
				TBAfterCombatDelay=new TextBox
				{
					 Margin=new Thickness(Margin.Left+5, Margin.Top, Margin.Right, Margin.Bottom),
					 Text=Bot.SettingsFunky.AfterCombatDelay.ToString(),
					 IsReadOnly=true,
				};
				StackPanel AfterCombatStackPanel=new StackPanel
				{
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
					 Orientation=Orientation.Horizontal,
				};
				AfterCombatStackPanel.Children.Add(sliderAfterCombatDelay);
				AfterCombatStackPanel.Children.Add(TBAfterCombatDelay);

				#endregion
				#region WaitTimerAfterContainers
				EnableWaitAfterContainersCB=new CheckBox
				{
					 Content="Apply Delay After Opening Containers",
					 Width=300,
					 Height=20,
					 IsChecked=(Bot.SettingsFunky.EnableWaitAfterContainers)
				};
				EnableWaitAfterContainersCB.Checked+=EnableWaitAfterContainersChecked;
				EnableWaitAfterContainersCB.Unchecked+=EnableWaitAfterContainersChecked;

				#endregion

				TextBlock CombatLootDelay_Text_Info=new TextBlock
				{
					 Text="End of Combat Delay Timer",
					 FontSize=11,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 TextAlignment=TextAlignment.Left,
				};
				AfterCombatDelayStackPanel.Children.Add(CombatLootDelay_Text_Info);
				AfterCombatDelayStackPanel.Children.Add(AfterCombatStackPanel);
				AfterCombatDelayStackPanel.Children.Add(EnableWaitAfterContainersCB);
				lbGeneralContent.Items.Add(AfterCombatDelayStackPanel);

				#endregion


				StackPanel spShrinePanel=new StackPanel();
				TextBlock Shrines_Header_Text=new TextBlock
				{
					 Text="Shrines",
					 FontSize=13,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
					 TextAlignment=TextAlignment.Left,
				};
				spShrinePanel.Children.Add(Shrines_Header_Text);
				StackPanel spShrineUseOptions=new StackPanel
				{
					 Orientation=Orientation.Horizontal,
				};
				CheckBox[] cbUseShrine=new CheckBox[6];
				string[] ShrineNames=Enum.GetNames(typeof(ShrineTypes));
				for (int i=0; i<6; i++)
				{
					 cbUseShrine[i]=new CheckBox
					 {
						  Content=ShrineNames[i],
						  Name=ShrineNames[i],
						  IsChecked=Bot.SettingsFunky.Targeting.UseShrineTypes[i],
						  Margin=new Thickness(Margin.Left+3, Margin.Top, Margin.Right, Margin.Bottom+5),
					 };
					 cbUseShrine[i].Checked+=UseShrineChecked;
					 cbUseShrine[i].Unchecked+=UseShrineChecked;
					 spShrineUseOptions.Children.Add(cbUseShrine[i]);
				}
				spShrinePanel.Children.Add(spShrineUseOptions);

				lbGeneralContent.Items.Add(spShrinePanel);

				GeneralTab.Content=lbGeneralContent;

				#region CoffeeBreaks
				TabItem CoffeeBreakTab=new TabItem();
				CoffeeBreakTab.Header="Coffee Breaks";
				tcGeneral.Items.Add(CoffeeBreakTab);
				ListBox LBCoffeebreak=new ListBox();

				StackPanel CoffeeBreaksStackPanel=new StackPanel
				{
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
					 Orientation=Orientation.Vertical,

				};

				TextBlock CoffeeBreaks_Header_Text=new TextBlock
				{
					 Text="Coffee Breaks",
					 FontSize=13,
					 Background=System.Windows.Media.Brushes.LightSeaGreen,
					 TextAlignment=TextAlignment.Center,
				};

				#region CoffeeBreakCheckBox
				CoffeeBreaks=new CheckBox
				{
					 Content="Enable Coffee Breaks",
					 Height=20,
					 IsChecked=(Bot.SettingsFunky.EnableCoffeeBreaks)

				};
				CoffeeBreaks.Checked+=EnableCoffeeBreaksChecked;
				CoffeeBreaks.Unchecked+=EnableCoffeeBreaksChecked;
				#endregion

				TextBlock CoffeeBreak_Minutes_Text=new TextBlock
				{
					 Text="Break time range (Minutues)",
					 FontSize=13,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 TextAlignment=TextAlignment.Left,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
				};

				#region BreakTimeMinMinutes
				TextBlock CoffeeBreaks_Min_Text=new TextBlock
				{
					 Text="Minimum",
					 FontSize=13,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 TextAlignment=TextAlignment.Center,
				};
				Slider sliderBreakMinMinutes=new Slider
				{
					 Width=100,
					 Maximum=20,
					 Minimum=0,
					 TickFrequency=5,
					 LargeChange=2,
					 SmallChange=1,
					 Value=Bot.SettingsFunky.MinBreakTime,
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
					 Height=30,
					 Orientation=Orientation.Horizontal,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
				};
				BreakTimeMinMinutestackPanel.Children.Add(CoffeeBreaks_Min_Text);
				BreakTimeMinMinutestackPanel.Children.Add(sliderBreakMinMinutes);
				BreakTimeMinMinutestackPanel.Children.Add(tbMinBreakTime);

				#endregion

				#region BreakTimeMaxMinutes
				TextBlock CoffeeBreaks_Max_Text=new TextBlock
				{
					 Text="Maximum",
					 FontSize=13,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 TextAlignment=TextAlignment.Center,
				};
				Slider sliderBreakMaxMinutes=new Slider
				{
					 Width=100,
					 Maximum=20,
					 Minimum=0,
					 TickFrequency=5,
					 LargeChange=2,
					 SmallChange=1,
					 Value=Bot.SettingsFunky.MaxBreakTime,
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
					 Height=20,
					 Orientation=Orientation.Horizontal,
					 Margin=new Thickness(Margin.Left+5, Margin.Top, Margin.Right, Margin.Bottom),
				};
				BreakTimeMaxMinutestackPanel.Children.Add(CoffeeBreaks_Max_Text);
				BreakTimeMaxMinutestackPanel.Children.Add(sliderBreakMaxMinutes);
				BreakTimeMaxMinutestackPanel.Children.Add(tbMaxBreakTime);
				#endregion

				StackPanel CoffeeBreakTimeRangeStackPanel=new StackPanel
				{
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
					 Orientation=Orientation.Horizontal,
				};

				CoffeeBreakTimeRangeStackPanel.Children.Add(BreakTimeMinMinutestackPanel);
				CoffeeBreakTimeRangeStackPanel.Children.Add(BreakTimeMaxMinutestackPanel);

				#region BreakTimeIntervalHour


				Slider sliderBreakTimeHour=new Slider
				{
					 Width=200,
					 Maximum=10,
					 Minimum=0,
					 TickFrequency=1,
					 LargeChange=0.50,
					 SmallChange=0.05,
					 Value=Bot.SettingsFunky.breakTimeHour,
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

				#endregion
				TextBlock CoffeeBreakInterval_Text=new TextBlock
				{
					 Text="Break Hour Interval (1 Equals One Hour)",
					 FontSize=13,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 TextAlignment=TextAlignment.Left,
				};

				CoffeeBreaksStackPanel.Children.Add(CoffeeBreaks_Header_Text);
				CoffeeBreaksStackPanel.Children.Add(CoffeeBreaks);
				CoffeeBreaksStackPanel.Children.Add(CoffeeBreak_Minutes_Text);
				CoffeeBreaksStackPanel.Children.Add(CoffeeBreakTimeRangeStackPanel);
				CoffeeBreaksStackPanel.Children.Add(CoffeeBreakInterval_Text);
				CoffeeBreaksStackPanel.Children.Add(BreakTimeHourStackPanel);
				LBCoffeebreak.Items.Add(CoffeeBreaksStackPanel);
				CoffeeBreakTab.Content=LBCoffeebreak;
				#endregion


				#region Demonbuddy
				TabItem DemonbuddyTab=new TabItem();
				DemonbuddyTab.Header="DemonBuddy";
				tcGeneral.Items.Add(DemonbuddyTab);
				ListBox LBDemonbuddy=new ListBox();

				StackPanel DemonbuddyStackPanel=new StackPanel
				{
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
					 Orientation=Orientation.Vertical,
					 HorizontalAlignment= System.Windows.HorizontalAlignment.Stretch,
				};
				TextBlock Demonbuddy_Header_Text=new TextBlock
				{
					 Text="DemonBuddy Settings",
					 FontSize=13,
					 Background=System.Windows.Media.Brushes.LightSeaGreen,
					 TextAlignment=TextAlignment.Center,
					 HorizontalAlignment= System.Windows.HorizontalAlignment.Stretch,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
				};
				DemonbuddyStackPanel.Children.Add(Demonbuddy_Header_Text);

				#region DemonbuddyCheckBox
				CheckBox CBDemonbuddy=new CheckBox
				{
					 Content="Enable Demonbuddy Settings Override",
					 Height=20,
					 IsChecked=(Bot.SettingsFunky.Demonbuddy.EnableDemonBuddyCharacterSettings)
				};
				CBDemonbuddy.Checked+=EnableDemonBuddySettingsChecked;
				CBDemonbuddy.Unchecked+=EnableDemonBuddySettingsChecked;
				DemonbuddyStackPanel.Children.Add(CBDemonbuddy);
				#endregion

				#region Demonbuddy Monsterpower
				TextBlock Demonbuddy_MonsterPower_Text=new TextBlock
				{
					 Text="Monster Power",
					 FontSize=13,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 TextAlignment=TextAlignment.Center,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
				};
				Slider sliderMonsterPower=new Slider
				{
					 Width=100,
					 Maximum=10,
					 Minimum=0,
					 TickFrequency=1,
					 LargeChange=1,
					 SmallChange=1,
					 Value=Bot.SettingsFunky.Demonbuddy.MonsterPower,
					 HorizontalAlignment= System.Windows.HorizontalAlignment.Left,
				};
				sliderMonsterPower.ValueChanged+=DemonBuddyMonsterPowerSliderChange;
				tbMonsterPower=new TextBox
				{
					 Text=sliderMonsterPower.Value.ToString(),
					 IsReadOnly=true,
				};
				StackPanel SPMonsterPower=new StackPanel
				{
					 Height=30,
					 HorizontalAlignment= System.Windows.HorizontalAlignment.Stretch,
					 Orientation=Orientation.Horizontal,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
				};
				SPMonsterPower.Children.Add(Demonbuddy_MonsterPower_Text);
				SPMonsterPower.Children.Add(sliderMonsterPower);
				SPMonsterPower.Children.Add(tbMonsterPower);
				DemonbuddyStackPanel.Children.Add(SPMonsterPower);
				#endregion

				LBDemonbuddy.Items.Add(DemonbuddyStackPanel);
				DemonbuddyTab.Content=LBDemonbuddy;

				#endregion
		  }
	 }
}
