using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace FunkyBot
{
	internal partial class FunkyWindow : Window
	{
		#region EventHandling
		private void AdventureModeEnableNavigateChecked(object sender, EventArgs e)
		{
			Bot.Settings.AdventureMode.NavigatePointsOfInterest = !Bot.Settings.AdventureMode.NavigatePointsOfInterest;
		}


		#endregion


		private ListBox lbAdventureModeContent;

		internal void InitAdventureModeControls()
		{
			TabItem AdventureTab = new TabItem();
			AdventureTab.Header = "Adventure Mode";
			tcGeneral.Items.Add(AdventureTab);
			lbAdventureModeContent = new ListBox();

			#region Naviagate Points of Intrest
			CheckBox cbAdventureModeEnableNavigate = new CheckBox
			{
				Content = "Navigate Points of Interest",
				Width = 500,
				Height = 30,
				IsChecked = (Bot.Settings.AdventureMode.NavigatePointsOfInterest),
			};
			cbAdventureModeEnableNavigate.Checked += AdventureModeEnableNavigateChecked;
			cbAdventureModeEnableNavigate.Unchecked += AdventureModeEnableNavigateChecked;
			lbAdventureModeContent.Items.Add(cbAdventureModeEnableNavigate);
			#endregion

			AdventureTab.Content = lbAdventureModeContent;

		}
	}
}
