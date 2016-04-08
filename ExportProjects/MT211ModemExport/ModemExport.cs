using SDKforAWS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace MT211ModemExport
{
	public class ModemExport : WExport
	{
		private const string TAG = "ModemExport";

		private ExportSetting mSetting;
		private ModemManager mModemManager;

		public ModemExport(List<WSensor> sensorlistclon)
			: base(sensorlistclon)
		{
			mSetting = ExportSetting.load();
			if( mSetting.isValid() )
			{
				startModemThread();
			}
		}

		public override string getDescription()
		{
			return "데이터를 3G 모뎀을 통해 서버로 보내는 Plug-in 입니다";
		}

		public override void Destroy()
		{
			mSetting = null;
			stopModemThread();
		}

		//센서값이 업데이트 될때마다 호출된다.
		public override void Update(DateTime time, List<WSensor> sensorList)
		{
			// __noop;
		}

		// 1분마다 데이터가 정리되면 호출된다.
		public override void Update(KmaDataStructure data)
		{
			lock( this )
			{
				if( (null != data) && (null != mModemManager) )
				{
					mModemManager.sendMsg(data);
				}
			}
		}

		//사용자설정이 필요한 경우 
		public override void Setup()
		{
			ExportSetting setting = new ExportSetting(mSetting);
			ModemExportSetupForm form = new ModemExportSetupForm(setting);
			DialogResult rv = form.ShowDialog();
			if( DialogResult.OK == rv )
			{
				// 대입만 하므로, 동기화할 필요는 없다.
				mSetting = setting;
				mSetting.save();

				stopModemThread();
				if( mSetting.isValid() )
				{
					startModemThread();
				}
			}
		}

		private void startModemThread()
		{
			lock( this )
			{
				Debug.Assert(null == mModemManager);

				mModemManager = new ModemManager(mSetting);
				mModemManager.ModemStatusChanged += new ModemStatusChangedHandler(MT211_ModemStatusChanged);
				mModemManager.start();
			}
		}

		private void MT211_ModemStatusChanged(ModemStatus status)
		{
			stopModemThread();
		}

		private void stopModemThread()
		{
			lock( this )
			{
				if( null != mModemManager )
				{
					mModemManager.stop();
					mModemManager = null;
				}
			}
		}
	}
}
