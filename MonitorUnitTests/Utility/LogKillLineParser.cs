using MonitorUnitTests.BO;
using MonitorUtilities.Extensions;
using System;

namespace MonitorUnitTests.Utility
{
    public static class LogKillLineParser
    {
        //разбираем строку, содержающую информацию о смерти процесса, в объект
        public static LogKillEntry Parse(string line)
        {
            var segments = line.Split(new[] { " - " }, StringSplitOptions.None);

            var killTime = DateTime.ParseExact(segments[0], "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
            var pID = int.Parse(segments[1].SubstringBetweenBoth('\''));
            var startTime = DateTime.ParseExact(segments[2].SubstringBetweenBoth('\''), 
                "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);

            return new LogKillEntry(killTime, startTime, pID);
        }
    }
}