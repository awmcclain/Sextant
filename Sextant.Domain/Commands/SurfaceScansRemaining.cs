// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Sextant.Domain.Events;
using Sextant.Domain.Entities;
using System.Collections.Generic;
using Sextant.Domain.Phrases;

namespace Sextant.Domain.Commands
{
    public class SurfaceScansRemainingCommand : ScansRemainingCommand
    {
        public override string SupportedCommand     => "surface_scans_remaining";
        public SurfaceScansRemainingCommand(ICommunicator communicator, INavigator navigator, IPlayerStatus playerStatus, ScansRemainingPhrases phrases)
          : base(communicator, navigator, playerStatus, phrases)
        { }

        protected override IEnumerable<Celestial> RemainingBodies() {
            string currentSystem = _playerStatus.Location;

            return _navigator.GetSystem(currentSystem)?
                                                .Celestials
                                                .Where(c => c.Scanned == false)
                                                .OrderBy(r => r.ShortName);
        }
    }
}
