using System;
using System.Globalization;
using System.Windows.Forms;

namespace FunkyBot.Config.UI
{
	public partial class UserControlAvoidance : UserControl
	{
		private readonly int _index;
		public UserControlAvoidance(int Index)
		{
			_index=Index;
			InitializeComponent();

			lbl_AvoidanceName.Text = Bot.Settings.Avoidance.Avoidances[Index].Type.ToString();

			trackbar_Health.Value=(int)(Bot.Settings.Avoidance.Avoidances[Index].Health*100);
			this.trackbar_Health.ValueChanged += new System.EventHandler(this.trackbar_Health_ValueChanged);

			trackbar_Radius.Value = (int)Bot.Settings.Avoidance.Avoidances[Index].Radius;
			this.trackbar_Radius.ValueChanged += new System.EventHandler(this.trackbar_Radius_ValueChanged);

			trackbar_Weight.Value = Bot.Settings.Avoidance.Avoidances[Index].Weight;
			this.trackbar_Weight.ValueChanged += new System.EventHandler(this.trackbar_Weight_ValueChanged);

			tb_Health.Text = Bot.Settings.Avoidance.Avoidances[Index].Health.ToString("F2", CultureInfo.InvariantCulture);
			tb_Radius.Text = ((int)Bot.Settings.Avoidance.Avoidances[Index].Radius).ToString();
			tb_Weight.Text = Bot.Settings.Avoidance.Avoidances[Index].Weight.ToString();
		}

		private void trackbar_Radius_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			string[] slider_info = slider_sender.Name.Split("_".ToCharArray());

			float currentValue = (int)slider_sender.Value;
			tb_Radius.Text = currentValue.ToString();
			Bot.Settings.Avoidance.Avoidances[_index].Radius = currentValue;
		}

		private void trackbar_Health_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			string[] slider_info = slider_sender.Name.Split("_".ToCharArray());

			double currentValue = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture)) / 100;
			tb_Health.Text = currentValue.ToString("F2", CultureInfo.InvariantCulture);

			Bot.Settings.Avoidance.Avoidances[_index].Health = currentValue;
		}

		private void trackbar_Weight_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			string[] slider_info = slider_sender.Name.Split("_".ToCharArray());
			int currentValue = Convert.ToInt16(slider_sender.Value);
			
			tb_Weight.Text = currentValue.ToString();
			Bot.Settings.Avoidance.Avoidances[_index].Weight = currentValue;
		}
	}
}
