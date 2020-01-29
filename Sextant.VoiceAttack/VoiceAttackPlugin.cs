// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Host;
using System;
using System.IO;

using Serilog;
using Logger = Serilog.Log;

namespace Sextant.VoiceAttack
{
    public class VoiceAttackPlugin
    {
        private static SextantHost _host;

        public static string VA_DisplayName()
        {
            return "Sextant v1.1.0";
        }

        public static string VA_DisplayInfo()
        {
            return "Sextant exploration assistant";
        }

        public static Guid VA_Id()
        {
            return new Guid("{3A7AC4CC-AAEE-407A-A496-B29DF1937AD1}");
        }

        public static void VA_Init1(dynamic vaProxy)
        {
            var basePath = Path.Combine(Environment.CurrentDirectory, "Apps", "Sextant");

            // Re-configure logging
            Log.Logger = VoiceAttackSinkExtensions.VoiceAttack(new LoggerConfiguration().WriteTo, vaProxy)
                             .Enrich.WithProperty("PluginVersion", VA_DisplayName())
                             .MinimumLevel.Information()
                             .WriteTo.RollingFile("log-{Date}.txt", Serilog.Events.LogEventLevel.Information, "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{PluginVersion}][{Level}] {Message}{NewLine}{Exception}")
                             .CreateLogger();

            _host = new SextantHost(basePath: basePath, pluginName: VA_DisplayName(), configureLogging: false);
            
        }

        public static void VA_Invoke1(dynamic vaProxy)
        {
            string context = vaProxy.Context;

            _host?.Handle(context);
        }

        public static void VA_Exit1(dynamic vaProxy) { }
        public static void VA_StopCommand() { }
    }
}
