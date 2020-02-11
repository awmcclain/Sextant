// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Sextant.Domain.Phrases
{
    public class GrammarPhrases
    {
        public string AndPhrase                     { get; set; }
        public string PluralPhrase                  { get; set; }

        public string PluralizePhrase(string phrase, int count) {
            return count == 1 ? phrase : phrase + PluralPhrase;
        }
    }
}