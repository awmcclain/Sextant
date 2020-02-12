﻿// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Entities;
using Sextant.Domain.Events;
using Sextant.Domain.Phrases;
using System.Collections.Generic;
using System.Linq;

using Serilog.Log;

namespace Sextant.Domain.Commands
{
    public class JumpCommand : ICommand
    {
        public string SupportedCommand => "StartJump";
        public bool Handles(IEvent @event) => @event.Event == SupportedCommand;

        private readonly ICommunicator _communicator;
        private readonly INavigator _navigator;
        private readonly ILogger _logger;
        private readonly CelestialValues _values;
        private readonly string _isPhrase;
        private readonly string _arePhrase;
        private readonly string _andPhrase;
        private readonly string _pluralPhrase;

        private readonly PhraseBook _jumpPhraseBook;
        private readonly PhraseBook _skipPhraseBook;
        private readonly PhraseBook _scanPhraseBook;
        private readonly PhraseBook _alreadyScannedBook;
        private readonly PhraseBook _systemValueBook;

        private readonly bool _communicateSkippableSystems, _onlyCommunicateDuringExpedition;

        private bool ShouldCommunicate
        {
            get {
                if (_onlyCommunicateDuringExpedition) {
                    if (!_navigator.ExpeditionStarted || _navigator.ExpeditionComplete) {
                        return false;
                    }
                }
                return true;
            }
        }
        public JumpCommand(ICommunicator communicator, INavigator navigator, JumpPhrases jumpPhrases, Preferences preferences, CelestialValues values, ILogger logger)
        {
            _communicator                = communicator;
            _navigator                   = navigator;
            _values                      = values;
            _logger                      = logger;

            _isPhrase                    = jumpPhrases.IsPhrase;
            _arePhrase                   = jumpPhrases.ArePhrase;
            _andPhrase                   = jumpPhrases.AndPhrase;
            _pluralPhrase                 = jumpPhrases.PluralPhrase;

            _jumpPhraseBook              = PhraseBook.Ingest(jumpPhrases.Jumping);
            _skipPhraseBook              = PhraseBook.Ingest(jumpPhrases.Skipping);
            _scanPhraseBook              = PhraseBook.Ingest(jumpPhrases.Scanning);
            _alreadyScannedBook          = PhraseBook.Ingest(jumpPhrases.AlreadyScanned);
            _systemValueBook             = PhraseBook.Ingest(jumpPhrases.SystemValue);

            _communicateSkippableSystems =  preferences.CommunicateSkippableSystems;
            _onlyCommunicateDuringExpedition = preferences.OnlyCommunicateDuringExpedition;
        }


        public void Handle(IEvent @event)
        {
            Dictionary<string, object> payload = @event.Payload;

            if (payload.ContainsKey("JumpType") && payload["JumpType"].ToString() == "Supercruise")
                return;

            string systemName = payload["StarSystem"].ToString();

            if (ShouldCommunicate) {
                _communicator.Communicate(string.Format(_jumpPhraseBook.GetRandomPhrase(), systemName));
            }

            StarSystem system = _navigator.GetSystem(systemName);

            if (system == null)
            {
                if (_communicateSkippableSystems && ShouldCommunicate) {
                    _communicator.Communicate(_skipPhraseBook.GetRandomPhrase());
                }

                return;
            }
            if (system.Celestials.All(c => c.Scanned))
            {
                if (_communicateSkippableSystems && ShouldCommunicate) {
                    _communicator.Communicate(_alreadyScannedBook.GetRandomPhrase());
                }
                return;
            }

            string script = BuildScanScript(system);

            if (ShouldCommunicate) {
                _communicator.Communicate(script);
            }

            script = BuildSystemValueScript(system);

            if (ShouldCommunicate) {
                _communicator.Communicate(script);
            }
        }

        private string BuildScanScript(StarSystem system)
        {
            string script = string.Format(_scanPhraseBook.GetRandomPhrase(), system.Celestials.Count(), PhraseBook.PluralizedEnding(system.Celestials.Count(), _pluralPhrase));

            var celestialsByCategory = system.Celestials
                                             .Where(c => !c.Scanned)
                                             .GroupBy(c => c.Classification)
                                             .ToDictionary(grp => grp.Key, grp => grp.Count());

            int counter = 0;

            bool single = celestialsByCategory.First().Value == 1;

            Log.Debug("Single celestial: {@single} {@celestialsByCategory}", single, celestialsByCategory);
            script += single ? $"{_isPhrase} " : $"{_arePhrase} ";

            foreach (var item in celestialsByCategory)
            {
                counter++;

                if (counter == celestialsByCategory.Count() && celestialsByCategory.Count() > 1)
                    script += $"{_andPhrase} ";

                script += $"{item.Value} {_values.NameFromClassification(item.Key)}";

                script += PhraseBook.PluralizedEnding(item.Value, _pluralPhrase);

                script += ", ";
            }

            return script;
        }

        private string BuildSystemValueScript(StarSystem system)
        {
            var scanOnlyValue  = system.Celestials
                                       .Where(c => !c.Scanned)
                                       .Sum(c => _values.ScanValue(c.Classification));

            var totalValue  = system.Celestials
                                    .Where(c => !c.Scanned)
                                    .Sum(c => _values.SurfaceScanValue(c.Classification));

            return _systemValueBook.GetRandomPhraseWith(totalValue.ToSpeakableString(), scanOnlyValue.ToSpeakableString());
        }

    }
}
