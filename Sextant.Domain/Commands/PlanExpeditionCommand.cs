﻿// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Sextant.Domain.Events;
using System.Collections.Generic;
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
    
        private readonly CelestialValues _celestialValues;
        protected readonly string _expeditionExists;
        protected readonly string _unableToPlot;
        protected readonly string _expeditionPlotted;
        protected readonly string _andPhrase;
        protected readonly string _pluralPhrase;

        public virtual string SupportedCommand => "plan_expedition";

        public virtual bool Handles(IEvent @event) => @event.Event == SupportedCommand;

        public PlanExpeditionCommand(ICommunicator communicator, INavigator navigator, IUserDataService userDataService, IPlayerStatus playerStatus, PlotExpeditionPhrases phrases, CelestialValues celestialValues)
        {
            _navigator         = navigator;
            _communicator      = communicator;
            _userDataService   = userDataService;
            _playerStatus      = playerStatus;
            _celestialValues   = celestialValues;

            _expeditionExists  = phrases.ExpeditionExists;
            _unableToPlot      = phrases.UnableToPlot;
            _expeditionPlotted = phrases.ExpeditionPlotted;
            _andPhrase         = phrases.AndPhrase;
            _pluralPhrase      = phrases.PluralPhrase;
        }

        public virtual void Handle(IEvent @event)
        {
            IEnumerable<StarSystem> expeditionData = _userDataService.GetExpeditionData();

            if (_navigator.ExpeditionStarted)
            {
                _communicator.Communicate(_expeditionExists);
                return;
            }

            bool success = _navigator.PlanExpedition(expeditionData);

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

            var celestialsByCategory = _navigator.GetAllRemainingCelestials()
                                                 .GroupBy(c => c.Classification)
                                                 .ToDictionary(grp => grp.Key, grp => grp.ToList());

            int counter = 0;

            foreach (var item in celestialsByCategory)
            {
                counter++;

                if (counter == celestialsByCategory.Count() && celestialsByCategory.Count() > 1)
                    script += $"{_andPhrase} ";

                string pluralized = item.Value.Count == 1 ? string.Empty : _pluralPhrase;
                
                script += $"{item.Value.Count} {_celestialValues.NameFromClassification(item.Key)}{pluralized}, ";
            }

            _communicator.Communicate(script);
        }
    }
}
