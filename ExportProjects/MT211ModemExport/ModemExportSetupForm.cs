using System;
using System.Drawing;
using System.IO.Ports;
using System.Net;
using System.Windows.Forms;
using com.koreadigital.common;

namespace MT211ModemExport
{
	public partial class ModemExportSetupForm : Form
	{
		private ExportSetting mSetting;

		private bool mComportValid;
		private bool mServerAddrValid;
		private bool mServerPortValid;

		private static Color ALERT_COLOR = Color.Salmon;
		private static Color NORMAL_COLOR = SystemColors.Control;

		public ModemExportSetupForm(ExportSetting setting)
		{
			InitializeComponent();

			mSetting = setting;
			initControls();
			checkValidAndUpdateValue();

			this.DialogResult = DialogResult.Cancel;
		}

		private void initControls()
		{
			// Port 번호를 채움
			{
				comboBox_comPort.Items.Clear();
				string[] portNames = SerialPort.GetPortNames();
				string selectedPort = mSetting.ComPortName;
				int selectedIndex = -1;

				for( int ii = 0; ii < portNames.Length; ++ii )
				{
					if( (null != selectedPort) && (portNames[ii] == selectedPort) )
					{
						selectedIndex = ii;
					}

					comboBox_comPort.Items.Add(portNames[ii]);
				}

				if( 0 <= selectedIndex )
				{
					comboBox_comPort.SelectedIndex = selectedIndex;
				}
			}

			// Server 주소를 채움
			textBox_serverAddr.Text = mSetting.ServerAddr;
			textBox_serverPort.Text = mSetting.ServerPort.ToString();
		}

		private void checkValidAndUpdateValue()
		{
			{
				// ComPort 체크
				mComportValid = (0 <= comboBox_comPort.SelectedIndex);
				if( mComportValid )
				{
					mSetting.ComPort = Int32.Parse(((string) comboBox_comPort.SelectedItem).Substring(3));
				}
				panel_comPort.BackColor = mComportValid ? NORMAL_COLOR : ALERT_COLOR;
			}

			{
				// ServerAddr 체크
				string serverAddr = textBox_serverAddr.Text;
				if( 0 == serverAddr.Length )
				{
					mServerAddrValid = false;
				}
				else
				{
					// IP 주소 형태인지 확인
					if( isNumberAndDotOnly(serverAddr) )
					{
						mServerAddrValid = isIpv4Format(serverAddr);
					}
					else
					{
						try
						{
							IPAddress[] addrs = Dns.GetHostAddresses(serverAddr);
							mServerAddrValid = (0 < addrs.Length);
						}
						catch
						{
							mServerAddrValid = false;
						}
					}
				}
				if( mServerAddrValid )
				{
					mSetting.ServerAddr = serverAddr;
				}
				panel_serverAddr.BackColor = mServerAddrValid ? NORMAL_COLOR : ALERT_COLOR;
			}

			{
				// ServerPort 체크
				try
				{
					int value = Int32.Parse(textBox_serverPort.Text);
					mServerPortValid = (0 < value);
					if( mServerPortValid )
					{
						mSetting.ServerPort = value;
					}
				}
				catch
				{
					mServerPortValid = false;
				}
				panel_serverPort.BackColor = mServerPortValid ? NORMAL_COLOR : ALERT_COLOR;
			}

			button_apply.Enabled = (mComportValid && mServerAddrValid && mServerPortValid);
		}

		private void comboBox_comPort_SelectedIndexChanged(object sender, EventArgs e)
		{
			checkValidAndUpdateValue();
		}

		private void textBox_serverAddr_TextChanged(object sender, EventArgs e)
		{
			checkValidAndUpdateValue();
		}

		private void textBox_serverPort_TextChanged(object sender, EventArgs e)
		{
			checkValidAndUpdateValue();
		}

		private void textBox_serverPort_KeyPress(object sender, KeyPressEventArgs e)
		{
			// 포트 번호는 숫자만 입력되도록 한다.

			bool isDigit = Char.IsDigit(e.KeyChar);
			bool isBackspace = ((int) Keys.Back == e.KeyChar);
			if( (false == isDigit) && (false == isBackspace) )
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

		public static bool isNumberAndDotOnly(string addr)
		{
			bool isNumber, isDot;
			foreach( char c in addr )
			{
				isNumber = Char.IsNumber(c);
				isDot = ('.' == c);

				if( !isNumber && !isDot )
				{
					return false;
				}
			}

			return true;
		}

		public static bool isIpv4Format(string addr)
		{
			if( null == addr )
			{
				return false;
			}

			string[] list = addr.Split(new char[] { '.' });
			if( 4 != list.Length )
			{
				return false;
			}

			foreach( string part in list )
			{
				if( 0 == part.Length )
				{
					return false;
				}

				foreach( char c in part )
				{
					// 숫자가 아니면
					if( false == Char.IsNumber(c) )
					{
						return false;
					}
				}

				if( 256 <= Int32.Parse(part) )
				{
					return false;
				}
			}

			return true;
		}
	}
}
