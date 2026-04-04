<#
.SYNOPSIS
    Rýchla kontrola stavu všetkých rozprávok.
.DESCRIPTION
    Zobrazí prehľad všetkých rozprávok a ich stavov.
    Voliteľne filtruje podľa stavu alebo zobrazí detail konkrétnej rozprávky.
.PARAMETER Id
    ID konkrétnej rozprávky pre zobrazenie detailu.
.PARAMETER Status
    Filtrovanie podľa stavu (napr. TextReady, AudioReady, FullyPublished).
.PARAMETER Summary
    Zobraziť iba súhrn počtov podľa stavu.
.EXAMPLE
    .\check-status.ps1
    .\check-status.ps1 -Id "2024-12-15-maly-hrdina"
    .\check-status.ps1 -Status "TextReady"
    .\check-status.ps1 -Summary
#>
param(
    [Parameter(HelpMessage = "ID konkrétnej rozprávky")]
    [string]$Id = "",

    [Parameter(HelpMessage = "Filtrovanie podľa stavu")]
    [string]$Status = "",

    [Parameter(HelpMessage = "Zobraziť iba súhrn")]
    [switch]$Summary
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$CliProject = Join-Path $ProjectRoot "src" "PoucneRozpravky.CLI"
$RozpravkyDir = Join-Path $ProjectRoot "rozpravky"

# Hlavička
Write-Host ""
Write-Host "╔══════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║       📊 Poučné Slovenské Rozprávky - Stav 📊           ║" -ForegroundColor Cyan
Write-Host "╚══════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# Súhrn adresárov
if (Test-Path $RozpravkyDir) {
    $storyDirs = Get-ChildItem -Path $RozpravkyDir -Directory

    if ($Summary) {
        Write-Host "  📁 Celkový počet rozprávok: $($storyDirs.Count)" -ForegroundColor White
        Write-Host ""

        # Zber štatistík zo súborov metadata.json
        $statusCounts = @{}
        $totalSize = 0

        foreach ($dir in $storyDirs) {
            $metadataFile = Join-Path $dir.FullName "metadata.json"
            if (Test-Path $metadataFile) {
                try {
                    $metadata = Get-Content $metadataFile -Raw -Encoding UTF8 | ConvertFrom-Json
                    $storyStatus = $metadata.status
                    if (-not $storyStatus) { $storyStatus = "Neznámy" }

                    if ($statusCounts.ContainsKey($storyStatus)) {
                        $statusCounts[$storyStatus]++
                    } else {
                        $statusCounts[$storyStatus] = 1
                    }
                } catch {
                    if ($statusCounts.ContainsKey("Chyba")) {
                        $statusCounts["Chyba"]++
                    } else {
                        $statusCounts["Chyba"] = 1
                    }
                }
            } else {
                if ($statusCounts.ContainsKey("BezMetadát")) {
                    $statusCounts["BezMetadát"]++
                } else {
                    $statusCounts["BezMetadát"] = 1
                }
            }

            # Veľkosť adresára
            $dirFiles = Get-ChildItem -Path $dir.FullName -Recurse -File -ErrorAction SilentlyContinue
            $totalSize += ($dirFiles | Measure-Object -Property Length -Sum).Sum
        }

        # Zobrazenie štatistík
        Write-Host "  📈 Rozdelenie podľa stavu:" -ForegroundColor Cyan
        $statusOrder = @(
            "OutlineDraft", "OutlineReady", "TextGenerating", "TextDraft",
            "GrammarChecked", "StyleChecked", "ContentReviewed", "TextReady",
            "AudioReady", "ImagesReady", "VideoReady",
            "PublishedText", "PublishedAudio", "PublishedVideo", "FullyPublished",
            "Neznámy", "BezMetadát", "Chyba"
        )

        foreach ($s in $statusOrder) {
            if ($statusCounts.ContainsKey($s)) {
                $count = $statusCounts[$s]
                $color = switch -Wildcard ($s) {
                    "Fully*"    { "Green" }
                    "Published*" { "Green" }
                    "*Ready"    { "Cyan" }
                    "*Checked"  { "Blue" }
                    "*Draft"    { "Yellow" }
                    "*Generating" { "Yellow" }
                    "Chyba"     { "Red" }
                    "BezMetadát" { "Red" }
                    default     { "White" }
                }
                $bar = "█" * [Math]::Min($count, 40)
                Write-Host "     $($s.PadRight(20)) " -ForegroundColor $color -NoNewline
                Write-Host "$bar " -ForegroundColor $color -NoNewline
                Write-Host "($count)" -ForegroundColor DarkGray
            }
        }

        # Celková veľkosť
        Write-Host ""
        $sizeStr = if ($totalSize -gt 1GB) {
            "{0:N2} GB" -f ($totalSize / 1GB)
        } elseif ($totalSize -gt 1MB) {
            "{0:N1} MB" -f ($totalSize / 1MB)
        } else {
            "{0:N1} KB" -f ($totalSize / 1KB)
        }
        Write-Host "  💾 Celková veľkosť: $sizeStr" -ForegroundColor White
        Write-Host ""
        return
    }
} else {
    Write-Host "  ℹ️  Adresár rozpravky/ neexistuje." -ForegroundColor Blue
    Write-Host ""
}

# Spustenie CLI príkazu status
try {
    $cliArgs = @("run", "--project", $CliProject, "--", "status")

    if ($Id -ne "") {
        $cliArgs += $Id
        Write-Host "  🔍 Detail rozprávky: $Id" -ForegroundColor White
    } elseif ($Status -ne "") {
        # Použiť list príkaz s filtrom
        $cliArgs = @("run", "--project", $CliProject, "--", "list", "--status", $Status)
        Write-Host "  🔍 Filter: stav = $Status" -ForegroundColor White
    } else {
        Write-Host "  📋 Všetky rozprávky:" -ForegroundColor White
    }

    Write-Host ""

    & dotnet @cliArgs 2>&1 | ForEach-Object {
        Write-Host "  $_" -ForegroundColor White
    }

    if ($LASTEXITCODE -ne 0) {
        Write-Host ""
        Write-Host "  ⚠️  CLI príkaz skončil s kódom $LASTEXITCODE" -ForegroundColor Yellow
    }
} catch {
    Write-Host "  ❌ Chyba pri spúšťaní CLI: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "  💡 Uistite sa, že projekt je zostavený:" -ForegroundColor Yellow
    Write-Host "     dotnet build src/PoucneRozpravky.sln" -ForegroundColor DarkGray
    exit 1
}

Write-Host ""
