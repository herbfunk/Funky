using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using FunkyBot.Cache.Enums;
using FunkyBot.Settings;
using CheckBox = System.Windows.Controls.CheckBox;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using ListBox = System.Windows.Controls.ListBox;
using Orientation = System.Windows.Controls.Orientation;
using TextBox = System.Windows.Controls.TextBox;
using ToolTip = System.Windows.Controls.ToolTip;

namespace FunkyBot
{
	internal partial class FunkyWindow : Window
	{
		#region EventHandling
		//

		private void MovementTargetGlobeChecked(object sender, EventArgs e)
		{
			if (Bot.Settings.Combat.CombatMovementTargetTypes.HasFlag(TargetType.Globe))
				Bot.Settings.Combat.CombatMovementTargetTypes &= ~TargetType.Globe;
			else
				Bot.Settings.Combat.CombatMovementTargetTypes |= TargetType.Globe;
		}
		private void MovementTargetGoldChecked(object sender, EventArgs e)
		{
			if (Bot.Settings.Combat.CombatMovementTargetTypes.HasFlag(TargetType.Gold))
				Bot.Settings.Combat.CombatMovementTargetTypes &= ~TargetType.Gold;
			else
				Bot.Settings.Combat.CombatMovementTargetTypes |= TargetType.Gold;
		}

		private void MovementTargetItemChecked(object sender, EventArgs e)
		{
			if (Bot.Settings.Combat.CombatMovementTargetTypes.HasFlag(TargetType.Item))
				Bot.Settings.Combat.CombatMovementTargetTypes &= ~TargetType.Item;
			else
				Bot.Settings.Combat.CombatMovementTargetTypes |= TargetType.Item;
		}

		private void UseAdvancedProjectileTestingChecked(object sender, EventArgs e)
		{
			Bot.Settings.Avoidance.UseAdvancedProjectileTesting = !Bot.Settings.Avoidance.UseAdvancedProjectileTesting;
		}
		private void AvoidanceAttemptMovementChecked(object sender, EventArgs e)
		{
			Bot.Settings.Avoidance.AttemptAvoidanceMovements = !Bot.Settings.Avoidance.AttemptAvoidanceMovements;
		}
		private void GlobeHealthSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
			Bot.Settings.Combat.GlobeHealthPercent = Value;
			TBGlobeHealth.Text = Value.ToString();
		}
		private void PotionHealthSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
			Bot.Settings.Combat.PotionHealthPercent = Value;
			TBPotionHealth.Text = Value.ToString();
		}
		private void WellHealthSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
			Bot.Settings.Combat.HealthWellHealthPercent = Value;
			TBWellHealth.Text = Value.ToString();
		}

		private void GroupingLoadXMLClicked(object sender, EventArgs e)
		{
			OpenFileDialog OFD = new OpenFileDialog
			{
				InitialDirectory = Path.Combine(FolderPaths.sTrinityPluginPath, "Config", "Defaults"),
				RestoreDirectory = false,
				Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*",
				Title = "Grouping Template",
			};
			DialogResult OFD_Result = OFD.ShowDialog();

			if (OFD_Result == System.Windows.Forms.DialogResult.OK)
			{
				try
				{
					//;
					SettingGrouping newSettings = SettingGrouping.DeserializeFromXML(OFD.FileName);
					Bot.Settings.Grouping = newSettings;

					funkyConfigWindow.Close();
				}
				catch
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
			TabItem CombatGeneralTabItem = new TabItem();
			CombatGeneralTabItem.Header = "General";
			CombatTabControl.Items.Add(CombatGeneralTabItem);
			ListBox CombatGeneralContentListBox = new ListBox();





			#region HealthOptions
			StackPanel HealthOptionsStackPanel = new StackPanel
			{
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
				Background = Brushes.DimGray,
			};
			TextBlock Health_Options_Text = new TextBlock
			{
				Text = "Health",
				FontSize = 13,
				Background = Brushes.DarkSeaGreen,
				TextAlignment = TextAlignment.Center,
			};
			HealthOptionsStackPanel.Children.Add(Health_Options_Text);

			TextBlock Health_Info_Text = new TextBlock
			{
				Text = "Actions will occur when life is below given value",
				FontSize = 12,
				Foreground = Brushes.GhostWhite,
				FontStyle = FontStyles.Italic,
				//Background = System.Windows.Media.Brushes.Crimson,
				TextAlignment = TextAlignment.Left,
			};
			HealthOptionsStackPanel.Children.Add(Health_Info_Text);

			#region GlobeHealthPercent
			TextBlock HealthGlobe_Info_Text = new TextBlock
			{
				Text = "Globe Health Percent",
				FontSize = 12,
				Foreground = Brushes.GhostWhite,
				//Background = System.Windows.Media.Brushes.Crimson,
				TextAlignment = TextAlignment.Left,
			};
			HealthOptionsStackPanel.Children.Add(HealthGlobe_Info_Text);

			Slider sliderGlobeHealth = new Slider
			{
				Width = 100,
				Maximum = 1,
				Minimum = 0,
				TickFrequency = 0.25,
				LargeChange = 0.20,
				SmallChange = 0.10,
				Value = Bot.Settings.Combat.GlobeHealthPercent,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderGlobeHealth.ValueChanged += GlobeHealthSliderChanged;
			TBGlobeHealth = new TextBox
			{
				Text = Bot.Settings.Combat.GlobeHealthPercent.ToString("F2", CultureInfo.InvariantCulture),
				IsReadOnly = true,
			};
			StackPanel GlobeHealthStackPanel = new StackPanel
			{
				Width = 600,
				Height = 20,
				Orientation = Orientation.Horizontal,
			};
			GlobeHealthStackPanel.Children.Add(sliderGlobeHealth);
			GlobeHealthStackPanel.Children.Add(TBGlobeHealth);
			HealthOptionsStackPanel.Children.Add(GlobeHealthStackPanel);
			#endregion

			#region PotionHealthPercent
			TextBlock HealthPotion_Info_Text = new TextBlock
			{
				Text = "Potion Health Percent",
				FontSize = 12,
				Foreground = Brushes.GhostWhite,
				//Background = System.Windows.Media.Brushes.Crimson,
				TextAlignment = TextAlignment.Left,
			};
			HealthOptionsStackPanel.Children.Add(HealthPotion_Info_Text);


			Slider sliderPotionHealth = new Slider
			{
				Width = 100,
				Maximum = 1,
				Minimum = 0,
				TickFrequency = 0.25,
				LargeChange = 0.20,
				SmallChange = 0.10,
				Value = Bot.Settings.Combat.PotionHealthPercent,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderPotionHealth.ValueChanged += PotionHealthSliderChanged;
			TBPotionHealth = new TextBox
			{
				Text = Bot.Settings.Combat.PotionHealthPercent.ToString("F2", CultureInfo.InvariantCulture),
				IsReadOnly = true,
			};
			StackPanel PotionHealthStackPanel = new StackPanel
			{
				Width = 600,
				Height = 20,
				Orientation = Orientation.Horizontal,
			};
			PotionHealthStackPanel.Children.Add(sliderPotionHealth);
			PotionHealthStackPanel.Children.Add(TBPotionHealth);
			HealthOptionsStackPanel.Children.Add(PotionHealthStackPanel);
			#endregion

			#region HealthWellhealthPercent
			TextBlock HealthWell_Info_Text = new TextBlock
			{
				Text = "Health Well Percent",
				FontSize = 12,
				Foreground = Brushes.GhostWhite,
				//Background = System.Windows.Media.Brushes.Crimson,
				TextAlignment = TextAlignment.Left,
			};
			HealthOptionsStackPanel.Children.Add(HealthWell_Info_Text);


			Slider sliderWellHealth = new Slider
			{
				Width = 100,
				Maximum = 1,
				Minimum = 0,
				TickFrequency = 0.25,
				LargeChange = 0.20,
				SmallChange = 0.10,
				Value = Bot.Settings.Combat.HealthWellHealthPercent,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderWellHealth.ValueChanged += WellHealthSliderChanged;
			TBWellHealth = new TextBox
			{
				Text = Bot.Settings.Combat.HealthWellHealthPercent.ToString("F2", CultureInfo.InvariantCulture),
				IsReadOnly = true,
			};
			StackPanel WellHealthStackPanel = new StackPanel
			{
				Width = 600,
				Height = 20,
				Orientation = Orientation.Horizontal,
			};
			WellHealthStackPanel.Children.Add(sliderWellHealth);
			WellHealthStackPanel.Children.Add(TBWellHealth);
			HealthOptionsStackPanel.Children.Add(WellHealthStackPanel);
			#endregion


			CombatGeneralContentListBox.Items.Add(HealthOptionsStackPanel);

			#endregion

			#region Skills
			StackPanel SkillsOptionsStackPanel = new StackPanel
			{
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
				Background = Brushes.DimGray,
				VerticalAlignment = VerticalAlignment.Stretch,
				Width = 600,
			};
			TextBlock Skills_Options_Text = new TextBlock
			{
				Text = "Skills",
				FontSize = 13,
				Background = Brushes.DarkSeaGreen,
				TextAlignment = TextAlignment.Center,
			};
			SkillsOptionsStackPanel.Children.Add(Skills_Options_Text);

			#region Skill Movement Options

			StackPanel SkillsMovementStackPanel = new StackPanel
			{
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
				Background = Brushes.DimGray,
			};
			SkillsOptionsStackPanel.Children.Add(SkillsMovementStackPanel);
			ToolTip ttSkillMovementOptions = new ToolTip
			{
				Content = "Enables additional targets allowed for special skill movement usage.",
			};
			TextBlock SkillsMovement_Options_Text = new TextBlock
			{
				Text = "Combat Movement Valid Targets",
				FontSize = 13,
				Background = Brushes.Gray,
				Foreground = Brushes.GhostWhite,
				TextAlignment = TextAlignment.Left,
				//FontStyle = FontStyles.Oblique,
				FontWeight = FontWeights.Bold,
				ToolTip = ttSkillMovementOptions,
			};
			SkillsMovementStackPanel.Children.Add(SkillsMovement_Options_Text);

			CheckBox cbMovementGold = new CheckBox
			{
				Content = "Gold",
				IsChecked = Bot.Settings.Combat.CombatMovementTargetTypes.HasFlag(TargetType.Gold),
			};
			cbMovementGold.Unchecked += MovementTargetGoldChecked;
			cbMovementGold.Checked += MovementTargetGoldChecked;
			SkillsMovementStackPanel.Children.Add(cbMovementGold);

			CheckBox cbMovementGlobes = new CheckBox
			{
				Content = "Globes",
				IsChecked = Bot.Settings.Combat.CombatMovementTargetTypes.HasFlag(TargetType.Globe),
			};
			cbMovementGlobes.Checked += MovementTargetGlobeChecked;
			cbMovementGlobes.Unchecked += MovementTargetGlobeChecked;
			SkillsMovementStackPanel.Children.Add(cbMovementGlobes);

			CheckBox cbMovementItems = new CheckBox
			{
				Content = "Items",
				IsChecked = Bot.Settings.Combat.CombatMovementTargetTypes.HasFlag(TargetType.Item),
			};
			cbMovementItems.Checked += MovementTargetItemChecked;
			cbMovementItems.Unchecked += MovementTargetItemChecked;
			SkillsMovementStackPanel.Children.Add(cbMovementItems);

			#endregion

			#region Skills Misc Options
			StackPanel SkillsMiscOptionsStackPanel = new StackPanel
			{
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
				Background = Brushes.DimGray,
			};
			SkillsOptionsStackPanel.Children.Add(SkillsMiscOptionsStackPanel);
			TextBlock SkillsMisc_Options_Text = new TextBlock
			{
				Text = "Misc",
				FontSize = 13,
				Background = Brushes.Gray,
				Foreground = Brushes.GhostWhite,
				TextAlignment = TextAlignment.Left,
				FontWeight = FontWeights.Bold,
			};
			SkillsMiscOptionsStackPanel.Children.Add(SkillsMisc_Options_Text);

			#region Default Attack
			ToolTip ttSkillAllowDefaultAttack = new ToolTip
			{
				Content = "Allows Default Attack to be Used When Its Not Suppose to be usable.",
			};
			CheckBox cbAllowDefaultAttackAlways = new CheckBox
			{
				Content = "Allow Default Attack Always",
				IsChecked = Bot.Settings.Class.AllowDefaultAttackAlways,
				ToolTip = ttSkillAllowDefaultAttack,
			};
			cbAllowDefaultAttackAlways.Checked += AllowDefaultAttackAlwaysChecked;
			cbAllowDefaultAttackAlways.Unchecked += AllowDefaultAttackAlwaysChecked;
			SkillsMiscOptionsStackPanel.Children.Add(cbAllowDefaultAttackAlways);
			#endregion

			#region OutOfCombatMovement
			CheckBox cbOutOfCombatMovement = new CheckBox
			{
				Content = "Allow Out Of Combat Skill Movements",
				IsChecked = (Bot.Settings.OutOfCombatMovement)
			};
			cbOutOfCombatMovement.Checked += OutOfCombatMovementChecked;
			cbOutOfCombatMovement.Unchecked += OutOfCombatMovementChecked;
			SkillsMiscOptionsStackPanel.Children.Add(cbOutOfCombatMovement);
			#endregion
			#endregion


			CombatGeneralContentListBox.Items.Add(SkillsOptionsStackPanel);
			#endregion

			CombatGeneralTabItem.Content = CombatGeneralContentListBox;
		}
	}

}
