using System;
using System.Threading;

namespace Monitor
{
    public static class TimeStampGenerator
    {
        private static long lastTimeStamp = DateTime.UtcNow.Ticks;
        public static long GetUniqueStamp()
        {
            long original, newValue;
            do
            {
                original = lastTimeStamp;
                long now = DateTime.UtcNow.Ticks;
                newValue = Math.Max(now, original + 1);
            } while (Interlocked.CompareExchange
                         (ref lastTimeStamp, newValue, original) != original);

            return newValue;
        }


    }
}
