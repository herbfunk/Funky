using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Zeta.Internals.Actors;

namespace FunkyBot
{
	 internal partial class FunkyWindow : Window
	 {
		  #region EventHandling
		  private void TreasureGoblinMinimumRangeSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				int Value=(int)slider_sender.Value;
				Bot.Settings.Class.GoblinMinimumRange=Value;
				TBGoblinMinRange.Text=Value.ToString();
		  }
		  #endregion


		  internal void InitPlayerClassControls()
		  {
				//Class Specific
				TabItem ClassTabItem=new TabItem();
				ClassTabItem.Header="Class";
				CombatTabControl.Items.Add(ClassTabItem);
				ListBox LBClass=new ListBox();

				switch (Bot.Character.Account.ActorClass)
				{
					 case ActorClass.Barbarian:
						  CheckBox cbbSelectiveWhirlwind=new CheckBox
						  {
								Content="Selective Whirlwind Targeting",
								Width=300,
								Height=30,
								IsChecked=(Bot.Settings.Class.bSelectiveWhirlwind)
						  };
						  cbbSelectiveWhirlwind.Checked+=bSelectiveWhirlwindChecked;
						  cbbSelectiveWhirlwind.Unchecked+=bSelectiveWhirlwindChecked;
						  LBClass.Items.Add(cbbSelectiveWhirlwind);

						  TextBlock txtblockWrathOptions=new TextBlock
						  {
								Text="Wrath of the Berserker Options",
								FontStyle=FontStyles.Oblique,
								Foreground=Brushes.GhostWhite,
								FontSize=11,
								TextAlignment=TextAlignment.Left,
						  };
						  LBClass.Items.Add(txtblockWrathOptions);

						  StackPanel spWrathOptions=new StackPanel
						  {
								Orientation=Orientation.Horizontal,
						  };
						  CheckBox cbbWaitForWrath=new CheckBox
						  {
								Content="Wait for Wrath",
								Height=30,
								IsChecked=(Bot.Settings.Class.bWaitForWrath),
								Margin=new Thickness(5),
						  };
						  cbbWaitForWrath.Checked+=bWaitForWrathChecked;
						  cbbWaitForWrath.Unchecked+=bWaitForWrathChecked;
						  spWrathOptions.Children.Add(cbbWaitForWrath);

						  CheckBox cbbGoblinWrath=new CheckBox
						  {
								Content="Use Wrath on Goblins",
								Height=30,
								IsChecked=(Bot.Settings.Class.bGoblinWrath),
								Margin=new Thickness(5),
						  };
						  cbbGoblinWrath.Checked+=bGoblinWrathChecked;
						  cbbGoblinWrath.Unchecked+=bGoblinWrathChecked;
						  spWrathOptions.Children.Add(cbbGoblinWrath);

						  CheckBox cbbBarbUseWOTBAlways=new CheckBox
						  {
								Content="Use Wrath on Always",
								Height=30,
								IsChecked=(Bot.Settings.Class.bBarbUseWOTBAlways),
								Margin=new Thickness(5),
						  };
						  cbbBarbUseWOTBAlways.Checked+=bBarbUseWOTBAlwaysChecked;
						  cbbBarbUseWOTBAlways.Unchecked+=bBarbUseWOTBAlwaysChecked;
						  spWrathOptions.Children.Add(cbbBarbUseWOTBAlways);
						  LBClass.Items.Add(spWrathOptions);



						  CheckBox cbbFuryDumpWrath=new CheckBox
						  {
								Content="Fury Dump during Wrath",
								Width=300,
								Height=30,
								IsChecked=(Bot.Settings.Class.bFuryDumpWrath)
						  };
						  cbbFuryDumpWrath.Checked+=bFuryDumpWrathChecked;
						  cbbFuryDumpWrath.Unchecked+=bFuryDumpWrathChecked;
						  LBClass.Items.Add(cbbFuryDumpWrath);

						  CheckBox cbbFuryDumpAlways=new CheckBox
						  {
								Content="Fury Dump Always",
								Width=300,
								Height=30,
								IsChecked=(Bot.Settings.Class.bFuryDumpAlways)
						  };
						  cbbFuryDumpAlways.Checked+=bFuryDumpAlwaysChecked;
						  cbbFuryDumpAlways.Unchecked+=bFuryDumpAlwaysChecked;
						  LBClass.Items.Add(cbbFuryDumpAlways);

						  break;
					 case ActorClass.DemonHunter:
						  LBClass.Items.Add("Reuse Vault Delay");
						  Slider iDHVaultMovementDelayslider=new Slider
						  {
								Width=200,
								Maximum=4000,
								Minimum=400,
								TickFrequency=5,
								LargeChange=5,
								SmallChange=1,
								Value=Bot.Settings.Class.iDHVaultMovementDelay,
								HorizontalAlignment=HorizontalAlignment.Left,
						  };
						  iDHVaultMovementDelayslider.ValueChanged+=iDHVaultMovementDelaySliderChanged;
						  TBiDHVaultMovementDelay=new TextBox
						  {
								Text=Bot.Settings.Class.iDHVaultMovementDelay.ToString(),
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
					 case ActorClass.Monk:
						  CheckBox cbbMonkSpamMantra=new CheckBox
						  {
								Content="Spam Mantra Ability",
								Width=300,
								Height=30,
								IsChecked=Bot.Settings.Class.bMonkSpamMantra,
						  };
						  cbbMonkSpamMantra.Checked+=bMonkSpamMantraChecked;
						  cbbMonkSpamMantra.Unchecked+=bMonkSpamMantraChecked;
						  LBClass.Items.Add(cbbMonkSpamMantra);


                          CheckBox cbbMonkMaintainSweepingWind = new CheckBox
						  {
								Content="Maintain Sweeping Wind",
								Width=300,
								Height=30,
								IsChecked=Bot.Settings.Class.bMonkMaintainSweepingWind,
						  };
                          cbbMonkMaintainSweepingWind.Checked += bMonkMaintainSweepingWindChecked;
                          cbbMonkMaintainSweepingWind.Unchecked += bMonkMaintainSweepingWindChecked;
                          LBClass.Items.Add(cbbMonkMaintainSweepingWind);

                          //

						  break;
					 case ActorClass.WitchDoctor:
					 case ActorClass.Wizard:
						  //CheckBox cbbEnableCriticalMass = new CheckBox
						  //{
						  //    Content = "Critical Mass",
						  //    Width = 300,
						  //    Height = 30,
						  //    IsChecked = (Bot.SettingsFunky.Class.bEnableCriticalMass)
						  //};
						  //cbbEnableCriticalMass.Checked += bEnableCriticalMassChecked;
						  //cbbEnableCriticalMass.Unchecked += bEnableCriticalMassChecked;
						  //LBClass.Items.Add(cbbEnableCriticalMass);

						  if (Bot.Character.Account.ActorClass == ActorClass.Wizard)
						  {
								CheckBox cbbWaitForArchon=new CheckBox
								{
									 Content="Wait for Archon",
									 Width=300,
									 Height=30,
									 IsChecked=(Bot.Settings.Class.bWaitForArchon)
								};
								cbbWaitForArchon.Checked+=bWaitForArchonChecked;
								cbbWaitForArchon.Unchecked+=bWaitForArchonChecked;
								LBClass.Items.Add(cbbWaitForArchon);

								CheckBox cbbKiteOnlyArchon=new CheckBox
								{
									 Content="Do NOT Kite During Archon",
									 Width=300,
									 Height=30,
									 IsChecked=(Bot.Settings.Class.bKiteOnlyArchon)
								};
								cbbKiteOnlyArchon.Checked+=bKiteOnlyArchonChecked;
								cbbKiteOnlyArchon.Unchecked+=bKiteOnlyArchonChecked;
								LBClass.Items.Add(cbbKiteOnlyArchon);

								CheckBox cbbCancelArchonRebuff=new CheckBox
								{
									 Content="Cancel Archon for Rebuff",
									 Height=30,
									 IsChecked=(Bot.Settings.Class.bCancelArchonRebuff),
								};
								cbbCancelArchonRebuff.Checked+=bCancelArchonRebuffChecked;
								cbbCancelArchonRebuff.Unchecked+=bCancelArchonRebuffChecked;
								LBClass.Items.Add(cbbCancelArchonRebuff);

								CheckBox cbbTeleportFleeWhenLowHP=new CheckBox
								{
									 Content="Teleport: Flee When Low HP",
									 Height=30,
									 IsChecked=(Bot.Settings.Class.bTeleportFleeWhenLowHP),
								};
								cbbTeleportFleeWhenLowHP.Checked+=bTeleportFleeWhenLowHPChecked;
								cbbTeleportFleeWhenLowHP.Unchecked+=bTeleportFleeWhenLowHPChecked;
								LBClass.Items.Add(cbbTeleportFleeWhenLowHP);

								CheckBox cbbTeleportIntoGrouping=new CheckBox
								{
									 Content="Teleport: Into Monster Groups",
									 Height=30,
									 IsChecked=(Bot.Settings.Class.bTeleportIntoGrouping),
								};
								cbbTeleportIntoGrouping.Checked+=bTeleportIntoGroupingChecked;
								cbbTeleportIntoGrouping.Unchecked+=bTeleportIntoGroupingChecked;
								LBClass.Items.Add(cbbTeleportIntoGrouping);
								//

						  }

						  break;
				}
				if (Bot.Character.Account.ActorClass == ActorClass.DemonHunter || Bot.Character.Account.ActorClass == ActorClass.WitchDoctor || Bot.Character.Account.ActorClass == ActorClass.Wizard)
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
						  Value=Bot.Settings.Class.GoblinMinimumRange,
						  HorizontalAlignment=HorizontalAlignment.Left,
					 };
					 sliderGoblinMinRange.ValueChanged+=TreasureGoblinMinimumRangeSliderChanged;
					 TBGoblinMinRange=new TextBox
					 {
						  Text=Bot.Settings.Class.GoblinMinimumRange.ToString(),
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


					 CheckBox cbMissleDampeningCloseRange=new CheckBox
					 {
						  Content="Close Range on Missile Dampening Monsters",
						  IsChecked=Bot.Settings.Targeting.MissleDampeningEnforceCloseRange,
					 };
					 cbMissleDampeningCloseRange.Checked+=MissileDampeningChecked;
					 cbMissleDampeningCloseRange.Unchecked+=MissileDampeningChecked;
					 LBClass.Items.Add(cbMissleDampeningCloseRange);
				}

				CheckBox cbAllowDefaultAttackAlways = new CheckBox
				{
					Content = "Allow Default Attack Always",
					IsChecked = Bot.Settings.Class.AllowDefaultAttackAlways,
				};
				cbAllowDefaultAttackAlways.Checked += AllowDefaultAttackAlwaysChecked;
				cbAllowDefaultAttackAlways.Unchecked += AllowDefaultAttackAlwaysChecked;
				LBClass.Items.Add(cbAllowDefaultAttackAlways);


				ClassTabItem.Content=LBClass;
		  }
	}
}
