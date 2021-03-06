﻿using System.IO;
using NLog;
using NLog.Layouts;
using NLog.Targets;
using NLog.Targets.Wrappers;
using NxPlx.Abstractions;
using NxPlx.Configuration;

namespace NxPlx.Infrastructure.Logging
{
    public class NLoggingService : ILoggingService
    {
        private readonly Logger _logger = LogManager.GetLogger("System");

        public NLoggingService()
        {
            var cfg = ConfigurationService.Current;
            
            var config = new NLog.Config.LoggingConfiguration();
            
            var logfile = new AsyncTargetWrapper
            {
                WrappedTarget = new FileTarget("logfile")
                {
                    Layout = new JsonLayout
                    {
                        Attributes =
                        {
                            new JsonAttribute("Time", Layout.FromString("${longdate}")),
                            new JsonAttribute("Level", Layout.FromString("${level}")),
                            new JsonAttribute("Message", Layout.FromString("${message}")),
                            new JsonAttribute("Template", Layout.FromString("${template}")),
                        },
                        IncludeAllProperties = true
                    },
                    FileName = Path.Combine(cfg.LogFolder, "log.current.json"),
                    ArchiveFileName = Path.Combine(cfg.LogFolder, "archives", "log.{#}.json"),
                    ArchiveAboveSize = 5000000,
                    ArchiveEvery = FileArchivePeriod.Day,
                    ArchiveNumbering = ArchiveNumberingMode.Date,
                    MaxArchiveFiles = 50,
                    OptimizeBufferReuse = true
                }
            };
            
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            
            #if DEBUG
            var logconsole = new ConsoleTarget("logconsole");
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole);
            #endif
            
            
            LogManager.Configuration = config;
        }

        public void Trace(string message, params object[] arguments) => _logger.Trace(message, arguments);

        public void Debug(string message, params object[] arguments) => _logger.Debug(message, arguments);

        public void Info(string message, params object[] arguments) => _logger.Info(message, arguments);

        public void Warn(string message, params object[] arguments) => _logger.Warn(message, arguments);

        public void Error(string message, params object[] arguments) => _logger.Error(message, arguments);

        public void Fatal(string message, params object[] arguments) => _logger.Fatal(message, arguments);
    }
}