namespace MT211ModemExport
{
	partial class ModemExportSetupForm
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
			System.Windows.Forms.Label label_comPort;
			System.Windows.Forms.Label label_serverAddr;
			System.Windows.Forms.Label label_serverPort;
			this.textBox_serverAddr = new System.Windows.Forms.TextBox();
			this.textBox_serverPort = new System.Windows.Forms.TextBox();
			this.comboBox_comPort = new System.Windows.Forms.ComboBox();
			this.button_cancel = new System.Windows.Forms.Button();
			this.button_apply = new System.Windows.Forms.Button();
			this.panel_comPort = new System.Windows.Forms.Panel();
			this.panel_serverAddr = new System.Windows.Forms.Panel();
			this.panel_serverPort = new System.Windows.Forms.Panel();
			label_comPort = new System.Windows.Forms.Label();
			label_serverAddr = new System.Windows.Forms.Label();
			label_serverPort = new System.Windows.Forms.Label();
			this.panel_comPort.SuspendLayout();
			this.panel_serverAddr.SuspendLayout();
			this.panel_serverPort.SuspendLayout();
			this.SuspendLayout();
			// 
			// label_comPort
			// 
			label_comPort.AutoSize = true;
			label_comPort.Location = new System.Drawing.Point(3, 10);
			label_comPort.Name = "label_comPort";
			label_comPort.Size = new System.Drawing.Size(72, 12);
			label_comPort.TabIndex = 6;
			label_comPort.Text = "Com 포트 : ";
			// 
			// label_serverAddr
			// 
			label_serverAddr.AutoSize = true;
			label_serverAddr.Location = new System.Drawing.Point(3, 10);
			label_serverAddr.Name = "label_serverAddr";
			label_serverAddr.Size = new System.Drawing.Size(69, 12);
			label_serverAddr.TabIndex = 8;
			label_serverAddr.Text = "서버 주소 : ";
			// 
			// label_serverPort
			// 
			label_serverPort.AutoSize = true;
			label_serverPort.Location = new System.Drawing.Point(3, 10);
			label_serverPort.Name = "label_serverPort";
			label_serverPort.Size = new System.Drawing.Size(95, 12);
			label_serverPort.TabIndex = 10;
			label_serverPort.Text = "서버 Port 번호 : ";
			// 
			// textBox_serverAddr
			// 
			this.textBox_serverAddr.Location = new System.Drawing.Point(78, 7);
			this.textBox_serverAddr.Name = "textBox_serverAddr";
			this.textBox_serverAddr.Size = new System.Drawing.Size(187, 21);
			this.textBox_serverAddr.TabIndex = 9;
			this.textBox_serverAddr.TextChanged += new System.EventHandler(this.textBox_serverAddr_TextChanged);
			// 
			// textBox_serverPort
			// 
			this.textBox_serverPort.Location = new System.Drawing.Point(104, 7);
			this.textBox_serverPort.Name = "textBox_serverPort";
			this.textBox_serverPort.Size = new System.Drawing.Size(51, 21);
			this.textBox_serverPort.TabIndex = 11;
			this.textBox_serverPort.TextChanged += new System.EventHandler(this.textBox_serverPort_TextChanged);
			this.textBox_serverPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_serverPort_KeyPress);
			// 
			// comboBox_comPort
			// 
			this.comboBox_comPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_comPort.FormattingEnabled = true;
			this.comboBox_comPort.Location = new System.Drawing.Point(81, 7);
			this.comboBox_comPort.Name = "comboBox_comPort";
			this.comboBox_comPort.Size = new System.Drawing.Size(121, 20);
			this.comboBox_comPort.TabIndex = 12;
			this.comboBox_comPort.SelectedIndexChanged += new System.EventHandler(this.comboBox_comPort_SelectedIndexChanged);
			// 
			// button_cancel
			// 
			this.button_cancel.Location = new System.Drawing.Point(213, 136);
			this.button_cancel.Name = "button_cancel";
			this.button_cancel.Size = new System.Drawing.Size(75, 23);
			this.button_cancel.TabIndex = 14;
			this.button_cancel.Text = "취소";
			this.button_cancel.UseVisualStyleBackColor = true;
			this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
			// 
			// button_apply
			// 
			this.button_apply.Location = new System.Drawing.Point(132, 136);
			this.button_apply.Name = "button_apply";
			this.button_apply.Size = new System.Drawing.Size(75, 23);
			this.button_apply.TabIndex = 13;
			this.button_apply.Text = "적용";
			this.button_apply.UseVisualStyleBackColor = true;
			this.button_apply.Click += new System.EventHandler(this.button_apply_Click);
			// 
			// panel_comPort
			// 
			this.panel_comPort.Controls.Add(label_comPort);
			this.panel_comPort.Controls.Add(this.comboBox_comPort);
			this.panel_comPort.Location = new System.Drawing.Point(12, 12);
			this.panel_comPort.Name = "panel_comPort";
			this.panel_comPort.Size = new System.Drawing.Size(276, 34);
			this.panel_comPort.TabIndex = 15;
			// 
			// panel_serverAddr
			// 
			this.panel_serverAddr.Controls.Add(this.textBox_serverAddr);
			this.panel_serverAddr.Controls.Add(label_serverAddr);
			this.panel_serverAddr.Location = new System.Drawing.Point(12, 52);
			this.panel_serverAddr.Name = "panel_serverAddr";
			this.panel_serverAddr.Size = new System.Drawing.Size(276, 35);
			this.panel_serverAddr.TabIndex = 16;
			// 
			// panel_serverPort
			// 
			this.panel_serverPort.BackColor = System.Drawing.SystemColors.Control;
			this.panel_serverPort.Controls.Add(label_serverPort);
			this.panel_serverPort.Controls.Add(this.textBox_serverPort);
			this.panel_serverPort.Location = new System.Drawing.Point(12, 93);
			this.panel_serverPort.Name = "panel_serverPort";
			this.panel_serverPort.Size = new System.Drawing.Size(276, 34);
			this.panel_serverPort.TabIndex = 17;
			// 
			// ModemExportSetupForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(297, 165);
			this.ControlBox = false;
			this.Controls.Add(this.panel_serverPort);
			this.Controls.Add(this.panel_serverAddr);
			this.Controls.Add(this.panel_comPort);
			this.Controls.Add(this.button_cancel);
			this.Controls.Add(this.button_apply);
			this.Name = "ModemExportSetupForm";
			this.Text = "ModemExportSetupForm";
			this.panel_comPort.ResumeLayout(false);
			this.panel_comPort.PerformLayout();
			this.panel_serverAddr.ResumeLayout(false);
			this.panel_serverAddr.PerformLayout();
			this.panel_serverPort.ResumeLayout(false);
			this.panel_serverPort.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox textBox_serverAddr;
		private System.Windows.Forms.TextBox textBox_serverPort;
		private System.Windows.Forms.ComboBox comboBox_comPort;
		private System.Windows.Forms.Button button_cancel;
		private System.Windows.Forms.Button button_apply;
		private System.Windows.Forms.Panel panel_comPort;
		private System.Windows.Forms.Panel panel_serverAddr;
		private System.Windows.Forms.Panel panel_serverPort;

	}
}