// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Sextant.Domain
{
    public interface IDetourPlanner
    {
        int DetourAmount { get; set; }

        void IncreaseDetourAmount();
        void DecreaseDetourAmount();

        string Destination { get; set; }
        bool DetourPlanned { get; }
        bool DetourSuggested { get; set; }
        int SystemsInDetour { get; }
        int PlanetsInDetour { get; }
        bool PlanDetour();
        bool ConfirmDetour();
    }
}
