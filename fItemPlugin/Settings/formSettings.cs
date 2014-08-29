using System;
using System.Windows.Forms;
using fItemPlugin.Townrun;
using Zeta.Bot;

namespace fItemPlugin
{
	public partial class formSettings : Form
	{
		public formSettings()
		{
			InitializeComponent();

			checkBox_EnableItemRules.Checked = FunkyTownRunPlugin.PluginSettings.UseItemRules;
			checkBox_EnableItemRules.CheckedChanged += ItemRulesChecked;

			string ruletype = FunkyTownRunPlugin.PluginSettings.ItemRuleType.ToLower();
			comboBox_ItemRulesType.SelectedIndex = ruletype.Contains("hard") ? 2 : ruletype.Contains("soft") ? 1 : 0;
			comboBox_ItemRulesType.SelectedIndexChanged += ItemRulesTypeChanged;

			textBox_ItemRulesCustomPath.Text = FunkyTownRunPlugin.PluginSettings.ItemRuleCustomPath;

			comboBox_ItemRules_Logging_Stashed.Text = FunkyTownRunPlugin.PluginSettings.ItemRuleLogKeep;
			comboBox_ItemRules_Logging_Stashed.SelectedIndexChanged += ItemRulesLogKeepChanged;

			checkBox_ItemRules_ItemIDs.Checked = FunkyTownRunPlugin.PluginSettings.ItemRuleUseItemIDs;
			checkBox_ItemRules_ItemIDs.CheckedChanged += ItemRulesItemIDsChecked;

			checkBox_ItemRules_Debugging.Checked = FunkyTownRunPlugin.PluginSettings.ItemRuleDebug;
			checkBox_ItemRules_Debugging.CheckedChanged += ItemRulesDebugChecked;

			checkBox_UseItemManager.Checked = FunkyTownRunPlugin.PluginSettings.UseItemManagerEvaluation;
			checkBox_UseItemManager.CheckedChanged += ItemManagerChecked;

			checkBox_StashHoradricCaches.Checked = FunkyTownRunPlugin.PluginSettings.StashHoradricCache;
			checkBox_StashHoradricCaches.CheckedChanged += StashHoradricCacheChecked;

			checkBox_IDLegendaries.Checked = FunkyTownRunPlugin.PluginSettings.IdentifyLegendaries;
			checkBox_IDLegendaries.CheckedChanged += IdentifyLegendariesChecked;

			trackBar_MaximumPotions.Value=FunkyTownRunPlugin.PluginSettings.PotionsCount;
			trackBar_MaximumPotions.ValueChanged += PotionCountSliderChanged;
			textBox_MaximumPotions.Text = FunkyTownRunPlugin.PluginSettings.PotionsCount.ToString();

			comboBox_SalvageWhiteItems.SelectedIndex = FunkyTownRunPlugin.PluginSettings.SalvageWhiteItemLevel == 0 ? 0 : FunkyTownRunPlugin.PluginSettings.SalvageWhiteItemLevel == 1 ? 2 : 1;
			comboBox_SalvageWhiteItems.SelectedIndexChanged += SalvageRuleTypeChanged;

			comboBox_SalvageMagicItems.SelectedIndex = FunkyTownRunPlugin.PluginSettings.SalvageMagicItemLevel == 0 ? 0 : FunkyTownRunPlugin.PluginSettings.SalvageMagicItemLevel == 1 ? 2 : 1;
			comboBox_SalvageMagicItems.SelectedIndexChanged += SalvageRuleTypeChanged;

			comboBox_SalvageRareItems.SelectedIndex = FunkyTownRunPlugin.PluginSettings.SalvageRareItemLevel == 0 ? 0 : FunkyTownRunPlugin.PluginSettings.SalvageRareItemLevel == 1 ? 2 : 1;
			comboBox_SalvageRareItems.SelectedIndexChanged += SalvageRuleTypeChanged;
			
			comboBox_SalvageLegendaryItems.SelectedIndex = FunkyTownRunPlugin.PluginSettings.SalvageLegendaryItemLevel == 0 ? 0 : FunkyTownRunPlugin.PluginSettings.SalvageLegendaryItemLevel == 1 ? 2 : 1;
			comboBox_SalvageLegendaryItems.SelectedIndexChanged += SalvageRuleTypeChanged;


			checkBox_EnableGambling.Checked = FunkyTownRunPlugin.PluginSettings.EnableBloodShardGambling;
			checkBox_EnableGambling.CheckedChanged += BloodShardGamblingChecked;

			panel_Gambling.Enabled = FunkyTownRunPlugin.PluginSettings.EnableBloodShardGambling;

			trackBar_GamblingBloodShards.Value = FunkyTownRunPlugin.PluginSettings.MinimumBloodShards;
			trackBar_GamblingBloodShards.ValueChanged += MinimumBloodShardSliderChanged;
			textBox_GamblingBloodShards.Text = FunkyTownRunPlugin.PluginSettings.MinimumBloodShards.ToString();

			bool noFlags = FunkyTownRunPlugin.PluginSettings.BloodShardGambleItems.Equals(BloodShardGambleItems.None);
			var gambleItems = Enum.GetValues(typeof(BloodShardGambleItems));
			Func<object, string> fRetrieveNames = s => Enum.GetName(typeof(BloodShardGambleItems), s);
			foreach (var gambleItem in gambleItems)
			{
				var thisGambleItem = (BloodShardGambleItems)gambleItem;
				if (thisGambleItem.Equals(BloodShardGambleItems.None) || thisGambleItem.Equals(BloodShardGambleItems.All)) continue;

				string gambleItemName = fRetrieveNames(gambleItem);
				CheckBox cb = new CheckBox
				{
					Name = gambleItemName,
					Text = gambleItemName,
					Checked = !noFlags && FunkyTownRunPlugin.PluginSettings.BloodShardGambleItems.HasFlag(thisGambleItem),
				};
				cb.CheckedChanged += BloodShardGambleItemsChanged;
				flowLayoutPanel_GamblingItemTypes.Controls.Add(cb);
			}


		
		}


		private void ItemRulesChecked(object sender, EventArgs e)
		{
			FunkyTownRunPlugin.PluginSettings.UseItemRules = !FunkyTownRunPlugin.PluginSettings.UseItemRules;
		}
		private void ItemRulesTypeChanged(object sender, EventArgs e)
		{
			FunkyTownRunPlugin.PluginSettings.ItemRuleType = comboBox_ItemRulesType.Items[comboBox_ItemRulesType.SelectedIndex].ToString();
		}
		private void ItemRulesBrowse_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog OFD = new FolderBrowserDialog
			{

			};
			DialogResult OFD_Result = OFD.ShowDialog();

			if (OFD_Result == System.Windows.Forms.DialogResult.OK)
			{
				try
				{
					
					FunkyTownRunPlugin.PluginSettings.ItemRuleCustomPath = OFD.SelectedPath;
					textBox_ItemRulesCustomPath.Text = FunkyTownRunPlugin.PluginSettings.ItemRuleCustomPath;
				}
				catch
				{

				}
			}
		}
		private void ItemRulesLogKeepChanged(object sender, EventArgs e)
		{

			FunkyTownRunPlugin.PluginSettings.ItemRuleLogKeep = comboBox_ItemRules_Logging_Stashed.Items[comboBox_ItemRules_Logging_Stashed.SelectedIndex].ToString();
		}
		private void ItemRulesItemIDsChecked(object sender, EventArgs e)
		{
			FunkyTownRunPlugin.PluginSettings.ItemRuleUseItemIDs = !FunkyTownRunPlugin.PluginSettings.ItemRuleUseItemIDs;
		}
		private void ItemRulesDebugChecked(object sender, EventArgs e)
		{
			FunkyTownRunPlugin.PluginSettings.ItemRuleDebug = !FunkyTownRunPlugin.PluginSettings.ItemRuleDebug;
		}

		private void ItemManagerChecked(object sender, EventArgs e)
		{
			FunkyTownRunPlugin.PluginSettings.UseItemManagerEvaluation = !FunkyTownRunPlugin.PluginSettings.UseItemManagerEvaluation;
		}
		private void StashHoradricCacheChecked(object sender, EventArgs e)
		{
			FunkyTownRunPlugin.PluginSettings.StashHoradricCache = !FunkyTownRunPlugin.PluginSettings.StashHoradricCache;
		}
		private void BuyPotionsDuringTownRunChecked(object sender, EventArgs e)
		{
			FunkyTownRunPlugin.PluginSettings.BuyPotionsDuringTownRun = !FunkyTownRunPlugin.PluginSettings.BuyPotionsDuringTownRun;
		}
		private void PotionCountSliderChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = Convert.ToInt32(slider_sender.Value);
			FunkyTownRunPlugin.PluginSettings.PotionsCount = Value;
			textBox_MaximumPotions.Text = Value.ToString();
		}
		private void IdentifyLegendariesChecked(object sender, EventArgs e)
		{
			FunkyTownRunPlugin.PluginSettings.IdentifyLegendaries = !FunkyTownRunPlugin.PluginSettings.IdentifyLegendaries;
		}
		private void SalvageRuleTypeChanged(object sender, EventArgs e)
		{
			ComboBox cbSender = (ComboBox)sender;
			string tag = Convert.ToString(cbSender.Tag);

			if (tag == "White")
				FunkyTownRunPlugin.PluginSettings.SalvageWhiteItemLevel = cbSender.SelectedIndex == 0 ? 0 : cbSender.SelectedIndex == 1 ? 61 : 1;
			else if (tag == "Magic")
				FunkyTownRunPlugin.PluginSettings.SalvageMagicItemLevel = cbSender.SelectedIndex == 0 ? 0 : cbSender.SelectedIndex == 1 ? 61 : 1;
			else if (tag == "Rare")
				FunkyTownRunPlugin.PluginSettings.SalvageRareItemLevel = cbSender.SelectedIndex == 0 ? 0 : cbSender.SelectedIndex == 1 ? 61 : 1;
			else if (tag == "Legendary")
				FunkyTownRunPlugin.PluginSettings.SalvageLegendaryItemLevel = cbSender.SelectedIndex == 0 ? 0 : cbSender.SelectedIndex == 1 ? 61 : 1;
		}

		private void BloodShardGamblingChecked(object sender, EventArgs e)
		{
			FunkyTownRunPlugin.PluginSettings.EnableBloodShardGambling = !FunkyTownRunPlugin.PluginSettings.EnableBloodShardGambling;
			panel_Gambling.Enabled = FunkyTownRunPlugin.PluginSettings.EnableBloodShardGambling;
		}
		private void MinimumBloodShardSliderChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = Convert.ToInt32(slider_sender.Value);
			FunkyTownRunPlugin.PluginSettings.MinimumBloodShards = Value;
			textBox_GamblingBloodShards.Text = Value.ToString();
		}

		private void BloodShardGambleItemsChanged(object sender, EventArgs e)
		{
			CheckBox cbSender = (CheckBox)sender;
			BloodShardGambleItems LogLevelValue = (BloodShardGambleItems)Enum.Parse(typeof(BloodShardGambleItems), cbSender.Name);

			if (FunkyTownRunPlugin.PluginSettings.BloodShardGambleItems.HasFlag(LogLevelValue))
				FunkyTownRunPlugin.PluginSettings.BloodShardGambleItems &= ~LogLevelValue;
			else
				FunkyTownRunPlugin.PluginSettings.BloodShardGambleItems |= LogLevelValue;
		}

		private void formSettings_FormClosing(object sender, FormClosingEventArgs e)
		{
			Settings.SerializeToXML(FunkyTownRunPlugin.PluginSettings);

			if (BotMain.IsRunning)
				FunkyTownRunPlugin.ItemRulesEval.reloadFromUI();

			base.OnClosed(e);
		}

	}
}
