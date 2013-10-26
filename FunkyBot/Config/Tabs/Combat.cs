using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using FunkyBot.Settings;

namespace FunkyBot
{
	 internal partial class FunkyWindow : Window
	 {
		  #region EventHandling
		  //


		 
		  private void UseAdvancedProjectileTestingChecked(object sender, EventArgs e)
		  {
				Bot.Settings.Avoidance.UseAdvancedProjectileTesting=!Bot.Settings.Avoidance.UseAdvancedProjectileTesting;
		  }
		  private void AvoidanceAttemptMovementChecked(object sender, EventArgs e)
		  {
				Bot.Settings.Avoidance.AttemptAvoidanceMovements=!Bot.Settings.Avoidance.AttemptAvoidanceMovements;
		  }
		  private void GlobeHealthSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				double Value=Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
				Bot.Settings.Combat.GlobeHealthPercent=Value;
				TBGlobeHealth.Text=Value.ToString();
		  }
		  private void PotionHealthSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				double Value=Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
				Bot.Settings.Combat.PotionHealthPercent=Value;
				TBPotionHealth.Text=Value.ToString();
		  }
		  private void WellHealthSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				double Value=Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
				Bot.Settings.Combat.HealthWellHealthPercent=Value;
				TBWellHealth.Text=Value.ToString();
		  }

		  private void GroupingLoadXMLClicked(object sender, EventArgs e)
		  {
				System.Windows.Forms.OpenFileDialog OFD=new System.Windows.Forms.OpenFileDialog
				{
					 InitialDirectory=Path.Combine(FolderPaths.sTrinityPluginPath, "Config", "Defaults"),
					 RestoreDirectory=false,
					 Filter="xml files (*.xml)|*.xml|All files (*.*)|*.*",
					 Title="Grouping Template",
				};
				System.Windows.Forms.DialogResult OFD_Result=OFD.ShowDialog();

				if (OFD_Result==System.Windows.Forms.DialogResult.OK)
				{
					 try
					 {
						  //;
						  SettingGrouping newSettings=SettingGrouping.DeserializeFromXML(OFD.FileName);
						  Bot.Settings.Grouping=newSettings;

						  FunkyWindow.funkyConfigWindow.Close();
					 } catch
					 {

					 }
				}

			
		  }
		  #endregion

		  private TextBox TBClusterDistance, TBClusterMinUnitCount,
								TBPotionHealth, TBGlobeHealth, TBWellHealth;

		  private StackPanel spGroupingOptions;





		  private CheckBox CBGroupingBehavior;
		  private Slider sliderGroupingMinimumUnitDistance, sliderGroupingMaximumDistance, sliderGroupingMinimumUnits, sliderGroupingMinimumCluster;
		 
	


		  internal void InitCombatControls()
		 {
			  //Combat
			  TabItem CombatGeneralTabItem=new TabItem();
			  CombatGeneralTabItem.Header="General";
			  CombatTabControl.Items.Add(CombatGeneralTabItem);
			  ListBox CombatGeneralContentListBox=new ListBox();



	

			  #region HealthOptions
			  StackPanel HealthOptionsStackPanel=new StackPanel
			  {
					Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
					Background=System.Windows.Media.Brushes.DimGray,
			  };
			  TextBlock Health_Options_Text=new TextBlock
			  {
					Text="Health",
					FontSize=13,
					Background=System.Windows.Media.Brushes.DarkSeaGreen,
					TextAlignment=TextAlignment.Center,
			  };
			  HealthOptionsStackPanel.Children.Add(Health_Options_Text);

			  TextBlock Health_Info_Text=new TextBlock
			  {
					Text="Actions will occur when life is below given value",
					FontSize=12,
					Foreground=System.Windows.Media.Brushes.GhostWhite,
					FontStyle=FontStyles.Italic,
					//Background = System.Windows.Media.Brushes.Crimson,
					TextAlignment=TextAlignment.Left,
			  };
			  HealthOptionsStackPanel.Children.Add(Health_Info_Text);

			  #region GlobeHealthPercent
			  TextBlock HealthGlobe_Info_Text=new TextBlock
			  {
					Text="Globe Health Percent",
					FontSize=12,
					Foreground=System.Windows.Media.Brushes.GhostWhite,
					//Background = System.Windows.Media.Brushes.Crimson,
					TextAlignment=TextAlignment.Left,
			  };
			  HealthOptionsStackPanel.Children.Add(HealthGlobe_Info_Text);

			  Slider sliderGlobeHealth=new Slider
			  {
					Width=100,
					Maximum=1,
					Minimum=0,
					TickFrequency=0.25,
					LargeChange=0.20,
					SmallChange=0.10,
					Value=Bot.Settings.Combat.GlobeHealthPercent,
					HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
			  };
			  sliderGlobeHealth.ValueChanged+=GlobeHealthSliderChanged;
			  TBGlobeHealth=new TextBox
			  {
					Text=Bot.Settings.Combat.GlobeHealthPercent.ToString("F2", CultureInfo.InvariantCulture),
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
			  HealthOptionsStackPanel.Children.Add(GlobeHealthStackPanel);
			  #endregion

			  #region PotionHealthPercent
			  TextBlock HealthPotion_Info_Text=new TextBlock
			  {
					Text="Potion Health Percent",
					FontSize=12,
					Foreground=System.Windows.Media.Brushes.GhostWhite,
					//Background = System.Windows.Media.Brushes.Crimson,
					TextAlignment=TextAlignment.Left,
			  };
			  HealthOptionsStackPanel.Children.Add(HealthPotion_Info_Text);
			 

			  Slider sliderPotionHealth=new Slider
			  {
					Width=100,
					Maximum=1,
					Minimum=0,
					TickFrequency=0.25,
					LargeChange=0.20,
					SmallChange=0.10,
					Value=Bot.Settings.Combat.PotionHealthPercent,
					HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
			  };
			  sliderPotionHealth.ValueChanged+=PotionHealthSliderChanged;
			  TBPotionHealth=new TextBox
			  {
					Text=Bot.Settings.Combat.PotionHealthPercent.ToString("F2", CultureInfo.InvariantCulture),
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
			  HealthOptionsStackPanel.Children.Add(PotionHealthStackPanel);
			  #endregion

			  #region HealthWellhealthPercent
			  TextBlock HealthWell_Info_Text=new TextBlock
			  {
					Text="Health Well Percent",
					FontSize=12,
					Foreground=System.Windows.Media.Brushes.GhostWhite,
					//Background = System.Windows.Media.Brushes.Crimson,
					TextAlignment=TextAlignment.Left,
			  };
			  HealthOptionsStackPanel.Children.Add(HealthWell_Info_Text);


			  Slider sliderWellHealth=new Slider
			  {
					Width=100,
					Maximum=1,
					Minimum=0,
					TickFrequency=0.25,
					LargeChange=0.20,
					SmallChange=0.10,
					Value=Bot.Settings.Combat.HealthWellHealthPercent,
					HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
			  };
			  sliderWellHealth.ValueChanged+=WellHealthSliderChanged;
			  TBWellHealth=new TextBox
			  {
					Text=Bot.Settings.Combat.HealthWellHealthPercent.ToString("F2", CultureInfo.InvariantCulture),
					IsReadOnly=true,
			  };
			  StackPanel WellHealthStackPanel=new StackPanel
			  {
					Width=600,
					Height=20,
					Orientation=Orientation.Horizontal,
			  };
			  WellHealthStackPanel.Children.Add(sliderWellHealth);
			  WellHealthStackPanel.Children.Add(TBWellHealth);
			  HealthOptionsStackPanel.Children.Add(WellHealthStackPanel);
			  #endregion
			  

			  CombatGeneralContentListBox.Items.Add(HealthOptionsStackPanel);

			  #endregion

			  CombatGeneralTabItem.Content=CombatGeneralContentListBox;
		 }
	}

}
