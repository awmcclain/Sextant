// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Sextant.Domain.Events;

namespace Sextant.Domain.Commands
{
    public class FSDTargetCommand : ICommand
    {
        public string SupportedCommand => "FSDTarget";
        public bool Handles(IEvent @event) => @event.Event == SupportedCommand;

        private readonly IDetourPlanner _detourPlanner;
        private readonly INavigator _navigator;
        private readonly ILogger _logger;
        private readonly ICommunicator _communicator;

        public FSDTargetCommand(IDetourPlanner detourPlanner, INavigator navigator, ICommunicator communicator, ILogger logger)
        {
            _detourPlanner = detourPlanner;
            _navigator     = navigator;
            _communicator  = communicator;
            _logger        = logger;
        }

        public void Handle(IEvent @event)
        {
            if (_navigator.OnExpedition) {
                return;
            }

            if (!_detourPlanner.DetourPlanned) {
                _logger.Information("Checking to see if we have a long route plotted");
                object val;
                @event.Payload.TryGetValue("JumpsRemaining", out val);
                int jumpsRemaining;
                if (Int32.TryParse(val.ToString(), out jumpsRemaining) && jumpsRemaining > 5) {
                    _communicator.Communicate("Commander, it looks as if you're going on a long route. If you'd like to scan high-value systems on the way, target your final destination and say 'take a detour'");
                }
            }

            string location = @event.Payload["Name"].ToString();

            _detourPlanner.SetDestination(location);

            _logger.Information($"Setting destination to {location}");
        }
    }
}
