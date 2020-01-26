// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Sextant.Domain.Entities;

namespace Sextant.Domain
{
    public interface IDetourPlanner
    {
        int DetourAmount { get; }

        void IncreaseDetourAmount();
        void DecreaseDetourAmount();        

        bool DetourPlanned { get; }
        int SystemsInDetour { get; }
        int PlanetsInDetour { get; }
        bool PlanDetour();
        bool ConfirmDetour();
    }
}
