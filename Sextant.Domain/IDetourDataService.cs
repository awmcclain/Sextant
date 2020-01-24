// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Entities;
using System.Collections.Generic;

namespace Sextant.Domain
{
    public interface IDetourDataService
    {
        IEnumerable<StarSystem> GetExpeditionData(string startSystem, string endSystem, int detourAmount);
    }
}
