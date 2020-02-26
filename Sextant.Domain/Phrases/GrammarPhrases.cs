// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;

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

    public static class SpeakableExtensions {
        public static string ToSpeakableString(this int credits) {
            if (credits >= 1000000000) {
                return credits.ToString("0,,.### billion");
            } else if (credits >= 1000000) {
                return credits.ToString("0,,.## million");
            }
            return credits.ToString("0,0");
        }
    }
}