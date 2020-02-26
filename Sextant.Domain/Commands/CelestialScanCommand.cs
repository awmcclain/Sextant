// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Sextant.Domain.Entities;
using Sextant.Domain.Events;
using System.Collections.Generic;
using Sextant.Domain.Phrases;

namespace Sextant.Domain.Commands
{
    public class CelestialScanCommand : ICommand
    {
        private const string ScanCompletePhrases = "ScanComplete";
        private readonly INavigator    _navigator;
        private readonly ICommunicator _communicator;
        private readonly IPlayerStatus _playerStatus;
        private readonly CelestialValues _values;

        public string SupportedCommand     => "Scan";
        public bool Handles(IEvent @event) => @event.Event == SupportedCommand;

        private readonly PhraseBook _scanCompletePhrases;
        private readonly PhraseBook _oneRemainingPhrases;
        private readonly PhraseBook _allScansCompletePhrases;
        private readonly PhraseBook _switchtoSurfacesPhrases;
        private readonly PhraseBook _multipleRemainingPhrases;
        private readonly PhraseBook _expeditionCompletePhrases;
        private readonly PhraseBook _classificationCompletePhrases;
        private readonly PhraseBook _finalDestinationPhrases;
        private readonly PhraseBook _systemValuePhrases;


        public CelestialScanCommand(ICommunicator communicator, INavigator navigator, IPlayerStatus playerStatus, CelestialScanPhrases phrases, CelestialValues values)
        {
            _communicator              = communicator;
            _navigator                 = navigator;
            _playerStatus              = playerStatus;
            _values                    = values;

            _scanCompletePhrases            = PhraseBook.Ingest(phrases.ScanComplete);
            _allScansCompletePhrases        = PhraseBook.Ingest(phrases.AllScansComplete);
            _switchtoSurfacesPhrases        = PhraseBook.Ingest(phrases.SwitchToSurfaces);
            _oneRemainingPhrases            = PhraseBook.Ingest(phrases.SingleScanRemaining);
            _multipleRemainingPhrases       = PhraseBook.Ingest(phrases.MultipleScansRemaining);
            _expeditionCompletePhrases      = PhraseBook.Ingest(phrases.ExpeditionComplete);
            _classificationCompletePhrases  = PhraseBook.Ingest(phrases.ClassificationComplete);
            _finalDestinationPhrases        = PhraseBook.Ingest(phrases.FinalDestination);
            _systemValuePhrases             = PhraseBook.Ingest(phrases.SystemValue);
        }

        public void Handle(IEvent @event)
        {
            Dictionary<string, object> eventPayload = @event.Payload;
            string currentSystem                    = _playerStatus.Location;
            bool expeditionSystem                   = _navigator.SystemInExpedition(currentSystem);
            string celestialName                    = eventPayload["BodyName"].ToString();

            bool celestialScanned = _navigator.ScanCelestial(celestialName);

            if (celestialScanned) {
                // Only speak if the scanned body was actually one we were looking for (to prevent spam from autodiscovery)
                string script = BuildScript(currentSystem, celestialName);

                _communicator.Communicate(script);
            }
        }

        private string BuildScript(string currentSystem, string celestialName)
        {
            string script = _scanCompletePhrases.GetRandomPhrase();

            StarSystem system = _navigator.GetSystem(currentSystem);
            bool exhaustedCelestialType = false;
            if (system.Celestials.Any(c => c.Scanned == false)) {
                Celestial celestial = system.Celestials.First(c => c.Name == celestialName);
                if (celestial != null) {
                    // See if there are any more of the same celestial type
                    if (!system.Celestials.Any(c => c.Scanned == false && c.Classification == celestial.Classification)) {
                        // We've scanned all the planets of this classification

                        exhaustedCelestialType = true;
                        // "You've completed all the Terraformable water worlds."
                        script += _classificationCompletePhrases.GetRandomPhraseWith(celestial.LongClassification(_values));
                    }
                }

                List<Celestial> remainingCelestials = _navigator.GetRemainingCelestials(currentSystem);
                int scansRemaining = remainingCelestials.Count();

                if (scansRemaining == 1) {
                    script += _oneRemainingPhrases.GetRandomPhrase();
                    if (exhaustedCelestialType) {
                        // "1 scan remains, a high metal content planet."
                        script += " a " + remainingCelestials.First().LongClassification(_values);
                    }
                } else {
                    script += _multipleRemainingPhrases.GetRandomPhraseWith(scansRemaining);
                    if (exhaustedCelestialType) {
                        // "2 scans remain, 2 high metal content planets."
                        script += _navigator.SpokenCelestialList(remainingCelestials);
                    }
                }

                return script;
            }

            if (_navigator.ExpeditionComplete) {
                script += _expeditionCompletePhrases.GetRandomPhrase();
                if (!String.IsNullOrEmpty(_playerStatus.Destination)) {
                    script += _finalDestinationPhrases.GetRandomPhrase();
                }

                return script;
            }

            script += _allScansCompletePhrases.GetRandomPhrase();
            script += _switchtoSurfacesPhrases.GetRandomPhrase();
            script += _systemValuePhrases.GetRandomPhraseWith(_navigator.ValueForSystem(currentSystem).ToSpeakableString());
            return script;

        }
    }
}