// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Phrases;

namespace Sextant.Domain.Commands
{
    public class DockSRVCommand : ResponseCommandBase
    {
        internal override string _supportedCommand => "DockSRV";

        internal override PhraseBook _phraseBook { get; }

        public DockSRVCommand(ICommunicator communicator, DockSRVPhrases phrases, Preferences preferences)
            : base(communicator)
        {
            if (preferences.EnableSRVCommands) {
                _phraseBook = PhraseBook.Ingest(phrases.Phrases);
            } else {
                _phraseBook = PhraseBook.Ingest(new string[] {string.Empty});
            }
        }
    }
}
