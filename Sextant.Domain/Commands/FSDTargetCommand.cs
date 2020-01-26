// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Events;

namespace Sextant.Domain.Commands
{
    public class FSDTargetCommand : ICommand
    {
        public string SupportedCommand => "FSDTarget";
        public bool Handles(IEvent @event) => @event.Event == SupportedCommand;

        private readonly IPlayerStatus _playerStatus;
        private readonly INavigator _navigator;
        private readonly ILogger _logger;

        public FSDTargetCommand(IPlayerStatus playerStatus, INavigator navigator, ILogger logger)
        {
            _playerStatus = playerStatus;
            _navigator    = navigator;
            _logger       = logger;
        }

        public void Handle(IEvent @event)
        {
            if (_navigator.OnExpedition) {
                return;
            }

            string location = @event.Payload["Name"].ToString();

            _playerStatus.SetDestination(location);

            _logger.Information($"Setting destination to {location}");
        }
    }
}
