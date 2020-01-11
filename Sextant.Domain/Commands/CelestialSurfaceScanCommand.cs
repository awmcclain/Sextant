// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Sextant.Domain.Events;
using System.Collections.Generic;
using Sextant.Domain.Phrases;

namespace Sextant.Domain.Commands
{
    public class CelestialSurfaceScanCommand : ICommand
    {
        private const string ScanCompletePhrases = "SurfaceScanComplete";
        private readonly INavigator    _navigator;
        private readonly ICommunicator _communicator;
        private readonly IPlayerStatus _playerStatus;

        public string SupportedCommand     => "SAAScanComplete";
        public bool Handles(IEvent @event) => @event.Event == SupportedCommand;

        private readonly PhraseBook _surfaceScanCompletePhrases;
        private readonly PhraseBook _oneSurfaceRemainingPhrases;
        private readonly PhraseBook _allSurfaceScansCompletePhrases;
        private readonly PhraseBook _multipleSurfacesRemainingPhrases;
        private readonly PhraseBook _expeditionCompletePhrases;

        public CelestialSurfaceScanCommand(ICommunicator communicator, INavigator navigator, IPlayerStatus playerStatus, CelestialScanPhrases phrases)
        {
            _communicator              = communicator;
            _navigator                 = navigator;
            _playerStatus              = playerStatus;

            _surfaceScanCompletePhrases       = PhraseBook.Ingest(phrases.SurfaceScanComplete);
            _allSurfaceScansCompletePhrases   = PhraseBook.Ingest(phrases.AllSurfaceScansComplete);
            _oneSurfaceRemainingPhrases       = PhraseBook.Ingest(phrases.SingleSurfaceScanRemaining);
            _multipleSurfacesRemainingPhrases = PhraseBook.Ingest(phrases.MultipleSurfaceScansRemaining);
            _expeditionCompletePhrases        = PhraseBook.Ingest(phrases.ExpeditionComplete);
        }

        public void Handle(IEvent @event)
        {
            Dictionary<string, object> eventPayload = @event.Payload;
            string currentSystem                    = _playerStatus.Location;
            bool expeditionSystem                   = _navigator.SystemInExpedition(currentSystem);

            bool wasInExpedition = _navigator.ScanCelestialSurface(eventPayload["BodyName"].ToString());

            if (wasInExpedition) {
                // Only speak if the scanned body was actually one we were looking for (to prevent spam from autodiscovery)
                string script = BuildScript(currentSystem, expeditionSystem);

                _communicator.Communicate(script);
            }
        }

        private string BuildScript(string currentSystem, bool expeditionSystem)
        {
            string script = _surfaceScanCompletePhrases.GetRandomPhrase();

            if (!expeditionSystem)
                return script;

            if (_navigator.GetSystem(currentSystem).Celestials.Any(c => c.SurfaceScanned == false))
            {
                int scansRemaining = _navigator.GetRemainingCelestials(currentSystem, onlySurfaceScans:true).Count();

                if (scansRemaining == 1)
                    return script += _oneSurfaceRemainingPhrases.GetRandomPhrase();

                return script += _multipleSurfacesRemainingPhrases.GetRandomPhraseWith(scansRemaining);
            }

            if (_navigator.ExpeditionComplete)
                return script += _expeditionCompletePhrases.GetRandomPhrase();

            return script += _allSurfaceScansCompletePhrases.GetRandomPhrase();
        }
    }
}