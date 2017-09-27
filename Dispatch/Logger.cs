using log4net;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch
{
    internal class Logger
    {
        public static ILog Log;

        public static void Init()
        {
            var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Darel Dispatcher";

            if (!Directory.Exists(appDataDir))
                Directory.CreateDirectory(appDataDir);

            var logDir = appDataDir + "\\Logs";

            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);

            //var filter = new log4net.Filter.LevelRangeFilter
            //{
            //    LevelMin = new log4net.Core.Level(1, "INFO"),
            //    LevelMax = new log4net.Core.Level(4, "FATAL"),
            //};

            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.RemoveAllAppenders();

            var patternLayout = new PatternLayout
            {
                ConversionPattern = "%date [%level] %logger - %message%newline"
            };
            patternLayout.ActivateOptions();

            var appender = new log4net.Appender.RollingFileAppender
            {
                File = logDir+"\\", //+ "\\log.txt",
                AppendToFile = true,
                MaximumFileSize = "5MB",
                MaxSizeRollBackups = 20,
                RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Composite,
                StaticLogFileName = false,
                DatePattern = "dd.MM.yyyy'.log'",
                Layout = patternLayout
            };

            appender.ActivateOptions();

            hierarchy.Root.AddAppender(appender);

            hierarchy.Root.Level = Level.Info;
            hierarchy.Configured = true;

            Log =LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            //appender.AddFilter(filter);

            //log4net.Config.BasicConfigurator.Configure(appender);
        }
    }
}
