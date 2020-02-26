// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Sextant.Domain
{
    public class CelestialData
    {
        // Average scan values from https://forums.frontier.co.uk/threads/exploration-value-formulae.232000/
        // FSSPlussDSS (non-efficient total) assumes 1.25 multiplier, but is rounded to 3 signficiant figures (thousands)
        public string Name { get; set; }
        public int FSS { get; set; }
        public int FSSPlusDSS { get; set; }
        public int FSSPlusDSSEfficient { get; set; }
    }
}
