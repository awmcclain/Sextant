// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Entities;

namespace Sextant.Infrastructure.Repository
{
    public class CelestialDocument
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string System { get; set; }
        public string Clasification { get; set; }
        public bool Scanned { get; set; }
        public bool SurfaceScanned { get; set; }
        public bool Landable { get; set; }
        public bool Efficient { get; set; }

        public CelestialDocument()
        { }

        public CelestialDocument(Celestial celestial)
        {
            Id             = celestial.Id;
            Name           = celestial.Name;
            System         = celestial.System;
            Clasification  = celestial.Classification;
            Scanned        = celestial.Scanned;
            SurfaceScanned = celestial.SurfaceScanned;
            Landable       = celestial.Landable;
            Efficient      = celestial.Efficient;
        }

        public Celestial ToEntity()
        {
            return new Celestial(Name, Clasification, System, Scanned, Id, SurfaceScanned, Efficient);
        }
    }
}
