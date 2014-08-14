using System;
using System.Drawing;
using System.Windows.Forms;

namespace fBaseXtensions.Settings
{
	public partial class UserControlDebugEntry : UserControl
	{
		public UserControlDebugEntry()
		{
			InitializeComponent();
		}
		public UserControlDebugEntry(string entryString, Color foreColor, Color backColor)
		{
			InitializeComponent();

			SetupColors(foreColor, backColor);
			SetupText(entryString);

			label1.DoubleClick += textBox1_MouseDoubleClick;
		}
		public UserControlDebugEntry(string entryString)
		{
			InitializeComponent();

			SetupText(entryString);


			label1.DoubleClick += textBox1_MouseDoubleClick;
		}

		private void SetupText(string text)
		{
			//if (text.Contains("\r\n"))
			//{
			//	int headerIndexEnd = text.IndexOf("\r\n", 0, StringComparison.InvariantCultureIgnoreCase);

			//	string headerText = text.Substring(0, headerIndexEnd);
			//	string bodyText = text.Substring(headerIndexEnd + 1);
			//	label2.Text = headerText;
			//	label1.Text = bodyText;
			//	label1.Hide();
			//}
			//else
			//{
			
			label1.Text = text;
				//label2.Hide();
			//}

			//Force redraw
			//Invalidate();
		}
		private void SetupColors(Color foreColor, Color backColor)
		{
			ForeColor=foreColor;
			BackColor=backColor;

			label1.ForeColor = foreColor;
			label1.BackColor = backColor;
		}

		private void textBox1_MouseDoubleClick(object sender, EventArgs e)
		{
			Clipboard.SetText(label1.Text);
		}

		private void UserControlDebugEntry_Load(object sender, EventArgs e)
		{

		}

		private void button1_Click_1(object sender, EventArgs e)
		{
			if (label1.Visible) label1.Hide(); else label1.Show();
		}
	}
}
