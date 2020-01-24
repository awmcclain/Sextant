// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain;
using System;
using Logger = Serilog.Log;

namespace Sextant.Infrastructure
{
    public class ConsoleLogger : ILogger
    {
        public void Information(string message)                => Console.WriteLine(message);
        public void Error(string message)                      => Console.WriteLine(message);
        public void Error(Exception exception, string message) 
        { 
            Console.WriteLine(message);
            Console.WriteLine(exception.ToString());
        }
    }
}
