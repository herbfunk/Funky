using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FunkyBot.Config.UI
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
			label1.Text=entryString;
			label1.ForeColor = foreColor;
			label1.BackColor = backColor;
			label1.DoubleClick += textBox1_MouseDoubleClick;
		}
		public UserControlDebugEntry(string entryString)
		{
			InitializeComponent();
			label1.Text = entryString;
			label1.ForeColor = Color.White;
			label1.BackColor = Color.Black;
			label1.DoubleClick += textBox1_MouseDoubleClick;
		}

		private void textBox1_MouseDoubleClick(object sender, EventArgs e)
		{
			Clipboard.SetText(label1.Text);
		}

		private void UserControlDebugEntry_Load(object sender, EventArgs e)
		{

		}
	}
}
