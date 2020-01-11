// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain;
using Sextant.Domain.Commands;
using Sextant.Domain.Phrases;
using Sextant.Tests.Builders;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace Sextant.Tests.Commands
{
    public class DockSRVCommandTests : CommandTestBase
    {
        [Fact]
        public void DockSRV_Command_Should_Communicate_Phrase()
        {
            var preferences               = new Preferences() { EnableSRVCommands = true };
            TestCommunicator communicator = CreateCommunicator();
            DockSRVPhrases phrases        = TestPhraseBuilder.Build<DockSRVPhrases>();
            DockSRVCommand sut            = new DockSRVCommand(communicator, phrases, preferences);

            TestEvent loadEvent = Build.An.Event.WithEvent("DockSRV");

            sut.Handle(loadEvent);

            communicator.MessagesCommunicated.Single().Should().Be(phrases.Phrases.Single());
        }

        [Fact]
        public void DockSRV_Command_Should_Not_Communicate_Phrase_When_Disabled()
        {
            var preferences               = new Preferences() { EnableSRVCommands = false };
            TestCommunicator communicator = CreateCommunicator();
            DockSRVPhrases phrases        = TestPhraseBuilder.Build<DockSRVPhrases>();
            DockSRVCommand sut            = new DockSRVCommand(communicator, phrases, preferences);

            TestEvent loadEvent = Build.An.Event.WithEvent("DockSRV");

            sut.Handle(loadEvent);

            communicator.MessagesCommunicated.Single().Should().BeEmpty();
        }
    }
}
