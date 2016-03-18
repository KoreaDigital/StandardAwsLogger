using SDKforAWS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MT211ModemExport
{
	class ModemManager
	{
		private MT211Modem mModem;
		private Thread mModemThread;
		private ExportSetting mSetting;

		public event ModemStatusChangedHandler ModemStatusChanged;

		private bool mRun;
		private bool mInitComplete;
		private object mMsgSync;

		private DateTime mLastDataTime;
		private List<WSensor> mLastSensorData;

		public ModemManager(ExportSetting setting)
		{
			mMsgSync = new object();
			mSetting = new ExportSetting(setting);
		}

		public void start()
		{
			mRun = true;
			mModemThread = new Thread(modemThreadMain);
			mModemThread.Start();
		}

		public void sendMsg(DateTime date, List<WSensor> sensorList)
		{
			lock( mMsgSync )
			{
				mLastDataTime = date;
				mLastSensorData = new List<WSensor>(sensorList);

				if( (true == mRun) && (true == mInitComplete) )
				{
					Monitor.Pulse(mMsgSync);
				}
			}
		}

		public void stop()
		{
			lock( mMsgSync )
			{
				mRun = false;
				Monitor.Pulse(mMsgSync);
			}

			if( mModemThread != Thread.CurrentThread )
			{
				mModemThread.Join();
			}
			mModemThread = null;
		}

		private void modemThreadMain()
		{
			mModem = new MT211Modem(mSetting.ServerIpAddr, mSetting.ServerPort);
			if( false == mModem.openModemSerialPort(mSetting.ComPort) )
			{
				Log.e("OpenModemSerialPort", "모뎀을 연결할 수 없습니다!");
				NotifyStatusChanged(ModemStatus.InvalidComPort);
				mModem = null;
				return;
			}

			// 모뎀 초기화
			if( false == mModem.initModem() )
			{
				Log.e("initModem", "모뎀 초기화 실패");
				NotifyStatusChanged(ModemStatus.InitializeFail);
				mModem.closeModemSerialPorts();
				mModem = null;
				return;
			}

			DateTime valueDateTimeCache;
			List<WSensor> valueSensorInfoCache;

			mInitComplete = true;
			while( mRun )
			{
				// 데이터가 오기를 기다림.
				lock( mMsgSync )
				{
					Monitor.Wait(mMsgSync);

					valueDateTimeCache = mLastDataTime;
					valueSensorInfoCache = mLastSensorData;
				}
				if( false == mRun )
				{
					break;
				}

				// 24시간 이상 접속이 되어 있으면 연결이 해제된다. (확인중)
				// ZIPCALL을 할 수 없는 상태면 다시 모뎀을 초기화 해본다.
				if( true != mModem.isZipCallOpen )
				{
					// 모뎀 초기화도 실패하면 리셋이 필요하다.
					if( false == mModem.initModem() )
					{
						Log.e("initModem", "ppp connection failed!!");
						break;
					}
				}

				// TCP 열고, 데이터 보내고, TCP 끊고.
				if( MT211Modem.RESULT_OK == mModem.openCon() )
				{
					StringBuilder strBuilder = new StringBuilder();
					{
						strBuilder.Append("Time=");
						long unixTimeStamp_InSec = valueDateTimeCache.Ticks / 10000000L;
						strBuilder.Append(unixTimeStamp_InSec.ToString());
					}

					foreach( WSensor sensor in valueSensorInfoCache )
					{
						foreach( WSensor.Unit unit in sensor.units )
						{
							string tmp = string.Format(",{0}={1}", unit.unit_name, unit.format(false));
							strBuilder.Append(tmp);
						}
					}

					if( MT211Modem.RESULT_OK != mModem.sendToServer(strBuilder.ToString()) )
					{
						mModem.resetModem();
					}

					mModem.closeCon();
				}
				else
				{
					mModem.closeCon();
				}
			}

			mModem.closeModemSerialPorts();
		}

		private void NotifyStatusChanged(ModemStatus status)
		{
			if( null != ModemStatusChanged )
			{
				ModemStatusChanged(status);
			}
		}
	}

	public enum ModemStatus
	{
		Success,
		InvalidComPort,
		InitializeFail
	}

	public delegate void ModemStatusChangedHandler(ModemStatus status);
}
