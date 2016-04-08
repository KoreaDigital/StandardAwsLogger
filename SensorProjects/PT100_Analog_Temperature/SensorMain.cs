using SDKforAWS;

namespace PT100_Analog_Temp
{
	public class SensorMain : WSensor
	{
		private const string TAG = "PT100_Analog_Temp";

		public SensorMain(int n1, int n2)
			: base(n1, n2)
		{
			this.input_type = SensorInputType.ANALOG_TMEPERATUE;
			this.minsamplingHz = 1;
			this.port_param.analog_Rsensor_ch = PortSetup_params.RSENSOR_CHANNEL.RSENSOR_CH1;

			this.sensor_name = "PT100";
			this.description = "PT100 - Analog Temp";

			this.units.Clear();
			{
				WSensor.Unit unit = new WSensor.Unit();
				unit.unit_name = "Temperature";
				unit.unit_text = "℃";
				unit.unit_format = "{0:#0.00}";
				unit.sensor_type = KMASensorType.Sensor_Temperature;

				this.units.Add(unit);
			}
		}

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

		public override void OnMeasure()
		{
			double temp = Interface.analog.getTemperature();

			if( false == double.IsNaN(temp) )
			{
				WSensor.Unit tempUnit = units[0];
				tempUnit.value = temp;
			}
		}

		/// <summary>
		/// 센서 측정이 멈췄을 때 호출
		/// </summary>
		public override void OnStop()
		{
			// __noop;
		}

		public override void OnDestroy()
		{
		}

		public override void Setup()
		{
		}
	}
}
