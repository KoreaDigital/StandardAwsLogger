using System;
using System.Windows.Forms;

namespace TestCSVExport
{
	public partial class CsvExportSetupForm : Form
	{
		private ExportSetting mSetting;

		private static string[] SPLIT_TEXT = new string[] { "하나의 파일로 저장", "매일 새로운 파일을 생성", "주마다 새로운 파일을 생성", "월마다 새로운 파일을 생성" };

		public CsvExportSetupForm(ExportSetting setting)
		{
			InitializeComponent();

			mSetting = setting;

			initControl();
			updateControl();
			this.DialogResult = DialogResult.Cancel;
		}

		private void initControl()
		{
			comboBox_fileSplit.Items.Clear();
			for( int ii = 0; ii < SPLIT_TEXT.Length; ++ii )
			{
				comboBox_fileSplit.Items.Add(SPLIT_TEXT[ii]);
			}

			comboBox_fileSplit.SelectedIndexChanged += new EventHandler(comboBox_fileSplit_SelectedIndexChanged);
		}

		private void updateControl()
		{
			textBox_directory.Text = mSetting.TargetDir;
			textBox_fileNamePrefix.Text = mSetting.FilePrefix;
			comboBox_fileSplit.SelectedIndex = (int) mSetting.SplitType - 1;

			label_fileNameExample.Text = "파일명 : " + mSetting.getFileName();
		}

		private void button_directory_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			dialog.ShowNewFolderButton = true;
			if( DialogResult.OK == dialog.ShowDialog() )
			{
				mSetting.TargetDir = dialog.SelectedPath;

				updateControl();
			}
		}

		private void comboBox_fileSplit_SelectedIndexChanged(object sender, EventArgs e)
		{
			FileSplit splitType = (FileSplit) (comboBox_fileSplit.SelectedIndex + 1);
			if( splitType != mSetting.SplitType )
			{
				mSetting.SplitType = splitType;
				updateControl();
			}
		}

		private void textBox_fileNamePrefix_TextChanged(object sender, EventArgs e)
		{
			if( false == string.Equals(textBox_fileNamePrefix.Text, mSetting.FilePrefix) )
			{
				mSetting.FilePrefix = textBox_fileNamePrefix.Text;
				updateControl();
			}
		}

		private void button_apply_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void button_cancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}
	}
}
