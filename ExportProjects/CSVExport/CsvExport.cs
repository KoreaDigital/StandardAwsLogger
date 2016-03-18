using SDKforAWS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace TestCSVExport
{
	public class CsvExport : WExport
	{
		private const string TAG = "CsvExport";

		private ExportSetting mSetting;
		private CSVFile mExporter;
		private string mExportFileName;

		public CsvExport(List<WSensor> sensorlistclon)
			: base(sensorlistclon)
		{
			mSetting = ExportSetting.load();
		}

		public override string getDescription()
		{
			return "데이터를 CSV 형식으로 내보내는 Plug-in 입니다";
		}

		public override void Destroy()
		{
			mSetting = null;
			if( null != mExporter )
			{
				mExporter.Close();
			}
		}

		//센서값이 업데이트 될때마다 호출된다.
		public override void Update(DateTime time, List<WSensor> sensorList)
		{
			string fileName = mSetting.getAbsoluteFilePath();

			if( null != mExporter )
			{
				if( fileName != mExportFileName )
				{
					mExporter.Close();
					mExporter = null;
				}
			}

			if( null == mExporter )
			{
				CSVFile exporter = new CSVFile();
				if( false == exporter.Open(fileName) )
				{
					Log.e(TAG, "CSV 파일을 열 수 없습니다!");
					return;
				}

				mExporter = exporter;
				mExportFileName = fileName;
			}

			mExporter.Write(time, sensorList);
		}

		//사용자설정이 필요한 경우 
		public override void Setup()
		{
			ExportSetting setting = new ExportSetting(mSetting);
			CsvExportSetupForm form = new CsvExportSetupForm(setting);
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
