namespace FunkyDebug
{
	partial class FormDebug
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPageObjects = new System.Windows.Forms.TabPage();
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnRefreshObjects = new System.Windows.Forms.Button();
			this.tabControl_Objects = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.tabPage4 = new System.Windows.Forms.TabPage();
			this.tabPageCharacter = new System.Windows.Forms.TabPage();
			this.tabControl3 = new System.Windows.Forms.TabControl();
			this.tabPage5 = new System.Windows.Forms.TabPage();
			this.panel2 = new System.Windows.Forms.Panel();
			this.btnRefreshCharacter = new System.Windows.Forms.Button();
			this.tabPage6 = new System.Windows.Forms.TabPage();
			this.panel3 = new System.Windows.Forms.Panel();
			this.btnRefreshCharacterInventory = new System.Windows.Forms.Button();
			this.tabPage7 = new System.Windows.Forms.TabPage();
			this.panel5 = new System.Windows.Forms.Panel();
			this.btnRefreshCharacterEquipped = new System.Windows.Forms.Button();
			this.tabPage8 = new System.Windows.Forms.TabPage();
			this.panel4 = new System.Windows.Forms.Panel();
			this.btn_dumpUIs = new System.Windows.Forms.Button();
			this.flowLayout_OutPut = new System.Windows.Forms.FlowLayoutPanel();
			this.tabPage9 = new System.Windows.Forms.TabPage();
			this.tabControl2 = new System.Windows.Forms.TabControl();
			this.tabPage10 = new System.Windows.Forms.TabPage();
			this.tabPage11 = new System.Windows.Forms.TabPage();
			this.tabControl4 = new System.Windows.Forms.TabControl();
			this.tabPage12 = new System.Windows.Forms.TabPage();
			this.tabPage13 = new System.Windows.Forms.TabPage();
			this.btn_DumpOpenWorldMarkers = new System.Windows.Forms.Button();
			this.btn_DumpCurrentWorldMarkers = new System.Windows.Forms.Button();
			this.tabPage14 = new System.Windows.Forms.TabPage();
			this.btn_DumpNormalMarkers = new System.Windows.Forms.Button();
			this.tabControl5 = new System.Windows.Forms.TabControl();
			this.tabPage15 = new System.Windows.Forms.TabPage();
			this.tabPage16 = new System.Windows.Forms.TabPage();
			this.btn_DumpBounties = new System.Windows.Forms.Button();
			this.btn_DumpQuests = new System.Windows.Forms.Button();
			this.tabControl1.SuspendLayout();
			this.tabPageObjects.SuspendLayout();
			this.panel1.SuspendLayout();
			this.tabControl_Objects.SuspendLayout();
			this.tabPageCharacter.SuspendLayout();
			this.tabControl3.SuspendLayout();
			this.tabPage5.SuspendLayout();
			this.panel2.SuspendLayout();
			this.tabPage6.SuspendLayout();
			this.panel3.SuspendLayout();
			this.tabPage7.SuspendLayout();
			this.panel5.SuspendLayout();
			this.tabPage8.SuspendLayout();
			this.panel4.SuspendLayout();
			this.tabPage9.SuspendLayout();
			this.tabControl2.SuspendLayout();
			this.tabPage10.SuspendLayout();
			this.tabPage11.SuspendLayout();
			this.tabControl4.SuspendLayout();
			this.tabPage12.SuspendLayout();
			this.tabPage13.SuspendLayout();
			this.tabPage14.SuspendLayout();
			this.tabControl5.SuspendLayout();
			this.tabPage15.SuspendLayout();
			this.tabPage16.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPageObjects);
			this.tabControl1.Controls.Add(this.tabPageCharacter);
			this.tabControl1.Controls.Add(this.tabPage9);
			this.tabControl1.Controls.Add(this.tabPage8);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(758, 134);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPageObjects
			// 
			this.tabPageObjects.Controls.Add(this.panel1);
			this.tabPageObjects.Controls.Add(this.tabControl_Objects);
			this.tabPageObjects.Location = new System.Drawing.Point(4, 22);
			this.tabPageObjects.Name = "tabPageObjects";
			this.tabPageObjects.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageObjects.Size = new System.Drawing.Size(750, 79);
			this.tabPageObjects.TabIndex = 0;
			this.tabPageObjects.Text = "Objects";
			this.tabPageObjects.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.btnRefreshObjects);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(3, 25);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(744, 41);
			this.panel1.TabIndex = 0;
			// 
			// btnRefreshObjects
			// 
			this.btnRefreshObjects.Location = new System.Drawing.Point(5, 3);
			this.btnRefreshObjects.Name = "btnRefreshObjects";
			this.btnRefreshObjects.Size = new System.Drawing.Size(94, 35);
			this.btnRefreshObjects.TabIndex = 0;
			this.btnRefreshObjects.Text = "Refresh";
			this.btnRefreshObjects.UseVisualStyleBackColor = true;
			this.btnRefreshObjects.Click += new System.EventHandler(this.btnRefreshObjects_Click);
			// 
			// tabControl_Objects
			// 
			this.tabControl_Objects.Controls.Add(this.tabPage1);
			this.tabControl_Objects.Controls.Add(this.tabPage2);
			this.tabControl_Objects.Controls.Add(this.tabPage3);
			this.tabControl_Objects.Controls.Add(this.tabPage4);
			this.tabControl_Objects.Dock = System.Windows.Forms.DockStyle.Top;
			this.tabControl_Objects.Location = new System.Drawing.Point(3, 3);
			this.tabControl_Objects.Name = "tabControl_Objects";
			this.tabControl_Objects.SelectedIndex = 0;
			this.tabControl_Objects.Size = new System.Drawing.Size(744, 22);
			this.tabControl_Objects.TabIndex = 1;
			// 
			// tabPage1
			// 
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(736, 0);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Monsters";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(736, 0);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Gizmos";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// tabPage3
			// 
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(736, 0);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Items";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// tabPage4
			// 
			this.tabPage4.Location = new System.Drawing.Point(4, 22);
			this.tabPage4.Name = "tabPage4";
			this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage4.Size = new System.Drawing.Size(736, 0);
			this.tabPage4.TabIndex = 3;
			this.tabPage4.Text = "Misc";
			this.tabPage4.UseVisualStyleBackColor = true;
			// 
			// tabPageCharacter
			// 
			this.tabPageCharacter.Controls.Add(this.tabControl3);
			this.tabPageCharacter.Location = new System.Drawing.Point(4, 22);
			this.tabPageCharacter.Name = "tabPageCharacter";
			this.tabPageCharacter.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageCharacter.Size = new System.Drawing.Size(750, 79);
			this.tabPageCharacter.TabIndex = 1;
			this.tabPageCharacter.Text = "Character";
			this.tabPageCharacter.UseVisualStyleBackColor = true;
			// 
			// tabControl3
			// 
			this.tabControl3.Controls.Add(this.tabPage5);
			this.tabControl3.Controls.Add(this.tabPage6);
			this.tabControl3.Controls.Add(this.tabPage7);
			this.tabControl3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl3.Location = new System.Drawing.Point(3, 3);
			this.tabControl3.Name = "tabControl3";
			this.tabControl3.SelectedIndex = 0;
			this.tabControl3.Size = new System.Drawing.Size(744, 73);
			this.tabControl3.TabIndex = 2;
			// 
			// tabPage5
			// 
			this.tabPage5.Controls.Add(this.panel2);
			this.tabPage5.Location = new System.Drawing.Point(4, 22);
			this.tabPage5.Name = "tabPage5";
			this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage5.Size = new System.Drawing.Size(736, 47);
			this.tabPage5.TabIndex = 0;
			this.tabPage5.Text = "Skills";
			this.tabPage5.UseVisualStyleBackColor = true;
			// 
			// panel2
			// 
			this.panel2.AutoSize = true;
			this.panel2.Controls.Add(this.btnRefreshCharacter);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(3, 3);
			this.panel2.Margin = new System.Windows.Forms.Padding(3, 3, 3, 25);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(730, 41);
			this.panel2.TabIndex = 1;
			// 
			// btnRefreshCharacter
			// 
			this.btnRefreshCharacter.Location = new System.Drawing.Point(5, 3);
			this.btnRefreshCharacter.Name = "btnRefreshCharacter";
			this.btnRefreshCharacter.Size = new System.Drawing.Size(94, 35);
			this.btnRefreshCharacter.TabIndex = 0;
			this.btnRefreshCharacter.Text = "Refresh";
			this.btnRefreshCharacter.UseVisualStyleBackColor = true;
			this.btnRefreshCharacter.Click += new System.EventHandler(this.btnRefreshCharacter_Click);
			// 
			// tabPage6
			// 
			this.tabPage6.Controls.Add(this.panel3);
			this.tabPage6.Location = new System.Drawing.Point(4, 22);
			this.tabPage6.Name = "tabPage6";
			this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage6.Size = new System.Drawing.Size(736, 47);
			this.tabPage6.TabIndex = 1;
			this.tabPage6.Text = "Inventory";
			this.tabPage6.UseVisualStyleBackColor = true;
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.btnRefreshCharacterInventory);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel3.Location = new System.Drawing.Point(3, 3);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(730, 42);
			this.panel3.TabIndex = 4;
			// 
			// btnRefreshCharacterInventory
			// 
			this.btnRefreshCharacterInventory.Location = new System.Drawing.Point(5, 3);
			this.btnRefreshCharacterInventory.Name = "btnRefreshCharacterInventory";
			this.btnRefreshCharacterInventory.Size = new System.Drawing.Size(94, 35);
			this.btnRefreshCharacterInventory.TabIndex = 0;
			this.btnRefreshCharacterInventory.Text = "Refresh";
			this.btnRefreshCharacterInventory.UseVisualStyleBackColor = true;
			this.btnRefreshCharacterInventory.Click += new System.EventHandler(this.btnRefreshCharacterInventory_Click);
			// 
			// tabPage7
			// 
			this.tabPage7.Controls.Add(this.panel5);
			this.tabPage7.Location = new System.Drawing.Point(4, 22);
			this.tabPage7.Name = "tabPage7";
			this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage7.Size = new System.Drawing.Size(736, 47);
			this.tabPage7.TabIndex = 2;
			this.tabPage7.Text = "Equipped";
			this.tabPage7.UseVisualStyleBackColor = true;
			// 
			// panel5
			// 
			this.panel5.Controls.Add(this.btnRefreshCharacterEquipped);
			this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel5.Location = new System.Drawing.Point(3, 3);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(730, 42);
			this.panel5.TabIndex = 5;
			// 
			// btnRefreshCharacterEquipped
			// 
			this.btnRefreshCharacterEquipped.Location = new System.Drawing.Point(5, 3);
			this.btnRefreshCharacterEquipped.Name = "btnRefreshCharacterEquipped";
			this.btnRefreshCharacterEquipped.Size = new System.Drawing.Size(94, 35);
			this.btnRefreshCharacterEquipped.TabIndex = 0;
			this.btnRefreshCharacterEquipped.Text = "Refresh";
			this.btnRefreshCharacterEquipped.UseVisualStyleBackColor = true;
			this.btnRefreshCharacterEquipped.Click += new System.EventHandler(this.btnRefreshCharacterEquipped_Click);
			// 
			// tabPage8
			// 
			this.tabPage8.Controls.Add(this.panel4);
			this.tabPage8.Location = new System.Drawing.Point(4, 22);
			this.tabPage8.Name = "tabPage8";
			this.tabPage8.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage8.Size = new System.Drawing.Size(750, 79);
			this.tabPage8.TabIndex = 2;
			this.tabPage8.Text = "UI";
			this.tabPage8.UseVisualStyleBackColor = true;
			// 
			// panel4
			// 
			this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel4.Controls.Add(this.btn_dumpUIs);
			this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel4.Location = new System.Drawing.Point(3, 3);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(744, 32);
			this.panel4.TabIndex = 0;
			// 
			// btn_dumpUIs
			// 
			this.btn_dumpUIs.Location = new System.Drawing.Point(5, 3);
			this.btn_dumpUIs.Name = "btn_dumpUIs";
			this.btn_dumpUIs.Size = new System.Drawing.Size(75, 23);
			this.btn_dumpUIs.TabIndex = 0;
			this.btn_dumpUIs.Text = "Dump";
			this.btn_dumpUIs.UseVisualStyleBackColor = true;
			this.btn_dumpUIs.Click += new System.EventHandler(this.btn_dumpUIs_Click);
			// 
			// flowLayout_OutPut
			// 
			this.flowLayout_OutPut.AutoScroll = true;
			this.flowLayout_OutPut.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.flowLayout_OutPut.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayout_OutPut.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayout_OutPut.Location = new System.Drawing.Point(0, 134);
			this.flowLayout_OutPut.Name = "flowLayout_OutPut";
			this.flowLayout_OutPut.Size = new System.Drawing.Size(758, 459);
			this.flowLayout_OutPut.TabIndex = 1;
			this.flowLayout_OutPut.WrapContents = false;
			this.flowLayout_OutPut.MouseEnter += new System.EventHandler(this.flowLayout_OutPut_MouseEnter);
			// 
			// tabPage9
			// 
			this.tabPage9.Controls.Add(this.tabControl2);
			this.tabPage9.Location = new System.Drawing.Point(4, 22);
			this.tabPage9.Name = "tabPage9";
			this.tabPage9.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage9.Size = new System.Drawing.Size(750, 108);
			this.tabPage9.TabIndex = 3;
			this.tabPage9.Text = "Game";
			this.tabPage9.UseVisualStyleBackColor = true;
			// 
			// tabControl2
			// 
			this.tabControl2.Controls.Add(this.tabPage10);
			this.tabControl2.Controls.Add(this.tabPage11);
			this.tabControl2.Dock = System.Windows.Forms.DockStyle.Top;
			this.tabControl2.Location = new System.Drawing.Point(3, 3);
			this.tabControl2.Name = "tabControl2";
			this.tabControl2.SelectedIndex = 0;
			this.tabControl2.Size = new System.Drawing.Size(744, 103);
			this.tabControl2.TabIndex = 0;
			// 
			// tabPage10
			// 
			this.tabPage10.Controls.Add(this.tabControl4);
			this.tabPage10.Location = new System.Drawing.Point(4, 22);
			this.tabPage10.Name = "tabPage10";
			this.tabPage10.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage10.Size = new System.Drawing.Size(736, 77);
			this.tabPage10.TabIndex = 0;
			this.tabPage10.Text = "Minimap";
			this.tabPage10.UseVisualStyleBackColor = true;
			// 
			// tabPage11
			// 
			this.tabPage11.Controls.Add(this.tabControl5);
			this.tabPage11.Location = new System.Drawing.Point(4, 22);
			this.tabPage11.Name = "tabPage11";
			this.tabPage11.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage11.Size = new System.Drawing.Size(736, 77);
			this.tabPage11.TabIndex = 1;
			this.tabPage11.Text = "Quests";
			this.tabPage11.UseVisualStyleBackColor = true;
			// 
			// tabControl4
			// 
			this.tabControl4.Controls.Add(this.tabPage12);
			this.tabControl4.Controls.Add(this.tabPage13);
			this.tabControl4.Controls.Add(this.tabPage14);
			this.tabControl4.Dock = System.Windows.Forms.DockStyle.Top;
			this.tabControl4.Location = new System.Drawing.Point(3, 3);
			this.tabControl4.Name = "tabControl4";
			this.tabControl4.SelectedIndex = 0;
			this.tabControl4.Size = new System.Drawing.Size(730, 74);
			this.tabControl4.TabIndex = 0;
			// 
			// tabPage12
			// 
			this.tabPage12.Controls.Add(this.btn_DumpOpenWorldMarkers);
			this.tabPage12.Location = new System.Drawing.Point(4, 22);
			this.tabPage12.Name = "tabPage12";
			this.tabPage12.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage12.Size = new System.Drawing.Size(722, 48);
			this.tabPage12.TabIndex = 0;
			this.tabPage12.Text = "Open World Markers";
			this.tabPage12.UseVisualStyleBackColor = true;
			// 
			// tabPage13
			// 
			this.tabPage13.Controls.Add(this.btn_DumpCurrentWorldMarkers);
			this.tabPage13.Location = new System.Drawing.Point(4, 22);
			this.tabPage13.Name = "tabPage13";
			this.tabPage13.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage13.Size = new System.Drawing.Size(722, 48);
			this.tabPage13.TabIndex = 1;
			this.tabPage13.Text = "Current World Markers";
			this.tabPage13.UseVisualStyleBackColor = true;
			// 
			// btn_DumpOpenWorldMarkers
			// 
			this.btn_DumpOpenWorldMarkers.Location = new System.Drawing.Point(6, 6);
			this.btn_DumpOpenWorldMarkers.Name = "btn_DumpOpenWorldMarkers";
			this.btn_DumpOpenWorldMarkers.Size = new System.Drawing.Size(75, 23);
			this.btn_DumpOpenWorldMarkers.TabIndex = 0;
			this.btn_DumpOpenWorldMarkers.Text = "Dump";
			this.btn_DumpOpenWorldMarkers.UseVisualStyleBackColor = true;
			this.btn_DumpOpenWorldMarkers.Click += new System.EventHandler(this.btn_DumpOpenWorldMarkers_Click);
			// 
			// btn_DumpCurrentWorldMarkers
			// 
			this.btn_DumpCurrentWorldMarkers.Location = new System.Drawing.Point(6, 6);
			this.btn_DumpCurrentWorldMarkers.Name = "btn_DumpCurrentWorldMarkers";
			this.btn_DumpCurrentWorldMarkers.Size = new System.Drawing.Size(75, 23);
			this.btn_DumpCurrentWorldMarkers.TabIndex = 1;
			this.btn_DumpCurrentWorldMarkers.Text = "Dump";
			this.btn_DumpCurrentWorldMarkers.UseVisualStyleBackColor = true;
			this.btn_DumpCurrentWorldMarkers.Click += new System.EventHandler(this.btn_DumpCurrentWorldMarkers_Click);
			// 
			// tabPage14
			// 
			this.tabPage14.Controls.Add(this.btn_DumpNormalMarkers);
			this.tabPage14.Location = new System.Drawing.Point(4, 22);
			this.tabPage14.Name = "tabPage14";
			this.tabPage14.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage14.Size = new System.Drawing.Size(722, 48);
			this.tabPage14.TabIndex = 2;
			this.tabPage14.Text = "Normal Markers";
			this.tabPage14.UseVisualStyleBackColor = true;
			// 
			// btn_DumpNormalMarkers
			// 
			this.btn_DumpNormalMarkers.Location = new System.Drawing.Point(6, 6);
			this.btn_DumpNormalMarkers.Name = "btn_DumpNormalMarkers";
			this.btn_DumpNormalMarkers.Size = new System.Drawing.Size(75, 23);
			this.btn_DumpNormalMarkers.TabIndex = 2;
			this.btn_DumpNormalMarkers.Text = "Dump";
			this.btn_DumpNormalMarkers.UseVisualStyleBackColor = true;
			this.btn_DumpNormalMarkers.Click += new System.EventHandler(this.btn_DumpNormalMarkers_Click);
			// 
			// tabControl5
			// 
			this.tabControl5.Controls.Add(this.tabPage15);
			this.tabControl5.Controls.Add(this.tabPage16);
			this.tabControl5.Dock = System.Windows.Forms.DockStyle.Top;
			this.tabControl5.Location = new System.Drawing.Point(3, 3);
			this.tabControl5.Name = "tabControl5";
			this.tabControl5.SelectedIndex = 0;
			this.tabControl5.Size = new System.Drawing.Size(730, 78);
			this.tabControl5.TabIndex = 0;
			// 
			// tabPage15
			// 
			this.tabPage15.Controls.Add(this.btn_DumpBounties);
			this.tabPage15.Location = new System.Drawing.Point(4, 22);
			this.tabPage15.Name = "tabPage15";
			this.tabPage15.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage15.Size = new System.Drawing.Size(722, 52);
			this.tabPage15.TabIndex = 0;
			this.tabPage15.Text = "Bounties";
			this.tabPage15.UseVisualStyleBackColor = true;
			// 
			// tabPage16
			// 
			this.tabPage16.Controls.Add(this.btn_DumpQuests);
			this.tabPage16.Location = new System.Drawing.Point(4, 22);
			this.tabPage16.Name = "tabPage16";
			this.tabPage16.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage16.Size = new System.Drawing.Size(722, 52);
			this.tabPage16.TabIndex = 1;
			this.tabPage16.Text = "Quests";
			this.tabPage16.UseVisualStyleBackColor = true;
			// 
			// btn_DumpBounties
			// 
			this.btn_DumpBounties.Location = new System.Drawing.Point(6, 6);
			this.btn_DumpBounties.Name = "btn_DumpBounties";
			this.btn_DumpBounties.Size = new System.Drawing.Size(75, 23);
			this.btn_DumpBounties.TabIndex = 3;
			this.btn_DumpBounties.Text = "Dump";
			this.btn_DumpBounties.UseVisualStyleBackColor = true;
			this.btn_DumpBounties.Click += new System.EventHandler(this.btn_DumpBounties_Click);
			// 
			// btn_DumpQuests
			// 
			this.btn_DumpQuests.Location = new System.Drawing.Point(6, 6);
			this.btn_DumpQuests.Name = "btn_DumpQuests";
			this.btn_DumpQuests.Size = new System.Drawing.Size(75, 23);
			this.btn_DumpQuests.TabIndex = 4;
			this.btn_DumpQuests.Text = "Dump";
			this.btn_DumpQuests.UseVisualStyleBackColor = true;
			this.btn_DumpQuests.Click += new System.EventHandler(this.btn_DumpQuests_Click);
			// 
			// FormDebug
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(758, 593);
			this.Controls.Add(this.flowLayout_OutPut);
			this.Controls.Add(this.tabControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormDebug";
			this.Text = "Test2";
			this.Load += new System.EventHandler(this.FormDebug_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabPageObjects.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.tabControl_Objects.ResumeLayout(false);
			this.tabPageCharacter.ResumeLayout(false);
			this.tabControl3.ResumeLayout(false);
			this.tabPage5.ResumeLayout(false);
			this.tabPage5.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.tabPage6.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.tabPage7.ResumeLayout(false);
			this.panel5.ResumeLayout(false);
			this.tabPage8.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.tabPage9.ResumeLayout(false);
			this.tabControl2.ResumeLayout(false);
			this.tabPage10.ResumeLayout(false);
			this.tabPage11.ResumeLayout(false);
			this.tabControl4.ResumeLayout(false);
			this.tabPage12.ResumeLayout(false);
			this.tabPage13.ResumeLayout(false);
			this.tabPage14.ResumeLayout(false);
			this.tabControl5.ResumeLayout(false);
			this.tabPage15.ResumeLayout(false);
			this.tabPage16.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPageObjects;
		private System.Windows.Forms.TabControl tabControl_Objects;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.TabPage tabPage4;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnRefreshObjects;
		private System.Windows.Forms.TabPage tabPageCharacter;
		private System.Windows.Forms.TabControl tabControl3;
		private System.Windows.Forms.TabPage tabPage5;
		private System.Windows.Forms.TabPage tabPage6;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button btnRefreshCharacter;
		private System.Windows.Forms.TabPage tabPage7;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Button btnRefreshCharacterInventory;
		private System.Windows.Forms.TabPage tabPage8;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Button btn_dumpUIs;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.Button btnRefreshCharacterEquipped;
		private System.Windows.Forms.FlowLayoutPanel flowLayout_OutPut;
		private System.Windows.Forms.TabPage tabPage9;
		private System.Windows.Forms.TabControl tabControl2;
		private System.Windows.Forms.TabPage tabPage10;
		private System.Windows.Forms.TabControl tabControl4;
		private System.Windows.Forms.TabPage tabPage12;
		private System.Windows.Forms.TabPage tabPage13;
		private System.Windows.Forms.TabPage tabPage11;
		private System.Windows.Forms.Button btn_DumpOpenWorldMarkers;
		private System.Windows.Forms.Button btn_DumpCurrentWorldMarkers;
		private System.Windows.Forms.TabPage tabPage14;
		private System.Windows.Forms.Button btn_DumpNormalMarkers;
		private System.Windows.Forms.TabControl tabControl5;
		private System.Windows.Forms.TabPage tabPage15;
		private System.Windows.Forms.Button btn_DumpBounties;
		private System.Windows.Forms.TabPage tabPage16;
		private System.Windows.Forms.Button btn_DumpQuests;
	}
}