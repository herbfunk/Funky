namespace fItemPlugin
{
	partial class formSettings
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox_ItemRules = new System.Windows.Forms.GroupBox();
			this.panel_ItemRules = new System.Windows.Forms.Panel();
			this.groupBox_ItemRules_Misc = new System.Windows.Forms.GroupBox();
			this.checkBox_ItemRules_Debugging = new System.Windows.Forms.CheckBox();
			this.checkBox_ItemRules_ItemIDs = new System.Windows.Forms.CheckBox();
			this.groupBox_ItemRules_Logging = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.comboBox_ItemRules_Logging_Stashed = new System.Windows.Forms.ComboBox();
			this.groupBox_ItemRule_Rules = new System.Windows.Forms.GroupBox();
			this.comboBox_ItemRulesType = new System.Windows.Forms.ComboBox();
			this.panel_ItemRulesCustom = new System.Windows.Forms.Panel();
			this.textBox_ItemRulesCustomPath = new System.Windows.Forms.TextBox();
			this.button_ItemRulesCustomBrowse = new System.Windows.Forms.Button();
			this.checkBox_EnableItemRules = new System.Windows.Forms.CheckBox();
			this.groupBox_General = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBox_MaximumPotions = new System.Windows.Forms.TextBox();
			this.trackBar_MaximumPotions = new System.Windows.Forms.TrackBar();
			this.checkBox_IDLegendaries = new System.Windows.Forms.CheckBox();
			this.checkBox_StashHoradricCaches = new System.Windows.Forms.CheckBox();
			this.checkBox_UseItemManager = new System.Windows.Forms.CheckBox();
			this.groupBox_Salavage = new System.Windows.Forms.GroupBox();
			this.comboBox_SalvageLegendaryItems = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.comboBox_SalvageRareItems = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.comboBox_SalvageMagicItems = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.comboBox_SalvageWhiteItems = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox_Gambling = new System.Windows.Forms.GroupBox();
			this.panel_Gambling = new System.Windows.Forms.Panel();
			this.flowLayoutPanel_GamblingItemTypes = new System.Windows.Forms.FlowLayoutPanel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.label7 = new System.Windows.Forms.Label();
			this.textBox_GamblingBloodShards = new System.Windows.Forms.TextBox();
			this.trackBar_GamblingBloodShards = new System.Windows.Forms.TrackBar();
			this.checkBox_EnableGambling = new System.Windows.Forms.CheckBox();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.groupBox_ItemRules.SuspendLayout();
			this.panel_ItemRules.SuspendLayout();
			this.groupBox_ItemRules_Misc.SuspendLayout();
			this.groupBox_ItemRules_Logging.SuspendLayout();
			this.groupBox_ItemRule_Rules.SuspendLayout();
			this.panel_ItemRulesCustom.SuspendLayout();
			this.groupBox_General.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_MaximumPotions)).BeginInit();
			this.groupBox_Salavage.SuspendLayout();
			this.groupBox_Gambling.SuspendLayout();
			this.panel_Gambling.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_GamblingBloodShards)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox_ItemRules
			// 
			this.groupBox_ItemRules.Controls.Add(this.panel_ItemRules);
			this.groupBox_ItemRules.Controls.Add(this.checkBox_EnableItemRules);
			this.groupBox_ItemRules.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox_ItemRules.Location = new System.Drawing.Point(0, 0);
			this.groupBox_ItemRules.Margin = new System.Windows.Forms.Padding(4);
			this.groupBox_ItemRules.Name = "groupBox_ItemRules";
			this.groupBox_ItemRules.Padding = new System.Windows.Forms.Padding(4);
			this.groupBox_ItemRules.Size = new System.Drawing.Size(716, 248);
			this.groupBox_ItemRules.TabIndex = 0;
			this.groupBox_ItemRules.TabStop = false;
			this.groupBox_ItemRules.Text = "Item Rules";
			// 
			// panel_ItemRules
			// 
			this.panel_ItemRules.Controls.Add(this.groupBox_ItemRules_Misc);
			this.panel_ItemRules.Controls.Add(this.groupBox_ItemRules_Logging);
			this.panel_ItemRules.Controls.Add(this.groupBox_ItemRule_Rules);
			this.panel_ItemRules.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_ItemRules.Location = new System.Drawing.Point(4, 41);
			this.panel_ItemRules.Margin = new System.Windows.Forms.Padding(4);
			this.panel_ItemRules.Name = "panel_ItemRules";
			this.panel_ItemRules.Size = new System.Drawing.Size(708, 203);
			this.panel_ItemRules.TabIndex = 1;
			// 
			// groupBox_ItemRules_Misc
			// 
			this.groupBox_ItemRules_Misc.Controls.Add(this.checkBox_ItemRules_Debugging);
			this.groupBox_ItemRules_Misc.Controls.Add(this.checkBox_ItemRules_ItemIDs);
			this.groupBox_ItemRules_Misc.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox_ItemRules_Misc.Location = new System.Drawing.Point(0, 134);
			this.groupBox_ItemRules_Misc.Name = "groupBox_ItemRules_Misc";
			this.groupBox_ItemRules_Misc.Size = new System.Drawing.Size(708, 49);
			this.groupBox_ItemRules_Misc.TabIndex = 7;
			this.groupBox_ItemRules_Misc.TabStop = false;
			this.groupBox_ItemRules_Misc.Text = "Misc";
			// 
			// checkBox_ItemRules_Debugging
			// 
			this.checkBox_ItemRules_Debugging.AutoSize = true;
			this.checkBox_ItemRules_Debugging.Location = new System.Drawing.Point(118, 22);
			this.checkBox_ItemRules_Debugging.Name = "checkBox_ItemRules_Debugging";
			this.checkBox_ItemRules_Debugging.Size = new System.Drawing.Size(96, 21);
			this.checkBox_ItemRules_Debugging.TabIndex = 1;
			this.checkBox_ItemRules_Debugging.Text = "Debugging";
			this.checkBox_ItemRules_Debugging.UseVisualStyleBackColor = true;
			// 
			// checkBox_ItemRules_ItemIDs
			// 
			this.checkBox_ItemRules_ItemIDs.AutoSize = true;
			this.checkBox_ItemRules_ItemIDs.Location = new System.Drawing.Point(6, 22);
			this.checkBox_ItemRules_ItemIDs.Name = "checkBox_ItemRules_ItemIDs";
			this.checkBox_ItemRules_ItemIDs.Size = new System.Drawing.Size(106, 21);
			this.checkBox_ItemRules_ItemIDs.TabIndex = 0;
			this.checkBox_ItemRules_ItemIDs.Text = "Use Item IDs";
			this.checkBox_ItemRules_ItemIDs.UseVisualStyleBackColor = true;
			// 
			// groupBox_ItemRules_Logging
			// 
			this.groupBox_ItemRules_Logging.Controls.Add(this.label1);
			this.groupBox_ItemRules_Logging.Controls.Add(this.comboBox_ItemRules_Logging_Stashed);
			this.groupBox_ItemRules_Logging.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox_ItemRules_Logging.Location = new System.Drawing.Point(0, 63);
			this.groupBox_ItemRules_Logging.Name = "groupBox_ItemRules_Logging";
			this.groupBox_ItemRules_Logging.Size = new System.Drawing.Size(708, 71);
			this.groupBox_ItemRules_Logging.TabIndex = 6;
			this.groupBox_ItemRules_Logging.TabStop = false;
			this.groupBox_ItemRules_Logging.Text = "Logging";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 17);
			this.label1.TabIndex = 1;
			this.label1.Text = "Stashed";
			// 
			// comboBox_ItemRules_Logging_Stashed
			// 
			this.comboBox_ItemRules_Logging_Stashed.FormattingEnabled = true;
			this.comboBox_ItemRules_Logging_Stashed.Items.AddRange(new object[] {
            "Common",
            "Normal",
            "Magic",
            "Rare",
            "Legendary"});
			this.comboBox_ItemRules_Logging_Stashed.Location = new System.Drawing.Point(11, 39);
			this.comboBox_ItemRules_Logging_Stashed.Name = "comboBox_ItemRules_Logging_Stashed";
			this.comboBox_ItemRules_Logging_Stashed.Size = new System.Drawing.Size(121, 24);
			this.comboBox_ItemRules_Logging_Stashed.TabIndex = 0;
			// 
			// groupBox_ItemRule_Rules
			// 
			this.groupBox_ItemRule_Rules.Controls.Add(this.comboBox_ItemRulesType);
			this.groupBox_ItemRule_Rules.Controls.Add(this.panel_ItemRulesCustom);
			this.groupBox_ItemRule_Rules.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox_ItemRule_Rules.Location = new System.Drawing.Point(0, 0);
			this.groupBox_ItemRule_Rules.Name = "groupBox_ItemRule_Rules";
			this.groupBox_ItemRule_Rules.Size = new System.Drawing.Size(708, 63);
			this.groupBox_ItemRule_Rules.TabIndex = 5;
			this.groupBox_ItemRule_Rules.TabStop = false;
			this.groupBox_ItemRule_Rules.Text = "Rule Set";
			// 
			// comboBox_ItemRulesType
			// 
			this.comboBox_ItemRulesType.FormattingEnabled = true;
			this.comboBox_ItemRulesType.Items.AddRange(new object[] {
            "Custom",
            "Soft",
            "Hard"});
			this.comboBox_ItemRulesType.Location = new System.Drawing.Point(7, 22);
			this.comboBox_ItemRulesType.Margin = new System.Windows.Forms.Padding(4);
			this.comboBox_ItemRulesType.Name = "comboBox_ItemRulesType";
			this.comboBox_ItemRulesType.Size = new System.Drawing.Size(160, 24);
			this.comboBox_ItemRulesType.TabIndex = 0;
			// 
			// panel_ItemRulesCustom
			// 
			this.panel_ItemRulesCustom.Controls.Add(this.textBox_ItemRulesCustomPath);
			this.panel_ItemRulesCustom.Controls.Add(this.button_ItemRulesCustomBrowse);
			this.panel_ItemRulesCustom.Location = new System.Drawing.Point(174, 22);
			this.panel_ItemRulesCustom.Name = "panel_ItemRulesCustom";
			this.panel_ItemRulesCustom.Size = new System.Drawing.Size(468, 34);
			this.panel_ItemRulesCustom.TabIndex = 3;
			// 
			// textBox_ItemRulesCustomPath
			// 
			this.textBox_ItemRulesCustomPath.Location = new System.Drawing.Point(3, 3);
			this.textBox_ItemRulesCustomPath.Name = "textBox_ItemRulesCustomPath";
			this.textBox_ItemRulesCustomPath.ReadOnly = true;
			this.textBox_ItemRulesCustomPath.Size = new System.Drawing.Size(381, 23);
			this.textBox_ItemRulesCustomPath.TabIndex = 1;
			// 
			// button_ItemRulesCustomBrowse
			// 
			this.button_ItemRulesCustomBrowse.Location = new System.Drawing.Point(390, 3);
			this.button_ItemRulesCustomBrowse.Name = "button_ItemRulesCustomBrowse";
			this.button_ItemRulesCustomBrowse.Size = new System.Drawing.Size(75, 23);
			this.button_ItemRulesCustomBrowse.TabIndex = 2;
			this.button_ItemRulesCustomBrowse.Text = "Browse";
			this.button_ItemRulesCustomBrowse.UseVisualStyleBackColor = true;
			this.button_ItemRulesCustomBrowse.Click += new System.EventHandler(this.ItemRulesBrowse_Click);
			// 
			// checkBox_EnableItemRules
			// 
			this.checkBox_EnableItemRules.AutoSize = true;
			this.checkBox_EnableItemRules.Dock = System.Windows.Forms.DockStyle.Top;
			this.checkBox_EnableItemRules.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkBox_EnableItemRules.Location = new System.Drawing.Point(4, 20);
			this.checkBox_EnableItemRules.Margin = new System.Windows.Forms.Padding(4);
			this.checkBox_EnableItemRules.Name = "checkBox_EnableItemRules";
			this.checkBox_EnableItemRules.Size = new System.Drawing.Size(708, 21);
			this.checkBox_EnableItemRules.TabIndex = 0;
			this.checkBox_EnableItemRules.Text = "Enable Item Rules";
			this.checkBox_EnableItemRules.UseVisualStyleBackColor = true;
			// 
			// groupBox_General
			// 
			this.groupBox_General.Controls.Add(this.label2);
			this.groupBox_General.Controls.Add(this.textBox_MaximumPotions);
			this.groupBox_General.Controls.Add(this.trackBar_MaximumPotions);
			this.groupBox_General.Controls.Add(this.checkBox_IDLegendaries);
			this.groupBox_General.Controls.Add(this.checkBox_StashHoradricCaches);
			this.groupBox_General.Controls.Add(this.checkBox_UseItemManager);
			this.groupBox_General.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox_General.Location = new System.Drawing.Point(0, 248);
			this.groupBox_General.Name = "groupBox_General";
			this.groupBox_General.Size = new System.Drawing.Size(716, 106);
			this.groupBox_General.TabIndex = 1;
			this.groupBox_General.TabStop = false;
			this.groupBox_General.Text = "General";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 46);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(117, 17);
			this.label2.TabIndex = 5;
			this.label2.Text = "Maximum Potions";
			// 
			// textBox_MaximumPotions
			// 
			this.textBox_MaximumPotions.Location = new System.Drawing.Point(205, 66);
			this.textBox_MaximumPotions.Name = "textBox_MaximumPotions";
			this.textBox_MaximumPotions.ReadOnly = true;
			this.textBox_MaximumPotions.Size = new System.Drawing.Size(77, 23);
			this.textBox_MaximumPotions.TabIndex = 4;
			// 
			// trackBar_MaximumPotions
			// 
			this.trackBar_MaximumPotions.LargeChange = 10;
			this.trackBar_MaximumPotions.Location = new System.Drawing.Point(6, 66);
			this.trackBar_MaximumPotions.Maximum = 100;
			this.trackBar_MaximumPotions.Name = "trackBar_MaximumPotions";
			this.trackBar_MaximumPotions.Size = new System.Drawing.Size(193, 42);
			this.trackBar_MaximumPotions.TabIndex = 3;
			this.trackBar_MaximumPotions.TickFrequency = 25;
			// 
			// checkBox_IDLegendaries
			// 
			this.checkBox_IDLegendaries.AutoSize = true;
			this.checkBox_IDLegendaries.Location = new System.Drawing.Point(332, 22);
			this.checkBox_IDLegendaries.Name = "checkBox_IDLegendaries";
			this.checkBox_IDLegendaries.Size = new System.Drawing.Size(155, 21);
			this.checkBox_IDLegendaries.TabIndex = 2;
			this.checkBox_IDLegendaries.Text = "Identify Legendaries";
			this.checkBox_IDLegendaries.UseVisualStyleBackColor = true;
			// 
			// checkBox_StashHoradricCaches
			// 
			this.checkBox_StashHoradricCaches.AutoSize = true;
			this.checkBox_StashHoradricCaches.Location = new System.Drawing.Point(154, 22);
			this.checkBox_StashHoradricCaches.Name = "checkBox_StashHoradricCaches";
			this.checkBox_StashHoradricCaches.Size = new System.Drawing.Size(172, 21);
			this.checkBox_StashHoradricCaches.TabIndex = 1;
			this.checkBox_StashHoradricCaches.Text = "Stash Horadric Caches";
			this.checkBox_StashHoradricCaches.UseVisualStyleBackColor = true;
			// 
			// checkBox_UseItemManager
			// 
			this.checkBox_UseItemManager.AutoSize = true;
			this.checkBox_UseItemManager.Location = new System.Drawing.Point(6, 22);
			this.checkBox_UseItemManager.Name = "checkBox_UseItemManager";
			this.checkBox_UseItemManager.Size = new System.Drawing.Size(142, 21);
			this.checkBox_UseItemManager.TabIndex = 0;
			this.checkBox_UseItemManager.Text = "Use Item Manager";
			this.checkBox_UseItemManager.UseVisualStyleBackColor = true;
			// 
			// groupBox_Salavage
			// 
			this.groupBox_Salavage.Controls.Add(this.comboBox_SalvageLegendaryItems);
			this.groupBox_Salavage.Controls.Add(this.label6);
			this.groupBox_Salavage.Controls.Add(this.comboBox_SalvageRareItems);
			this.groupBox_Salavage.Controls.Add(this.label5);
			this.groupBox_Salavage.Controls.Add(this.comboBox_SalvageMagicItems);
			this.groupBox_Salavage.Controls.Add(this.label4);
			this.groupBox_Salavage.Controls.Add(this.comboBox_SalvageWhiteItems);
			this.groupBox_Salavage.Controls.Add(this.label3);
			this.groupBox_Salavage.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox_Salavage.Location = new System.Drawing.Point(0, 354);
			this.groupBox_Salavage.Name = "groupBox_Salavage";
			this.groupBox_Salavage.Size = new System.Drawing.Size(716, 82);
			this.groupBox_Salavage.TabIndex = 2;
			this.groupBox_Salavage.TabStop = false;
			this.groupBox_Salavage.Text = "Salavge Options";
			// 
			// comboBox_SalvageLegendaryItems
			// 
			this.comboBox_SalvageLegendaryItems.FormattingEnabled = true;
			this.comboBox_SalvageLegendaryItems.Items.AddRange(new object[] {
            "None",
            "ROS",
            "All"});
			this.comboBox_SalvageLegendaryItems.Location = new System.Drawing.Point(396, 48);
			this.comboBox_SalvageLegendaryItems.Name = "comboBox_SalvageLegendaryItems";
			this.comboBox_SalvageLegendaryItems.Size = new System.Drawing.Size(121, 24);
			this.comboBox_SalvageLegendaryItems.TabIndex = 7;
			this.comboBox_SalvageLegendaryItems.Tag = "Legendary";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(393, 28);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(113, 17);
			this.label6.TabIndex = 6;
			this.label6.Text = "Legendary Items";
			// 
			// comboBox_SalvageRareItems
			// 
			this.comboBox_SalvageRareItems.FormattingEnabled = true;
			this.comboBox_SalvageRareItems.Items.AddRange(new object[] {
            "None",
            "ROS",
            "All"});
			this.comboBox_SalvageRareItems.Location = new System.Drawing.Point(269, 48);
			this.comboBox_SalvageRareItems.Name = "comboBox_SalvageRareItems";
			this.comboBox_SalvageRareItems.Size = new System.Drawing.Size(121, 24);
			this.comboBox_SalvageRareItems.TabIndex = 5;
			this.comboBox_SalvageRareItems.Tag = "Rare";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(266, 28);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(76, 17);
			this.label5.TabIndex = 4;
			this.label5.Text = "Rare Items";
			// 
			// comboBox_SalvageMagicItems
			// 
			this.comboBox_SalvageMagicItems.FormattingEnabled = true;
			this.comboBox_SalvageMagicItems.Items.AddRange(new object[] {
            "None",
            "ROS",
            "All"});
			this.comboBox_SalvageMagicItems.Location = new System.Drawing.Point(142, 48);
			this.comboBox_SalvageMagicItems.Name = "comboBox_SalvageMagicItems";
			this.comboBox_SalvageMagicItems.Size = new System.Drawing.Size(121, 24);
			this.comboBox_SalvageMagicItems.TabIndex = 3;
			this.comboBox_SalvageMagicItems.Tag = "Magic";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(139, 28);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(82, 17);
			this.label4.TabIndex = 2;
			this.label4.Text = "Magic Items";
			// 
			// comboBox_SalvageWhiteItems
			// 
			this.comboBox_SalvageWhiteItems.FormattingEnabled = true;
			this.comboBox_SalvageWhiteItems.Items.AddRange(new object[] {
            "None",
            "ROS",
            "All"});
			this.comboBox_SalvageWhiteItems.Location = new System.Drawing.Point(15, 48);
			this.comboBox_SalvageWhiteItems.Name = "comboBox_SalvageWhiteItems";
			this.comboBox_SalvageWhiteItems.Size = new System.Drawing.Size(121, 24);
			this.comboBox_SalvageWhiteItems.TabIndex = 1;
			this.comboBox_SalvageWhiteItems.Tag = "White";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 28);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(81, 17);
			this.label3.TabIndex = 0;
			this.label3.Text = "White Items";
			// 
			// groupBox_Gambling
			// 
			this.groupBox_Gambling.Controls.Add(this.panel_Gambling);
			this.groupBox_Gambling.Controls.Add(this.checkBox_EnableGambling);
			this.groupBox_Gambling.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox_Gambling.Location = new System.Drawing.Point(0, 436);
			this.groupBox_Gambling.Name = "groupBox_Gambling";
			this.groupBox_Gambling.Size = new System.Drawing.Size(716, 213);
			this.groupBox_Gambling.TabIndex = 3;
			this.groupBox_Gambling.TabStop = false;
			this.groupBox_Gambling.Text = "Gambling";
			// 
			// panel_Gambling
			// 
			this.panel_Gambling.Controls.Add(this.flowLayoutPanel_GamblingItemTypes);
			this.panel_Gambling.Controls.Add(this.panel2);
			this.panel_Gambling.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Gambling.Location = new System.Drawing.Point(3, 40);
			this.panel_Gambling.Name = "panel_Gambling";
			this.panel_Gambling.Size = new System.Drawing.Size(710, 170);
			this.panel_Gambling.TabIndex = 1;
			// 
			// flowLayoutPanel_GamblingItemTypes
			// 
			this.flowLayoutPanel_GamblingItemTypes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel_GamblingItemTypes.Location = new System.Drawing.Point(0, 58);
			this.flowLayoutPanel_GamblingItemTypes.Name = "flowLayoutPanel_GamblingItemTypes";
			this.flowLayoutPanel_GamblingItemTypes.Size = new System.Drawing.Size(710, 112);
			this.flowLayoutPanel_GamblingItemTypes.TabIndex = 9;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.label7);
			this.panel2.Controls.Add(this.textBox_GamblingBloodShards);
			this.panel2.Controls.Add(this.trackBar_GamblingBloodShards);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(710, 58);
			this.panel2.TabIndex = 10;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(3, 0);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(208, 17);
			this.label7.TabIndex = 11;
			this.label7.Text = "Minimum Bloodshards Required";
			// 
			// textBox_GamblingBloodShards
			// 
			this.textBox_GamblingBloodShards.Location = new System.Drawing.Point(200, 20);
			this.textBox_GamblingBloodShards.Name = "textBox_GamblingBloodShards";
			this.textBox_GamblingBloodShards.ReadOnly = true;
			this.textBox_GamblingBloodShards.Size = new System.Drawing.Size(77, 23);
			this.textBox_GamblingBloodShards.TabIndex = 10;
			// 
			// trackBar_GamblingBloodShards
			// 
			this.trackBar_GamblingBloodShards.LargeChange = 10;
			this.trackBar_GamblingBloodShards.Location = new System.Drawing.Point(6, 20);
			this.trackBar_GamblingBloodShards.Maximum = 500;
			this.trackBar_GamblingBloodShards.Minimum = 5;
			this.trackBar_GamblingBloodShards.Name = "trackBar_GamblingBloodShards";
			this.trackBar_GamblingBloodShards.Size = new System.Drawing.Size(193, 42);
			this.trackBar_GamblingBloodShards.TabIndex = 9;
			this.trackBar_GamblingBloodShards.TickFrequency = 25;
			this.trackBar_GamblingBloodShards.Value = 5;
			// 
			// checkBox_EnableGambling
			// 
			this.checkBox_EnableGambling.AutoSize = true;
			this.checkBox_EnableGambling.Dock = System.Windows.Forms.DockStyle.Top;
			this.checkBox_EnableGambling.Location = new System.Drawing.Point(3, 19);
			this.checkBox_EnableGambling.Name = "checkBox_EnableGambling";
			this.checkBox_EnableGambling.Size = new System.Drawing.Size(710, 21);
			this.checkBox_EnableGambling.TabIndex = 0;
			this.checkBox_EnableGambling.Text = "Enable Gambling";
			this.checkBox_EnableGambling.UseVisualStyleBackColor = true;
			// 
			// formSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(716, 650);
			this.Controls.Add(this.groupBox_Gambling);
			this.Controls.Add(this.groupBox_Salavage);
			this.Controls.Add(this.groupBox_General);
			this.Controls.Add(this.groupBox_ItemRules);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "formSettings";
			this.Text = "Settings";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formSettings_FormClosing);
			this.groupBox_ItemRules.ResumeLayout(false);
			this.groupBox_ItemRules.PerformLayout();
			this.panel_ItemRules.ResumeLayout(false);
			this.groupBox_ItemRules_Misc.ResumeLayout(false);
			this.groupBox_ItemRules_Misc.PerformLayout();
			this.groupBox_ItemRules_Logging.ResumeLayout(false);
			this.groupBox_ItemRules_Logging.PerformLayout();
			this.groupBox_ItemRule_Rules.ResumeLayout(false);
			this.panel_ItemRulesCustom.ResumeLayout(false);
			this.panel_ItemRulesCustom.PerformLayout();
			this.groupBox_General.ResumeLayout(false);
			this.groupBox_General.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_MaximumPotions)).EndInit();
			this.groupBox_Salavage.ResumeLayout(false);
			this.groupBox_Salavage.PerformLayout();
			this.groupBox_Gambling.ResumeLayout(false);
			this.groupBox_Gambling.PerformLayout();
			this.panel_Gambling.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_GamblingBloodShards)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox_ItemRules;
		private System.Windows.Forms.Panel panel_ItemRules;
		private System.Windows.Forms.GroupBox groupBox_ItemRules_Misc;
		private System.Windows.Forms.CheckBox checkBox_ItemRules_Debugging;
		private System.Windows.Forms.CheckBox checkBox_ItemRules_ItemIDs;
		private System.Windows.Forms.GroupBox groupBox_ItemRules_Logging;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBox_ItemRules_Logging_Stashed;
		private System.Windows.Forms.GroupBox groupBox_ItemRule_Rules;
		private System.Windows.Forms.ComboBox comboBox_ItemRulesType;
		private System.Windows.Forms.Panel panel_ItemRulesCustom;
		private System.Windows.Forms.TextBox textBox_ItemRulesCustomPath;
		private System.Windows.Forms.Button button_ItemRulesCustomBrowse;
		private System.Windows.Forms.CheckBox checkBox_EnableItemRules;
		private System.Windows.Forms.GroupBox groupBox_General;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox_MaximumPotions;
		private System.Windows.Forms.TrackBar trackBar_MaximumPotions;
		private System.Windows.Forms.CheckBox checkBox_IDLegendaries;
		private System.Windows.Forms.CheckBox checkBox_StashHoradricCaches;
		private System.Windows.Forms.CheckBox checkBox_UseItemManager;
		private System.Windows.Forms.GroupBox groupBox_Salavage;
		private System.Windows.Forms.ComboBox comboBox_SalvageLegendaryItems;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ComboBox comboBox_SalvageRareItems;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox comboBox_SalvageMagicItems;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox comboBox_SalvageWhiteItems;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox_Gambling;
		private System.Windows.Forms.Panel panel_Gambling;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel_GamblingItemTypes;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textBox_GamblingBloodShards;
		private System.Windows.Forms.TrackBar trackBar_GamblingBloodShards;
		private System.Windows.Forms.CheckBox checkBox_EnableGambling;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
	}
}