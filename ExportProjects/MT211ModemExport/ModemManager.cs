using SDKforAWS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MT211ModemExport
{
	class ModemManager
	{
		private const string TAG = "ModemManager";

		private MT211Modem mModem;
		private Thread mModemThread;
		private ExportSetting mSetting;

		public event ModemStatusChangedHandler ModemStatusChanged;

		private bool mRun;
		private bool mInitComplete;
		private object mMsgSync;

		private byte[] mLastData;

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

		public void sendMsg(KmaDataStructure data)
		{
			lock( mMsgSync )
			{
				data.changeStationId(mSetting.StationId);
				mLastData = data.getBufferClone();
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

			byte[] buffer;

			mInitComplete = true;
			while( mRun )
			{
				// 데이터가 오기를 기다림.
				lock( mMsgSync )
				{
					Monitor.Wait(mMsgSync, 100);
					buffer = mLastData;
					mLastData = null;
				}
				if( false == mRun )
				{
					break;
				}
				if( null == buffer )
				{
					continue;
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
					if( MT211Modem.RESULT_OK != mModem.sendToServer(buffer) )
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
