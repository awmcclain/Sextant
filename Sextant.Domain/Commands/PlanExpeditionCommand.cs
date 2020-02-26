// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Sextant.Domain.Events;
using System.Collections.Generic;
using Sextant.Domain;
using Sextant.Domain.Entities;
using Sextant.Domain.Phrases;

namespace Sextant.Domain.Commands
{
    public class PlanExpeditionCommand : ICommand
    {
        protected readonly INavigator _navigator;
        protected readonly ICommunicator _communicator;
        protected readonly IUserDataService _userDataService;
        private readonly IPlayerStatus _playerStatus;
        private readonly IDetourPlanner _detourPlanner;
    
        private readonly CelestialValues _celestialValues;
        protected readonly string _expeditionExists;
        protected readonly string _unableToPlot;
        protected readonly string _expeditionPlotted;

        public virtual string SupportedCommand => "plan_expedition";

        public virtual bool Handles(IEvent @event) => @event.Event == SupportedCommand;

        public PlanExpeditionCommand(ICommunicator communicator, INavigator navigator, IUserDataService userDataService, IPlayerStatus playerStatus, PlotExpeditionPhrases phrases, CelestialValues celestialValues, IDetourPlanner detourPlanner)
        {
            _navigator         = navigator;
            _communicator      = communicator;
            _userDataService   = userDataService;
            _playerStatus      = playerStatus;
            _celestialValues   = celestialValues;
            _detourPlanner     = detourPlanner;

            _expeditionExists  = phrases.ExpeditionExists;
            _unableToPlot      = phrases.UnableToPlot;
            _expeditionPlotted = phrases.ExpeditionPlotted;
        }

        public virtual void Handle(IEvent @event)
        {

            if (_navigator.ExpeditionStarted) {
                if (_navigator.ExpeditionComplete) {
                    _communicator.Communicate("I'll clear your earlier expedition.");
                    _navigator.CancelExpedition();
                } else {
                    _communicator.Communicate(_expeditionExists);
                    return;
                }
            }

            bool success;

            if (_detourPlanner.DetourPlanned) {
                success = _detourPlanner.ConfirmDetour();
            } else {
                IEnumerable<StarSystem> expeditionData = _userDataService.GetExpeditionData();
                success = _navigator.PlanExpedition(expeditionData);
            }

            if (!success)
            {
                _communicator.Communicate(_unableToPlot);
                return;
            }

            _playerStatus.SetExpeditionStart(DateTimeOffset.Now);
            CommunicateExpedition();
        }

        protected void CommunicateExpedition()
        {
            int totalSystems = _navigator.SystemsRemaining();
            int totalPlanets = _navigator.CelestialsRemaining();

            string script = string.Format(_expeditionPlotted, totalSystems, totalPlanets);

            script += _navigator.SpokenCelestialList(_navigator.GetAllRemainingCelestials());
            _communicator.Communicate(script);
        }
    }
}
