using Microsoft.Win32;
using System.Net;

namespace NetworkExport
{
	public class ExportSetting
	{
		private const string REG_KEY = @"Software\Korea Digital\Standard AWS\NetworkExport";
		private const string VALUE_SERVER_ADDR = "ServerAddr";
		private const string VALUE_SERVER_PORT = "ServerPort";
		private const string VALUE_STATION_ID = "StationId";

		public static ExportSetting load()
		{
			ExportSetting setting = new ExportSetting();

			RegistryKey myKey = Registry.LocalMachine.OpenSubKey(REG_KEY, false);
			if( null != myKey )
			{
				setting.ServerAddr = (string) myKey.GetValue(VALUE_SERVER_ADDR, null);
				setting.ServerPort = (int) myKey.GetValue(VALUE_SERVER_PORT, 0);
				setting.StationId = (int) myKey.GetValue(VALUE_STATION_ID, 0);
			}

			if( null == setting.ServerAddr )
			{
				setting.ServerAddr = string.Empty;
			}

			return setting;
		}

		public string ServerAddr;
		public int ServerPort;
		public int StationId;

		private ExportSetting()
		{
			ServerAddr = string.Empty;
			ServerPort = 0;
			StationId = 0;
		}

		// copy contructor
		public ExportSetting(ExportSetting other)
		{
			this.ServerAddr = other.ServerAddr;
			this.ServerPort = other.ServerPort;
			this.StationId = other.StationId;
		}

		public void save()
		{
			RegistryKey myKey = Registry.LocalMachine.CreateSubKey(REG_KEY);
			if( null != myKey )
			{
				myKey.SetValue(VALUE_SERVER_ADDR, ServerAddr, RegistryValueKind.String);
				myKey.SetValue(VALUE_SERVER_PORT, ServerPort, RegistryValueKind.DWord);
				myKey.SetValue(VALUE_STATION_ID, StationId, RegistryValueKind.DWord);
			}
		}

		public bool isValid()
		{
			return (0 < StationId) && (StationId < 32768) && (0 < ServerAddr.Length) && (0 < ServerPort);
		}

		public string ServerIpAddr
		{
			get
			{
				IPAddress[] addrs = Dns.GetHostAddresses(ServerAddr);
				if( 0 < addrs.Length )
				{
					return addrs[0].ToString();
				}
				else
				{
					return string.Empty;
				}
			}
		}
	}
}
