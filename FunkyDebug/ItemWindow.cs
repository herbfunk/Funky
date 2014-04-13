using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FunkyDebug
{
	public partial class ItemWindow : Form
	{
		public ItemWindow()
		{
			InitializeComponent();
		}

		public ItemWindow(CacheACDItem item)
		{
			InitializeComponent();
			string itemRulesString=FunkyDebug.FunkyDebugger.ItemRules.getFullItem(item);
			lbl_ItemText.Text = String.Format(item.ItemStatString + "\r\n" + itemRulesString);
		}
	}
}
