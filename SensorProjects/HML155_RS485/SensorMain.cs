using SDKforAWS;
using System;
using System.Text;
using System.Threading;

namespace HML155_RS485
{
	public class SensorMain : WSensor
	{
		private const string TAG = "HML155_RS485";

		private static byte[] CMD_SEND = Encoding.ASCII.GetBytes("send\r\n");
		private static byte[] CMD_START = Encoding.ASCII.GetBytes("r\r\n");
		private static byte[] CMD_STOP = Encoding.ASCII.GetBytes("s\r\n");
		// 메시지 예시 > "[ 23.12, 18.03,EA]\r\n"
		// 고정길이 msg   [TTT.TT,HHH.HH,CS] 형식임
		// handleMsg 는 이 고정길이 문자열에 맞게 코딩되었음. 이 format 이 변경되면 handleMsg 함수도 변경해줘야 함.
		private static byte[] CMD_FORM = Encoding.ASCII.GetBytes("form \"[\" 3.2 t \",\" 3.2 rh \",\" CS2 \"]\" #r#n\r\n");
		private static byte[] CMD_INTERVAL = Encoding.ASCII.GetBytes("intv 1 s\r\n");

		private const string DELIMITOR_CMD = "\r\n";
		private const string PARSING_ERROR = "Parsing Error";

		private const byte SPECIAL_CHAR_DOLLAR = (byte) '$';
		private const byte SPECIAL_CHAR_ASTERIK = (byte) '*';

		private string mMsgBuffer;
		private double mTemperature;
		private double mHumidity;

		public SensorMain(int n1, int n2)
			: base(n1, n2)
		{
			this.input_type = SensorInputType.COMM_RS485;
			this.minsamplingHz = 1;

			this.sensor_name = "VAISALA HML155";
			this.description = "VAISALA HML155 - Digital Temperature/Humidity";

			this.units.Clear();

			{
				WSensor.Unit unit = new WSensor.Unit();
				unit.unit_name = "Temperature";
				unit.unit_text = "℃";
				unit.unit_format = "{0:#0.00}";

				unit.min = -80;
				unit.max = 60;

				this.units.Add(unit);
			}

			{
				WSensor.Unit unit = new WSensor.Unit();
				unit.unit_name = "Humidity";
				unit.unit_text = "RH %";
				unit.unit_format = "{0:#0.0}";

				unit.min = 0;
				unit.min = 100;

				this.units.Add(unit);
			}

			mMsgBuffer = string.Empty;
		}

		/// <summary>
		/// 센서초기화 1회만 호출
		/// </summary>
		/// <returns></returns>
		public override bool OnInitialize()
		{
			// 485 초기화
			WInterface.Comm_Input.RS232_485_SDI comm = Interface.comm.rs232_rs485_sdi12;
			comm.BaudRate = 4800;
			comm.Parity = System.IO.Ports.Parity.Even;
			comm.DataBits = 7;
			comm.StopBits = System.IO.Ports.StopBits.One;

			comm.DataReceived += CommDataReceived;
			// comm.FlowControl = None;

			return true;
		}

		private void CommDataReceived(byte[] buffer)
		{
			mMsgBuffer += Encoding.ASCII.GetString(buffer);

			int index;
			string msg;
			while( 0 < (index = mMsgBuffer.IndexOf(DELIMITOR_CMD)) )
			{
				msg = mMsgBuffer.Substring(0, index).Trim();
				mMsgBuffer = mMsgBuffer.Substring(index + 2);

				if( (0 < msg.Length) && msg.StartsWith("[") )
				{
					handleMsg(msg);
				}
				else
				{
					Log.e(TAG, "VAISALA HML155 - RS485 - Msg Received : " + msg);
				}
			}
		}

		// CMD_FORM 에 따라 고정길이 문자열을 가정하고 코딩한 것임.
		// CMD_FORM 이 바뀌면 이 코드도 수정해야 함.
		private void handleMsg(string msg)
		{
			// 012345678901234567
			// [TTT.TT,HHH.HH,CS]
			// 데이터 검증
			if( (18 != msg.Length) || ('[' != msg[0]) || (',' != msg[7]) || (',' != msg[14]) || (']' != msg[17]) )
			{
				Log.e(TAG, "데이터 포맷 불일치 : " + msg);
				return;
			}

			string tmp = PARSING_ERROR;

			int checkSum = 0;
			try
			{
				tmp = msg.Substring(15, 2);
				checkSum = Convert.ToInt32(tmp, 16);
			}
			catch( Exception e )
			{
				Log.e(TAG, "Checksum Parsing Error : " + tmp);
				Log.e(TAG, e.ToString());
				return;
			}

			// CheckSum 앞부분에서 자르기
			int calCheckSum = calculateCheckSum(msg.Substring(0, 15));
			if( checkSum != calCheckSum )
			{
				Log.e(TAG, "CheckSum Fail = " + checkSum + ", Cal = " + calCheckSum);
				return;
			}

			double temperature = 0;
			tmp = PARSING_ERROR;
			try
			{
				tmp = msg.Substring(1, 6);
				temperature = Double.Parse(tmp);
			}
			catch( Exception e )
			{
				Log.e(TAG, "Temperature Parsing Error : " + tmp);
				Log.e(TAG, e.ToString());
				return;
			}

			double humidity = 0;
			tmp = PARSING_ERROR;
			try
			{
				tmp = msg.Substring(8, 6);
				humidity = Double.Parse(tmp);
			}
			catch( Exception e )
			{
				Log.e(TAG, "Humidity Parsing Error : " + tmp);
				Log.e(TAG, e.ToString());
				return;
			}

			Log.d(TAG, "Success : " + msg);

			mTemperature = temperature;
			mHumidity = humidity;
		}

		private int calculateCheckSum(string msg)
		{
			byte[] buffer = Encoding.ASCII.GetBytes(msg);
			int sum = 0;
			byte tmp;

			for( int ii = 0; ii < buffer.Length; ++ii )
			{
				tmp = buffer[ii];
				if( (SPECIAL_CHAR_ASTERIK != tmp) && (SPECIAL_CHAR_DOLLAR != tmp) )
				{
					sum += tmp;
				}
			}

			return sum % 256;
		}

		/// <summary>
		/// 센서 측정 시작 전에 호출
		/// </summary>
		/// <returns></returns>
		public override bool OnStart()
		{
			WInterface.Comm_Input.RS232_485_SDI comm = Interface.comm.rs232_rs485_sdi12;
			bool rv = comm.open();
			if( true == rv )
			{
				Log.i(TAG, "Send Stop Command");
				rv = comm.write(CMD_STOP);
				if( false == rv )
				{
					Log.e(TAG, "Stop Command Fail");
					return false;
				}

				Thread.Sleep(300);

				Log.i(TAG, "Send Form Command");
				rv = comm.write(CMD_FORM);
				if( false == rv )
				{
					Log.e(TAG, "Form Command Fail");
					return false;
				}

				Thread.Sleep(300);

				Log.i(TAG, "Send Intv Command");
				rv = comm.write(CMD_INTERVAL);
				if( false == rv )
				{
					Log.e(TAG, "Intv Command Fail");
					return false;
				}

				Thread.Sleep(300);

				Log.i(TAG, "Send Start Command");
				rv = comm.write(CMD_START);
				if( false == rv )
				{
					Log.e(TAG, "Start Command Fail");
					return false;
				}

				Thread.Sleep(300);

				return true;
			}
			return false;
		}

		/// <summary>
		/// 센서 값을 측정한다. 로거프로그램에서 주기적으로 호출된다.
		/// </summary>
		public override void OnMeasure()
		{
			{
				WSensor.Unit tempUnit = units[0];
				tempUnit.value = mTemperature;
			}

			{
				WSensor.Unit humiUnit = units[1];
				humiUnit.value = mHumidity;
			}
		}

		/// <summary>
		/// 센서 측정이 멈췄을 때 호출
		/// </summary>
		public override void OnStop()
		{
			WInterface.Comm_Input.RS232_485_SDI comm = Interface.comm.rs232_rs485_sdi12;

			bool rv;
			Log.i(TAG, "Send Stop Command");
			rv = comm.write(CMD_STOP);
			if( false == rv )
			{
				Log.e(TAG, "Stop Command Fail");
			}

			Thread.Sleep(300);

			rv = comm.close();
			if( false == rv )
			{
				Log.e(TAG, "Comm Close Fail");
			}
		}

		/// <summary>
		/// 해당 dll삭제될때 호출됨.  해당기능을 더이상 사용하지 않음. 생성한 리소스나 쓰레드를 반드시 종료시킨다.
		/// </summary>
		public override void OnDestroy()
		{
			WInterface.Comm_Input.RS232_485_SDI comm = Interface.comm.rs232_rs485_sdi12;
			comm.DataReceived -= CommDataReceived;
		}

		/// <summary>
		/// 센서설정이 필요한 경우 구현
		/// </summary>
		public override void Setup()
		{
		}
	}
}
