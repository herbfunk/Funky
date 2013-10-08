using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FunkyBot.Cache.Enums;


namespace FunkyBot
{
	 internal partial class FunkyWindow : Window
	 {
		  #region EventHandling
		  private void OutOfCombatMovementChecked(object sender, EventArgs e)
		  {
				Bot.Settings.OutOfCombatMovement=!Bot.Settings.OutOfCombatMovement;
		  }
		  private void AllowBuffingInTownChecked(object sender, EventArgs e)
		  {
				Bot.Settings.AllowBuffingInTown=!Bot.Settings.AllowBuffingInTown;
		  }
		  private void AfterCombatDelaySliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				int Value=(int)slider_sender.Value;
				Bot.Settings.AfterCombatDelay=Value;
				TBAfterCombatDelay.Text=Value.ToString();
		  }
		  private void BuyPotionsDuringTownRunChecked(object sender, EventArgs e)
		  {
				Bot.Settings.BuyPotionsDuringTownRun=!Bot.Settings.BuyPotionsDuringTownRun;
		  }
		  private void EnableWaitAfterContainersChecked(object sender, EventArgs e)
		  {
				Bot.Settings.EnableWaitAfterContainers=!Bot.Settings.EnableWaitAfterContainers;
		  }



		  private void EnableDemonBuddySettingsChecked(object sender, EventArgs e)
		  {
				Bot.Settings.Demonbuddy.EnableDemonBuddyCharacterSettings=!Bot.Settings.Demonbuddy.EnableDemonBuddyCharacterSettings;
		  }
		  private void DemonBuddyMonsterPowerSliderChange(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				int Value=(int)slider_sender.Value;
				Bot.Settings.Demonbuddy.MonsterPower=Value;
				tbMonsterPower.Text=Value.ToString();
		  }
		  #endregion


		  private TextBox tbMonsterPower;
		  private CheckBox BuyPotionsDuringTownRunCB;
		  private CheckBox EnableWaitAfterContainersCB;
		  private TextBox  TBAfterCombatDelay;

		  internal void InitGeneralControls()
		  {
				TabItem GeneralTab=new TabItem();
				GeneralTab.Header="General";
				tcGeneral.Items.Add(GeneralTab);
				lbGeneralContent=new ListBox();

				#region PotionsDuringTownRun
				BuyPotionsDuringTownRunCB=new CheckBox
				{
					 Content="Buy Potions During Town Run (Uses Maximum Potion Count Setting)",
					 Width=500,
					 Height=30,
					 IsChecked=(Bot.Settings.BuyPotionsDuringTownRun)
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
					 IsChecked=(Bot.Settings.OutOfCombatMovement)
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
					 IsChecked=(Bot.Settings.AllowBuffingInTown)
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
					 Value=Bot.Settings.AfterCombatDelay,
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
				};
				sliderAfterCombatDelay.ValueChanged+=AfterCombatDelaySliderChanged;
				TBAfterCombatDelay=new TextBox
				{
					 Margin=new Thickness(Margin.Left+5, Margin.Top, Margin.Right, Margin.Bottom),
					 Text=Bot.Settings.AfterCombatDelay.ToString(),
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
					 IsChecked=(Bot.Settings.EnableWaitAfterContainers)
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
						  IsChecked=Bot.Settings.Targeting.UseShrineTypes[i],
						  Margin=new Thickness(Margin.Left+3, Margin.Top, Margin.Right, Margin.Bottom+5),
					 };
					 cbUseShrine[i].Checked+=UseShrineChecked;
					 cbUseShrine[i].Unchecked+=UseShrineChecked;
					 spShrineUseOptions.Children.Add(cbUseShrine[i]);
				}
				spShrinePanel.Children.Add(spShrineUseOptions);

				lbGeneralContent.Items.Add(spShrinePanel);

				GeneralTab.Content=lbGeneralContent;

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
					 IsChecked=(Bot.Settings.Demonbuddy.EnableDemonBuddyCharacterSettings)
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
					 Value=Bot.Settings.Demonbuddy.MonsterPower,
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
