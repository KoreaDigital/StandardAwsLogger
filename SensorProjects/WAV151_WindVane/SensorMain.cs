using SDKforAWS;
using System;
using System.Collections;

namespace WAV151_WindVane
{
	public class SensorMain : WSensor
	{
		private const string TAG = "WAV151";

		private const float RESOLUTION = 5.625f;

		public SensorMain(int n1, int n2)
			: base(n1, n2)
		{
			this.input_type = SensorInputType.DIGITAL_BINARYCODE;
			this.minsamplingHz = 1;

			this.sensor_name = "VAISALA WAV151";
			this.description = "VAISALA WAV151 - Wind Vane";

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
			foreach( WSensor.Unit un in units )
			{
				int grayCode = Interface.digital.getBinaryCode();
				if( Int32.MinValue != grayCode )
				{
					Log.i(TAG, "Wind Vane : " + Convert.ToString(grayCode, 2));

					int value = GrayCode.GrayToInt(new BitArray(new int[] { grayCode }));
					value = value & 0x3F;

					un.value = value * RESOLUTION;
				}
				else
				{
					Log.e(TAG, "Wind Vane : Data Read Error!");
				}
			}
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
