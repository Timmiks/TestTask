using System;

namespace MonitorUnitTests.Utility
{
    public static class KillTimeCalculator
    {
        //вычисляем ожидаемое время "смерти" самостоятельно
        public static DateTime Calculate(DateTime monitorStartTime, DateTime monitoredProcessStartTime, double numLifetime, double numPolling)
        {
            var killTime = monitorStartTime;
            var lifeMark = monitoredProcessStartTime.AddMinutes(numLifetime);
            while (killTime < lifeMark)
            {
                killTime = killTime.AddMinutes(numPolling);
            }
            return killTime;
        }
    }
}
