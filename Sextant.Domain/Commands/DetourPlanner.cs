﻿// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Sextant.Domain.Events;
using System.Collections.Generic;
using Sextant.Domain.Entities;
using Sextant.Domain.Phrases;

namespace Sextant.Domain
{
    public class DetourPlanner : IDetourPlanner
    {
        private const int _defaultDetourAmount = 30;
        private const int _detourMax = 50;
        private const int _detourMin = 10;

        private readonly IDetourDataService _detourDataService;
        private readonly IPlayerStatus _playerStatus;
        private readonly ILogger _logger;
        private readonly INavigator _navigator;
    
        private int _detourAmount;
        private IEnumerable<StarSystem> _detourData;

        public int DetourAmount => _detourAmount;
        public bool DetourPlanned => _detourData != null;

        public void IncreaseDetourAmount() {
            _detourAmount += 5;
            if (_detourAmount > _detourMax)
                _detourAmount = _detourMax;
        }

        public void DecreaseDetourAmount() {
            _detourAmount -= 5;
            if (_detourAmount < _detourMin)
                _detourAmount = _detourMin;
        }
        
        public int SystemsInDetour => _detourData == null ? 0 : _detourData.Count();
        public int PlanetsInDetour
        {
            get {
                if (_detourData == null) {
                    return 0;
                }

                return _detourData.Sum(s => s.Celestials.Count());
            }
        }

        public DetourPlanner(INavigator navigator, IDetourDataService detourDataService, IPlayerStatus playerStatus, ILogger logger)
        {
            _navigator         = navigator;
            _detourDataService = detourDataService;
            _playerStatus      = playerStatus;
            _logger            = logger;

        }

        public bool PlanDetour()
        {
            // Get the start and destination
            if (String.IsNullOrEmpty(_playerStatus.Location)) {
                _logger.Error("No location known, can't find detour");
                return false;
            }

            if (String.IsNullOrEmpty(_playerStatus.Destination)) {
                _logger.Error("No destination found, can't plot detour");
                return false;
            }

            _logger.Information("Searching for detour...");

            // try...catch here?
            _detourData = _detourDataService.GetExpeditionData(_playerStatus.Location, _playerStatus.Destination, _detourAmount);
            if (_detourData == null) {
                return false;
            }

            return true;
        }

        public bool ConfirmDetour()
        {
            if (_detourData == null) {
                return false;
            }

            if (_detourData.Count() == 0) {
                return false;
            }

            bool result = _navigator.PlanExpedition(_detourData);
            _detourData = null;
            return result;
        }



    }
}
