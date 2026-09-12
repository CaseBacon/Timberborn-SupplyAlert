using System.Linq;
using GoodStatistics.Analytics;
using Timberborn.Goods;
using Timberborn.ResourceCountingSystem;

namespace SupplyAlert.Monitoring {

  // Unlike Water, "Food" is not a single good in Timberborn - it's a GoodGroup made up of
  // many interchangeable edible goods (Berries, Bread, GrilledPotato, ...), each sampled and
  // trended independently by Goods Statistics. It exposes no aggregate "Food" trend, and
  // Goods Statistics' own EMA analysis is internal, so rebuilding it isn't an option.
  //
  // Smallest clean integration available: read each food good's already-computed trend
  // (GoodTrendsRegistry.GetTrend, from Goods Statistics) and its current stock (via the core
  // Timberborn ResourceCountingService, which Goods Statistics itself is built on), then
  // combine them into one colony-wide days-remaining figure:
  //   totalStock = sum of current stock across all food goods
  //   totalDepletionPerDay = sum of (stock / daysLeft) for goods currently trending as depleting
  //   daysRemaining = totalStock / totalDepletionPerDay
  // Non-depleting goods still count towards totalStock (they are real food on hand) but
  // contribute nothing to the depletion rate, which is a deliberately conservative choice:
  // ignoring their growth cannot cause us to under-warn.
  internal class FoodSupplyEstimator {

    private const string FoodGoodGroupId = "Food";

    private readonly IGoodService _goodService;
    private readonly ResourceCountingService _resourceCountingService;

    public FoodSupplyEstimator(IGoodService goodService,
                               ResourceCountingService resourceCountingService) {
      _goodService = goodService;
      _resourceCountingService = resourceCountingService;
    }

    public SupplyEstimate Estimate(GoodTrendsRegistry goodTrendsRegistry) {
      var foodGoodIds = _goodService.GetGoodsForGroup(FoodGoodGroupId).ToList();
      if (foodGoodIds.Count == 0) {
        return SupplyEstimate.Safe;
      }

      var totalStock = 0f;
      var totalDepletionPerDay = 0f;
      foreach (var goodId in foodGoodIds) {
        var stock = _resourceCountingService.GetGlobalResourceCount(goodId).AvailableStock;
        totalStock += stock;
        if (stock <= 0) {
          continue;
        }
        var trend = goodTrendsRegistry.GetTrend(goodId);
        if (trend.TrendType.IsDepleting() && trend.DaysLeft > 0) {
          totalDepletionPerDay += stock / trend.DaysLeft;
        }
      }

      if (totalStock <= 0) {
        return SupplyEstimate.Depleted;
      }
      return totalDepletionPerDay > 0
          ? SupplyEstimate.At(totalStock / totalDepletionPerDay)
          : SupplyEstimate.Safe;
    }

  }

}
