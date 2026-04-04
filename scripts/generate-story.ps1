<#
.SYNOPSIS
    Hlavný automatizačný skript pre generovanie rozprávky.
.DESCRIPTION
    Orchestruje celý workflow generovania rozprávky:
    1. Spustí CLI príkaz "prepare" na výber témy a vytvorenie adresára
    2. Vypíše inštrukcie pre Copilot CLI na vygenerovanie textu
    3. Po vygenerovaní textu (rozpravka.md) spustí review, audio, images, video
    4. Zobrazí finálny stav
.PARAMETER Minutes
    Cieľová dĺžka rozprávky v minútach (predvolené: 12).
.PARAMETER Theme
    Voliteľná téma rozprávky. Ak nie je zadaná, téma sa vyberie automaticky.
.PARAMETER SkipAudio
    Preskočiť generovanie audia.
.PARAMETER SkipImages
    Preskočiť generovanie ilustrácií.
.PARAMETER SkipVideo
    Preskočiť generovanie videa.
.EXAMPLE
    .\generate-story.ps1
    .\generate-story.ps1 -Minutes 8 -Theme "odvaha"
    .\generate-story.ps1 -SkipVideo -SkipImages
#>
param(
    [Parameter(HelpMessage = "Cieľová dĺžka rozprávky v minútach")]
    [ValidateRange(5, 30)]
    [int]$Minutes = 12,

    [Parameter(HelpMessage = "Téma rozprávky (voliteľné)")]
    [string]$Theme = "",

    [Parameter(HelpMessage = "Preskočiť generovanie audia")]
    [switch]$SkipAudio,

    [Parameter(HelpMessage = "Preskočiť generovanie ilustrácií")]
    [switch]$SkipImages,

    [Parameter(HelpMessage = "Preskočiť generovanie videa")]
    [switch]$SkipVideo
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# Koreňový adresár projektu
$ProjectRoot = Split-Path -Parent $PSScriptRoot
$CliProject = Join-Path $ProjectRoot "src" "PoucneRozpravky.CLI"

function Write-Step {
    param([string]$Message, [int]$Step, [int]$Total)
    Write-Host ""
    Write-Host "[$Step/$Total] " -ForegroundColor Cyan -NoNewline
    Write-Host $Message -ForegroundColor White
    Write-Host ("-" * 60) -ForegroundColor DarkGray
}

function Write-Success {
    param([string]$Message)
    Write-Host "  ✅ $Message" -ForegroundColor Green
}

function Write-Warning {
    param([string]$Message)
    Write-Host "  ⚠️  $Message" -ForegroundColor Yellow
}

function Write-Failure {
    param([string]$Message)
    Write-Host "  ❌ $Message" -ForegroundColor Red
}

function Write-Info {
    param([string]$Message)
    Write-Host "  ℹ️  $Message" -ForegroundColor Blue
}

# Hlavička
Write-Host ""
Write-Host "╔══════════════════════════════════════════════════════════╗" -ForegroundColor Magenta
Write-Host "║       🧚 Poučné Slovenské Rozprávky - Generátor 🧚      ║" -ForegroundColor Magenta
Write-Host "╚══════════════════════════════════════════════════════════╝" -ForegroundColor Magenta
Write-Host ""

$totalSteps = 2
if (-not $SkipAudio) { $totalSteps++ }
if (-not $SkipImages) { $totalSteps++ }
if (-not $SkipVideo) { $totalSteps++ }
# +1 pre prepare, +1 pre review = základ 2, ostatné sú voliteľné
$totalSteps += 2  # prepare + čakanie na text
$currentStep = 0

# --- Krok 1: Príprava ---
$currentStep++
Write-Step "Príprava novej rozprávky (prepare)" $currentStep $totalSteps

$prepareArgs = @("run", "--project", $CliProject, "--", "prepare", "--minutes", $Minutes.ToString())
if ($Theme -ne "") {
    $prepareArgs += "--theme"
    $prepareArgs += $Theme
}

Write-Info "Spúšťam: dotnet $($prepareArgs -join ' ')"

try {
    $prepareOutput = & dotnet @prepareArgs 2>&1
    $prepareExitCode = $LASTEXITCODE

    $prepareOutput | ForEach-Object { Write-Host "  $_" -ForegroundColor DarkGray }

    if ($prepareExitCode -ne 0) {
        Write-Failure "Príkaz 'prepare' zlyhal s kódom $prepareExitCode."
        exit 1
    }

    # Extrahovanie ID rozprávky z výstupu
    $storyId = $prepareOutput | Select-String -Pattern "([0-9]{4}-[0-9]{2}-[0-9]{2}-[\w-]+)" |
        ForEach-Object { $_.Matches[0].Value } | Select-Object -First 1

    if (-not $storyId) {
        # Záložný spôsob: nájsť najnovší adresár v rozpravky/
        $rozpravkyDir = Join-Path $ProjectRoot "rozpravky"
        if (Test-Path $rozpravkyDir) {
            $storyId = Get-ChildItem -Path $rozpravkyDir -Directory |
                Sort-Object Name -Descending |
                Select-Object -First 1 -ExpandProperty Name
        }
    }

    if (-not $storyId) {
        Write-Failure "Nepodarilo sa zistiť ID novej rozprávky."
        exit 1
    }

    Write-Success "Rozprávka pripravená: $storyId"
} catch {
    Write-Failure "Chyba pri príprave rozprávky: $_"
    exit 1
}

$storyDir = Join-Path $ProjectRoot "rozpravky" $storyId
$storyFile = Join-Path $storyDir "rozpravka.md"

# --- Krok 2: Čakanie na vygenerovanie textu ---
$currentStep++
Write-Step "Generovanie textu rozprávky" $currentStep $totalSteps

if (Test-Path $storyFile) {
    Write-Success "Súbor rozpravka.md už existuje."
} else {
    Write-Host ""
    Write-Host "  ┌─────────────────────────────────────────────────────┐" -ForegroundColor Yellow
    Write-Host "  │  Text rozprávky ešte nebol vygenerovaný.           │" -ForegroundColor Yellow
    Write-Host "  │                                                     │" -ForegroundColor Yellow
    Write-Host "  │  Použite Copilot CLI na vygenerovanie textu:       │" -ForegroundColor Yellow
    Write-Host "  │                                                     │" -ForegroundColor Yellow
    Write-Host "  │  ghcs 'Vygeneruj rozprávku $storyId'" -ForegroundColor Cyan -NoNewline
    Write-Host "              │" -ForegroundColor Yellow
    Write-Host "  │                                                     │" -ForegroundColor Yellow
    Write-Host "  │  Alebo spustite pipeline príkaz:                   │" -ForegroundColor Yellow
    Write-Host "  │  dotnet run --project src/PoucneRozpravky.CLI" -ForegroundColor Cyan -NoNewline
    Write-Host "    │" -ForegroundColor Yellow
    Write-Host "  │    -- pipeline $storyId" -ForegroundColor Cyan -NoNewline
    Write-Host "                              │" -ForegroundColor Yellow
    Write-Host "  └─────────────────────────────────────────────────────┘" -ForegroundColor Yellow
    Write-Host ""

    # Čakanie na súbor s timeoutom
    $maxWaitSeconds = 600
    $waitedSeconds = 0
    $checkInterval = 5

    Write-Info "Čakám na vytvorenie súboru rozpravka.md (max $($maxWaitSeconds / 60) minút)..."

    while (-not (Test-Path $storyFile) -and $waitedSeconds -lt $maxWaitSeconds) {
        Start-Sleep -Seconds $checkInterval
        $waitedSeconds += $checkInterval
        if ($waitedSeconds % 30 -eq 0) {
            Write-Host "  ⏳ Čakám... ($waitedSeconds s)" -ForegroundColor DarkGray
        }
    }

    if (-not (Test-Path $storyFile)) {
        Write-Failure "Časový limit vypršal. Súbor rozpravka.md nebol vytvorený."
        Write-Info "Spustite skript znova po vygenerovaní textu."
        exit 1
    }

    Write-Success "Súbor rozpravka.md bol vytvorený!"
}

# --- Krok 3: Review ---
$currentStep++
Write-Step "Kontrola kvality (review)" $currentStep $totalSteps

try {
    Write-Info "Spúšťam: dotnet run --project $CliProject -- review $storyId"
    $reviewOutput = & dotnet run --project $CliProject -- review $storyId 2>&1
    $reviewExitCode = $LASTEXITCODE

    $reviewOutput | ForEach-Object { Write-Host "  $_" -ForegroundColor DarkGray }

    if ($reviewExitCode -ne 0) {
        Write-Warning "Review hlási problémy (kód $reviewExitCode). Skontrolujte výstup."
        Write-Info "Pokračujem v generovaní médií..."
    } else {
        Write-Success "Kontrola kvality úspešne dokončená."
    }
} catch {
    Write-Warning "Chyba pri kontrole kvality: $_"
    Write-Info "Pokračujem v generovaní médií..."
}

# --- Krok 4: Audio ---
if (-not $SkipAudio) {
    $currentStep++
    Write-Step "Generovanie audia (audio)" $currentStep $totalSteps

    try {
        Write-Info "Spúšťam: dotnet run --project $CliProject -- audio $storyId"
        $audioOutput = & dotnet run --project $CliProject -- audio $storyId 2>&1
        $audioExitCode = $LASTEXITCODE

        $audioOutput | ForEach-Object { Write-Host "  $_" -ForegroundColor DarkGray }

        if ($audioExitCode -ne 0) {
            Write-Warning "Generovanie audia zlyhalo (kód $audioExitCode)."
        } else {
            Write-Success "Audio úspešne vygenerované."
        }
    } catch {
        Write-Warning "Chyba pri generovaní audia: $_"
    }
}

# --- Krok 5: Ilustrácie ---
if (-not $SkipImages) {
    $currentStep++
    Write-Step "Generovanie ilustrácií (images)" $currentStep $totalSteps

    try {
        Write-Info "Spúšťam: dotnet run --project $CliProject -- images $storyId"
        $imagesOutput = & dotnet run --project $CliProject -- images $storyId 2>&1
        $imagesExitCode = $LASTEXITCODE

        $imagesOutput | ForEach-Object { Write-Host "  $_" -ForegroundColor DarkGray }

        if ($imagesExitCode -ne 0) {
            Write-Warning "Generovanie ilustrácií zlyhalo (kód $imagesExitCode)."
        } else {
            Write-Success "Ilustrácie úspešne vygenerované."
        }
    } catch {
        Write-Warning "Chyba pri generovaní ilustrácií: $_"
    }
}

# --- Krok 6: Video ---
if (-not $SkipVideo) {
    $currentStep++
    Write-Step "Generovanie videa (video)" $currentStep $totalSteps

    try {
        Write-Info "Spúšťam: dotnet run --project $CliProject -- video $storyId"
        $videoOutput = & dotnet run --project $CliProject -- video $storyId 2>&1
        $videoExitCode = $LASTEXITCODE

        $videoOutput | ForEach-Object { Write-Host "  $_" -ForegroundColor DarkGray }

        if ($videoExitCode -ne 0) {
            Write-Warning "Generovanie videa zlyhalo (kód $videoExitCode)."
        } else {
            Write-Success "Video úspešne vygenerované."
        }
    } catch {
        Write-Warning "Chyba pri generovaní videa: $_"
    }
}

# --- Finálny stav ---
Write-Host ""
Write-Host "╔══════════════════════════════════════════════════════════╗" -ForegroundColor Magenta
Write-Host "║                    📊 Finálny stav                      ║" -ForegroundColor Magenta
Write-Host "╚══════════════════════════════════════════════════════════╝" -ForegroundColor Magenta
Write-Host ""

try {
    & dotnet run --project $CliProject -- status $storyId 2>&1 |
        ForEach-Object { Write-Host "  $_" -ForegroundColor White }
} catch {
    Write-Warning "Nepodarilo sa zobraziť stav rozprávky."
}

# Súhrn súborov
Write-Host ""
Write-Host "  📁 Adresár rozprávky: " -ForegroundColor Cyan -NoNewline
Write-Host $storyDir -ForegroundColor White

if (Test-Path $storyDir) {
    $files = Get-ChildItem -Path $storyDir -Recurse -File
    Write-Host "  📄 Počet súborov: $($files.Count)" -ForegroundColor Cyan

    foreach ($file in $files) {
        $relativePath = $file.FullName.Substring($storyDir.Length + 1)
        $size = if ($file.Length -gt 1MB) {
            "{0:N1} MB" -f ($file.Length / 1MB)
        } elseif ($file.Length -gt 1KB) {
            "{0:N1} KB" -f ($file.Length / 1KB)
        } else {
            "$($file.Length) B"
        }
        Write-Host "     $relativePath ($size)" -ForegroundColor DarkGray
    }
}

Write-Host ""
Write-Host "  🎉 Hotovo!" -ForegroundColor Green
Write-Host ""
