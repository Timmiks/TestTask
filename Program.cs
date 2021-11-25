using System;
using System.Text.RegularExpressions;

namespace Monitor
{
    class Program
    {

        static void Main(string[] args)
        {
            //инициализируем логгер. файлнейм содержит уникальный таймстэмп
            Logger.Init($"log_{TimeStampGenerator.GetUniqueStamp()}.log");

            //стартуем валидацию. сначала проверяем кол-во параметров
            if (args.Length != 3)
            {
                Logger.LogMessage("Specify all nececary parameters like \"notepad 5 1\"");
            }
            //проверяем на пустое значение имени процесса
            else if (string.IsNullOrEmpty(args[0]) || string.IsNullOrWhiteSpace(args[0]))
            {
                Logger.LogMessage("Process name can't be empty!");
            }
            //проверяем на наличие запрещенных для имени файла символов
            else if (!Regex.IsMatch(args[0], "^((?![\\\\/:*?\"<>|]).)*$"))
            {
                Logger.LogMessage("Don't use any of restricted symbols for process name: \\/:*?\"<>|");
            }
            //проверяем "время жизни", должно быть больше нуля и являться числом c плавающей точкой
            else if (args[1].Contains("-") || !double.TryParse(args[1], out var l) || l <= 0)
            {
                Logger.LogMessage("'Lifetime' parameter should be positive number of minutes!");
            }
            //проверяем "интервал", должно быть больше нуля и являться числом c плавающей точкой
            else if (args[2].Contains("-") || !double.TryParse(args[2], out var p) || p <= 0)
            {
                Logger.LogMessage("'Polling interval' parameter should be positive number of minutes!");
            }
            else
            {
                string processName = args[0].Trim();
                TimeSpan lifetime = TimeSpan.FromMinutes(l);
                TimeSpan polling = TimeSpan.FromMinutes(p);

                Monitor.Init(processName, lifetime, polling);
                //сервисное сообщение о заданных параметрах
                Logger.LogMessage($"Start monitoring of '{processName}' that lives longer than '{lifetime}' minutes with polling interval '{polling}' in minutes");
                Monitor.StartMonitoring();
            }
        } 
    }
}
