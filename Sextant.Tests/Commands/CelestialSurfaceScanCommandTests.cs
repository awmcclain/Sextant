﻿// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using System.Linq;
using Sextant.Domain;
using Sextant.Domain.Commands;
using Sextant.Domain.Entities;
using Sextant.Infrastructure.Repository;
using Sextant.Tests.Builders;
using System.Collections.Generic;
using Xunit;
using Sextant.Domain.Phrases;

namespace Sextant.Tests.Commands
{
    public class CelestialSurfaceScanCommandTests : CommandTestBase
    {
        private CelestialSurfaceScanCommand CreateSut(Navigator navigator, PlayerStatusRepository playerStatusRepository, CelestialScanPhrases phrases) 
            => new CelestialSurfaceScanCommand(CreateCommunicator(), navigator, playerStatusRepository, phrases);

        private CelestialData CreateCelestialData(string name)
            => new CelestialData() { Name = name, FSS = _FSSValue, FSSPlusDSS = _DSSValue };
        private CelestialValues CreateCelestialValues()
        {
            return new CelestialValues() { 
                CelestialData = new Dictionary<string, CelestialData>() {
                    { "Data1", CreateCelestialData("Data1Name") },
                    { "Data2", CreateCelestialData("Data2Name") }
                },
                EfficiencyMultiplier = _EfficiencyMultiplier
            };
        }
        private const string _payloadKey = "BodyName";
        private const string _payloadProbe = "ProbesUsed";
        private const string _payloadTarget = "EfficiencyTarget";
        private const int _FSSValue = 10;
        private const int _DSSValue = 20;
        private const float _EfficiencyMultiplier = 1.25f;

        private CelestialScanPhrases _phrases = TestPhraseBuilder.Build<CelestialScanPhrases>();

        [Fact]
        public void SurfaceScanEvent_WithLastCelestialScanned_ShouldUpdateDataStore()
        {
            IDataStore<StarSystemDocument> dataStore = CreateDataStore();
            Navigator navigator                      = CreateNavigator(dataStore);
            PlayerStatusRepository playerStatus      = CreatePlayerStatusRepository();
            CelestialSurfaceScanCommand sut          = CreateSut(navigator, playerStatus, _phrases);
            Celestial celestial                      = Build.A.Celestial;
            StarSystem currentSystem                 = Build.A.StarSystem.WithCelestial(celestial);
            StarSystem nextSystem                    = Build.A.StarSystem.WithCelestial(Build.A.Celestial);
            List<StarSystem> starSystems             = Build.Many.StarSystems(currentSystem, nextSystem);

            navigator.PlanExpedition(starSystems);
            playerStatus.SetLocation(currentSystem.Name);

            TestEvent testEvent = Build.An.Event.WithEvent(sut.SupportedCommand)
                                                .WithPayload(_payloadKey, celestial.Name);

            sut.Handle(testEvent);

            dataStore.FindOne(s => s.Name == currentSystem.Name).Celestials.Single().SurfaceScanned.Should().BeTrue();
            dataStore.FindOne(s => s.Celestials.Any(c => c.SurfaceScanned == false)).Name.Should().Be(nextSystem.Name);
        }

        [Fact]
        public void SurfaceScanEvent_WithCelestialsRemaining_ShouldUpdateDataStore()
        {
            TestPhraseBuilder.Build<CelestialScanPhrases>();

            IDataStore<StarSystemDocument> dataStore = CreateDataStore();
            Navigator navigator                      = CreateNavigator(dataStore);
            PlayerStatusRepository playerStatus      = CreatePlayerStatusRepository();
            CelestialSurfaceScanCommand sut          = CreateSut(navigator, playerStatus, _phrases);

            Celestial celestial                      = Build.A.Celestial;
            Celestial nextCelestial                  = Build.A.Celestial;
            StarSystem currentSystem                 = Build.A.StarSystem.WithCelestials(celestial, nextCelestial);
            List<StarSystem> starSystems             = Build.Many.StarSystems(currentSystem);

            navigator.PlanExpedition(starSystems);
            playerStatus.SetLocation(currentSystem.Name);

            TestEvent testEvent = Build.An.Event.WithEvent(sut.SupportedCommand)
                                                .WithPayload(_payloadKey, celestial.Name);

            sut.Handle(testEvent);

            dataStore.FindOne(s => s.Name == currentSystem.Name).Celestials.Single(c => c.Name == celestial.Name).SurfaceScanned.Should().BeTrue();
            dataStore.FindOne(s => s.Celestials.Any(c => c.SurfaceScanned == false)).Name.Should().Be(currentSystem.Name);
        }

        [Fact]
        public void SurfaceScanEvent_WithSameCelestialScanned_ShouldDoNothing()
        {
            IDataStore<StarSystemDocument> dataStore  = CreateDataStore();
            Navigator navigator                       = CreateNavigator(dataStore);
            PlayerStatusRepository playerStatus       = CreatePlayerStatusRepository();
            CelestialSurfaceScanCommand sut           = CreateSut(navigator, playerStatus, _phrases);

            Celestial celestial                      = Build.A.Celestial;
            StarSystem currentSystem                 = Build.A.StarSystem.WithCelestial(celestial);
            List<StarSystem> starSystems             = Build.Many.StarSystems(currentSystem);

            navigator.PlanExpedition(starSystems);
            playerStatus.SetLocation(currentSystem.Name);

            TestEvent testEvent = Build.An.Event.WithEvent(sut.SupportedCommand)
                                                .WithPayload(_payloadKey, celestial.Name);

            sut.Handle(testEvent);

            var storedSystem = dataStore.FindOne(s => s.Name == currentSystem.Name);

            storedSystem.SurfaceScanned.Should().BeTrue();
            storedSystem.Celestials.Single().SurfaceScanned.Should().BeTrue();
        }

        [Fact]
        public void SurfaceScanEvent_WithSystemNotNextInSequence_ShouldUpdateCorrectSystem()
        {
            IDataStore<StarSystemDocument> dataStore = CreateDataStore();
            Navigator navigator                      = CreateNavigator(dataStore);
            PlayerStatusRepository playerStatus      = CreatePlayerStatusRepository();
            CelestialSurfaceScanCommand sut          = CreateSut(navigator, playerStatus, _phrases);
            Celestial celestial                      = Build.A.Celestial;
            StarSystem firstSystem                   = Build.A.StarSystem.WithCelestial(Build.A.Celestial);
            StarSystem secondSystem                  = Build.A.StarSystem.WithCelestial(celestial);
            StarSystem thirdSystem                   = Build.A.StarSystem.WithCelestial(celestial);
            List<StarSystem> starSystems             = Build.Many.StarSystems(firstSystem, secondSystem, thirdSystem);

            navigator.PlanExpedition(starSystems);
            playerStatus.SetLocation(secondSystem.Name);

            TestEvent testEvent = Build.An.Event.WithEvent(sut.SupportedCommand)
                                                .WithPayload(_payloadKey, celestial.Name);

            sut.Handle(testEvent);

            StarSystemDocument firstSystemStored  = dataStore.FindOne(s => s.Name == firstSystem.Name);
            StarSystemDocument secondSystemStored = dataStore.FindOne(s => s.Name == secondSystem.Name);
            StarSystemDocument thirdSystemStored  = dataStore.FindOne(s => s.Name == thirdSystem.Name);

            firstSystemStored.SurfaceScanned.Should().BeFalse();
            firstSystemStored.Celestials.All(c => c.SurfaceScanned == false).Should().BeTrue();

            secondSystemStored.SurfaceScanned.Should().BeTrue();
            secondSystemStored.Celestials.All(c => c.SurfaceScanned).Should().BeTrue();

            thirdSystemStored.SurfaceScanned.Should().BeFalse();
            thirdSystemStored.Celestials.All(c => c.SurfaceScanned == false).Should().BeTrue();
        }

        [Fact]
        public void ScannedEvent_WithProbesLessThanTarget_ShouldUpdateEfficient_True()
        {
            IDataStore<StarSystemDocument> dataStore = CreateDataStore();
            Navigator navigator                      = CreateNavigator(dataStore);
            PlayerStatusRepository playerStatus       = CreatePlayerStatusRepository();
            CelestialSurfaceScanCommand sut           = CreateSut(navigator, playerStatus, _phrases);

            Celestial celestial                      = Build.A.Celestial;
            StarSystem currentSystem                 = Build.A.StarSystem.WithCelestial(celestial);
            List<StarSystem> starSystems             = Build.Many.StarSystems(currentSystem);

            navigator.PlanExpedition(starSystems);
            playerStatus.SetLocation(currentSystem.Name);

            TestEvent testEvent = Build.An.Event.WithEvent(sut.SupportedCommand)
                                                .WithPayload(_payloadKey, celestial.Name)
                                                .WithPayload(_payloadProbe, 5)
                                                .WithPayload(_payloadTarget, 7);

            sut.Handle(testEvent);

            dataStore.FindOne(s => s.Name == currentSystem.Name).Celestials.Single().Efficient.Should().BeTrue();
        }

        [Fact]
        public void ScannedEvent_WithProbesGreaterThanTarget_ShouldUpdateEfficient_False()
        {
            IDataStore<StarSystemDocument> dataStore = CreateDataStore();
            Navigator navigator                      = CreateNavigator(dataStore);
            PlayerStatusRepository playerStatus       = CreatePlayerStatusRepository();
            CelestialSurfaceScanCommand sut           = CreateSut(navigator, playerStatus, _phrases);

            Celestial celestial                      = Build.A.Celestial;
            StarSystem currentSystem                 = Build.A.StarSystem.WithCelestial(celestial);
            List<StarSystem> starSystems             = Build.Many.StarSystems(currentSystem);

            navigator.PlanExpedition(starSystems);
            playerStatus.SetLocation(currentSystem.Name);

            TestEvent testEvent = Build.An.Event.WithEvent(sut.SupportedCommand)
                                                .WithPayload(_payloadKey, celestial.Name)
                                                .WithPayload(_payloadProbe, 8)
                                                .WithPayload(_payloadTarget, 4);

            sut.Handle(testEvent);

            dataStore.FindOne(s => s.Name == currentSystem.Name).Celestials.Single().Efficient.Should().BeFalse();
        }

    }
}