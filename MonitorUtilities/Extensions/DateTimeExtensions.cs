using System;

namespace MonitorUtilities.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToStringWithMillis(this DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
