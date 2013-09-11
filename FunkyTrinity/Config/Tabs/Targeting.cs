using System.Windows;
using System.Windows.Controls;

namespace FunkyTrinity
{
	 internal partial class FunkyWindow : Window
	 {
		  internal void InitTargetingGeneralControls()
		  {
				TabItem TargetingMiscTabItem=new TabItem();

				TargetingMiscTabItem.Header="General";
				tcTargeting.Items.Add(TargetingMiscTabItem);
				ListBox Target_General_ContentListBox=new ListBox
				{
					 Focusable=false,
				};

				StackPanel Targeting_General_Options_Stackpanel=new StackPanel
				{
					 Orientation=Orientation.Vertical,
					 Focusable=false,
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
					 Background=System.Windows.Media.Brushes.DimGray,
				};
				TextBlock Target_General_Text=new TextBlock
				{
					 Text="General Targeting Options",
					 FontSize=13,
					 Background=System.Windows.Media.Brushes.OrangeRed,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 TextAlignment=TextAlignment.Center,
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
				};



				#region IgnoreElites
				CheckBox cbIgnoreElites=new CheckBox
				{
					 Content="Ignore Rare/Elite/Unique Monsters",
					 Width=300,
					 Height=30,
					 IsChecked=(Bot.SettingsFunky.Targeting.IgnoreAboveAverageMobs),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
				};
				cbIgnoreElites.Checked+=IgnoreEliteMonstersChecked;
				cbIgnoreElites.Unchecked+=IgnoreEliteMonstersChecked;
				#endregion

				#region IgnoreCorpses
				CheckBox cbIgnoreCorpses=new CheckBox
				{
					 Content="Ignore Looting Corpses",
					 Width=300,
					 Height=30,
					 IsChecked=(Bot.SettingsFunky.Targeting.IgnoreCorpses),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
				};
				cbIgnoreCorpses.Checked+=IgnoreCorpsesChecked;
				cbIgnoreCorpses.Unchecked+=IgnoreCorpsesChecked;
				#endregion

				#region ExtendedRepChestRange
				ToolTip TTExtendedRareChestRange=new System.Windows.Controls.ToolTip
				{
					 Content="This will use double the Container Range Setting for all rare chests.",
				};
				CheckBox UseExtendedRangeRepChestCB=new CheckBox
				{
					 Content="Increased range for rare chests",
					 Width=300,
					 Height=20,
					 IsChecked=(Bot.SettingsFunky.Targeting.UseExtendedRangeRepChest),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 ToolTip=TTExtendedRareChestRange,
				};
				UseExtendedRangeRepChestCB.Checked+=ExtendRangeRepChestChecked;
				UseExtendedRangeRepChestCB.Unchecked+=ExtendRangeRepChestChecked;
				#endregion

				#region GoblinPriority
				ToolTip TTGoblinPriority=new System.Windows.Controls.ToolTip
				{
					 Content="Note: Priority above normal will consider goblins as special objects",
				};
				StackPanel GoblinPriority_StackPanel=new StackPanel
				{
					 Orientation=Orientation.Horizontal,
					 ToolTip=TTGoblinPriority,
				};
				TextBlock Target_GoblinPriority_Text=new TextBlock
				{
					 Text="Goblin Priority",
					 FontSize=12,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 Margin=new Thickness(4),
				};
				GoblinPriority_StackPanel.Children.Add(Target_GoblinPriority_Text);
				ComboBox CBGoblinPriority=new ComboBox
				{
					 Height=25,
					 Width=300,
					 ItemsSource=new GoblinPriority(),
					 SelectedIndex=Bot.SettingsFunky.Targeting.GoblinPriority,
					 Margin=new Thickness(4),
				};
				CBGoblinPriority.SelectionChanged+=GoblinPriorityChanged;
				GoblinPriority_StackPanel.Children.Add(CBGoblinPriority);
				#endregion

				Targeting_General_Options_Stackpanel.Children.Add(Target_General_Text);
				Targeting_General_Options_Stackpanel.Children.Add(cbIgnoreElites);
				Targeting_General_Options_Stackpanel.Children.Add(cbIgnoreCorpses);
				Targeting_General_Options_Stackpanel.Children.Add(UseExtendedRangeRepChestCB);
				Targeting_General_Options_Stackpanel.Children.Add(GoblinPriority_StackPanel);
				Target_General_ContentListBox.Items.Add(Targeting_General_Options_Stackpanel);





				TargetingMiscTabItem.Content=Target_General_ContentListBox;
		  }
	}
}
