using SDKforAWS;

namespace KD_SWHT_7101
{
	public class SensorMain : WSensor
	{
		private const string TAG = "SWHT_7101";

		private byte[] mWriteBuffer;
		private byte[] mReadBuffer;

		public SensorMain(int n1, int n2)
			: base(n1, n2)
		{
			this.input_type = SensorInputType.COMM_I2C;
			this.minsamplingHz = 1;

			this.sensor_name = "KD SWHT7101";
			this.description = "KD SWHT7101 - Temperature/Humidity";

			this.units.Clear();

			{
				WSensor.Unit unit = new WSensor.Unit();
				unit.unit_name = "Temperature";
				unit.unit_text = "℃";
				unit.unit_format = "{0:#0.00}";
				unit.sensor_type = KMASensorType.Sensor_Normal;

				unit.min = -80;
				unit.max = 60;

				this.units.Add(unit);
			}

			{
				WSensor.Unit unit = new WSensor.Unit();
				unit.unit_name = "Humidity";
				unit.unit_text = "RH %";
				unit.unit_format = "{0:#0.0}";
				unit.sensor_type = KMASensorType.Sensor_Normal;

				unit.min = 0;
				unit.min = 100;

				this.units.Add(unit);
			}

			mWriteBuffer = new byte[1];
			mReadBuffer = new byte[2];
		}

		/// <summary>
		/// 센서초기화 1회만 호출
		/// </summary>
		/// <returns></returns>
		public override bool OnInitialize()
		{
			return true;
		}

		/// <summary>
		/// 센서 측정 시작 전에 호출
		/// </summary>
		/// <returns></returns>
		public override bool OnStart()
		{
			// __noop;
			return true;
		}

		/// <summary>
		/// 센서 값을 측정한다. 로거프로그램에서 주기적으로 호출된다.
		/// </summary>
		public override void OnMeasure()
		{
			mWriteBuffer[0] = 0x0;

			double temperature;
			{
				bool ret = Interface.comm.i2c.writeread(0x48, mWriteBuffer, mReadBuffer);
				if( ret == true )
				{
					byte sflag = (byte) (mReadBuffer[0] & 0x80);
					int upperbyte = ((mReadBuffer[0] & 0x7F) << 5);
					int lowerbyte = ((mReadBuffer[1] & 0xF8) >> 3);
					double T_ambient = upperbyte + lowerbyte;

					if( sflag == 0x80 )   //if 5th bit of upperbyte is a signed bit which means that ambient temperature is lower thatn 0 deg.C
					{
						T_ambient = (T_ambient - 4096) / 16;
					}
					else
					{
						T_ambient = (T_ambient) / 16;
					}

					temperature = T_ambient;
				}
				else
				{
					temperature = 9999;
				}
			}
			units[0].value = temperature;

			double humidity;
			{
				bool ret = Interface.comm.i2c.writeread(0x28, mWriteBuffer, mReadBuffer);
				if( ret == true )
				{
					humidity = ((mReadBuffer[0] & 0x3f) << 8) + (mReadBuffer[1] & 0xff);
					humidity = humidity / 16384.0 * 100;
				}
				else
				{
					humidity = 9999;
				}
			}
			units[1].value = humidity;
		}

		/// <summary>
		/// 센서 측정이 멈췄을 때 호출
		/// </summary>
		public override void OnStop()
		{
			// __noop;
		}

		/// <summary>
		/// 해당 dll삭제될때 호출됨.  해당기능을 더이상 사용하지 않음. 생성한 리소스나 쓰레드를 반드시 종료시킨다.
		/// </summary>
		public override void OnDestroy()
		{
		}

		/// <summary>
		/// 센서설정이 필요한 경우 구현
		/// </summary>
		public override void Setup()
		{
		}
	}
}
