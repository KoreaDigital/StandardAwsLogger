using Microsoft.Win32;
using System;
using System.IO;

namespace TestCSVExport
{
	public enum FileSplit : int
	{
		Undefined = 0,
		SingleFile,	// 1 개의 통짜 파일로 저장
		SplitPerDay,	// 일별로 나누어서 저장
		SplitPerWeek,	// 주별로 나누어서 저장
		SplitPerMonth,	// 월별로 나누어서 저장
	}

	public class ExportSetting
	{
		private const string REG_KEY = @"Software\Korea Digital\Standard AWS\CsvExport";
		private const string VALUE_TARGET_DIR = "TargetDir";
		private const string VALUE_FILE_PREFIX = "FilePrefix";
		private const string VALUE_SPLIT_TYPE = "SplitType";

		private const string DEFAULT_FILE_PREFIX = "LoggerData";

		public static ExportSetting load()
		{
			ExportSetting setting = new ExportSetting();

			RegistryKey myKey = Registry.LocalMachine.OpenSubKey(REG_KEY, false);
			if( null != myKey )
			{
				setting.TargetDir = (string) myKey.GetValue(VALUE_TARGET_DIR, null);
				setting.FilePrefix = (string) myKey.GetValue(VALUE_FILE_PREFIX, null);
				setting.SplitType = (FileSplit) ((int) myKey.GetValue(VALUE_SPLIT_TYPE, 0));
			}

			// 설정값 load 시에, 값이 정의되어 있지 않으면, 여기에서 기본값을 지정해준다.
			if( null == setting.TargetDir )
			{
				setting.TargetDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			}

			if( null == setting.FilePrefix )
			{
				setting.FilePrefix = DEFAULT_FILE_PREFIX;
			}

			if( FileSplit.Undefined == setting.SplitType )
			{
				setting.SplitType = FileSplit.SingleFile;
			}

			return setting;
		}

		public string TargetDir;
		public string FilePrefix;
		public FileSplit SplitType;

		private string mSingleFileMode_FileNameCache;

		private ExportSetting()
		{
			TargetDir = null;
			FilePrefix = null;
			SplitType = FileSplit.Undefined;
		}

		// copy contructor
		public ExportSetting(ExportSetting other)
		{
			this.TargetDir = other.TargetDir;
			this.FilePrefix = other.FilePrefix;
			this.SplitType = other.SplitType;
		}

		public void save()
		{
			RegistryKey myKey = Registry.LocalMachine.CreateSubKey(REG_KEY);
			if( null != myKey )
			{
				myKey.SetValue(VALUE_TARGET_DIR, TargetDir, RegistryValueKind.String);
				myKey.SetValue(VALUE_FILE_PREFIX, FilePrefix, RegistryValueKind.String);
				myKey.SetValue(VALUE_SPLIT_TYPE, (int) SplitType, RegistryValueKind.DWord);
			}
		}

		public string getFileName()
		{
			return getFileName(DateTime.Now);
		}

		public string getAbsoluteFilePath()
		{
			return Path.Combine(TargetDir, getFileName(DateTime.Now));
		}

		private string getFileName(DateTime time)
		{
			string fileName = null;
			switch( SplitType )
			{
			case FileSplit.SingleFile:
				if( null == mSingleFileMode_FileNameCache )
				{
					mSingleFileMode_FileNameCache = string.Format("{0}_{1:yyyy_MM_dd_hh_mm_ss}.csv", FilePrefix, time);
				}
				fileName = mSingleFileMode_FileNameCache;
				break;
			case FileSplit.SplitPerDay:
				fileName = string.Format("{0}_{1:yyyy_MM_dd}.csv", FilePrefix, time);
				break;
			case FileSplit.SplitPerWeek:
				fileName = string.Format("{0}_{1:yyyy}_Week_{2}.csv", FilePrefix, time, DateTimeUtil.getWeekNum(time));
				break;
			case FileSplit.SplitPerMonth:
				fileName = string.Format("{0}_{1:yyyy_MM}.csv", FilePrefix, time);
				break;
			}

			return fileName;
		}
	}
}
