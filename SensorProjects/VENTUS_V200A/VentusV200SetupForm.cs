using System;
using System.Drawing;
using System.Windows.Forms;

namespace VENTUS_V200A
{
	public partial class VentusV200SetupForm : Form
	{
		private SensorSetting mSetting;

		private bool mDeviceAddrValid;

		private static Color ALERT_COLOR = Color.Salmon;
		private static Color NORMAL_COLOR = SystemColors.Control;

		public VentusV200SetupForm(SensorSetting setting)
		{
			InitializeComponent();

			mSetting = setting;
			initControls();
			checkValidAndUpdateValue();

			this.DialogResult = DialogResult.Cancel;
		}

		private void initControls()
		{
			// Server 주소를 채움
			textBox_deviceAddr.Text = mSetting.DeviceAddr.ToString("X");
		}

		private void checkValidAndUpdateValue()
		{
			{
				// ServerPort 체크
				try
				{
					int value = Convert.ToInt32(textBox_deviceAddr.Text, 16);
					mDeviceAddrValid = (0 < value);
					if( mDeviceAddrValid )
					{
						mSetting.DeviceAddr = value;
					}
				}
				catch
				{
					mDeviceAddrValid = false;
				}
				panel_deviceAddr.BackColor = mDeviceAddrValid ? NORMAL_COLOR : ALERT_COLOR;
			}

			button_apply.Enabled = mDeviceAddrValid;
		}

		private void textBox_deviceAddr_TextChanged(object sender, EventArgs e)
		{
			checkValidAndUpdateValue();
		}

		private void textBox_deviceAddr_KeyPress(object sender, KeyPressEventArgs e)
		{
			// 포트 번호는 숫자만 입력되도록 한다.
			bool isDigit = Char.IsDigit(e.KeyChar);
			bool isBackspace = ((int) Keys.Back == e.KeyChar);
			bool isAF = (('A' <= e.KeyChar) && (e.KeyChar <= 'F')) || (('a' <= e.KeyChar) && (e.KeyChar <= 'f'));
			if( (false == isDigit) && (false == isBackspace) && (false == isAF) )
			{
				// 지정한 문자 외에는 무시
				e.Handled = true;
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
