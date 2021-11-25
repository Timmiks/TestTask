using System.IO;
using System.Reflection;
using System.Threading;

namespace Monitor
{
    public static class Logger
    {
        private static string LogFileName { get; set; } = null;
        private static ReaderWriterLockSlim Lock { get; } = new ReaderWriterLockSlim();

        public static void Init(string logFileName)
        {
            LogFileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), logFileName);
            using (var f = File.Create(LogFileName))
            {

            }
        }

        public static void LogMessage(string message)
        {
            Lock.EnterWriteLock();
            try
            {
                using (FileStream fs = File.Open(LogFileName, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    using (StreamWriter w = new StreamWriter(fs))
                    {
                        w.WriteLine(message);
                        w.Flush();
                    }
                }
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }
    }
}
