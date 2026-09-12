# lib/

This folder is where you provide the compiled assemblies of Supply Alert's two required
dependencies so the project can compile against their real, released APIs. **Nothing here is
committed to the repository or redistributed** - see the licensing note in the root
[README](../README.md#license--credits).

Populate it like this:

```
lib/
  GoodStatistics/
    GoodStatistics.Analytics.dll
    GoodStatistics.Sampling.dll
  ModSettings/
    ModSettings.Core.dll
```

The easiest way to get these files: subscribe to [Goods Statistics] and [Mod Settings] on the
Steam Workshop (or mod.io), then copy the DLLs out of their installed mod folder, e.g. on
Windows:

```
%ProgramFiles(x86)%\Steam\steamapps\workshop\content\1062090\3321521358\version-<latest>\Scripts\
%ProgramFiles(x86)%\Steam\steamapps\workshop\content\1062090\3283831040\version-<latest>\Scripts\
```

Alternatively, build them yourself from [eMkaQQ's timberborn-modding source](
https://github.com/eMkaQQ/timberborn-modding).

If you'd rather not copy files here, point the build at your own copies instead by setting the
`GOODSTATISTICS_LIB_DIR` and `MODSETTINGS_LIB_DIR` environment variables to the folders that
contain those DLLs - see [SupplyAlert.csproj](../Source/SupplyAlert/SupplyAlert.csproj).

[Goods Statistics]: https://steamcommunity.com/workshop/filedetails/?id=3321521358
[Mod Settings]: https://github.com/eMkaQQ/timberborn-modding
