﻿using SDKforAWS;

namespace HML155_Analog_Temp
{
	public class SensorMain : WSensor
	{
		private const string TAG = "HML155_A_T";

		public SensorMain(int n1, int n2)
			: base(n1, n2)
		{
			this.input_type = SensorInputType.ANALOG_SINGLE_ENDED_10V;
			this.minsamplingHz = 1;

			this.sensor_name = "VAISALA HML155";
			this.description = "VAISALA HML155 - Analog Temperature";

			WSensor.Unit unit = new WSensor.Unit();
			{
				unit.unit_name = "Temperature";
				unit.unit_text = "℃";
				unit.unit_format = "{0:#0.00}";

				unit.sensor_type = KMASensorType.Sensor_Temperature;

				unit.min = -80;
				unit.max = 60;
			}

			this.units.Clear();
			this.units.Add(unit);
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
				// -80 ~ +60
				double voltage = Interface.analog.getVoltage();
				Log.d(TAG, "Analog Temperature : " + voltage);
				double temp = voltage * 140 - 80;

				un.value = temp;
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
