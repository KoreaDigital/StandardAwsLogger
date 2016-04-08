using SDKforAWS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TestCSVExport
{
	public class CSVFile
	{
		private const string TAG = "CSVFile";

		private StreamWriter mWriter;
		private bool mFirstTime = true;

		public bool Open(string fileName)
		{
			fileName = getValidFileName(fileName);

			FileStream fs = null;
			try
			{
				fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
			}
			catch( Exception e )
			{
				Log.e(TAG, "파일을 생성할 수 없습니다.");
				Log.e(TAG, e.ToString());
				return false;
			}

			mWriter = new StreamWriter(fs, Encoding.Unicode);
			mWriter.AutoFlush = true;

			return true;
		}

		private string getValidFileName(string fileName)
		{
			if( false == File.Exists(fileName) )
			{
				return fileName;
			}

			string prefix = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName));
			string extention = Path.GetExtension(fileName);
			for( int ii = 1; ; ++ii )
			{
				string newFileName = string.Format("{0} ({1}){2}", prefix, ii, extention);
				if( false == File.Exists(newFileName) )
				{
					return newFileName;
				}
			}
		}

		public void Write(DateTime time, List<WSensor> sensorList)
		{
			// write Column Header
			if( true == mFirstTime )
			{
				StringBuilder str = new StringBuilder();
				str.Append("Time");

				foreach( WSensor sensor in sensorList )
				{
					foreach( WSensor.Unit unit in sensor.units )
					{
						str.Append(',');
						str.Append(unit.unit_name);
					}
				}
				mFirstTime = false;

				mWriter.WriteLine(str.ToString());
			}

			// write Data
			{
				StringBuilder str = new StringBuilder();
				str.Append(time.ToString());

				foreach( WSensor sensor in sensorList )
				{
					foreach( WSensor.Unit unit in sensor.units )
					{
						str.Append(',');
						str.Append(unit.format(false));
					}
				}

				mWriter.WriteLine(str.ToString());
			}
		}

		public void Close()
		{
			if( null != mWriter )
			{
				mWriter.Close();
			}
		}
	}
}
