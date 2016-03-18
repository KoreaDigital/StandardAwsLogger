namespace TestCSVExport
{
	partial class CsvExportSetupForm
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
			System.Windows.Forms.Label label_fileNamePrefix;
			System.Windows.Forms.Label label_fileSplit;
			this.textBox_directory = new System.Windows.Forms.TextBox();
			this.button_directory = new System.Windows.Forms.Button();
			this.button_apply = new System.Windows.Forms.Button();
			this.button_cancel = new System.Windows.Forms.Button();
			this.textBox_fileNamePrefix = new System.Windows.Forms.TextBox();
			this.comboBox_fileSplit = new System.Windows.Forms.ComboBox();
			this.label_fileNameExample = new System.Windows.Forms.Label();
			label_fileNamePrefix = new System.Windows.Forms.Label();
			label_fileSplit = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textBox_directory
			// 
			this.textBox_directory.Enabled = false;
			this.textBox_directory.Location = new System.Drawing.Point(13, 13);
			this.textBox_directory.Name = "textBox_directory";
			this.textBox_directory.Size = new System.Drawing.Size(256, 21);
			this.textBox_directory.TabIndex = 0;
			// 
			// button_directory
			// 
			this.button_directory.Location = new System.Drawing.Point(275, 12);
			this.button_directory.Name = "button_directory";
			this.button_directory.Size = new System.Drawing.Size(97, 23);
			this.button_directory.TabIndex = 1;
			this.button_directory.Text = "디렉토리 지정";
			this.button_directory.UseVisualStyleBackColor = true;
			this.button_directory.Click += new System.EventHandler(this.button_directory_Click);
			// 
			// button_apply
			// 
			this.button_apply.Location = new System.Drawing.Point(216, 165);
			this.button_apply.Name = "button_apply";
			this.button_apply.Size = new System.Drawing.Size(75, 23);
			this.button_apply.TabIndex = 2;
			this.button_apply.Text = "적용";
			this.button_apply.UseVisualStyleBackColor = true;
			this.button_apply.Click += new System.EventHandler(this.button_apply_Click);
			// 
			// button_cancel
			// 
			this.button_cancel.Location = new System.Drawing.Point(297, 165);
			this.button_cancel.Name = "button_cancel";
			this.button_cancel.Size = new System.Drawing.Size(75, 23);
			this.button_cancel.TabIndex = 3;
			this.button_cancel.Text = "취소";
			this.button_cancel.UseVisualStyleBackColor = true;
			this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
			// 
			// label_fileNamePrefix
			// 
			label_fileNamePrefix.AutoSize = true;
			label_fileNamePrefix.Location = new System.Drawing.Point(13, 51);
			label_fileNamePrefix.Name = "label_fileNamePrefix";
			label_fileNamePrefix.Size = new System.Drawing.Size(89, 12);
			label_fileNamePrefix.TabIndex = 4;
			label_fileNamePrefix.Text = "파일명 Prefix : ";
			// 
			// textBox_fileNamePrefix
			// 
			this.textBox_fileNamePrefix.Location = new System.Drawing.Point(108, 48);
			this.textBox_fileNamePrefix.Name = "textBox_fileNamePrefix";
			this.textBox_fileNamePrefix.Size = new System.Drawing.Size(161, 21);
			this.textBox_fileNamePrefix.TabIndex = 5;
			this.textBox_fileNamePrefix.TextChanged += new System.EventHandler(this.textBox_fileNamePrefix_TextChanged);
			// 
			// label_fileSplit
			// 
			label_fileSplit.AutoSize = true;
			label_fileSplit.Location = new System.Drawing.Point(13, 92);
			label_fileSplit.Name = "label_fileSplit";
			label_fileSplit.Size = new System.Drawing.Size(93, 12);
			label_fileSplit.TabIndex = 6;
			label_fileSplit.Text = "파일 분할 단위 :";
			// 
			// comboBox_fileSplit
			// 
			this.comboBox_fileSplit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_fileSplit.FormattingEnabled = true;
			this.comboBox_fileSplit.Location = new System.Drawing.Point(112, 89);
			this.comboBox_fileSplit.Name = "comboBox_fileSplit";
			this.comboBox_fileSplit.Size = new System.Drawing.Size(157, 20);
			this.comboBox_fileSplit.TabIndex = 7;
			// 
			// label_fileNameExample
			// 
			this.label_fileNameExample.AutoSize = true;
			this.label_fileNameExample.Location = new System.Drawing.Point(13, 137);
			this.label_fileNameExample.Name = "label_fileNameExample";
			this.label_fileNameExample.Size = new System.Drawing.Size(53, 12);
			this.label_fileNameExample.TabIndex = 8;
			this.label_fileNameExample.Text = "파일명 : ";
			// 
			// CsvExportSetupForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(384, 200);
			this.ControlBox = false;
			this.Controls.Add(this.label_fileNameExample);
			this.Controls.Add(this.comboBox_fileSplit);
			this.Controls.Add(label_fileSplit);
			this.Controls.Add(this.textBox_fileNamePrefix);
			this.Controls.Add(label_fileNamePrefix);
			this.Controls.Add(this.button_cancel);
			this.Controls.Add(this.button_apply);
			this.Controls.Add(this.button_directory);
			this.Controls.Add(this.textBox_directory);
			this.Name = "CsvExportSetupForm";
			this.Text = "CSV Export Setting";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBox_directory;
		private System.Windows.Forms.Button button_directory;
		private System.Windows.Forms.Button button_apply;
		private System.Windows.Forms.Button button_cancel;
		private System.Windows.Forms.TextBox textBox_fileNamePrefix;
		private System.Windows.Forms.ComboBox comboBox_fileSplit;
		private System.Windows.Forms.Label label_fileNameExample;
	}
}