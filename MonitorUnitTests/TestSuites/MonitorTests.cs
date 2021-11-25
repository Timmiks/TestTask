using MonitorUnitTests.Configuration;
using MonitorUnitTests.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MonitorUnitTests.TestSuites
{
    [TestFixture, Timeout(120000)]
    public class MonitorTests
    {
        private Process monitor;
        private readonly List<Process> monitoredProcesses = new List<Process>();
        
        [Test]
        [TestCase("notepad", "1", "1")]
        [TestCase("notepad", "2", "2")]
        [TestCase("notepad", "3", "100")]
        [TestCase("notepad", "4.5", "11.5")]
        [TestCase("notepad", "0.5", "0.3")]
        [TestCase("calc", "1", "1")]
        [TestCase("calc", "2", "2")]
        [TestCase("calc", "3", "100")]
        [TestCase("calc", "4.5", "11.5")]
        [TestCase("calc", "0.5", "0.3")]
        public void CheckThatMonitorProcessIsStartedCorrectly(string processName, string lifeTime, string pollingInterval)
        {
            var expectedLogString =
                $"Start monitoring of '{processName}' that lives longer than '{TimeSpan.FromMinutes(double.Parse(lifeTime))}' minutes" +
                $" with polling interval '{TimeSpan.FromMinutes(double.Parse(pollingInterval))}' in minutes\r\n";

            monitor = Process.Start(Config.MonitorExecutable, $"{processName} {lifeTime} {pollingInterval}");
            string actualLogText = LogObserver.WaitLinesAndGetContent(monitor, 1);
            Assert.AreEqual(expectedLogString, actualLogText);
        }

        [Test]
        [TestCase("\"\"", "1", "1")]
        [TestCase("\"           \"", "1", "1")]
        [TestCase("\"\n\n\n\n\n\n\"", "1", "1")]
        public void CheckThatMonitorProcessNameShouldNotBeEmptyOrWhitespace(string processName, string lifeTime, string pollingInterval)
        {
            monitor = Process.Start(Config.MonitorExecutable, $"{processName} {lifeTime} {pollingInterval}");
            string actualLogText = LogObserver.WaitLinesAndGetContent(monitor, 1);
            Assert.AreEqual("Process name can't be empty!\r\n", actualLogText);
        }

        [Test]
        [TestCase("\"notepad \\\\\"", "1", "1")]
        [TestCase("\"notepad /\"", "1", "1")]
        [TestCase("\"notepad :\"", "1", "1")]
        [TestCase("\"notepad *\"", "1", "1")]
        [TestCase("\"notepad ?\"", "1", "1")]
        [TestCase("\"notepad \"\"\"", "1", "1")]
        [TestCase("\"notepad <\"", "1", "1")]
        [TestCase("\"notepad >\"", "1", "1")]
        [TestCase("\"notepad |\"", "1", "1")]
        public void CheckThatMonitorProcessNameShouldNotContainRestrictedSymbols(string processName, string lifeTime, string pollingInterval)
        {
            monitor = Process.Start(Config.MonitorExecutable, $"{processName} {lifeTime} {pollingInterval}");
            string actualLogText = LogObserver.WaitLinesAndGetContent(monitor, 1);
            Assert.AreEqual("Don't use any of restricted symbols for process name: \\/:*?\"<>|\r\n", actualLogText);
        }

        [Test]
        [TestCase("notepad", "0", "1")]
        [TestCase("notepad", "-1", "1")]
        [TestCase("notepad", "-15.5", "1")]
        [TestCase("notepad", "iowefo21342", "1")]
        [TestCase("notepad", "234234iowefo", "1")]
        [TestCase("notepad", "234234.5iowefo", "1")]
        [TestCase("notepad", "iowefo-21342", "1")]
        [TestCase("notepad", "-234234iowefo", "1")]
        [TestCase("notepad", "-234234.5iowefo", "1")]
        public void CheckMonitorProcessLifeTimeInvalidValues(string processName, string lifeTime, string pollingInterval)
        {
            monitor = Process.Start(Config.MonitorExecutable, $"{processName} {lifeTime} {pollingInterval}");
            string actualLogText = LogObserver.WaitLinesAndGetContent(monitor, 1);
            Assert.AreEqual("'Lifetime' parameter should be positive number of minutes!\r\n", actualLogText);
        }

        [Test]
        [TestCase("notepad", "1", "0")]
        [TestCase("notepad", "1", "-1")]
        [TestCase("notepad", "1", "-15.5")]
        [TestCase("notepad", "1", "iowefo21342")]
        [TestCase("notepad", "1", "234234iowefo")]
        [TestCase("notepad", "1", "234234.5iowefo")]
        [TestCase("notepad", "1", "iowefo-21342")]
        [TestCase("notepad", "1", "-234234iowefo")]
        [TestCase("notepad", "1", "-234234.5iowefo")]
        public void CheckMonitorProcessPollingInvalidValues(string processName, string lifeTime, string pollingInterval)
        {
            monitor = Process.Start(Config.MonitorExecutable, $"{processName} {lifeTime} {pollingInterval}");
            string actualLogText = LogObserver.WaitLinesAndGetContent(monitor, 1);
            Assert.AreEqual("'Polling interval' parameter should be positive number of minutes!\r\n", actualLogText);
        }

        [Test]
        [TestCase("notepad", "0.5", "0.3", false)]
        [TestCase("cmd", "0.5", "0.1", false)]
        [TestCase("Monitor", "0.3", "0.04", true)]
        public void CheckMonitorSingleProcessWork(string startProcessName, string lifeTime, string pollingInterval, bool isSelfCheck)
        {
            monitor = Process.Start(Config.MonitorExecutable, $"{startProcessName} {lifeTime} {pollingInterval}");
            monitoredProcesses.Add(isSelfCheck? monitor : Process.Start(startProcessName));
            double numLifetime = double.Parse(lifeTime);
            double numPolling = double.Parse(pollingInterval);

            var killTime = KillTimeCalculator.Calculate(monitor.StartTime, monitoredProcesses[0].StartTime, numLifetime, numPolling);
            var killEntry = LogObserver.WaitLinesAndGetKillEntries(monitor, 2).Single();

            Assert.Multiple(() =>
            {
                CustomAssert.AreEqual(killTime, killEntry.KillTime, TimeSpan.FromSeconds(15));
                Assert.IsTrue(isSelfCheck ? monitoredProcesses[0].WaitForExit(500) : monitoredProcesses[0].HasExited);
            });
            
        }

        [Test]
        [TestCase("notepad", "0.5", "0.3", 10, 1000)]
        [TestCase("cmd", "0.5", "0.1", 20, 500)]
        public void CheckMonitorMultipleProcessWork(string startProcessName, string lifeTime, string pollingInterval, int amount, int sleepInterval)
        {
            monitor = Process.Start(Config.MonitorExecutable, $"{startProcessName} {lifeTime} {pollingInterval}");
            List<DateTime> expectedKillTimes = new List<DateTime>(amount);
            double numLifetime = double.Parse(lifeTime);
            double numPolling = double.Parse(pollingInterval);

            for (int i = 0; i < amount; i++)
            {
                monitoredProcesses.Add(Process.Start(startProcessName));
                expectedKillTimes.Add(KillTimeCalculator.Calculate(monitor.StartTime, monitoredProcesses[i].StartTime, numLifetime, numPolling));
                System.Threading.Thread.Sleep(sleepInterval);
            }

            var killEntries = LogObserver.WaitLinesAndGetKillEntries(monitor, amount + 1)
                .OrderBy(ke => monitoredProcesses.FindIndex(p => p.Id == ke.PID))
                .ToList();
            Assert.Multiple(() =>
            {
                for (int i = 0; i < amount; i++)
                {
                    CustomAssert.AreEqual(expectedKillTimes[i], killEntries[i].KillTime, TimeSpan.FromSeconds(15));
                    Assert.IsTrue(monitoredProcesses[i].HasExited);
                }
            });
        }

        [TearDown]
        public void KillMonitor()
        {
            using (var killer = Process.Start("taskkill", $"/F /PID {monitor.Id}"))
            {
                killer.WaitForExit();
            }
            monitor.WaitForExit();
            monitor.Dispose();
            monitoredProcesses.ForEach(p => p.Dispose());
            monitoredProcesses.Clear();
        }
    }
}
