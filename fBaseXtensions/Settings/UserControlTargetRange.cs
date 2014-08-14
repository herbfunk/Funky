using System;
using System.Windows.Forms;

namespace fBaseXtensions.Settings
{
	public partial class UserControlTargetRange : UserControl
	{
		private int Setting;
		public UserControlTargetRange(int setting, string labelName)
		{
			Setting=setting;
			InitializeComponent();
			label1.Text = labelName;
			trackBar1.Value = Setting;
			textBox1.Text = Setting.ToString();
			trackBar1.ValueChanged += trackBar1_ValueChanged;

		}

		private void trackBar1_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = slider_sender.Value;
			Setting = Value;
			textBox1.Text = Value.ToString();
			UpdateValue.Invoke(Value);


		}

		public Action<int> UpdateValue { get; set; } 
	}
}
