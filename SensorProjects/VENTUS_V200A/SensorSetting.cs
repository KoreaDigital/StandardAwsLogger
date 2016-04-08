using Microsoft.Win32;

namespace VENTUS_V200A
{
	public enum FileSplit : int
	{
		Undefined = 0,
		SingleFile,	// 1 개의 통짜 파일로 저장
		SplitPerDay,	// 일별로 나누어서 저장
		SplitPerWeek,	// 주별로 나누어서 저장
		SplitPerMonth,	// 월별로 나누어서 저장
	}

	public class SensorSetting
	{
		private const string REG_KEY = @"Software\Korea Digital\Standard AWS\VentusV200";
		private const string VALUE_DEVICE_ADDR = "DeviceAddr";

		public static SensorSetting load()
		{
			SensorSetting setting = new SensorSetting();

			RegistryKey myKey = Registry.LocalMachine.OpenSubKey(REG_KEY, false);
			if( null != myKey )
			{
				setting.DeviceAddr = (int) myKey.GetValue(VALUE_DEVICE_ADDR, 0);
			}

			// 설정값 load 시에, 값이 정의되어 있지 않으면, 여기에서 기본값을 지정해준다.
			return setting;
		}

		public int DeviceAddr;

		private SensorSetting()
		{
			DeviceAddr = 0;
		}

		// copy contructor
		public SensorSetting(SensorSetting other)
		{
			this.DeviceAddr = other.DeviceAddr;
		}

		public void save()
		{
			RegistryKey myKey = Registry.LocalMachine.CreateSubKey(REG_KEY);
			if( null != myKey )
			{
				myKey.SetValue(VALUE_DEVICE_ADDR, DeviceAddr, RegistryValueKind.DWord);
			}
		}
	}
}
