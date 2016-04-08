using SDKforAWS;
using System.Text;

namespace JY_WD160C_RS232
{
	public class SensorMain : WSensor
	{
		private const string TAG = "WD160C_RS232";
		private const string DELIMINATOR = "W/D: ";

		private double mWindVane;

		public SensorMain(int n1, int n2)
			: base(n1, n2)
		{
			this.input_type = SensorInputType.COMM_RS232;
			this.minsamplingHz = 1;

			this.sensor_name = "JINYANG WD160C";
			this.description = "JINYANG WD160C - WindVane";

			this.units.Clear();

			{
				WSensor.Unit unit = new WSensor.Unit();
				unit.unit_name = "Wind Vane";
				unit.unit_text = "˚";
				unit.unit_format = "{0:0.0}";
				unit.sensor_type = KMASensorType.Sensor_WindVane;

				unit.min = 0;
				unit.max = 360;

				this.units.Add(unit);
			}
		}

		/// <summary>
		/// 센서초기화 1회만 호출
		/// </summary>
		/// <returns></returns>
		public override bool OnInitialize()
		{
			// 485 초기화
			WInterface.Comm_Input.RS232_485_SDI comm = Interface.comm.rs232_rs485_sdi12;
			comm.BaudRate = 9600;
			comm.Parity = System.IO.Ports.Parity.None;
			comm.DataBits = 8;
			comm.StopBits = System.IO.Ports.StopBits.One;

			comm.DataReceived += CommDataReceived;

			return true;
		}

		private void CommDataReceived(byte[] buffer)
		{
			// W/S: 000.0  W/D: 244.7
			string msg = Encoding.ASCII.GetString(buffer);
			int pos = msg.IndexOf(DELIMINATOR);
			if( 0 <= pos )
			{
				msg = msg.Substring(pos + DELIMINATOR.Length);

				try
				{
					mWindVane = double.Parse(msg);
				}
				catch { }
			}
		}

		/// <summary>
		/// 센서 측정 시작 전에 호출
		/// </summary>
		/// <returns></returns>
		public override bool OnStart()
		{
			WInterface.Comm_Input.RS232_485_SDI comm = Interface.comm.rs232_rs485_sdi12;
			return comm.open();
		}

		/// <summary>
		/// 센서 값을 측정한다. 로거프로그램에서 주기적으로 호출된다.
		/// </summary>
		public override void OnMeasure()
		{
			WSensor.Unit windVaneUnit = units[0];
			windVaneUnit.value = mWindVane;
		}

		/// <summary>
		/// 센서 측정이 멈췄을 때 호출
		/// </summary>
		public override void OnStop()
		{
			WInterface.Comm_Input.RS232_485_SDI comm = Interface.comm.rs232_rs485_sdi12;
			comm.close();
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
