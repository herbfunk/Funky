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
			this.tabControl_Objects = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.tabPage4 = new System.Windows.Forms.TabPage();
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnRefreshObjects = new System.Windows.Forms.Button();
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
			this.tabControl1.SuspendLayout();
			this.tabPageObjects.SuspendLayout();
			this.tabControl_Objects.SuspendLayout();
			this.panel1.SuspendLayout();
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
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPageObjects);
			this.tabControl1.Controls.Add(this.tabPageCharacter);
			this.tabControl1.Controls.Add(this.tabPage8);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(758, 105);
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
			this.tabPage1.Size = new System.Drawing.Size(678, 0);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Monsters";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(678, 0);
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
			this.tabPage4.Size = new System.Drawing.Size(678, 0);
			this.tabPage4.TabIndex = 3;
			this.tabPage4.Text = "Misc";
			this.tabPage4.UseVisualStyleBackColor = true;
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
			// tabPageCharacter
			// 
			this.tabPageCharacter.Controls.Add(this.tabControl3);
			this.tabPageCharacter.Location = new System.Drawing.Point(4, 22);
			this.tabPageCharacter.Name = "tabPageCharacter";
			this.tabPageCharacter.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageCharacter.Size = new System.Drawing.Size(691, 79);
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
			this.tabControl3.Size = new System.Drawing.Size(685, 73);
			this.tabControl3.TabIndex = 2;
			// 
			// tabPage5
			// 
			this.tabPage5.Controls.Add(this.panel2);
			this.tabPage5.Location = new System.Drawing.Point(4, 22);
			this.tabPage5.Name = "tabPage5";
			this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage5.Size = new System.Drawing.Size(677, 47);
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
			this.panel2.Size = new System.Drawing.Size(671, 41);
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
			this.tabPage6.Size = new System.Drawing.Size(677, 47);
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
			this.panel3.Size = new System.Drawing.Size(671, 42);
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
			this.tabPage7.Size = new System.Drawing.Size(677, 47);
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
			this.panel5.Size = new System.Drawing.Size(671, 42);
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
			this.tabPage8.Size = new System.Drawing.Size(691, 79);
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
			this.panel4.Size = new System.Drawing.Size(685, 32);
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
			this.flowLayout_OutPut.Location = new System.Drawing.Point(0, 105);
			this.flowLayout_OutPut.Name = "flowLayout_OutPut";
			this.flowLayout_OutPut.Size = new System.Drawing.Size(758, 488);
			this.flowLayout_OutPut.TabIndex = 1;
			this.flowLayout_OutPut.WrapContents = false;
			this.flowLayout_OutPut.MouseEnter += new System.EventHandler(this.flowLayout_OutPut_MouseEnter);
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
			this.tabControl_Objects.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
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
	}
}