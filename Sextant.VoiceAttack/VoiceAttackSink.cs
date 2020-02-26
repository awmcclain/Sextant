// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Configuration;

namespace Sextant.VoiceAttack
{
    public class VoiceAttackSink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;
        private readonly dynamic _vaProxy;
        public VoiceAttackSink(dynamic vaProxy, IFormatProvider formatProvider=null)
        {
            _formatProvider = formatProvider;
            _vaProxy = vaProxy;
        }

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage(_formatProvider);
            string color = null;
            if (logEvent.Level >= LogEventLevel.Error) {
                color = "red";
            } else if (logEvent.Level == LogEventLevel.Warning) {
                color = "yellow";
            }
            _vaProxy.WriteToLog(message, color);
        }

    }

    public static class VoiceAttackSinkExtensions
    {
        public static LoggerConfiguration VoiceAttack(
            this LoggerSinkConfiguration loggerConfiguration,
            dynamic vaProxy,
            IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.Sink(new VoiceAttackSink(vaProxy, formatProvider));
        }
    }
}
