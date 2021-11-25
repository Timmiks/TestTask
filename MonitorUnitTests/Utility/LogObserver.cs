using MonitorUnitTests.BO;
using MonitorUnitTests.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MonitorUnitTests.Utility
{
    //т.к. мы не знаем, когда точно монитор завершит все процессы и предполагая, что он может убить любой процесс (не нашел неубиваемого процесса, без вреда системе)
    //то для сбора лога решил отталкиваться от того, что механизм логирования известен и мы можем  дождаться известного числа записей в логе
    public static class LogObserver
    {
        public static List<LogKillEntry> WaitLinesAndGetKillEntries(System.Diagnostics.Process monitor, int expectedLogLines)
        {
            List<LogKillEntry> killEntries = new List<LogKillEntry>();
            var logKillLines = WaitLinesAndGetContent(monitor, expectedLogLines)
                .Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(1);
            if (logKillLines.Any())
            {
                foreach (var line in logKillLines)
                {
                    killEntries.Add(LogKillLineParser.Parse(line));
                }
            }
            return killEntries;
        }

        public static string WaitLinesAndGetContent(System.Diagnostics.Process monitor, int expectedLogLines)
        {
            var fileInfo = GetLogFile(monitor);
            int actualLogLinesCount = 0;
            string fileContent = null;
            do
            {
                try
                {
                    using (FileStream fs = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader w = new StreamReader(fs))
                        {
                            fileContent = w.ReadToEnd();
                            actualLogLinesCount = fileContent.Count(s => s == '\n');
                        }
                    }
                }
                catch (IOException)
                {

                }
            } while (actualLogLinesCount < expectedLogLines);

            return fileContent;
        }

        private static FileInfo GetLogFile(System.Diagnostics.Process monitor)
        {
            FileInfo fileInfo = Config.LogDirectory.EnumerateFiles(Config.FilterExtension).FirstOrDefault(f => f.CreationTime > monitor.StartTime);
            var timer = System.Diagnostics.Stopwatch.StartNew();
            while (fileInfo == null
                && timer.Elapsed < TimeSpan.FromSeconds(2))
            {
                fileInfo = Config.LogDirectory.EnumerateFiles(Config.FilterExtension).FirstOrDefault(f => f.CreationTime > monitor.StartTime);
            }
            return fileInfo;
        }
    }
}
