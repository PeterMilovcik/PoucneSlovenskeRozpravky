<#
.SYNOPSIS
    Kontrola a nastavenie prostredia pre Poučné Slovenské Rozprávky.
.DESCRIPTION
    Skontroluje a nastaví všetky prerekvizity:
    1. Kontrola .NET 10 SDK
    2. Kontrola FFmpeg
    3. Kontrola Docker (pre LanguageTool)
    4. Spustenie LanguageTool Docker kontajnera
    5. Kontrola API kľúčov (ELEVENLABS_API_KEY, OPENAI_API_KEY)
    6. Spustenie dotnet restore
    7. Spustenie dotnet build
    Zobrazí jasný ✅/❌ stav pre každú prerekvizitu.
.PARAMETER SkipBuild
    Preskočiť kroky restore a build.
.PARAMETER SkipDocker
    Preskočiť kontrolu a spustenie Docker kontajnera.
.PARAMETER StartLanguageTool
    Automaticky spustiť LanguageTool Docker kontajner ak nebeží.
.EXAMPLE
    .\setup-environment.ps1
    .\setup-environment.ps1 -SkipBuild
    .\setup-environment.ps1 -StartLanguageTool
#>
param(
    [Parameter(HelpMessage = "Preskočiť kroky restore a build")]
    [switch]$SkipBuild,

    [Parameter(HelpMessage = "Preskočiť kontrolu Docker")]
    [switch]$SkipDocker,

    [Parameter(HelpMessage = "Automaticky spustiť LanguageTool ak nebeží")]
    [switch]$StartLanguageTool
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Continue"

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$SolutionFile = Join-Path $ProjectRoot "src" "PoucneRozpravky.sln"

$allPassed = $true
$checkResults = @()

function Add-CheckResult {
    param(
        [string]$Name,
        [bool]$Passed,
        [string]$Detail = ""
    )
    $script:checkResults += [PSCustomObject]@{
        Name   = $Name
        Passed = $Passed
        Detail = $Detail
    }
    if (-not $Passed) { $script:allPassed = $false }
}

# Hlavička
Write-Host ""
Write-Host "╔══════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║    🔧 Kontrola prostredia - Poučné Rozprávky 🔧         ║" -ForegroundColor Cyan
Write-Host "╚══════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# --- 1. Kontrola .NET SDK ---
Write-Host "  [1/7] Kontrola .NET SDK..." -ForegroundColor White

try {
    $dotnetVersion = & dotnet --version 2>&1
    $dotnetExitCode = $LASTEXITCODE

    if ($dotnetExitCode -eq 0 -and $dotnetVersion) {
        $majorVersion = [int]($dotnetVersion.ToString().Split('.')[0])
        if ($majorVersion -ge 10) {
            Write-Host "  ✅ .NET SDK $dotnetVersion" -ForegroundColor Green
            Add-CheckResult ".NET 10 SDK" $true ".NET SDK $dotnetVersion"
        } else {
            Write-Host "  ❌ .NET SDK $dotnetVersion (vyžaduje sa .NET 10+)" -ForegroundColor Red
            Write-Host "     Stiahnite z: https://dotnet.microsoft.com/download/dotnet/10.0" -ForegroundColor DarkGray
            Add-CheckResult ".NET 10 SDK" $false "Nájdená verzia $dotnetVersion, vyžaduje sa 10+"
        }
    } else {
        Write-Host "  ❌ .NET SDK nie je nainštalovaný" -ForegroundColor Red
        Write-Host "     Stiahnite z: https://dotnet.microsoft.com/download/dotnet/10.0" -ForegroundColor DarkGray
        Add-CheckResult ".NET 10 SDK" $false "Nie je nainštalovaný"
    }
} catch {
    Write-Host "  ❌ .NET SDK nie je dostupný: $_" -ForegroundColor Red
    Add-CheckResult ".NET 10 SDK" $false "Chyba: $_"
}

# --- 2. Kontrola FFmpeg ---
Write-Host "  [2/7] Kontrola FFmpeg..." -ForegroundColor White

try {
    $ffmpegOutput = & ffmpeg -version 2>&1 | Select-Object -First 1
    if ($LASTEXITCODE -eq 0 -and $ffmpegOutput) {
        $ffmpegVersion = ($ffmpegOutput -split ' ')[2]
        Write-Host "  ✅ FFmpeg $ffmpegVersion" -ForegroundColor Green
        Add-CheckResult "FFmpeg" $true "FFmpeg $ffmpegVersion"
    } else {
        Write-Host "  ❌ FFmpeg nie je dostupný" -ForegroundColor Red
        Write-Host "     Nainštalujte: winget install Gyan.FFmpeg" -ForegroundColor DarkGray
        Write-Host "     Alebo: choco install ffmpeg" -ForegroundColor DarkGray
        Add-CheckResult "FFmpeg" $false "Nie je nainštalovaný"
    }
} catch {
    Write-Host "  ❌ FFmpeg nie je dostupný" -ForegroundColor Red
    Write-Host "     Nainštalujte: winget install Gyan.FFmpeg" -ForegroundColor DarkGray
    Add-CheckResult "FFmpeg" $false "Nie je nainštalovaný"
}

# --- 3. Kontrola Docker ---
Write-Host "  [3/7] Kontrola Docker..." -ForegroundColor White

if ($SkipDocker) {
    Write-Host "  ⏭️  Docker kontrola preskočená" -ForegroundColor DarkGray
    Add-CheckResult "Docker" $true "Preskočené"
} else {
    try {
        $dockerVersion = & docker --version 2>&1
        if ($LASTEXITCODE -eq 0 -and $dockerVersion) {
            # Kontrola, či Docker daemon beží
            $dockerInfo = & docker info 2>&1
            if ($LASTEXITCODE -eq 0) {
                Write-Host "  ✅ $dockerVersion" -ForegroundColor Green
                Add-CheckResult "Docker" $true "$dockerVersion"
            } else {
                Write-Host "  ⚠️  Docker je nainštalovaný, ale daemon nebeží" -ForegroundColor Yellow
                Write-Host "     Spustite Docker Desktop" -ForegroundColor DarkGray
                Add-CheckResult "Docker" $false "Daemon nebeží"
            }
        } else {
            Write-Host "  ❌ Docker nie je nainštalovaný" -ForegroundColor Red
            Write-Host "     Stiahnite z: https://www.docker.com/products/docker-desktop" -ForegroundColor DarkGray
            Add-CheckResult "Docker" $false "Nie je nainštalovaný"
        }
    } catch {
        Write-Host "  ❌ Docker nie je dostupný: $_" -ForegroundColor Red
        Add-CheckResult "Docker" $false "Chyba: $_"
    }
}

# --- 4. Kontrola LanguageTool Docker kontajnera ---
Write-Host "  [4/7] Kontrola LanguageTool kontajnera..." -ForegroundColor White

if ($SkipDocker) {
    Write-Host "  ⏭️  LanguageTool kontrola preskočená" -ForegroundColor DarkGray
    Add-CheckResult "LanguageTool" $true "Preskočené"
} else {
    $ltRunning = $false
    try {
        # Kontrola, či kontajner beží
        $ltContainers = & docker ps --filter "ancestor=erikvl87/languagetool" --format "{{.Names}}" 2>&1
        if ($LASTEXITCODE -eq 0 -and $ltContainers) {
            $ltRunning = $true
        }

        # Kontrola dostupnosti cez HTTP
        if (-not $ltRunning) {
            try {
                $ltResponse = Invoke-WebRequest -Uri "http://localhost:8010/v2/languages" -TimeoutSec 3 -ErrorAction SilentlyContinue
                if ($ltResponse.StatusCode -eq 200) {
                    $ltRunning = $true
                }
            } catch {
                # LanguageTool nie je dostupný
            }
        }

        if ($ltRunning) {
            Write-Host "  ✅ LanguageTool beží na localhost:8010" -ForegroundColor Green
            Add-CheckResult "LanguageTool" $true "Beží na localhost:8010"
        } else {
            if ($StartLanguageTool) {
                Write-Host "  ⏳ Spúšťam LanguageTool kontajner..." -ForegroundColor Yellow
                & docker run -d --name languagetool -p 8010:8010 erikvl87/languagetool 2>&1 | Out-Null

                # Čakanie na spustenie
                $maxWait = 30
                $waited = 0
                while ($waited -lt $maxWait) {
                    Start-Sleep -Seconds 2
                    $waited += 2
                    try {
                        $ltCheck = Invoke-WebRequest -Uri "http://localhost:8010/v2/languages" -TimeoutSec 2 -ErrorAction SilentlyContinue
                        if ($ltCheck.StatusCode -eq 200) {
                            $ltRunning = $true
                            break
                        }
                    } catch { }
                }

                if ($ltRunning) {
                    Write-Host "  ✅ LanguageTool spustený na localhost:8010" -ForegroundColor Green
                    Add-CheckResult "LanguageTool" $true "Spustený na localhost:8010"
                } else {
                    Write-Host "  ❌ LanguageTool sa nepodarilo spustiť" -ForegroundColor Red
                    Add-CheckResult "LanguageTool" $false "Nepodarilo sa spustiť"
                }
            } else {
                Write-Host "  ❌ LanguageTool nebeží" -ForegroundColor Red
                Write-Host "     Spustite: docker run -d --name languagetool -p 8010:8010 erikvl87/languagetool" -ForegroundColor DarkGray
                Write-Host "     Alebo: .\setup-environment.ps1 -StartLanguageTool" -ForegroundColor DarkGray
                Add-CheckResult "LanguageTool" $false "Nebeží"
            }
        }
    } catch {
        Write-Host "  ❌ Chyba pri kontrole LanguageTool: $_" -ForegroundColor Red
        Add-CheckResult "LanguageTool" $false "Chyba: $_"
    }
}

# --- 5. Kontrola API kľúčov ---
Write-Host "  [5/7] Kontrola API kľúčov..." -ForegroundColor White

# ElevenLabs
$elevenLabsKey = $env:ELEVENLABS_API_KEY
if ($elevenLabsKey) {
    $maskedKey = $elevenLabsKey.Substring(0, [Math]::Min(4, $elevenLabsKey.Length)) + "****"
    Write-Host "  ✅ ELEVENLABS_API_KEY je nastavený ($maskedKey)" -ForegroundColor Green
    Add-CheckResult "ELEVENLABS_API_KEY" $true "Nastavený"
} else {
    Write-Host "  ❌ ELEVENLABS_API_KEY nie je nastavený" -ForegroundColor Red
    Write-Host "     Nastavte: `$env:ELEVENLABS_API_KEY = 'váš-kľúč'" -ForegroundColor DarkGray
    Write-Host "     Alebo pridajte do systémových premenných prostredia" -ForegroundColor DarkGray
    Add-CheckResult "ELEVENLABS_API_KEY" $false "Nie je nastavený"
}

# OpenAI
$openaiKey = $env:OPENAI_API_KEY
if ($openaiKey) {
    $maskedKey = $openaiKey.Substring(0, [Math]::Min(4, $openaiKey.Length)) + "****"
    Write-Host "  ✅ OPENAI_API_KEY je nastavený ($maskedKey)" -ForegroundColor Green
    Add-CheckResult "OPENAI_API_KEY" $true "Nastavený"
} else {
    Write-Host "  ❌ OPENAI_API_KEY nie je nastavený" -ForegroundColor Red
    Write-Host "     Nastavte: `$env:OPENAI_API_KEY = 'váš-kľúč'" -ForegroundColor DarkGray
    Add-CheckResult "OPENAI_API_KEY" $false "Nie je nastavený"
}

# --- 6. dotnet restore ---
Write-Host "  [6/7] Obnova závislostí (dotnet restore)..." -ForegroundColor White

if ($SkipBuild) {
    Write-Host "  ⏭️  Preskočené" -ForegroundColor DarkGray
    Add-CheckResult "dotnet restore" $true "Preskočené"
} else {
    try {
        $restoreOutput = & dotnet restore $SolutionFile --verbosity quiet 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  ✅ Závislosti obnovené" -ForegroundColor Green
            Add-CheckResult "dotnet restore" $true "Úspešné"
        } else {
            Write-Host "  ❌ Obnova závislostí zlyhala" -ForegroundColor Red
            $restoreOutput | ForEach-Object { Write-Host "     $_" -ForegroundColor DarkGray }
            Add-CheckResult "dotnet restore" $false "Zlyhanie"
        }
    } catch {
        Write-Host "  ❌ Chyba pri obnove závislostí: $_" -ForegroundColor Red
        Add-CheckResult "dotnet restore" $false "Chyba: $_"
    }
}

# --- 7. dotnet build ---
Write-Host "  [7/7] Zostavenie projektu (dotnet build)..." -ForegroundColor White

if ($SkipBuild) {
    Write-Host "  ⏭️  Preskočené" -ForegroundColor DarkGray
    Add-CheckResult "dotnet build" $true "Preskočené"
} else {
    try {
        $buildOutput = & dotnet build $SolutionFile --verbosity quiet --no-restore 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  ✅ Projekt úspešne zostavený" -ForegroundColor Green
            Add-CheckResult "dotnet build" $true "Úspešné"
        } else {
            Write-Host "  ❌ Zostavenie projektu zlyhalo" -ForegroundColor Red
            # Zobraziť len chybové riadky
            $buildOutput | Where-Object { $_ -match "error" } | ForEach-Object {
                Write-Host "     $_" -ForegroundColor DarkGray
            }
            Add-CheckResult "dotnet build" $false "Zlyhanie"
        }
    } catch {
        Write-Host "  ❌ Chyba pri zostavení projektu: $_" -ForegroundColor Red
        Add-CheckResult "dotnet build" $false "Chyba: $_"
    }
}

# --- Súhrn ---
Write-Host ""
Write-Host "╔══════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║                    📋 Súhrn kontroly                     ║" -ForegroundColor Cyan
Write-Host "╚══════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

$passedCount = ($checkResults | Where-Object { $_.Passed }).Count
$totalCount = $checkResults.Count

foreach ($result in $checkResults) {
    $icon = if ($result.Passed) { "✅" } else { "❌" }
    $color = if ($result.Passed) { "Green" } else { "Red" }
    Write-Host "  $icon $($result.Name.PadRight(25)) $($result.Detail)" -ForegroundColor $color
}

Write-Host ""
if ($allPassed) {
    Write-Host "  🎉 Všetky kontroly prešli ($passedCount/$totalCount)" -ForegroundColor Green
    Write-Host "     Prostredie je pripravené na generovanie rozprávok!" -ForegroundColor Green
} else {
    $failedCount = $totalCount - $passedCount
    Write-Host "  ⚠️  $failedCount z $totalCount kontrol zlyhalo" -ForegroundColor Yellow
    Write-Host "     Opravte problémy vyššie a spustite skript znova." -ForegroundColor Yellow
}

Write-Host ""

# Návratový kód
if (-not $allPassed) {
    exit 1
}
