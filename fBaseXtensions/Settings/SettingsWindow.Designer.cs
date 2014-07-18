namespace fBaseXtensions.Settings
{
	partial class SettingsWindow
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
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.panel16 = new System.Windows.Forms.Panel();
			this.label20 = new System.Windows.Forms.Label();
			this.txt_GeneralGoldInactivityValue = new System.Windows.Forms.TextBox();
			this.tb_GeneralGoldInactivityValue = new System.Windows.Forms.TrackBar();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.groupBox23 = new System.Windows.Forms.GroupBox();
			this.flowLayout_DebugFunkyLogLevels = new System.Windows.Forms.FlowLayoutPanel();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.panel16.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tb_GeneralGoldInactivityValue)).BeginInit();
			this.tabPage2.SuspendLayout();
			this.groupBox23.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(292, 273);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.panel16);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(284, 247);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "General";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// panel16
			// 
			this.panel16.Controls.Add(this.label20);
			this.panel16.Controls.Add(this.txt_GeneralGoldInactivityValue);
			this.panel16.Controls.Add(this.tb_GeneralGoldInactivityValue);
			this.panel16.Location = new System.Drawing.Point(8, 6);
			this.panel16.Name = "panel16";
			this.panel16.Size = new System.Drawing.Size(270, 72);
			this.panel16.TabIndex = 18;
			// 
			// label20
			// 
			this.label20.AutoSize = true;
			this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label20.Location = new System.Drawing.Point(7, 9);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(210, 17);
			this.label20.TabIndex = 7;
			this.label20.Text = "Gold Inactivity Timeout Seconds";
			// 
			// txt_GeneralGoldInactivityValue
			// 
			this.txt_GeneralGoldInactivityValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txt_GeneralGoldInactivityValue.Location = new System.Drawing.Point(188, 29);
			this.txt_GeneralGoldInactivityValue.Name = "txt_GeneralGoldInactivityValue";
			this.txt_GeneralGoldInactivityValue.ReadOnly = true;
			this.txt_GeneralGoldInactivityValue.Size = new System.Drawing.Size(58, 26);
			this.txt_GeneralGoldInactivityValue.TabIndex = 9;
			// 
			// tb_GeneralGoldInactivityValue
			// 
			this.tb_GeneralGoldInactivityValue.LargeChange = 10;
			this.tb_GeneralGoldInactivityValue.Location = new System.Drawing.Point(10, 29);
			this.tb_GeneralGoldInactivityValue.Maximum = 900;
			this.tb_GeneralGoldInactivityValue.Name = "tb_GeneralGoldInactivityValue";
			this.tb_GeneralGoldInactivityValue.Size = new System.Drawing.Size(172, 42);
			this.tb_GeneralGoldInactivityValue.TabIndex = 8;
			this.tb_GeneralGoldInactivityValue.TickFrequency = 60;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.groupBox23);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(284, 247);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Logging";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// groupBox23
			// 
			this.groupBox23.Controls.Add(this.flowLayout_DebugFunkyLogLevels);
			this.groupBox23.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox23.Location = new System.Drawing.Point(3, 3);
			this.groupBox23.Name = "groupBox23";
			this.groupBox23.Size = new System.Drawing.Size(278, 220);
			this.groupBox23.TabIndex = 2;
			this.groupBox23.TabStop = false;
			this.groupBox23.Text = "Funky Logging";
			// 
			// flowLayout_DebugFunkyLogLevels
			// 
			this.flowLayout_DebugFunkyLogLevels.Dock = System.Windows.Forms.DockStyle.Top;
			this.flowLayout_DebugFunkyLogLevels.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayout_DebugFunkyLogLevels.Location = new System.Drawing.Point(3, 16);
			this.flowLayout_DebugFunkyLogLevels.Name = "flowLayout_DebugFunkyLogLevels";
			this.flowLayout_DebugFunkyLogLevels.Size = new System.Drawing.Size(272, 198);
			this.flowLayout_DebugFunkyLogLevels.TabIndex = 0;
			// 
			// SettingsWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.DarkGray;
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.tabControl1);
			this.ForeColor = System.Drawing.Color.Black;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SettingsWindow";
			this.Text = "SettingsWindow";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsWindow_FormClosing);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.panel16.ResumeLayout(false);
			this.panel16.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.tb_GeneralGoldInactivityValue)).EndInit();
			this.tabPage2.ResumeLayout(false);
			this.groupBox23.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Panel panel16;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.TextBox txt_GeneralGoldInactivityValue;
		private System.Windows.Forms.TrackBar tb_GeneralGoldInactivityValue;
		private System.Windows.Forms.GroupBox groupBox23;
		private System.Windows.Forms.FlowLayoutPanel flowLayout_DebugFunkyLogLevels;
	}
}