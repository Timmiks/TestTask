using MonitorUnitTests.Configuration;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace MonitorUnitTests
{
    //NUnit адаптер ранит тесты 
    [SetUpFixture]
    public class TestsGlobalSetupTeardown
    {
        //чистим логи "монитора", который запускается из bin/debug тестовой сборки
        [OneTimeSetUp]
        public void GlobalSetup()
        {
            var initialFolderFiles = Config.LogDirectory.GetFiles(Config.FilterExtension);
            for (int i = 0; i < initialFolderFiles.Length; i++)
            {
                File.Delete(initialFolderFiles[i].FullName);
            }
        }

        //складываем логи "мониторов" в отдельную папку после каждого прогона тестов
        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            var initialFolderFiles = Config.LogDirectory.GetFiles(Config.FilterExtension);
            if (initialFolderFiles.Any())
            {
                Directory.CreateDirectory(Config.OutputFolder);

                for (int i = 0; i < initialFolderFiles.Length; i++)
                {
                    File.Move(initialFolderFiles[i].FullName, Path.Combine(Config.OutputFolder, initialFolderFiles[i].Name));
                }
            }
        }
    }
}
