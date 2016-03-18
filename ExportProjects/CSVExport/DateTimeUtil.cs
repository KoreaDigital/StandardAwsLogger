using System;
using System.Globalization;

namespace TestCSVExport
{
	public class DateTimeUtil
	{
		public static int getWeekNum(DateTime time)
		{
			CultureInfo cul = CultureInfo.CurrentCulture;
			return cul.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
		}
	}
}
