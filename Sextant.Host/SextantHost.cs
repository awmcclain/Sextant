// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Serilog;
using Sextant.Domain;
using Sextant.Domain.Commands;
using Sextant.Infrastructure;
using Sextant.Infrastructure.Journal;
using SimpleInjector;
using System;
using System.Collections.Generic;

namespace Sextant.Host
{
    public class SextantHost
    {
        private static readonly Container container = new Container();
        private static ICommandExecutor _executor;
        private static Serilog.ILogger _logger;
        private readonly string _pluginName;

        public SextantHost(string basePath, string pluginName, bool configureLogging=true)
        {
            _pluginName = pluginName;
            if (configureLogging) {
                InitializeLogging();
            } else {
                _logger = Log.Logger;
            }
            

            try
            {
                new Bootstrapper().Bootstrap(basePath, container);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception during bootstrapping");
            }
        }

        public void Initialize()
        {
            _executor        = container.GetInstance<ICommandExecutor>();
            var communicator = container.GetInstance<ICommunicator>();
            var watcher      = container.GetInstance<IJournalWatcher>();

            watcher.Initialize();
            communicator.Initialize();

            _executor.Handle(new VoiceAttackEvent("initialized"));

            _logger.Information($"{_pluginName} Initialized");
        }

        public void Handle(string context, Dictionary<string, object> payload=null)
        {
            try
            {
                _executor.Handle(EventFactory.FromVoiceAttack(context, payload));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Exception handling VoiceAttack command. Context : {context}");
            }
        }

        public void HandleDebug(string[] parts)
        {
            try
            {
                _executor.Handle(new JournalEvent(parts[0], new Dictionary<string, object> { { parts[1], parts[2]} }));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Exception handling Journal command. Context : {String.Join(" ", parts)}");
            }
 
        }

        public static LoggerConfiguration DefaultLoggingConfiguration(string pluginName)
        {
            return new LoggerConfiguration()
                             .Enrich.WithProperty("PluginVersion", pluginName)
                             .WriteTo.RollingFile("log-{Date}.txt", Serilog.Events.LogEventLevel.Information, "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{PluginVersion}][{Level}] {Message}{NewLine}{Exception}");
        }
        private void InitializeLogging()
        {
            Log.Logger = DefaultLoggingConfiguration(_pluginName).CreateLogger();
            _logger = Log.Logger;
        }
    }
}
