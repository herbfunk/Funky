using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FunkyTrinity.Enums;

namespace FunkyTrinity
{
	 internal partial class FunkyWindow : Window
	 {

		 internal void InitAvoidanceControls()
		 {
			  TabItem AvoidanceTabItem=new TabItem
			  {
					Header="Avoidances",
					HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
					VerticalAlignment=System.Windows.VerticalAlignment.Stretch,
			  };
			  AvoidanceTabItem.Header="Avoidances";
			  CombatTabControl.Items.Add(AvoidanceTabItem);
			  ListBox LBcharacterAvoidance=new ListBox
			  {
					HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
					VerticalAlignment=System.Windows.VerticalAlignment.Stretch,

			  };




			  Grid AvoidanceLayoutGrid=new Grid
			  {
					UseLayoutRounding=true,
					ShowGridLines=false,
					VerticalAlignment=System.Windows.VerticalAlignment.Stretch,
					HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
					FlowDirection=System.Windows.FlowDirection.LeftToRight,
					Focusable=false,
			  };

			  ColumnDefinition colDef1=new ColumnDefinition();
			  ColumnDefinition colDef2=new ColumnDefinition();
			  ColumnDefinition colDef3=new ColumnDefinition();
			  AvoidanceLayoutGrid.ColumnDefinitions.Add(colDef1);
			  AvoidanceLayoutGrid.ColumnDefinitions.Add(colDef2);
			  AvoidanceLayoutGrid.ColumnDefinitions.Add(colDef3);
			  RowDefinition rowDef1=new RowDefinition();
			  AvoidanceLayoutGrid.RowDefinitions.Add(rowDef1);

			  TextBlock ColumnHeader1=new TextBlock
			  {
					Text="Type",
					FontSize=12,
					TextAlignment=System.Windows.TextAlignment.Center,
					Background=System.Windows.Media.Brushes.DarkTurquoise,
					Foreground=System.Windows.Media.Brushes.GhostWhite,
			  };
			  TextBlock ColumnHeader2=new TextBlock
			  {
					Text="Radius",
					FontSize=12,
					TextAlignment=System.Windows.TextAlignment.Center,
					Background=System.Windows.Media.Brushes.DarkGoldenrod,
					Foreground=System.Windows.Media.Brushes.GhostWhite,
			  };
			  TextBlock ColumnHeader3=new TextBlock
			  {
					Text="Health",
					FontSize=12,
					TextAlignment=System.Windows.TextAlignment.Center,
					Background=System.Windows.Media.Brushes.DarkRed,
					Foreground=System.Windows.Media.Brushes.GhostWhite,
			  };
			  Grid.SetColumn(ColumnHeader1, 0);
			  Grid.SetColumn(ColumnHeader2, 1);
			  Grid.SetColumn(ColumnHeader3, 2);
			  Grid.SetRow(ColumnHeader1, 0);
			  Grid.SetRow(ColumnHeader2, 0);
			  Grid.SetRow(ColumnHeader3, 0);
			  AvoidanceLayoutGrid.Children.Add(ColumnHeader1);
			  AvoidanceLayoutGrid.Children.Add(ColumnHeader2);
			  AvoidanceLayoutGrid.Children.Add(ColumnHeader3);

			  Dictionary<AvoidanceType, double> currentDictionaryAvoidance=Funky.ReturnDictionaryUsingActorClass(Bot.ActorClass);
			  AvoidanceType[] avoidanceTypes=currentDictionaryAvoidance.Keys.ToArray();
			  TBavoidanceHealth=new TextBox[avoidanceTypes.Length-1];
			  TBavoidanceRadius=new TextBox[avoidanceTypes.Length-1];
			  int alternatingColor=0;

			  for (int i=0; i<avoidanceTypes.Length-1; i++)
			  {
					if (alternatingColor>1) alternatingColor=0;

					string avoidanceString=avoidanceTypes[i].ToString();

					float defaultRadius=0f;
					Funky.dictAvoidanceRadius.TryGetValue(avoidanceTypes[i], out defaultRadius);
					Slider avoidanceRadius=new Slider
					{
						 Width=125,
						 Name=avoidanceString+"_radius_"+i.ToString(),
						 Maximum=30,
						 Minimum=0,
						 TickFrequency=5,
						 LargeChange=5,
						 SmallChange=1,
						 Value=defaultRadius,
						 HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
						 VerticalAlignment=System.Windows.VerticalAlignment.Center,
						 //Padding=new Thickness(2),
						 Margin=new Thickness(5),
					};
					avoidanceRadius.ValueChanged+=AvoidanceRadiusSliderValueChanged;
					TBavoidanceRadius[i]=new TextBox
					{
						 Text=defaultRadius.ToString(),
						 IsReadOnly=true,
						 VerticalAlignment=System.Windows.VerticalAlignment.Top,
						 HorizontalAlignment=System.Windows.HorizontalAlignment.Right,
					};

					double defaultHealth=0d;
					Funky.ReturnDictionaryUsingActorClass(Bot.ActorClass).TryGetValue(avoidanceTypes[i], out defaultHealth);
					Slider avoidanceHealth=new Slider
					{
						 Name=avoidanceString+"_health_"+i.ToString(),
						 Width=125,
						 Maximum=1,
						 Minimum=0,
						 TickFrequency=0.10,
						 LargeChange=0.10,
						 SmallChange=0.05,
						 Value=defaultHealth,
						 HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
						 VerticalAlignment=System.Windows.VerticalAlignment.Center,
						 Margin=new Thickness(5),
					};
					avoidanceHealth.ValueChanged+=AvoidanceHealthSliderValueChanged;
					TBavoidanceHealth[i]=new TextBox
					{
						 Text=defaultHealth.ToString("F2", CultureInfo.InvariantCulture),
						 IsReadOnly=true,
						 VerticalAlignment=System.Windows.VerticalAlignment.Top,
						 HorizontalAlignment=System.Windows.HorizontalAlignment.Right,
					};

					RowDefinition newRow=new RowDefinition();
					AvoidanceLayoutGrid.RowDefinitions.Add(newRow);


					TextBlock txt1=new TextBlock
					{
						 Text=avoidanceString,
						 FontSize=12,
						 VerticalAlignment=System.Windows.VerticalAlignment.Stretch,
						 Background=alternatingColor==0?System.Windows.Media.Brushes.DarkSeaGreen:Background=System.Windows.Media.Brushes.SlateGray,
						 Foreground=System.Windows.Media.Brushes.GhostWhite,
						 FontStretch=FontStretches.SemiCondensed,
					};

					StackPanel avoidRadiusStackPanel=new StackPanel
					{
						 Width=175,
						 Height=25,
						 Orientation=Orientation.Horizontal,
						 Background=alternatingColor==0?System.Windows.Media.Brushes.DarkSeaGreen:Background=System.Windows.Media.Brushes.SlateGray,

					};
					avoidRadiusStackPanel.Children.Add(avoidanceRadius);
					avoidRadiusStackPanel.Children.Add(TBavoidanceRadius[i]);

					StackPanel avoidHealthStackPanel=new StackPanel
					{
						 Width=175,
						 Height=25,
						 Orientation=Orientation.Horizontal,
						 Background=alternatingColor==0?System.Windows.Media.Brushes.DarkSeaGreen:Background=System.Windows.Media.Brushes.SlateGray,

					};
					avoidHealthStackPanel.Children.Add(avoidanceHealth);
					avoidHealthStackPanel.Children.Add(TBavoidanceHealth[i]);

					Grid.SetColumn(txt1, 0);
					Grid.SetColumn(avoidRadiusStackPanel, 1);
					Grid.SetColumn(avoidHealthStackPanel, 2);

					int currentIndex=AvoidanceLayoutGrid.RowDefinitions.Count-1;
					Grid.SetRow(avoidRadiusStackPanel, currentIndex);
					Grid.SetRow(avoidHealthStackPanel, currentIndex);
					Grid.SetRow(txt1, currentIndex);

					AvoidanceLayoutGrid.Children.Add(txt1);
					AvoidanceLayoutGrid.Children.Add(avoidRadiusStackPanel);
					AvoidanceLayoutGrid.Children.Add(avoidHealthStackPanel);
					alternatingColor++;
			  }

			  LBcharacterAvoidance.Items.Add(AvoidanceLayoutGrid);


			  AvoidanceTabItem.Content=LBcharacterAvoidance;
		 }
	}
}
