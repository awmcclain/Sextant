// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Serilog;

namespace Sextant.Domain
{
    public class CelestialValues
    {
        public Dictionary<string, CelestialData> CelestialData { get; set; }
        public string NameFromClassification(string classification) {
            CelestialData data;
            if (CelestialData != null && CelestialData.TryGetValue(classification, out data)) {
                Log.Logger.Debug("Getting classification for {classification}: {$data}", classification, data);
                return data.Name;
            } else {
                return "Unknown";
            }
        }

        public int ScanValue(string classification) {
            CelestialData data;
            if (CelestialData != null && CelestialData.TryGetValue(classification, out data)) {
                return data.FSS;
            } else {
                return 0;
            }
        }
        public int SurfaceScanValue(string classification) {
            CelestialData data;
            if (CelestialData != null && CelestialData.TryGetValue(classification, out data)) {
                return data.FSSPlusDSSEfficient;
            } else {
                return 0;
            }
        }

    }
}
