using System;
using System.Drawing;
using System.IO.Ports;
using System.Net;
using System.Windows.Forms;

namespace NetworkExport
{
	public partial class NetworkExportSetupForm : Form
	{
		private ExportSetting mSetting;

		private bool mServerAddrValid;
		private bool mServerPortValid;
		private bool mStationIdValid;

		private static Color ALERT_COLOR = Color.Salmon;
		private static Color NORMAL_COLOR = SystemColors.Control;

		public NetworkExportSetupForm(ExportSetting setting)
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
			textBox_serverAddr.Text = mSetting.ServerAddr;
			textBox_serverPort.Text = mSetting.ServerPort.ToString();
			textBox_station_id.Text = mSetting.StationId.ToString();
		}

		private void checkValidAndUpdateValue()
		{
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

			{
				// Station Id 체크
				try
				{
					int value = Int32.Parse(textBox_station_id.Text);
					mStationIdValid = (0 < value) && (value < 32768);
					if( mStationIdValid )
					{
						mSetting.StationId = value;
					}
				}
				catch
				{
					mStationIdValid = false;
				}
				panel_StationId.BackColor = mStationIdValid ? NORMAL_COLOR : ALERT_COLOR;
			}

			button_apply.Enabled = (mStationIdValid && mServerAddrValid && mServerPortValid);
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

		private void textBox_station_id_TextChanged(object sender, EventArgs e)
		{
			checkValidAndUpdateValue();
		}

		private void textBox_station_id_KeyPress(object sender, KeyPressEventArgs e)
		{
			// Station ID 는 숫자만 입력되도록 한다.
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
