using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using FunkyBot.Cache;
using FunkyBot.Cache.Avoidance;
using FunkyBot.Cache.Enums;
using FunkyBot.Settings;
using Brushes = System.Drawing.Brushes;
using Color = System.Drawing.Color;
using FontStyle = System.Drawing.FontStyle;
using FlowDirection = System.Windows.Forms.FlowDirection;

namespace FunkyBot.Config
{
	public partial class SettingsForm : Form
	{
		public SettingsForm()
		{
			InitializeComponent();
		}


		private void tb_GlobeHealth_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture))/100;
			Bot.Settings.Combat.GlobeHealthPercent = Value;
			txt_GlobeHealth.Text = Value.ToString();
		}

		private void tb_WellHealth_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture)) / 100;
			Bot.Settings.Combat.HealthWellHealthPercent = Value;
			txt_WellHealth.Text = Value.ToString();
		}

		private void tb_PotionHealth_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture)) / 100;
			Bot.Settings.Combat.PotionHealthPercent = Value;
			txt_PotionHealth.Text = Value.ToString();
		}

		private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			Settings_Funky.SerializeToXML(Bot.Settings);
			base.OnClosed(e);
		}

		private void SettingsFor_Load(object sender, EventArgs e)
		{
			txt_GlobeHealth.Text = Bot.Settings.Combat.GlobeHealthPercent.ToString("F2", CultureInfo.InvariantCulture);
			txt_WellHealth.Text = Bot.Settings.Combat.HealthWellHealthPercent.ToString("F2", CultureInfo.InvariantCulture);
			txt_PotionHealth.Text = Bot.Settings.Combat.PotionHealthPercent.ToString("F2", CultureInfo.InvariantCulture);
			tb_GlobeHealth.Value = (int)(Bot.Settings.Combat.GlobeHealthPercent * 100);
			tb_PotionHealth.Value = (int)(Bot.Settings.Combat.PotionHealthPercent * 100);
			tb_WellHealth.Value = (int)(Bot.Settings.Combat.HealthWellHealthPercent * 100);
			cb_CombatMovementGlobes.Checked = Bot.Settings.Combat.CombatMovementTargetTypes.HasFlag(TargetType.Globe);
			cb_CombatMovementGold.Checked = Bot.Settings.Combat.CombatMovementTargetTypes.HasFlag(TargetType.Gold);
			cb_CombatMovementItems.Checked = Bot.Settings.Combat.CombatMovementTargetTypes.HasFlag(TargetType.Item);
			
			cb_ClusterTargetLogic.Checked = Bot.Settings.Cluster.EnableClusteringTargetLogic;
			cb_ClusterLogicDisableHealth.Checked = Bot.Settings.Cluster.IgnoreClusteringWhenLowHP;
			txt_ClusterLogicDisableHealth.Text = Bot.Settings.Cluster.IgnoreClusterLowHPValue.ToString("F2", CultureInfo.InvariantCulture);
			txt_ClusterLogicDistance.Text = Bot.Settings.Cluster.ClusterDistance.ToString("F2", CultureInfo.InvariantCulture);
			txt_ClusterLogicMinimumUnits.Text = Bot.Settings.Cluster.ClusterMinimumUnitCount.ToString("F2", CultureInfo.InvariantCulture);
			tb_ClusterLogicDisableHealth.Value = (int)(Bot.Settings.Cluster.IgnoreClusterLowHPValue * 100);
			tb_ClusterLogicDistance.Value = (int)(Bot.Settings.Cluster.ClusterDistance);
			tb_ClusterLogicMinimumUnits.Value = (int)(Bot.Settings.Cluster.ClusterMinimumUnitCount);

			cb_GroupingLogic.Checked = Bot.Settings.Grouping.AttemptGroupingMovements;
			txt_GroupingMaxDistance.Text=Bot.Settings.Grouping.GroupingMaximumDistanceAllowed.ToString();
			txt_GroupingMinBotHealth.Text=Bot.Settings.Grouping.GroupingMinimumBotHealth.ToString();
			txt_GroupingMinCluster.Text=Bot.Settings.Grouping.GroupingMinimumClusterCount.ToString();
			txt_GroupingMinDistance.Text=Bot.Settings.Grouping.GroupingMinimumUnitDistance.ToString();
			txt_GroupingMinUnitsInCluster.Text=Bot.Settings.Grouping.GroupingMinimumUnitsInCluster.ToString();

			cb_AvoidanceLogic.Checked=Bot.Settings.Avoidance.AttemptAvoidanceMovements;

			AvoidanceValue[] avoidanceValues = Bot.Settings.Avoidance.Avoidances.ToArray();
			TBavoidanceHealth = new TextBox[avoidanceValues.Length - 1];
			TBavoidanceRadius = new TextBox[avoidanceValues.Length - 1];
			TBavoidanceWeight = new TextBox[avoidanceValues.Length - 1];

			int alternatingColor = 0;
			for (int i = 0; i < avoidanceValues.Length - 1; i++)
			{
				if (alternatingColor > 1) alternatingColor = 0;

				string avoidanceString = avoidanceValues[i].Type.ToString();

				int defaultRadius = (int)avoidanceValues[i].Radius;
				//Bot.SettingsFunky.Avoidance.AvoidanceRadiusValues.TryGetValue(avoidanceTypes[i], out defaultRadius);

				TrackBar avoidanceRadius = new TrackBar
				{
					Width = 100,
					Name = avoidanceString + "_radius_" + i.ToString(),
					Maximum = 30,
					Minimum = 0,
					TickFrequency = 5,
					LargeChange = 5,
					SmallChange = 1,
					Value = defaultRadius,
					//Padding=new Thickness(2),
					Margin = new Padding(5),
				};
				avoidanceRadius.ValueChanged += AvoidanceRadiusSliderValueChanged;
				TBavoidanceRadius[i] = new TextBox
				{
					Text = defaultRadius.ToString(),
				};

				int defaultHealth = (int)avoidanceValues[i].Health;
				//Bot.SettingsFunky.Avoidance.AvoidanceHealthValues.TryGetValue(avoidanceTypes[i], out defaultHealth);
				TrackBar avoidanceHealth = new TrackBar
				{
					Name = avoidanceString + "_health_" + i.ToString(),
					Width = 100,
					Maximum = 100,
					Minimum = 0,
					TickFrequency = 10,
					LargeChange = 10,
					SmallChange = 5,
					Value = defaultHealth,
					Margin = new Padding(5),
				};
				avoidanceHealth.ValueChanged += AvoidanceHealthSliderValueChanged;
				TBavoidanceHealth[i] = new TextBox
				{
					Text = defaultHealth.ToString("F2", CultureInfo.InvariantCulture),
				};

				int defaultWeight = avoidanceValues[i].Weight;
				TrackBar avoidanceWeight = new TrackBar
				{
					Name = avoidanceString + "_weight_" + i.ToString(),
					Width = 100,
					Maximum = 20,
					Minimum = 0,
					TickFrequency = 2,
					LargeChange = 5,
					SmallChange = 1,
					Value = defaultWeight,
					Margin = new Padding(5),
				};
				avoidanceWeight.ValueChanged += AvoidanceWeightSliderValueChanged;
				TBavoidanceWeight[i] = new TextBox
				{
					Text = defaultWeight.ToString("F2", CultureInfo.InvariantCulture),
				};

				
				Label txt1 = new Label
				{
					Text = avoidanceString,
					Font=new Font(this.Font.FontFamily, 12, FontStyle.Bold),
					TextAlign= ContentAlignment.MiddleCenter,
					BackColor = alternatingColor == 0 ? Color.DarkSeaGreen : BackColor = Color.SlateGray,
		 			ForeColor = Color.GhostWhite,
				};

				FlowLayoutPanel avoidRadiusStackPanel = new FlowLayoutPanel
				{
					Width = 155,
					Height = 25,
					FlowDirection = FlowDirection.LeftToRight,
					BackColor = alternatingColor == 0 ? Color.DarkSeaGreen : BackColor = Color.SlateGray,
				};
				avoidRadiusStackPanel.Controls.Add(avoidanceRadius);
				avoidRadiusStackPanel.Controls.Add(TBavoidanceRadius[i]);

				FlowLayoutPanel avoidHealthStackPanel = new FlowLayoutPanel
				{
					Width = 155,
					Height = 25,
					FlowDirection = FlowDirection.LeftToRight,
					BackColor = alternatingColor == 0 ? Color.DarkSeaGreen : BackColor = Color.SlateGray,
				};
				avoidHealthStackPanel.Controls.Add(avoidanceHealth);
				avoidHealthStackPanel.Controls.Add(TBavoidanceHealth[i]);
				FlowLayoutPanel avoidWeightStackPanel = new FlowLayoutPanel
				{
					Width = 155,
					Height = 25,
					FlowDirection = FlowDirection.LeftToRight,
					BackColor = alternatingColor == 0 ? Color.DarkSeaGreen : BackColor = Color.SlateGray,
				};
				avoidWeightStackPanel.Controls.Add(avoidanceWeight);
				avoidWeightStackPanel.Controls.Add(TBavoidanceWeight[i]);

				FlowLayoutPanel avoidanceStackPanel = new FlowLayoutPanel
				{
					FlowDirection = FlowDirection.LeftToRight,
					BackColor = alternatingColor == 0 ? Color.DarkSeaGreen : BackColor = Color.SlateGray,
				};

				avoidanceStackPanel.Controls.Add(txt1);
				avoidanceStackPanel.Controls.Add(avoidRadiusStackPanel);
				avoidanceStackPanel.Controls.Add(avoidHealthStackPanel);
				avoidanceStackPanel.Controls.Add(avoidWeightStackPanel);

				flowLayoutPanel_Avoidances.Controls.Add(avoidanceStackPanel);

				alternatingColor++;
			}
		}

		private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
		{

		}

		private void AvoidanceRadiusSliderValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			string[] slider_info = slider_sender.Name.Split("_".ToCharArray());
			int tb_index = Convert.ToInt16(slider_info[2]);
			float currentValue = (int)slider_sender.Value;

			TBavoidanceRadius[tb_index].Text = currentValue.ToString();
			Bot.Settings.Avoidance.Avoidances[tb_index].Radius = currentValue;
			//AvoidanceType avoidancetype=(AvoidanceType)Enum.Parse(typeof(AvoidanceType), slider_info[0]);
			//Bot.SettingsFunky.Avoidance.AvoidanceRadiusValues[avoidancetype]=currentValue;
			//Bot.SettingsFunky.Avoidance.RecreateAvoidances();
		}

		private void AvoidanceHealthSliderValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			string[] slider_info = slider_sender.Name.Split("_".ToCharArray());
			double currentValue = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
			int tb_index = Convert.ToInt16(slider_info[2]);

			TBavoidanceHealth[tb_index].Text = currentValue.ToString();
			Bot.Settings.Avoidance.Avoidances[tb_index].Health = currentValue;
			//AvoidanceType avoidancetype=(AvoidanceType)Enum.Parse(typeof(AvoidanceType), slider_info[0]);
			//Bot.SettingsFunky.Avoidance.AvoidanceHealthValues[avoidancetype]=currentValue;
			//Bot.SettingsFunky.Avoidance.RecreateAvoidances();
		}

		private void AvoidanceWeightSliderValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			string[] slider_info = slider_sender.Name.Split("_".ToCharArray());
			int currentValue = Convert.ToInt16(slider_sender.Value);
			int tb_index = Convert.ToInt16(slider_info[2]);

			TBavoidanceWeight[tb_index].Text = currentValue.ToString();
			Bot.Settings.Avoidance.Avoidances[tb_index].Weight = currentValue;
			//AvoidanceType avoidancetype=(AvoidanceType)Enum.Parse(typeof(AvoidanceType), slider_info[0]);
			//Bot.SettingsFunky.Avoidance.AvoidanceHealthValues[avoidancetype]=currentValue;
			//Bot.SettingsFunky.Avoidance.RecreateAvoidances();
		}

		private void AvoidanceAttemptMovementChecked(object sender, EventArgs e)
		{
			Bot.Settings.Avoidance.AttemptAvoidanceMovements = !Bot.Settings.Avoidance.AttemptAvoidanceMovements;
		}
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
		private void AllowDefaultAttackAlwaysChecked(object sender, EventArgs e)
		{
			Bot.Settings.Combat.AllowDefaultAttackAlways = !Bot.Settings.Combat.AllowDefaultAttackAlways;
		}
		private void OutOfCombatMovementChecked(object sender, EventArgs e)
		{
			Bot.Settings.OutOfCombatMovement = !Bot.Settings.OutOfCombatMovement;
		}

		private void cb_ClusterTargetLogic_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Cluster.EnableClusteringTargetLogic = !Bot.Settings.Cluster.EnableClusteringTargetLogic;
		}

		private void cb_ClusterLogicDisableHealth_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Cluster.IgnoreClusteringWhenLowHP = !Bot.Settings.Cluster.IgnoreClusteringWhenLowHP;
		}

		private void tb_ClusterLogicDisableHealth_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture)) / 100;
			Bot.Settings.Cluster.IgnoreClusterLowHPValue = Value;
			txt_ClusterLogicDisableHealth.Text = Value.ToString();
		}

		private void tb_ClusterLogicDistance_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = slider_sender.Value;
			Bot.Settings.Cluster.ClusterDistance = Value;
			txt_ClusterLogicDistance.Text = Value.ToString();
		}

		private void tb_ClusterLogicMinimumUnits_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = slider_sender.Value;
			Bot.Settings.Cluster.ClusterMinimumUnitCount = Value;
			txt_WellHealth.Text = Value.ToString();
		}


		private void bWaitForArchonChecked(object sender, EventArgs e)
		{
			Bot.Settings.Wizard.bWaitForArchon = !Bot.Settings.Wizard.bWaitForArchon;
		}
		private void bKiteOnlyArchonChecked(object sender, EventArgs e)
		{
			Bot.Settings.Wizard.bKiteOnlyArchon = !Bot.Settings.Wizard.bKiteOnlyArchon;
		}
		private void bCancelArchonRebuffChecked(object sender, EventArgs e)
		{
			Bot.Settings.Wizard.bCancelArchonRebuff = !Bot.Settings.Wizard.bCancelArchonRebuff;
		}
		private void bTeleportFleeWhenLowHPChecked(object sender, EventArgs e)
		{
			Bot.Settings.Wizard.bTeleportFleeWhenLowHP = !Bot.Settings.Wizard.bTeleportFleeWhenLowHP;
		}
		private void bTeleportIntoGroupingChecked(object sender, EventArgs e)
		{
			Bot.Settings.Wizard.bTeleportIntoGrouping = !Bot.Settings.Wizard.bTeleportIntoGrouping;
		}
		private void bSelectiveWhirlwindChecked(object sender, EventArgs e)
		{
			Bot.Settings.Barbarian.bSelectiveWhirlwind = !Bot.Settings.Barbarian.bSelectiveWhirlwind;
		}
		private void bWaitForWrathChecked(object sender, EventArgs e)
		{
			Bot.Settings.Barbarian.bWaitForWrath = !Bot.Settings.Barbarian.bWaitForWrath;
		}
		private void bGoblinWrathChecked(object sender, EventArgs e)
		{
			Bot.Settings.Barbarian.bGoblinWrath = !Bot.Settings.Barbarian.bGoblinWrath;
		}
		private void bBarbUseWOTBAlwaysChecked(object sender, EventArgs e)
		{
			Bot.Settings.Barbarian.bBarbUseWOTBAlways = !Bot.Settings.Barbarian.bBarbUseWOTBAlways;
		}
		private void bFuryDumpWrathChecked(object sender, EventArgs e)
		{
			Bot.Settings.Barbarian.bFuryDumpWrath = !Bot.Settings.Barbarian.bFuryDumpWrath;
		}
		private void bFuryDumpAlwaysChecked(object sender, EventArgs e)
		{
			Bot.Settings.Barbarian.bFuryDumpAlways = !Bot.Settings.Barbarian.bFuryDumpAlways;
		}
		private void bMonkMaintainSweepingWindChecked(object sender, EventArgs e)
		{
			Bot.Settings.Monk.bMonkMaintainSweepingWind = !Bot.Settings.Monk.bMonkMaintainSweepingWind;
		}
		private void bMonkSpamMantraChecked(object sender, EventArgs e)
		{
			Bot.Settings.Monk.bMonkSpamMantra = !Bot.Settings.Monk.bMonkSpamMantra;
		}
		private void iDHVaultMovementDelaySliderChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.DemonHunter.iDHVaultMovementDelay = Value;
			//TBiDHVaultMovementDelay.Text = Value.ToString();
		}
		private void DebugStatusBarChecked(object sender, EventArgs e)
		{
			Bot.Settings.Debug.DebugStatusBar = !Bot.Settings.Debug.DebugStatusBar;
		}
		private void SkipAheadChecked(object sender, EventArgs e)
		{
			Bot.Settings.Debug.SkipAhead = !Bot.Settings.Debug.SkipAhead;
		}

		private void GroupBotHealthSliderChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
			Bot.Settings.Grouping.GroupingMinimumBotHealth = Value;
			txt_GroupingMinBotHealth.Text = Value.ToString();
		}
		private void GroupMinimumUnitDistanceSliderChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Grouping.GroupingMinimumUnitDistance = Value;
			txt_GroupingMinDistance.Text = Value.ToString();
		}
		private void GroupMaxDistanceSliderChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Grouping.GroupingMaximumDistanceAllowed = Value;
			txt_GroupingMaxDistance.Text = Value.ToString();
		}
		private void GroupMinimumClusterCountSliderChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Grouping.GroupingMinimumClusterCount = Value;
			txt_GroupingMinCluster.Text = Value.ToString();
		}
		private void GroupMinimumUnitsInClusterSliderChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Grouping.GroupingMinimumUnitsInCluster = Value;
			txt_GroupingMinUnitsInCluster.Text = Value.ToString();
		}
		private void GroupingBehaviorChecked(object sender, EventArgs e)
		{
			Bot.Settings.Grouping.AttemptGroupingMovements = !Bot.Settings.Grouping.AttemptGroupingMovements;
			bool enabled = Bot.Settings.Grouping.AttemptGroupingMovements;
			//spGroupingOptions.IsEnabled = enabled;
		}
	}
}
