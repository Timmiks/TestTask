using MonitorUtilities.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Monitor
{
    public static class Monitor
    {
        private static string ProcessName { get; set; } = null;
        private static TimeSpan LifeTime { get; set; } = default;
        private static TimeSpan Polling { get; set; } = default;
        private static List<int> FoundIDs { get; } = new List<int>();
        private static object Locker { get; } = new object();

        public static void StartMonitoring()
        {
            //сохраняем информацию о самом себе в случае, если сам "монитор" должен совершить сеппуку =)
            bool isSelfKillInitiated = false;
            int selfPID = 0;
            DateTime selfStartTime = default;
            using (var self = Process.GetCurrentProcess())
            {
                if (ProcessName == self.ProcessName)
                {
                    selfPID = self.Id;
                    selfStartTime = self.StartTime;
                }
            }

            do
            {
                try
                {
                    List<Process> processes;

                    //'FoundIDs' хранит идентификаторы уже выбранных для убийства процессов
                    //т.к.убийство выполняется через таски без ожидания (не в случае себя), то, для избежания многопоточных проблем, использую лок
                    //от использования Concurrent коллекций отказался
                    lock (Locker)
                    {
                        processes = Process.GetProcesses()
                            //т.к. по моим исследованиям GetProcessesByName игнорирует регистр, а процессы его используют, то ввел доп условие на equality неймов
                            .Where(p => p.ProcessName == ProcessName && DateTime.Now - p.StartTime >= LifeTime && !FoundIDs.Contains(p.Id))
                            .ToList();
                        FoundIDs.AddRange(processes.Select(p => p.Id));
                    }
                    if (processes.Any())
                    {
                        if (selfPID != 0)
                        {
                            //в случае "самоубийства" убираем свой процесс из кил-листа, убиваем все другие выбранные мониторы, а потом выходим из цикла и завершаемся
                            processes.Remove(processes.Find(p => p.Id == selfPID));
                            var tasks = new Task[processes.Count];

                            for (int i = 0; i < processes.Count; i++)
                            {
                                var process = processes[i];
                                tasks[i] = Task.Run(() =>
                                {
                                    using (process)
                                    {
                                        int id = process.Id;
                                        DateTime startTime = process.StartTime;
                                        KillProcess(process);
                                        Logger.LogMessage($"{DateTime.Now.ToStringWithMillis()} - Process with PID = '{id}' " +
                                            $"- Started at '{startTime.ToStringWithMillis()}' was killed");
                                    }
                                });
                            }
                            Task.WaitAll(tasks);
                            isSelfKillInitiated = true;
                        }
                        else
                        {
                            for (int i = 0; i < processes.Count; i++)
                            {
                                var process = processes[i];
                                _ = Task.Run(() =>
                                {
                                    using (process)
                                    {
                                        int id = process.Id;
                                        DateTime startTime = process.StartTime;
                                        KillProcess(process);
                                        Logger.LogMessage($"{DateTime.Now.ToStringWithMillis()} - Process with PID = '{id}' " +
                                            $"- Started at '{startTime.ToStringWithMillis()}' was killed");
                                    }
                                });
                            }
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogMessage($"{DateTime.Now.ToStringWithMillis()} - CommonError - '{ex.Message}' : '{ex.StackTrace}'");
                }
                if (!isSelfKillInitiated)
                {
                    Thread.Sleep(Polling);
                }
            } while (!isSelfKillInitiated);
            Logger.LogMessage($"{DateTime.Now.ToStringWithMillis()} - Process with PID = '{selfPID}' " +
                $"- Started at '{selfStartTime.ToStringWithMillis()}' was killed");
        }

        internal static void Init(string processName, TimeSpan lifeTime, TimeSpan polling)
        {
            ProcessName = processName;
            LifeTime = lifeTime;
            Polling = polling;
        }

        private static void KillProcess(Process p)
        {
            int id = p.Id;
            DateTime startTime = p.StartTime;
            try
            {
                //проверяем наличие окна, пробуем закрыть оконное приложение или вызываем Kill для консольных апп
                bool hasWindow = p.MainWindowHandle != IntPtr.Zero;
                if (hasWindow)
                {
                    p.CloseMainWindow();
                }
                else
                {
                    p.Kill();
                }
                _ = p.WaitForExit(5000);
                
                //если процесс не умер и он оконный (возможно, требовался промпт), то пробуем убить его
                if (!p.HasExited && hasWindow)
                {
                    p.Kill();
                    _ = p.WaitForExit(5000);
                }

                //если процесс пережил всё, то убиваем намертво
                if (!p.HasExited)
                {
                    using (Process.Start("taskkill", $"/F /PID {p.Id}"))
                    {

                    }
                    _ = p.WaitForExit(5000);
                }
            }
            catch (Exception ex)
            {
                Logger.LogMessage($"{DateTime.Now.ToStringWithMillis()} - KillingError - Process with id='{id}' " +
                    $"- Started at '{startTime.ToStringWithMillis()}' - '{ex.Message}' : '{ex.StackTrace}'");
            }
            finally
            {
                //убираем убитый процесс из списка найденных
                lock (Locker)
                {
                    FoundIDs.Remove(id);
                }
            }
        }
    }
}
