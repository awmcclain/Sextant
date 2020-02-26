// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Entities;
using System.Collections.Generic;

namespace Sextant.Domain
{
    public interface INavigator
    {
        bool OnExpedition { get; }
        bool ExpeditionComplete { get; }
        bool ExpeditionStarted { get; }

        Celestial GetNextCelestial(StarSystem system);
        StarSystem GetNextSystem();
        StarSystem GetSystem(string systemName);

        int SystemsRemaining();
        int CelestialsRemaining();

        int ValueForSystem(string systemName);
        int ValueForExpedition();

        bool PlanExpedition(IEnumerable<StarSystem> systems);
        bool ExtendExpedition(IEnumerable<StarSystem> systems);

        List<Celestial> GetRemainingCelestials(string systemName, bool onlySurfaceScans=false);
        List<Celestial> GetAllRemainingCelestials();
        List<StarSystem> GetAllExpeditionSystems();

        bool CancelExpedition();
        bool ScanCelestial(string celestial);
        bool ScanCelestialSurface(string celestial, bool efficient);
        bool ScanSystem(string system);
        bool UnscanSystem(string system);
        bool SystemInExpedition(string currentSystem);

        string SpokenCelestialList(List<Celestial> celestials);
    }
}