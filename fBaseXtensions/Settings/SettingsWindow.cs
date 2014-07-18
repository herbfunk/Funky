using fBaseXtensions.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace fBaseXtensions.Settings
{
	public partial class SettingsWindow : Form
	{
		public SettingsWindow()
		{
			InitializeComponent();

			tb_GeneralGoldInactivityValue.Value = FunkyBaseExtension.Settings.Monitoring.GoldInactivityTimeoutSeconds;
			tb_GeneralGoldInactivityValue.ValueChanged += tb_GeneralGoldInactivityValue_ValueChanged;

			txt_GeneralGoldInactivityValue.Text = FunkyBaseExtension.Settings.Monitoring.GoldInactivityTimeoutSeconds.ToString();


			var LogLevels = Enum.GetValues(typeof(LogLevel));
			Func<object, string> fRetrieveLogLevelNames = s => Enum.GetName(typeof(LogLevel), s);
			bool noLogFlags = FunkyBaseExtension.Settings.Logging.LogFlags.Equals(LogLevel.None);
			foreach (var logLevel in LogLevels)
			{
				LogLevel thisloglevel = (LogLevel)logLevel;
				if (thisloglevel.Equals(LogLevel.None) || thisloglevel.Equals(LogLevel.All)) continue;

				string loglevelName = fRetrieveLogLevelNames(logLevel);
				CheckBox cb = new CheckBox
				{
					Name = loglevelName,
					Text = loglevelName,
					Checked = !noLogFlags && FunkyBaseExtension.Settings.Logging.LogFlags.HasFlag(thisloglevel),
				};
				cb.CheckedChanged += FunkyLogLevelChanged;

				flowLayout_DebugFunkyLogLevels.Controls.Add(cb);
			}
		}
		private void tb_GeneralGoldInactivityValue_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			FunkyBaseExtension.Settings.Monitoring.GoldInactivityTimeoutSeconds = Value;
			txt_GeneralGoldInactivityValue.Text = Value.ToString();
		}
		private void FunkyLogLevelChanged(object sender, EventArgs e)
		{
			CheckBox cbSender = (CheckBox)sender;
			LogLevel LogLevelValue = (LogLevel)Enum.Parse(typeof(LogLevel), cbSender.Name);

			if (FunkyBaseExtension.Settings.Logging.LogFlags.HasFlag(LogLevelValue))
				FunkyBaseExtension.Settings.Logging.LogFlags &= ~LogLevelValue;
			else
				FunkyBaseExtension.Settings.Logging.LogFlags |= LogLevelValue;
		}

		private void SettingsWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			Settings.PluginSettings.SerializeToXML(FunkyBaseExtension.Settings);
		}
	}
}
