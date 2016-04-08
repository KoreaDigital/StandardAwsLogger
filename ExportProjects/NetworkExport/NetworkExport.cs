using SDKforAWS;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NetworkExport
{
	public class NetworkExport : WExport
	{
		private const string TAG = "ModemExport";

		private ExportSetting mSetting;
		private SocketWriter mSocketWriter;

		public NetworkExport(List<WSensor> sensorlistclon)
			: base(sensorlistclon)
		{
			mSocketWriter = new SocketWriter();
			mSetting = ExportSetting.load();
			if( mSetting.isValid() )
			{
				mSocketWriter.start(mSetting);
			}
		}

		public override string getDescription()
		{
			return "데이터를 네트워크를 통해 서버로 보내는 Plug-in 입니다";
		}

		public override void Destroy()
		{
			mSetting = null;
			mSocketWriter.stop();
			mSocketWriter = null;
		}

		// 센서값이 업데이트 될때마다 호출된다.
		public override void Update(DateTime time, List<WSensor> sensorList)
		{
			// __noop;
		}

		// 1분마다 데이터가 정리되면 호출된다.
		public override void Update(KmaDataStructure data)
		{
			if( false == mSocketWriter.isConnected() )
			{
				mSocketWriter.stop();
				if( mSetting.isValid() )
				{
					mSocketWriter.start(mSetting);
				}
			}

			mSocketWriter.push_back(data);
		}

		// 사용자설정이 필요한 경우 
		public override void Setup()
		{
			ExportSetting setting = new ExportSetting(mSetting);
			NetworkExportSetupForm form = new NetworkExportSetupForm(setting);
			DialogResult rv = form.ShowDialog();
			if( DialogResult.OK == rv )
			{
				// 대입만 하므로, 동기화할 필요는 없다.
				mSetting = setting;
				mSetting.save();

				mSocketWriter.stop();
				if( mSetting.isValid() )
				{
					mSocketWriter.start(mSetting);
				}
			}
		}
	}
}
