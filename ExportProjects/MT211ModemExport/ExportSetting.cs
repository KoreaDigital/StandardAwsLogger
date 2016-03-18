using System;
using System.Net;
using Microsoft.Win32;

namespace MT211ModemExport
{
	public class ExportSetting
	{
		private const string REG_KEY = @"Software\Korea Digital\Standard AWS\MT211Export";
		private const string VALUE_COM_PORT = "ComPort";
		private const string VALUE_SERVER_ADDR = "ServerAddr";
		private const string VALUE_SERVER_PORT = "ServerPort";

		public static ExportSetting load()
		{
			ExportSetting setting = new ExportSetting();

			RegistryKey myKey = Registry.LocalMachine.OpenSubKey(REG_KEY, false);
			if( null != myKey )
			{
				setting.ComPort = (int) myKey.GetValue(VALUE_COM_PORT, null);
				setting.ServerAddr = (string) myKey.GetValue(VALUE_SERVER_ADDR, null);
				setting.ServerPort = (int) myKey.GetValue(VALUE_SERVER_PORT, null);
			}

			if( null == setting.ServerAddr )
			{
				setting.ServerAddr = string.Empty;
			}

			return setting;
		}

		public int ComPort;
		public string ServerAddr;
		public int ServerPort;

		private ExportSetting()
		{
			ComPort = 0;
			ServerAddr = string.Empty;
			ServerPort = 0;
		}

		// copy contructor
		public ExportSetting(ExportSetting other)
		{
			this.ComPort = other.ComPort;
			this.ServerAddr = other.ServerAddr;
			this.ServerPort = other.ServerPort;
		}

		public void save()
		{
			RegistryKey myKey = Registry.LocalMachine.CreateSubKey(REG_KEY);
			if( null != myKey )
			{
				myKey.SetValue(VALUE_COM_PORT, ComPort, RegistryValueKind.DWord);
				myKey.SetValue(VALUE_SERVER_ADDR, ServerAddr, RegistryValueKind.String);
				myKey.SetValue(VALUE_SERVER_PORT, ServerPort, RegistryValueKind.DWord);
			}
		}

		public bool isValid()
		{
			return (0 < ComPort) && (0 < ServerAddr.Length) && (0 < ServerPort);
		}

		public string ComPortName
		{
			get
			{
				if( 0 == ComPort )
				{
					return null;
				}
				else
				{
					return "COM" + ComPort;
				}
			}
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
