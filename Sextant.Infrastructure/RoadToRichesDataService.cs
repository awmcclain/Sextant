// Copyright (c) Stickymadness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain;
using Sextant.Domain.Entities;

using System.Collections.Generic;
using System;
using HtmlAgilityPack;

namespace Sextant.Infrastructure
{
    public class RoadToRichesDataService : IDetourDataService
    {
        private static ILogger _logger;
        private static IExpeditionParser _parser;

        private const string RoadToRichesURL = "http://edtools.ddns.net/expl.php?a=v&x=on&ap=on";

        public RoadToRichesDataService(ILogger logger, IExpeditionParser parser)
        {
            _logger = logger;
            _parser = parser;
        }

        public IEnumerable<StarSystem> GetExpeditionData(string startSystem, string endSystem, int detourAmount)
        {
            // Read the website
            var result = GetRoadToRichesData(startSystem, endSystem, detourAmount);
            if (String.IsNullOrEmpty(result)) {
                //_logger.Error("Nothing returned from data");
                return null;
            }

            _logger.Information("Parsing results...");
            return _parser.ParseExpeditionData(result);
        }

        public string GetRoadToRichesData(string startSystem, string endSystem, int detourAmount)
        {
            if (String.IsNullOrEmpty(startSystem) || String.IsNullOrEmpty(endSystem)) {
                _logger.Error("Invalid sytem passed to GetRoadToRichesData");
                return null;
            }
            try {
                var uri = RoadToRichesURL + $"&f={Uri.EscapeDataString(startSystem)}&t={Uri.EscapeDataString(endSystem)}&r={detourAmount}";
                var web = new HtmlWeb();
                _logger.Information($"Loading {uri}");
                    
                var doc = web.Load(uri);
                if (doc == null) {
                    _logger.Error("Couldn't retrieve webpage");
                    return null;
                }
                var documentNode = doc.DocumentNode;
                var textArea = documentNode.SelectSingleNode("//textarea");
                if (textArea == null) {
                    _logger.Error($"Couldn't find data on {uri}");
                    return null;
                }
                var result = textArea.InnerText;
                _logger.Information($"Got back {result}");

                return result;
            }
            catch (System.Exception e)
            {
                _logger.Error(e, "Error retrieving route data");
                return null;
            }
            
        }

    }
    
}
