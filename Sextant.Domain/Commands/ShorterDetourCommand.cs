// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Sextant.Domain.Events;
using Sextant.Domain.Phrases;

namespace Sextant.Domain.Commands
{
    public class ShorterDetourCommand : TakeADetourCommand
    {
    

        public override string SupportedCommand => "shorter_detour";
        
        public ShorterDetourCommand(INavigator navigator, ICommunicator communicator, IDetourPlanner detourPlanner, IPlayerStatus playerStatus, PlotExpeditionPhrases phrases, ILogger logger, GrammarPhrases grammar)
            : base(navigator, communicator, detourPlanner, playerStatus, phrases, logger, grammar)
        { }

        public override void Handle(IEvent @event)
        {
            _detourPlanner.DecreaseDetourAmount();
            HandleDetour();
        }

    }
}
