using System;
using System.Text;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace AskodOnline.Editor.Helpers
{
    public static class Log4Net
    {
        private static readonly object SyncRoot = new Object();

        static Log4Net()
        {
            ConfigureLog4Net();
        }

        public static ILog GetLogger(Type type)
        {
            lock (SyncRoot)
            {
                if (LogManager.GetCurrentLoggers().Length == 0)
                    ConfigureLog4Net();
                return LogManager.GetLogger(type);
            }
        }

        public static void ConfigureLog4Net()
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

            PatternLayout patternLayout = new PatternLayout { ConversionPattern = "%date [%thread] %-5level %logger - %message%newline" };
            patternLayout.ActivateOptions();

            // declare a RollingFileAppender with 5MB per file and max. 10 files
            RollingFileAppender appenderNH = new RollingFileAppender
            {
                Name = "RollingLogFileAppenderNHibernate",
                AppendToFile = true,
                MaximumFileSize = "5MB",
                MaxSizeRollBackups = 10,
                RollingStyle = RollingFileAppender.RollingMode.Size,
                StaticLogFileName = true,
                LockingModel = new FileAppender.MinimalLock(),
                File = @"Log4net\log-nhibernate.log",
                Layout = new PatternLayout("%date - %message%newline"), 
                Encoding = Encoding.UTF8
            };
            // this activates the FileAppender (without it, nothing would be written)
            appenderNH.ActivateOptions();

            // This is required, so that we can access the Logger by using 
            // LogManager.GetLogger("NHibernate.SQL") and it can used by NHibernate
            Logger loggerNH = hierarchy.GetLogger("NHibernate.SQL") as Logger;
            loggerNH.Level = Level.Error;
            loggerNH.AddAppender(appenderNH);

            Logger loggerNH1 = hierarchy.GetLogger("NHibernate") as Logger;
            loggerNH1.Level = Level.Error;
            loggerNH1.AddAppender(appenderNH);


            RollingFileAppender roller = new RollingFileAppender
            {
                AppendToFile = true,
                File = @"Log4net\EventLog.log",
                Layout = patternLayout,
                MaxSizeRollBackups = 50,
                MaximumFileSize = "2MB",
                RollingStyle = RollingFileAppender.RollingMode.Size,
                StaticLogFileName = true,
                Encoding = Encoding.UTF8
            };
            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            MemoryAppender memory = new MemoryAppender();
            memory.ActivateOptions();
            hierarchy.Root.AddAppender(memory);

            hierarchy.Root.Level = Level.Debug;
            hierarchy.Configured = true;
        }
    }
}