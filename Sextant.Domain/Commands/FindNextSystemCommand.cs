// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Sextant.Domain.Events;
using Sextant.Domain.Entities;
using Sextant.Domain.Phrases;

namespace Sextant.Domain.Commands
{
    public class FindNextSystemCommand : ICommand
    {
        private readonly ICommunicator _communicator;
        private readonly INavigator _navigator;
        private readonly IGalaxyMap _galaxyMap;
        private readonly IPlayerStatus _playerStatus;

        public string SupportedCommand => "find_next_system";
        public bool Handles(IEvent @event) => @event.Event == SupportedCommand;

        private PhraseBook _phraseBook, _finalDestination;

        public FindNextSystemCommand(ICommunicator communicator, INavigator navigator, IGalaxyMap galaxyMap, FindNextSystemPhrases phrases, IPlayerStatus playerStatus)
        {
            _communicator = communicator;
            _navigator    = navigator;
            _galaxyMap    = galaxyMap;
            _playerStatus = playerStatus;

            _phraseBook   = PhraseBook.Ingest(phrases.Phrases);
            _finalDestination = PhraseBook.Ingest(phrases.FinalDestination);
        }

        public void Handle(IEvent @event)
        {

            StarSystem nextSystem = _navigator.GetNextSystem();
            string nextSystemName;

            if (nextSystem == null && !String.IsNullOrEmpty(_playerStatus.Destination)) {
                _communicator.Communicate(_finalDestination.GetRandomPhrase());
                nextSystemName = _playerStatus.Destination;
            } else {
                _communicator.Communicate(_phraseBook.GetRandomPhrase());
                nextSystemName = nextSystem.Name;
            }

            _galaxyMap.FindSystem(nextSystemName);
        }
    }
}
