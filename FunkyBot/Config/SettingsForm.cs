using System;
using System.Globalization;
using System.Windows.Forms;
using FunkyBot.Cache.Enums;
using FunkyBot.Settings;

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
		}

		private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
		{

		}
		private void MovementTargetGlobeChecked(object sender, EventArgs e)
		{

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
			Bot.Settings.Class.AllowDefaultAttackAlways = !Bot.Settings.Class.AllowDefaultAttackAlways;
		}
		private void OutOfCombatMovementChecked(object sender, EventArgs e)
		{
			Bot.Settings.OutOfCombatMovement = !Bot.Settings.OutOfCombatMovement;
		}
	}
}
