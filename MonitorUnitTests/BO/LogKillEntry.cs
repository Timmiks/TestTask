using System;

namespace MonitorUnitTests.BO
{
    public class LogKillEntry
    {
        public LogKillEntry(DateTime killTime, DateTime startTime, int pID)
        {
            KillTime = killTime;
            StartTime = startTime;
            PID = pID;
        }

        public DateTime KillTime { get; private set; }
        public DateTime StartTime { get; private set; }
        public int PID { get; private set; }
    }
}
