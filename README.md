# Supply Alert

A [Timberborn](https://store.steampowered.com/app/1062090/Timberborn/) mod that warns you when
your colony's **food** or **water** is approaching depletion, with independently configurable
thresholds for each. It never spams: you get one warning when a supply crosses its threshold,
and the alert re-arms once that supply recovers safely above it.

Supply Alert doesn't compute any depletion estimates of its own - it reads them from
[**Goods Statistics**](#dependency-goods-statistics), which is a **required** dependency.

> ```
> Low Food: approximately 0.9 days remaining.
> ```

## Installation

1. Subscribe to and enable **[Goods Statistics]** (Steam Workshop id `3321521358`) and its own
   dependency, **Mod Settings** - both by eMkaQQ.
2. Subscribe to and enable **Supply Alert**.
3. Load into a save (or start a new colony). Supply Alert has no UI of its own beyond its
   settings panel - alerts appear as native Timberborn warning banners when a supply gets low.

Building from source instead? See [Building](#building) below.

## Dependency: Goods Statistics

[Goods Statistics] samples every good's stock over time and estimates, per good, how many days
remain until it depletes (or until it fills up, if it's growing). Supply Alert is a thin layer
on top of that: it reads Goods Statistics' own per-good trend and days-left estimates through
its public API (`GoodTrendsRegistry` / `GoodTrend`, from its `eMka.GoodsStatistics` mod) rather
than re-sampling goods or reimplementing its trend analysis. Supply Alert does not bundle,
modify, or redistribute any part of Goods Statistics - it must be installed and enabled
separately, and this repository does not include its source or binaries.

### Why "Food" isn't a straight read

Water is simple: it's a single good in vanilla Timberborn, so Goods Statistics' own trend for
the `Water` good *is* the colony's water depletion estimate.

Food isn't a single good - it's a whole group of interchangeable edible goods (Berries, Bread,
GrilledPotato, and so on), each sampled and trended independently by Goods Statistics, which
exposes no combined "Food" estimate. Rebuilding its sampling/trend-analysis engine ourselves was
out of scope, so Supply Alert instead combines the pieces Goods Statistics *does* expose:

- for every good in the game's `Food` good group, it reads that good's current stock (via
  Timberborn's own `ResourceCountingService` - the same core service Goods Statistics itself is
  built on) and its trend/days-left (via Goods Statistics' `GoodTrendsRegistry`);
- it sums the stock across all of them, and sums the implied daily depletion rate across
  whichever of them are currently trending downward;
- `days remaining = total stock / total depletion rate`.

Goods that are stable or growing still count toward the stock total but not the depletion rate,
which is a deliberately conservative choice - it can only make the estimate warn *earlier*, never
later. See [`FoodSupplyEstimator.cs`](Source/SupplyAlert/Monitoring/FoodSupplyEstimator.cs) for
the exact logic.

## Settings

Open via the gear icon next to Supply Alert in the mod list (main menu or in-game):

| Setting | Default | Notes |
|---|---|---|
| Enable food alerts | On | |
| Enable water alerts | On | |
| Food threshold (days) | 1.0 | Decimals supported (e.g. `0.5`), and values above 1 |
| Water threshold (days) | 1.0 | Decimals supported (e.g. `0.5`), and values above 1 |

## Alert behaviour

- Food and water are tracked completely independently, each with its own on/off switch,
  threshold, and alert state.
- An alert fires **once**, the moment an estimate crosses from above its threshold to at or
  below it.
- After firing, that supply's alert goes quiet - it will not fire again until the supply
  recovers to a safe margin above the threshold (15% hysteresis) and then drops below it again.
  This avoids repeated pings while a value hovers right at the threshold.
- Nothing fires on load or when a new game starts just because a supply already happens to be
  low - only a live crossing during play triggers a warning.
- Zero stock, ever-increasing stock, no consumption, unavailable/insufficient statistics, and
  missing goods (e.g. a total-conversion modpack without one of these goods) are all treated as
  "not currently at risk" or handled defensively - none of them can crash the mod. Any
  unexpected failure while evaluating one supply is logged once and skipped, and never affects
  the other supply or the rest of the game.
- Alerts are evaluated each time Goods Statistics finishes a sampling pass, so behaviour
  (including staying quiet while paused) follows its own sampling settings.
- State is per-session: a new game or a freshly loaded save starts with a clean slate.

## Compatibility

- Requires Timberborn 1.1.2 or later, and Goods Statistics 1.1.2+ / Mod Settings 1.0.6+.
- Adds no buildings, goods, or permanent UI - only a settings panel and occasional notification
  banners - so it should be compatible with essentially any other mod, including other Goods
  Statistics add-ons.
- Multi-district colonies: Supply Alert currently monitors colony-wide (global) totals, not
  per-district figures.

## Building

Requirements: the [.NET SDK](https://dotnet.microsoft.com/download) and a local Timberborn
install (for its game assemblies).

1. Get the two dependency DLLs into `lib/` - see [lib/README.md](lib/README.md).
2. If Timberborn isn't installed at the default Steam location
   (`C:\Program Files (x86)\Steam\steamapps\common\Timberborn`), set the
   `TIMBERBORN_INSTALL_DIR` environment variable to your install path.
3. Build and stage the mod:

   ```
   ./build.ps1
   ```

   This compiles [`Source/SupplyAlert/SupplyAlert.csproj`](Source/SupplyAlert/SupplyAlert.csproj)
   and copies the resulting DLL into `Mod/Scripts/`. `Mod/` is then a complete, installable copy
   of the mod (manifest, localization, compiled assembly) - copy it to
   `%USERPROFILE%\Documents\Timberborn\Mods\SupplyAlert` for local testing, or zip its contents
   for Steam Workshop / mod.io upload.

No assets are built with Unity - this is a plain C# class library, like most code-only
Timberborn mods.

## Project layout

```
Source/SupplyAlert/     Mod source (C# class library)
  Settings/              Mod Settings integration
  Monitoring/            Depletion estimation + alert state machine
Mod/                     The installable/publishable mod package
  manifest.json
  Localizations/
lib/                     Where you provide the two dependency DLLs (not committed)
```

## License & credits

Supply Alert is released under the [MIT License](LICENSE).

All credit for the underlying goods-depletion analysis goes to **eMkaQQ**'s
[**Goods Statistics**] mod and its companion **Mod Settings** framework
([source](https://github.com/eMkaQQ/timberborn-modding)); Supply Alert only adds alerting on top
of the estimates that mod already computes. Both are separate mods with their own licensing and
must be installed independently - no part of either is redistributed in this repository.

## Contributing / issues

Bug reports, compatibility reports, and pull requests are welcome via
[GitHub Issues](https://github.com/CaseBacon/Timberborn-SupplyAlert/issues).

[Goods Statistics]: https://steamcommunity.com/workshop/filedetails/?id=3321521358
