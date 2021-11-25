using System;
using System.IO;

namespace MonitorUnitTests.Configuration
{
    public static class Config
    {
        public static string MonitorExecutable { get; } = $@"{AppDomain.CurrentDomain.BaseDirectory}\Monitor.exe";
        public static DirectoryInfo LogDirectory { get; } = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        public static string FilterExtension { get; } = "*.log";
        public static string OutputFolder { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"MonitorLogs - {DateTime.Now.ToString("dd MMM HH-mm-ss")}");
    }
}
