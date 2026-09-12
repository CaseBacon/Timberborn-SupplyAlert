using System;
using GoodStatistics.Analytics;
using GoodStatistics.Sampling;
using SupplyAlert.Settings;
using Timberborn.Localization;
using Timberborn.QuickNotificationSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace SupplyAlert.Monitoring {

  // Re-evaluates food/water depletion on every GoodsSampledEvent - the same event Goods
  // Statistics posts each time it finishes a sampling pass. Piggybacking on that cadence means
  // we automatically respect its sampling rate and pause behaviour instead of polling ourselves.
  internal class SupplyMonitor : ILoadableSingleton {

    private const string LowFoodLocKey = "CaseBacon.SupplyAlert.LowFoodWarning";
    private const string LowWaterLocKey = "CaseBacon.SupplyAlert.LowWaterWarning";

    private readonly EventBus _eventBus;
    private readonly GlobalGoodTrendsRegistry _globalGoodTrendsRegistry;
    private readonly SupplyAlertSettings _settings;
    private readonly FoodSupplyEstimator _foodSupplyEstimator;
    private readonly WaterSupplyEstimator _waterSupplyEstimator;
    private readonly QuickNotificationService _quickNotificationService;
    private readonly ILoc _loc;
    private readonly SupplyAlertState _foodState = new();
    private readonly SupplyAlertState _waterState = new();

    public SupplyMonitor(EventBus eventBus,
                         GlobalGoodTrendsRegistry globalGoodTrendsRegistry,
                         SupplyAlertSettings settings,
                         FoodSupplyEstimator foodSupplyEstimator,
                         WaterSupplyEstimator waterSupplyEstimator,
                         QuickNotificationService quickNotificationService,
                         ILoc loc) {
      _eventBus = eventBus;
      _globalGoodTrendsRegistry = globalGoodTrendsRegistry;
      _settings = settings;
      _foodSupplyEstimator = foodSupplyEstimator;
      _waterSupplyEstimator = waterSupplyEstimator;
      _quickNotificationService = quickNotificationService;
      _loc = loc;
    }

    public void Load() {
      _eventBus.Register(this);
    }

    [OnEvent]
    public void OnGoodsSampled(GoodsSampledEvent goodsSampledEvent) {
      Evaluate();
    }

    private void Evaluate() {
      // Null until Goods Statistics' own singleton has loaded; every subsequent sample event
      // arrives well after that, but this keeps a load-order hiccup from ever throwing.
      var goodTrendsRegistry = _globalGoodTrendsRegistry.GoodTrendsRegistry;
      if (goodTrendsRegistry == null) {
        return;
      }
      EvaluateSupply(SupplyKind.Food, _foodState, _settings.FoodAlertsEnabled.Value,
                    _settings.FoodThresholdDays.Value, LowFoodLocKey,
                    () => _foodSupplyEstimator.Estimate(goodTrendsRegistry));
      EvaluateSupply(SupplyKind.Water, _waterState, _settings.WaterAlertsEnabled.Value,
                    _settings.WaterThresholdDays.Value, LowWaterLocKey,
                    () => _waterSupplyEstimator.Estimate(goodTrendsRegistry));
    }

    private void EvaluateSupply(SupplyKind supplyKind, SupplyAlertState state, bool alertsEnabled,
                                float thresholdDays, string locKey,
                                Func<SupplyEstimate> estimate) {
      if (!alertsEnabled) {
        state.Reset();
        return;
      }

      SupplyEstimate result;
      try {
        result = estimate();
      } catch (Exception exception) {
        Debug.LogWarning(
            $"[SupplyAlert] Could not evaluate {supplyKind} supply, skipping this check: "
            + exception.Message);
        return;
      }

      if (state.ShouldAlert(result.DaysRemaining, thresholdDays)) {
        var daysText = result.DaysRemaining.GetValueOrDefault().ToString("0.0");
        _quickNotificationService.SendWarningNotification(_loc.T(locKey, daysText));
      }
    }

  }

}
