// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Entities;
using Sextant.Domain.Events;
using Sextant.Domain.Phrases;
using System.Linq;

namespace Sextant.Domain.Commands
{
    public class NextScanCommand : ICommand
    {
        private readonly INavigator    _navigator;
        private readonly ICommunicator _communicator;
        private readonly IPlayerStatus _playerStatus;

        public string SupportedCommand     => "next_scan";
        public bool Handles(IEvent @event) => @event.Event == SupportedCommand;

        private PhraseBook _skipPhrases;
        private PhraseBook _nextScanPhrases;
        private PhraseBook _completePhrases;
        private PhraseBook _surfacePhrases;

        public NextScanCommand(ICommunicator communicator, INavigator navigator, IPlayerStatus playerStatus, NextScanPhrases phrases)
        {
            _navigator       = navigator;
            _communicator    = communicator;
            _playerStatus    = playerStatus;

            _skipPhrases     = PhraseBook.Ingest(phrases.SkipSystem);
            _nextScanPhrases = PhraseBook.Ingest(phrases.NextScan);
            _completePhrases = PhraseBook.Ingest(phrases.ScansComplete);
            _surfacePhrases  = PhraseBook.Ingest(phrases.NeedToScanSurface);
        }

        public void Handle(IEvent @event)
        {
            string currentSystem = _playerStatus.Location;
            StarSystem system    = _navigator.GetSystem(currentSystem);

            if (system == null)
            {
                _communicator.Communicate(_skipPhrases.GetRandomPhrase());
                return;
            }

            Celestial nextToScan = _navigator.GetNextCelestial(system);

            _communicator.Communicate(BuildScript(nextToScan));
        }

        private string BuildScript(Celestial nextToScan) {
            if (nextToScan == null) {
                return _completePhrases.GetRandomPhrase();
            }

            string script = _nextScanPhrases.GetRandomPhraseWith(nextToScan.ShortName);
            if (nextToScan.Scanned == true && nextToScan.SurfaceScanned == false) {
                script += _surfacePhrases.GetRandomPhrase();
            }

            return script;
            
        }


    }
}
