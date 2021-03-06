﻿// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain;
using Sextant.Domain.Entities;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sextant.Infrastructure
{
    public class ExpeditionParser : IExpeditionParser
    {
        private const string Header = "  #   Jump System/Planets";

        private static ILogger _logger;

        public ExpeditionParser(ILogger logger)
        {
            _logger = logger;
        }

        public IEnumerable<StarSystem> ParseExpeditionData(string input) {
            try {
                input = input.Replace(Environment.NewLine, "\n");
                string[] lines = input.Split('\n');

                List<StarSystem> systems = new List<StarSystem>();
                StarSystem currentSystem = null;

                if (!lines.First().Contains(Header)) {
                    //_logger.Error($"First line '{lines.First()}' doesn't match header '{Header}'");
                    return null;
                }

                //_logger.Information($"I found {lines.Count()} lines");

                foreach (var line in lines.Skip(2))
                {
                    //_logger.Information($"Processing {line}");
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    if (!line.StartsWith("\t"))
                    {
                        var systemName = line.Substring(11);

                        // Remove * at the end of the data (populated systems)
                        systemName = systemName.TrimEnd(' ', '*');

                        currentSystem = new StarSystem(systemName);
                        systems.Add(currentSystem);
                        continue;
                    }

                    var length = line.IndexOf('(') - 13;
                    var planet = line.Substring(12, length);

                    var index = line.IndexOf(')') + 2;
                    var classification = line.Substring(index);

                    currentSystem.AddCelestial(string.Join(" ", currentSystem.Name, planet), classification);
                }

                return systems;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception parsing expedition data");
                return null;
            }
        }
    }
}
