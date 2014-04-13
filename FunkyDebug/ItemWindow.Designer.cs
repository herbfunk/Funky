namespace FunkyDebug
{
	partial class ItemWindow
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
			this.lbl_ItemText = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lbl_ItemText
			// 
			this.lbl_ItemText.AutoSize = true;
			this.lbl_ItemText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbl_ItemText.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.lbl_ItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbl_ItemText.Location = new System.Drawing.Point(0, 0);
			this.lbl_ItemText.Name = "lbl_ItemText";
			this.lbl_ItemText.Size = new System.Drawing.Size(46, 17);
			this.lbl_ItemText.TabIndex = 0;
			this.lbl_ItemText.Text = "label1";
			// 
			// ItemWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.ControlBox = false;
			this.Controls.Add(this.lbl_ItemText);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "ItemWindow";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lbl_ItemText;
	}
}