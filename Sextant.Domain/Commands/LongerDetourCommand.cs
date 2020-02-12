// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Sextant.Domain.Events;
using Sextant.Domain.Phrases;

namespace Sextant.Domain.Commands
{
    public class LongerDetourCommand : TakeADetourCommand
    {
        public override string SupportedCommand => "longer_detour";
        
        public LongerDetourCommand(INavigator navigator, ICommunicator communicator, IDetourPlanner detourPlanner, IPlayerStatus playerStatus, PlotExpeditionPhrases phrases, ILogger logger, GrammarPhrases grammar)
            : base(navigator, communicator, detourPlanner, playerStatus, phrases, logger, grammar)
        { }

        public override void Handle(IEvent @event)
        {
            _detourPlanner.IncreaseDetourAmount();
            HandleDetour();
        }
    }
}
