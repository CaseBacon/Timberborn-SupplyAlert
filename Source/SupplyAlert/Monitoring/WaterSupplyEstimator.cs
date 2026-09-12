using GoodStatistics.Analytics;
using Timberborn.Goods;
using Timberborn.ResourceCountingSystem;

namespace SupplyAlert.Monitoring {

  // Water is a single, dedicated good in vanilla Timberborn (GoodGroup "Water" with
  // SingleResourceGroup = true), so Goods Statistics' own per-good trend for it already
  // is the colony-wide water depletion estimate - no aggregation needed.
  internal class WaterSupplyEstimator {

    private const string WaterGoodId = "Water";

    private readonly IGoodService _goodService;
    private readonly ResourceCountingService _resourceCountingService;

    public WaterSupplyEstimator(IGoodService goodService,
                                ResourceCountingService resourceCountingService) {
      _goodService = goodService;
      _resourceCountingService = resourceCountingService;
    }

    public SupplyEstimate Estimate(GoodTrendsRegistry goodTrendsRegistry) {
      if (!_goodService.HasGood(WaterGoodId)) {
        return SupplyEstimate.Safe;
      }
      if (_resourceCountingService.GetGlobalResourceCount(WaterGoodId).AvailableStock <= 0) {
        return SupplyEstimate.Depleted;
      }
      var trend = goodTrendsRegistry.GetTrend(WaterGoodId);
      return trend.TrendType.IsDepleting() && trend.DaysLeft > 0
          ? SupplyEstimate.At(trend.DaysLeft)
          : SupplyEstimate.Safe;
    }

  }

}
