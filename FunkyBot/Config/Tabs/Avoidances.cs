using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using FunkyBot.Cache;
using FunkyBot.Settings;
using Button = System.Windows.Controls.Button;
using CheckBox = System.Windows.Controls.CheckBox;
using FlowDirection = System.Windows.FlowDirection;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Orientation = System.Windows.Controls.Orientation;
using TextBox = System.Windows.Controls.TextBox;
using ToolTip = System.Windows.Controls.ToolTip;

namespace FunkyBot
{
	internal partial class FunkyWindow : Window
	{
		#region EventHandling
		private void AvoidanceLoadSettingsButtonClicked(object sender, EventArgs e)
		{

			OpenFileDialog OFD = new OpenFileDialog
			{
				InitialDirectory = Path.Combine(FolderPaths.SettingsDefaultPath, "Specific"),
				RestoreDirectory = false,
				Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*",
				Title = "Avoidance Template",
			};
			DialogResult OFD_Result = OFD.ShowDialog();

			if (OFD_Result == System.Windows.Forms.DialogResult.OK)
			{
				try
				{
					//;
					SettingAvoidance newSettings = SettingAvoidance.DeserializeFromXML(OFD.FileName);
					Bot.Settings.Avoidance = newSettings;
					funkyConfigWindow.Close();
				}
				catch
				{

				}
			}
		}
		private void AvoidanceRadiusSliderValueChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			string[] slider_info = slider_sender.Name.Split("_".ToCharArray());
			int tb_index = Convert.ToInt16(slider_info[2]);
			float currentValue = (int)slider_sender.Value;

			TBavoidanceRadius[tb_index].Text = currentValue.ToString();
			Bot.Settings.Avoidance.Avoidances[tb_index].Radius = currentValue;
			//AvoidanceType avoidancetype=(AvoidanceType)Enum.Parse(typeof(AvoidanceType), slider_info[0]);
			//Bot.SettingsFunky.Avoidance.AvoidanceRadiusValues[avoidancetype]=currentValue;
			//Bot.SettingsFunky.Avoidance.RecreateAvoidances();
		}

		private void AvoidanceHealthSliderValueChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			string[] slider_info = slider_sender.Name.Split("_".ToCharArray());
			double currentValue = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
			int tb_index = Convert.ToInt16(slider_info[2]);

			TBavoidanceHealth[tb_index].Text = currentValue.ToString();
			Bot.Settings.Avoidance.Avoidances[tb_index].Health = currentValue;
			//AvoidanceType avoidancetype=(AvoidanceType)Enum.Parse(typeof(AvoidanceType), slider_info[0]);
			//Bot.SettingsFunky.Avoidance.AvoidanceHealthValues[avoidancetype]=currentValue;
			//Bot.SettingsFunky.Avoidance.RecreateAvoidances();
		}

		private void AvoidanceWeightSliderValueChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			string[] slider_info = slider_sender.Name.Split("_".ToCharArray());
			int currentValue = Convert.ToInt16(slider_sender.Value);
			int tb_index = Convert.ToInt16(slider_info[2]);

			TBavoidanceWeight[tb_index].Text = currentValue.ToString();
			Bot.Settings.Avoidance.Avoidances[tb_index].Weight = currentValue;
			//AvoidanceType avoidancetype=(AvoidanceType)Enum.Parse(typeof(AvoidanceType), slider_info[0]);
			//Bot.SettingsFunky.Avoidance.AvoidanceHealthValues[avoidancetype]=currentValue;
			//Bot.SettingsFunky.Avoidance.RecreateAvoidances();
		}

		#endregion

		private TextBox[] TBavoidanceRadius;
		private TextBox[] TBavoidanceHealth;
		private TextBox[] TBavoidanceWeight;
		internal void InitAvoidanceControls()
		{
			TabItem AvoidanceTabItem = new TabItem
			{
				Header = "Avoidances",
			};
			AvoidanceTabItem.Header = "Avoidances";
			CombatTabControl.Items.Add(AvoidanceTabItem);
			StackPanel LBcharacterAvoidance = new StackPanel
			{
				Orientation = Orientation.Vertical,
			};
			#region Avoidances

			StackPanel AvoidanceOptionsStackPanel = new StackPanel
			{
				//Orientation= System.Windows.Controls.Orientation.Vertical,
				//HorizontalAlignment= System.Windows.HorizontalAlignment.Stretch,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
				Background = Brushes.DimGray,
			};

			TextBlock Avoidance_Text_Header = new TextBlock
			{
				Text = "Avoidances",
				FontSize = 12,
				Background = Brushes.MediumSeaGreen,
				TextAlignment = TextAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Stretch,

			};

			#region AvoidanceCheckboxes

			StackPanel AvoidanceCheckBoxesPanel = new StackPanel
			{
				Orientation = Orientation.Vertical,
			};

			CheckBox CBAttemptAvoidanceMovements = new CheckBox
			{
				Content = "Enable Avoidance",
				IsChecked = Bot.Settings.Avoidance.AttemptAvoidanceMovements,

			};
			CBAttemptAvoidanceMovements.Checked += AvoidanceAttemptMovementChecked;
			CBAttemptAvoidanceMovements.Unchecked += AvoidanceAttemptMovementChecked;

			CheckBox CBAdvancedProjectileTesting = new CheckBox
			{
				Content = "Use Advanced Avoidance Projectile Test",
				IsChecked = Bot.Settings.Avoidance.UseAdvancedProjectileTesting,
			};
			CBAdvancedProjectileTesting.Checked += UseAdvancedProjectileTestingChecked;
			CBAdvancedProjectileTesting.Unchecked += UseAdvancedProjectileTestingChecked;
			AvoidanceCheckBoxesPanel.Children.Add(CBAttemptAvoidanceMovements);
			AvoidanceCheckBoxesPanel.Children.Add(CBAdvancedProjectileTesting);
			#endregion;





			AvoidanceOptionsStackPanel.Children.Add(Avoidance_Text_Header);
			AvoidanceOptionsStackPanel.Children.Add(AvoidanceCheckBoxesPanel);
			LBcharacterAvoidance.Children.Add(AvoidanceOptionsStackPanel);
			#endregion



			Grid AvoidanceLayoutGrid = new Grid
			{
				ShowGridLines = false,
				//VerticalAlignment=System.Windows.VerticalAlignment.Stretch,
				//HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
				FlowDirection = FlowDirection.LeftToRight,
				Focusable = false,
			};

			ColumnDefinition colDef1 = new ColumnDefinition();
			ColumnDefinition colDef2 = new ColumnDefinition();
			ColumnDefinition colDef3 = new ColumnDefinition();
			ColumnDefinition colDef4 = new ColumnDefinition();
			AvoidanceLayoutGrid.ColumnDefinitions.Add(colDef1);
			AvoidanceLayoutGrid.ColumnDefinitions.Add(colDef2);
			AvoidanceLayoutGrid.ColumnDefinitions.Add(colDef3);
			AvoidanceLayoutGrid.ColumnDefinitions.Add(colDef4);
			RowDefinition rowDef1 = new RowDefinition();
			AvoidanceLayoutGrid.RowDefinitions.Add(rowDef1);

			TextBlock ColumnHeader1 = new TextBlock
			{
				Text = "Type",
				FontSize = 12,
				TextAlignment = TextAlignment.Center,
				Background = Brushes.DarkTurquoise,
				Foreground = Brushes.GhostWhite,
			};
			TextBlock ColumnHeader2 = new TextBlock
			{
				Text = "Radius",
				FontSize = 12,
				TextAlignment = TextAlignment.Center,
				Background = Brushes.DarkGoldenrod,
				Foreground = Brushes.GhostWhite,
			};
			TextBlock ColumnHeader3 = new TextBlock
			{
				Text = "Health",
				FontSize = 12,
				TextAlignment = TextAlignment.Center,
				Background = Brushes.DarkRed,
				Foreground = Brushes.GhostWhite,
			};


			ToolTip TT_AvoidanceWeight = new ToolTip
			{
				Content = "Weight compared to other avoidances (higher is deadlier)",
			};
			TextBlock ColumnHeader4 = new TextBlock
			{
				Text = "Weight",
				FontSize = 12,
				TextAlignment = TextAlignment.Center,
				Background = Brushes.DarkSlateBlue,
				Foreground = Brushes.GhostWhite,
				ToolTip = TT_AvoidanceWeight,
			};

			Grid.SetColumn(ColumnHeader1, 0);
			Grid.SetColumn(ColumnHeader2, 1);
			Grid.SetColumn(ColumnHeader3, 2);
			Grid.SetColumn(ColumnHeader4, 3);
			Grid.SetRow(ColumnHeader1, 0);
			Grid.SetRow(ColumnHeader2, 0);
			Grid.SetRow(ColumnHeader3, 0);
			Grid.SetRow(ColumnHeader4, 0);
			AvoidanceLayoutGrid.Children.Add(ColumnHeader1);
			AvoidanceLayoutGrid.Children.Add(ColumnHeader2);
			AvoidanceLayoutGrid.Children.Add(ColumnHeader3);
			AvoidanceLayoutGrid.Children.Add(ColumnHeader4);

			//Dictionary<AvoidanceType, double> currentDictionaryAvoidance=Bot.SettingsFunky.Avoidance.AvoidanceHealthValues;
			AvoidanceValue[] avoidanceValues = Bot.Settings.Avoidance.Avoidances.ToArray();
			TBavoidanceHealth = new TextBox[avoidanceValues.Length - 1];
			TBavoidanceRadius = new TextBox[avoidanceValues.Length - 1];
			TBavoidanceWeight = new TextBox[avoidanceValues.Length - 1];

			int alternatingColor = 0;

			for (int i = 0; i < avoidanceValues.Length - 1; i++)
			{
				if (alternatingColor > 1) alternatingColor = 0;

				string avoidanceString = avoidanceValues[i].Type.ToString();

				double defaultRadius = avoidanceValues[i].Radius;
				//Bot.SettingsFunky.Avoidance.AvoidanceRadiusValues.TryGetValue(avoidanceTypes[i], out defaultRadius);

				Slider avoidanceRadius = new Slider
				{
					Width = 100,
					Name = avoidanceString + "_radius_" + i.ToString(),
					Maximum = 30,
					Minimum = 0,
					TickFrequency = 5,
					LargeChange = 5,
					SmallChange = 1,
					Value = defaultRadius,
					HorizontalAlignment = HorizontalAlignment.Stretch,
					VerticalAlignment = VerticalAlignment.Center,
					//Padding=new Thickness(2),
					Margin = new Thickness(5),
				};
				avoidanceRadius.ValueChanged += AvoidanceRadiusSliderValueChanged;
				TBavoidanceRadius[i] = new TextBox
				{
					Text = defaultRadius.ToString(),
					IsReadOnly = true,
					VerticalAlignment = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Right,
				};

				double defaultHealth = avoidanceValues[i].Health;
				//Bot.SettingsFunky.Avoidance.AvoidanceHealthValues.TryGetValue(avoidanceTypes[i], out defaultHealth);
				Slider avoidanceHealth = new Slider
				{
					Name = avoidanceString + "_health_" + i.ToString(),
					Width = 100,
					Maximum = 1,
					Minimum = 0,
					TickFrequency = 0.10,
					LargeChange = 0.10,
					SmallChange = 0.05,
					Value = defaultHealth,
					HorizontalAlignment = HorizontalAlignment.Stretch,
					VerticalAlignment = VerticalAlignment.Center,
					Margin = new Thickness(5),
				};
				avoidanceHealth.ValueChanged += AvoidanceHealthSliderValueChanged;
				TBavoidanceHealth[i] = new TextBox
				{
					Text = defaultHealth.ToString("F2", CultureInfo.InvariantCulture),
					IsReadOnly = true,
					VerticalAlignment = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Right,
				};

				int defaultWeight = avoidanceValues[i].Weight;
				Slider avoidanceWeight = new Slider
				{
					Name = avoidanceString + "_weight_" + i.ToString(),
					Width = 100,
					Maximum = 20,
					Minimum = 0,
					TickFrequency = 2,
					LargeChange = 5,
					SmallChange = 1,
					Value = defaultWeight,
					HorizontalAlignment = HorizontalAlignment.Stretch,
					VerticalAlignment = VerticalAlignment.Center,
					Margin = new Thickness(5),
				};
				avoidanceWeight.ValueChanged += AvoidanceWeightSliderValueChanged;
				TBavoidanceWeight[i] = new TextBox
				{
					Text = defaultWeight.ToString("F2", CultureInfo.InvariantCulture),
					IsReadOnly = true,
					VerticalAlignment = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Right,
				};

				RowDefinition newRow = new RowDefinition();
				AvoidanceLayoutGrid.RowDefinitions.Add(newRow);


				TextBlock txt1 = new TextBlock
				{
					Text = avoidanceString,
					FontSize = 12,
					VerticalAlignment = VerticalAlignment.Stretch,
					HorizontalAlignment = HorizontalAlignment.Stretch,
					Background = alternatingColor == 0 ? Brushes.DarkSeaGreen : Background = Brushes.SlateGray,
					Foreground = Brushes.GhostWhite,
					FontStretch = FontStretches.Medium,
					TextAlignment = TextAlignment.Center,
				};

				StackPanel avoidRadiusStackPanel = new StackPanel
				{
					Width = 155,
					Height = 25,
					Orientation = Orientation.Horizontal,
					Background = alternatingColor == 0 ? Brushes.DarkSeaGreen : Background = Brushes.SlateGray,

				};
				avoidRadiusStackPanel.Children.Add(avoidanceRadius);
				avoidRadiusStackPanel.Children.Add(TBavoidanceRadius[i]);

				StackPanel avoidHealthStackPanel = new StackPanel
				{
					Width = 155,
					Height = 25,
					Orientation = Orientation.Horizontal,
					Background = alternatingColor == 0 ? Brushes.DarkSeaGreen : Background = Brushes.SlateGray,
				};
				avoidHealthStackPanel.Children.Add(avoidanceHealth);
				avoidHealthStackPanel.Children.Add(TBavoidanceHealth[i]);
				StackPanel avoidWeightStackPanel = new StackPanel
				{
					Width = 155,
					Height = 25,
					Orientation = Orientation.Horizontal,
					Background = alternatingColor == 0 ? Brushes.DarkSeaGreen : Background = Brushes.SlateGray,
				};
				avoidWeightStackPanel.Children.Add(avoidanceWeight);
				avoidWeightStackPanel.Children.Add(TBavoidanceWeight[i]);

				Grid.SetColumn(txt1, 0);
				Grid.SetColumn(avoidRadiusStackPanel, 1);
				Grid.SetColumn(avoidHealthStackPanel, 2);
				Grid.SetColumn(avoidWeightStackPanel, 3);

				int currentIndex = AvoidanceLayoutGrid.RowDefinitions.Count - 1;
				Grid.SetRow(avoidWeightStackPanel, currentIndex);
				Grid.SetRow(avoidRadiusStackPanel, currentIndex);
				Grid.SetRow(avoidHealthStackPanel, currentIndex);
				Grid.SetRow(txt1, currentIndex);

				AvoidanceLayoutGrid.Children.Add(txt1);
				AvoidanceLayoutGrid.Children.Add(avoidRadiusStackPanel);
				AvoidanceLayoutGrid.Children.Add(avoidHealthStackPanel);
				AvoidanceLayoutGrid.Children.Add(avoidWeightStackPanel);

				alternatingColor++;
			}
			ScrollViewer AvoidanceGridScrollViewer = new ScrollViewer
			{
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
			};


			LBcharacterAvoidance.Children.Add(AvoidanceLayoutGrid);

			Button BtnAvoidanceLoadTemplate = new Button
			{
				Content = "Load Setup",
				Background = Brushes.OrangeRed,
				Foreground = Brushes.GhostWhite,
				FontStyle = FontStyles.Italic,
				FontSize = 12,

				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Top,
				Width = 75,
				Height = 30,

				Margin = new Thickness(Margin.Left, Margin.Top + 5, Margin.Right, Margin.Bottom + 5),
			};
			BtnAvoidanceLoadTemplate.Click += AvoidanceLoadSettingsButtonClicked;
			LBcharacterAvoidance.Children.Add(BtnAvoidanceLoadTemplate);

			AvoidanceGridScrollViewer.Content = LBcharacterAvoidance;
			AvoidanceTabItem.Content = AvoidanceGridScrollViewer;
		}
	}
}