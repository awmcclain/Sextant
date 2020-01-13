// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Sextant.Domain
{
    public class CelestialValues
    {
        public Dictionary<string, CelestialData> CelestialData { get; set; }
        public float EfficiencyMultiplier { get; set; }

        public string NameFromClassification(string classification) {
            CelestialData data;
            if (CelestialData != null && CelestialData.TryGetValue(classification, out data)) {
                return data.Name;
            } else {
                return "Unknown";
            }
        }
    }
}
