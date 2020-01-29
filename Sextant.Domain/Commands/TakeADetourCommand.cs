// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Sextant.Domain.Events;
using System.Collections.Generic;
using Sextant.Domain.Entities;
using Sextant.Domain.Phrases;

namespace Sextant.Domain.Commands
{
    public class TakeADetourCommand : ICommand
    {
        protected readonly INavigator _navigator;
        protected readonly ICommunicator _communicator;
        protected readonly IDetourPlanner _detourPlanner;
        private readonly IPlayerStatus _playerStatus;
        private readonly ILogger _logger;
    
        protected readonly string _expeditionExists;
        protected readonly string _detourFound;


        public virtual string SupportedCommand => "take_a_detour";

        public virtual bool Handles(IEvent @event) => @event.Event == SupportedCommand;

        public TakeADetourCommand(INavigator navigator, ICommunicator communicator, IDetourPlanner detourPlanner, IPlayerStatus playerStatus, PlotExpeditionPhrases phrases, ILogger logger)
        {
            _navigator         = navigator;
            _communicator      = communicator;
            _detourPlanner     = detourPlanner;
            _playerStatus      = playerStatus;
            _logger            = logger;

            _expeditionExists  = phrases.ExpeditionExists;
            _detourFound       = phrases.ExpeditionPlotted;

        }

        public virtual void Handle(IEvent @event) => HandleDetour();

        protected void HandleDetour()
        {
            if (_navigator.OnExpedition) {
                _communicator.Communicate(_expeditionExists);
                return;
            }

            // Get the start and destination
            if (String.IsNullOrEmpty(_playerStatus.Location)) {
                _logger.Error("No location known, can't find detour");
                _communicator.Communicate("Unable to find detour, no starting location known."); 
                return;
            }

            if (String.IsNullOrEmpty(_playerStatus.Destination)) {
                _logger.Error("No destination found, can't plot detour");
                _communicator.Communicate("Unable to find detour, no destination known."); 
                return;
            }

            _communicator.Communicate($"Searching for high-value systems within {_detourPlanner.DetourAmount} light years of your route to your target: {_playerStatus.Destination}. ");

            // try...catch here?
            bool result =  _detourPlanner.PlanDetour();
            if (result == false) {
                _communicator.Communicate("Sorry, I had a problem getting detour data.");
                return;
            }

            if (_detourPlanner.SystemsInDetour == 0) {
                _communicator.Communicate("I couldn't find any systems close enough to your route.");
                return;
            }
            
            string script = string.Format("I found {0} high-value {1} within {2} light years from your route. ", _detourPlanner.SystemsInDetour, 
                                          PhraseBook.PluralizedEnding(_detourPlanner.SystemsInDetour, "system"),
                                          _detourPlanner.DetourAmount);

            _communicator.Communicate(script);
            _communicator.Communicate("To confirm this detour, say 'plan expedition'");
        }

    }
}
