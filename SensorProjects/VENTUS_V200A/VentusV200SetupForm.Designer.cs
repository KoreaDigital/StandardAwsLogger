namespace VENTUS_V200A
{
	partial class VentusV200SetupForm
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
			if( disposing && (components != null) )
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
			System.Windows.Forms.Label label_deviceAddr;
			this.textBox_deviceAddr = new System.Windows.Forms.TextBox();
			this.button_cancel = new System.Windows.Forms.Button();
			this.button_apply = new System.Windows.Forms.Button();
			this.panel_deviceAddr = new System.Windows.Forms.Panel();
			label_deviceAddr = new System.Windows.Forms.Label();
			this.panel_deviceAddr.SuspendLayout();
			this.SuspendLayout();
			// 
			// label_deviceAddr
			// 
			label_deviceAddr.AutoSize = true;
			label_deviceAddr.Location = new System.Drawing.Point(3, 10);
			label_deviceAddr.Name = "label_deviceAddr";
			label_deviceAddr.Size = new System.Drawing.Size(147, 12);
			label_deviceAddr.TabIndex = 8;
			label_deviceAddr.Text = "V200 Device Addr(Hex) : ";
			// 
			// textBox_deviceAddr
			// 
			this.textBox_deviceAddr.Location = new System.Drawing.Point(156, 7);
			this.textBox_deviceAddr.Name = "textBox_deviceAddr";
			this.textBox_deviceAddr.Size = new System.Drawing.Size(109, 21);
			this.textBox_deviceAddr.TabIndex = 9;
			this.textBox_deviceAddr.TextChanged += new System.EventHandler(this.textBox_deviceAddr_TextChanged);
			this.textBox_deviceAddr.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_deviceAddr_KeyPress);
			// 
			// button_cancel
			// 
			this.button_cancel.Location = new System.Drawing.Point(213, 54);
			this.button_cancel.Name = "button_cancel";
			this.button_cancel.Size = new System.Drawing.Size(75, 23);
			this.button_cancel.TabIndex = 14;
			this.button_cancel.Text = "취소";
			this.button_cancel.UseVisualStyleBackColor = true;
			this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
			// 
			// button_apply
			// 
			this.button_apply.Location = new System.Drawing.Point(132, 54);
			this.button_apply.Name = "button_apply";
			this.button_apply.Size = new System.Drawing.Size(75, 23);
			this.button_apply.TabIndex = 13;
			this.button_apply.Text = "적용";
			this.button_apply.UseVisualStyleBackColor = true;
			this.button_apply.Click += new System.EventHandler(this.button_apply_Click);
			// 
			// panel_deviceAddr
			// 
			this.panel_deviceAddr.Controls.Add(this.textBox_deviceAddr);
			this.panel_deviceAddr.Controls.Add(label_deviceAddr);
			this.panel_deviceAddr.Location = new System.Drawing.Point(12, 10);
			this.panel_deviceAddr.Name = "panel_deviceAddr";
			this.panel_deviceAddr.Size = new System.Drawing.Size(276, 35);
			this.panel_deviceAddr.TabIndex = 16;
			// 
			// NetworkExportSetupForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(297, 84);
			this.ControlBox = false;
			this.Controls.Add(this.panel_deviceAddr);
			this.Controls.Add(this.button_cancel);
			this.Controls.Add(this.button_apply);
			this.Name = "NetworkExportSetupForm";
			this.Text = "VentusV200SetupForm";
			this.panel_deviceAddr.ResumeLayout(false);
			this.panel_deviceAddr.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox textBox_deviceAddr;
		private System.Windows.Forms.Button button_cancel;
		private System.Windows.Forms.Button button_apply;
		private System.Windows.Forms.Panel panel_deviceAddr;

	}
}