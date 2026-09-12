#!/usr/bin/env pwsh
# Builds Supply Alert and stages the compiled assembly into Mod/Scripts, ready to be
# zipped/uploaded as-is or copied into %USERPROFILE%\Documents\Timberborn\Mods\SupplyAlert.
param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot

dotnet build "$root/Source/SupplyAlert/SupplyAlert.csproj" -c $Configuration
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

New-Item -ItemType Directory -Force -Path "$root/Mod/Scripts" | Out-Null
Copy-Item "$root/Source/SupplyAlert/bin/$Configuration/SupplyAlert.dll" `
    "$root/Mod/Scripts/SupplyAlert.dll" -Force

Write-Host "Staged Mod/Scripts/SupplyAlert.dll ($Configuration). The Mod/ folder is now a" `
    "complete, installable copy of the mod."
