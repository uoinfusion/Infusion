#!/usr/bin/env pwsh
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Run {
    param(
        [Parameter(Mandatory = $true)][string]$Command,
        [string[]]$Args = @(),
        [string]$WorkingDirectory = $null
    )

    $display = "$Command $($Args -join ' ')"
    Write-Host ">>> $display" -ForegroundColor Cyan

    if ($WorkingDirectory) { Push-Location $WorkingDirectory }
    try {
        & $Command @Args
        if ($LASTEXITCODE -ne 0) {
            throw "Command failed with exit code ${LASTEXITCODE}: $display"
        }
    }
    finally {
        if ($WorkingDirectory) { Pop-Location }
    }
}

$repoRoot = (Resolve-Path $PSScriptRoot).Path
$buildRoot = Join-Path $repoRoot "build"

Push-Location $buildRoot
try {
    # Clean
    $dirsToClean = @("output", "testresults", "ExampleScripts")
    foreach ($dir in $dirsToClean) {
        $path = Join-Path $buildRoot $dir
        if (Test-Path $path) {
            Remove-Item $path -Recurse -Force
        }
    }

    # Build
    Run "dotnet" @("build", "..\Infusion.sln", "-c", "Release", "/p:RuntimeIdentifiers=win-x64 ")

    New-Item -ItemType Directory -Force -Path (Join-Path $buildRoot "testresults") | Out-Null
    
    Run "dotnet" @("test", "..\Infusion.Tests", "--logger", "trx;LogFileName=$(Join-Path $buildRoot 'testresults\Infusion.Tests.trx')")
    Run "dotnet" @("test", "..\Infusion.LegacyApi.Tests", "--logger", "trx;LogFileName=$(Join-Path $buildRoot 'testresults\Infusion.LegacyApi.Tests.trx')")

    $uoePath = "..\ExampleScripts\UOErebor\Infusion.Scripts.UOErebor.Extensions"
    Run "dotnet" @("build", "$uoePath\Infusion.Scripts.UOErebor.Extensions.sln", "-c", "Release", "/p:RuntimeIdentifiers=win-x64")

    # PreparePackage
    $releaseRoot = Join-Path $buildRoot "output\release"
    $releaseBin = Join-Path $releaseRoot "bin"
    $releaseScripts = Join-Path $releaseRoot "scripts"

    foreach ($dir in @($releaseRoot, $releaseBin, (Join-Path $releaseRoot "logs"), (Join-Path $releaseRoot "profiles"), $releaseScripts, (Join-Path $buildRoot "testresults"))) {
        New-Item -ItemType Directory -Force -Path $dir | Out-Null
    }

    $launcherFile = Join-Path $buildRoot "..\Infusion.Launcher\bin\release\Infusion.exe"
    if (Test-Path $launcherFile) {
        Copy-Item -Path $launcherFile -Destination $releaseRoot -Force
    }
    else {
        throw "Launcher file not found: $launcherFile"
    }

    $binPath = Join-Path $buildRoot "..\Infusion.Desktop\bin\release\net8.0-windows"
    if (Test-Path $binPath) {
        Copy-Item -Path (Join-Path $binPath "*") -Destination $releaseBin -Recurse -Force
    }
    else {
        throw "Bin path not found: $binPath"
    }

    $exampleScriptsRoot = Join-Path $buildRoot "ExampleScripts"
    Run "git" @("checkout-index", "-a", "-f", "--prefix", "${buildRoot}\") -WorkingDirectory (Join-Path $buildRoot "..\ExampleScripts")

    if (Test-Path $exampleScriptsRoot) {
        Copy-Item -Path (Join-Path $exampleScriptsRoot "*") -Destination $releaseScripts -Recurse -Force
    }
    else {
        throw "Example scripts were not prepared at $exampleScriptsRoot"
    }

    $uoeBinRoot = Join-Path $buildRoot "..\ExampleScripts\UOErebor\Infusion.Scripts.UOErebor.Extensions\Infusion.Scripts.UOErebor.Extensions\bin\Release"
    $uoeExtensionDll = Get-ChildItem -Path $uoeBinRoot -Recurse -Filter "Infusion.Scripts.UOErebor.Extensions.dll" -File | Sort-Object LastWriteTime -Descending | Select-Object -First 1
    $destUO = Join-Path $releaseScripts "UOErebor"
    New-Item -ItemType Directory -Force -Path $destUO | Out-Null
    if ($uoeExtensionDll) {
        Copy-Item -Path $uoeExtensionDll.FullName -Destination $destUO -Force
    }
    else {
        throw "Extension binary not found under $uoeBinRoot"
    }

    Run "dotnet" @("pack", "..\Infusion.Cli\Infusion.Cli.csproj", "--configuration", "Release", "-p:", "TargetFramework=net8.0", "--output", "output")
}
finally {
    Pop-Location
}
