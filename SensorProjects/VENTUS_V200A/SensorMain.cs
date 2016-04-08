using SDKforAWS;
using System.Diagnostics;
using System.IO.Ports;
using System.Windows.Forms;

namespace VENTUS_V200A
{
	public class SensorMain : WSensor
	{
		private const string TAG = "V200_Wind";

		public const ushort CH_WIND_SPEED = 400;
		public const ushort CH_WIND_DIRECTION = 500;

		private ByteCircularBuffer mCircularBuffer;
		private SensorSetting mSetting;

		private double mWindSpeed;
		private double mWindVane;

		public SensorMain(int n1, int n2)
			: base(n1, n2)
		{
			this.input_type = SensorInputType.COMM_RS485;
			this.minsamplingHz = 1;

			this.sensor_name = "V200A";
			this.description = "V200A - Wind";

			this.units.Clear();
			{
				WSensor.Unit unit = new WSensor.Unit();
				unit.unit_name = "Anemometer";
				unit.unit_text = "m/s";
				unit.unit_format = "{0:0.##}";
				unit.sensor_type = KMASensorType.Sensor_WindSpeed;

				unit.min = 0.4;
				unit.max = 75;

				this.units.Add(unit);
			}

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

			mCircularBuffer = new ByteCircularBuffer(2048, true);
			mSetting = SensorSetting.load();
		}

		public override bool OnInitialize()
		{
			WInterface.Comm_Input.RS232_485_SDI comm = Interface.comm.rs232_rs485_sdi12;
			comm.BaudRate = 19200;
			comm.Parity = Parity.None;
			comm.DataBits = 8;
			comm.StopBits = StopBits.One;

			comm.DataReceived += CommDataReceived;

			return true;
		}

		private void CommDataReceived(byte[] buffer)
		{
			mCircularBuffer.pushBytes(buffer, 0, buffer.Length);
			while( true )
			{
				UmbBinaryProtocol packet = UmbBinaryProtocol.parsePacket(mCircularBuffer);
				if( null == packet )
				{
					break;
				}

				if( false == packet.veryfyChecksum() )
				{
					continue;
				}

				if( packet.To != UmbBinaryProtocol.ADDR_PC )
				{
					continue;
				}

				if( packet.From != (ushort) mSetting.DeviceAddr )
				{
					continue;
				}

				Debug.WriteLine(packet.ToString());
				handleMsg(packet);
			}
		}

		private void handleMsg(UmbBinaryProtocol packet)
		{
			switch( packet.Cmd )
			{
			case UmbBinaryProtocol.CMD_MULTI_CHANNEL_ONLINE_DATA_REQ:
				{
					ByteBuffer buffer = ByteBuffer.wrap(packet.Payload);
					byte status = buffer.get();
					if( 0 != status )
					{
						Debug.WriteLine("Invalid Status : " + status + ", packet : " + packet.ToString());
						return;
					}

					int chCount = buffer.get();
					for( int ii = 0; ii < chCount; ++ii )
					{
						byte ch_size = buffer.get();
						byte ch_status = buffer.get();
						if( 0 != ch_status )
						{
							// 나머지 데이터는 의미없다.
							buffer.position(buffer.position() + (ch_size - 1));
							continue;
						}

						ushort ch = buffer.getUShort();
						byte ch_type = buffer.get();
						double value = getValue(ch_type, buffer);

						switch( ch )
						{
						case CH_WIND_SPEED:
							mWindSpeed = value;
							break;
						case CH_WIND_DIRECTION:
							mWindVane = value;
							break;
						}
					}

					Debug.WriteLine("WindSpeed : " + mWindSpeed + ", WindVane : " + mWindVane);
				}
				break;
			default:
				Debug.WriteLine("Invalid Packet : " + packet.ToString());
				break;
			}
		}

		private double getValue(byte dataType, ByteBuffer buffer)
		{
			double value = 0;
			switch( dataType )
			{
			case UmbBinaryProtocol.TYPE_UCHAR:
				value = (double) buffer.get();
				break;
			case UmbBinaryProtocol.TYPE_CHAR:
				{
					byte rawValue = buffer.get();
					bool minus = 0 != (rawValue & 0x80);
					value = rawValue & 0x7F;
					value = minus ? -value : value;
				}
				break;
			case UmbBinaryProtocol.TYPE_USHORT:
				value = (double) buffer.getUShort();
				break;
			case UmbBinaryProtocol.TYPE_SHORT:
				value = (double) buffer.getShort();
				break;
			case UmbBinaryProtocol.TYPE_ULONG:
				value = (double) buffer.getUInt();
				break;
			case UmbBinaryProtocol.TYPE_LONG:
				value = (double) buffer.getInt();
				break;
			case UmbBinaryProtocol.TYPE_FLOAT:
				value = (double) buffer.getFloat();
				break;
			case UmbBinaryProtocol.TYPE_DOUBLE:
				value = (double) buffer.getDouble();
				break;
			}

			return value;
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

		public override void OnMeasure()
		{
			{
				WSensor.Unit windSpeedUnit = units[0];
				windSpeedUnit.value = mWindSpeed;
			}

			{
				WSensor.Unit windVaneUnit = units[1];
				windVaneUnit.value = mWindVane;
			}

			UmbBinaryProtocol protocol = new UmbBinaryProtocol(UmbBinaryProtocol.CMD_MULTI_CHANNEL_ONLINE_DATA_REQ, 5);
			protocol.To = (ushort) mSetting.DeviceAddr;
			protocol.putPayload(0, (byte) 2);
			protocol.putPayload(1, CH_WIND_SPEED);
			protocol.putPayload(3, CH_WIND_DIRECTION);

			byte[] buffer = protocol.getBuffer();

			WInterface.Comm_Input.RS232_485_SDI comm = Interface.comm.rs232_rs485_sdi12;
			comm.write(buffer);
		}

		/// <summary>
		/// 센서 측정이 멈췄을 때 호출
		/// </summary>
		public override void OnStop()
		{
			WInterface.Comm_Input.RS232_485_SDI comm = Interface.comm.rs232_rs485_sdi12;
			comm.close();
		}

		public override void OnDestroy()
		{
			WInterface.Comm_Input.RS232_485_SDI comm = Interface.comm.rs232_rs485_sdi12;
			comm.DataReceived -= CommDataReceived;
		}

		public override void Setup()
		{
			SensorSetting setting = new SensorSetting(mSetting);
			VentusV200SetupForm form = new VentusV200SetupForm(setting);
			DialogResult rv = form.ShowDialog();
			if( DialogResult.OK == rv )
			{
				// 대입만 하므로, 동기화할 필요는 없다.
				mSetting = setting;
				mSetting.save();
			}
		}
	}
}
