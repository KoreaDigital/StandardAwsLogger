using SDKforAWS;
using System;
using System.Diagnostics;

namespace WAA151_Anemometer
{
	public class SensorMain : WSensor
	{
		private const string TAG = "WAA151";

		public SensorMain(int n1, int n2)
			: base(n1, n2)
		{
			this.input_type = SensorInputType.DIGITAL_PWM_HZ;
			this.port_param.digital_sampling_duration_ms = 1000;
			this.minsamplingHz = 1;

			this.sensor_name = "VAISALA WAA151";
			this.description = "VAISALA WAA151 - Anemometer";

			this.units.Clear();

			{
				WSensor.Unit unit = new WSensor.Unit();
				unit.unit_name = "Anemometer";
				unit.unit_text = "m/s";
				unit.unit_format = "{0:0.##}";

				unit.min = 0.4;
				unit.max = 75;

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
				double hz = Interface.digital.getPWMHz();
				if( false == Double.IsNaN(hz) )
				{
					Log.i(TAG, "HZ : " + hz);
					double calcValue = 0.1007 * hz + 0.3278;
					calcValue = Math.Min(calcValue, un.max);
					if( calcValue < un.min )
					{
						calcValue = 0;
					}
					un.value = calcValue;
				}
				else
				{
					Log.e(TAG, "HZ : Data Read Error");
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
