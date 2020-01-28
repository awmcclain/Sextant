// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Host;
using System;

namespace Sextant.TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            SextantHost sextant = new SextantHost(basePath: Environment.CurrentDirectory, pluginName: "TestHarness");
            sextant.Initialize();

            string input;

            while(true) {
                input = Console.ReadLine();
                if (input.StartsWith("q")) {
                    break;
                }

                string[] parts = input.Split(' ');
                if (parts.Length > 1) {
                    // Create a journal entry
                    sextant.HandleDebug(parts);
                } else {
                    sextant.Handle(input);
                }

            } 
        }
    }
}
