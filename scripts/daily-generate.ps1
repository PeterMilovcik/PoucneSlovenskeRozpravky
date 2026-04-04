<#
.SYNOPSIS
    Denný skript pre automatické generovanie rozprávky.
.DESCRIPTION
    Skript pre Windows Task Scheduler:
    - Skontroluje, či dnes už bola rozprávka vygenerovaná
    - Ak nie, spustí generate-story.ps1 s predvolenými nastaveniami
    - Loguje výstup do adresára logs/
    - Možno naplánovať cez Task Scheduler pomocou setup-scheduler.ps1
.PARAMETER Minutes
    Cieľová dĺžka rozprávky v minútach (predvolené: 12).
.PARAMETER Force
    Vynútiť generovanie aj keď dnes už rozprávka existuje.
.EXAMPLE
    .\daily-generate.ps1
    .\daily-generate.ps1 -Force
    .\daily-generate.ps1 -Minutes 8
#>
param(
    [Parameter(HelpMessage = "Cieľová dĺžka rozprávky v minútach")]
    [ValidateRange(5, 30)]
    [int]$Minutes = 12,

    [Parameter(HelpMessage = "Vynútiť generovanie aj keď dnes už rozprávka existuje")]
    [switch]$Force
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$LogDir = Join-Path $ProjectRoot "logs"
$RozpravkyDir = Join-Path $ProjectRoot "rozpravky"
$Today = Get-Date -Format "yyyy-MM-dd"
$LogFile = Join-Path $LogDir "daily-generate-$Today.log"

# Vytvorenie adresára pre logy
if (-not (Test-Path $LogDir)) {
    New-Item -ItemType Directory -Path $LogDir -Force | Out-Null
}

function Write-Log {
    param([string]$Message, [string]$Level = "INFO")
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logMessage = "[$timestamp] [$Level] $Message"
    Add-Content -Path $LogFile -Value $logMessage -Encoding UTF8

    switch ($Level) {
        "INFO"    { Write-Host $logMessage -ForegroundColor White }
        "SUCCESS" { Write-Host $logMessage -ForegroundColor Green }
        "WARN"    { Write-Host $logMessage -ForegroundColor Yellow }
        "ERROR"   { Write-Host $logMessage -ForegroundColor Red }
        default   { Write-Host $logMessage }
    }
}

# Hlavička
Write-Log "=========================================="
Write-Log "Denné generovanie rozprávky - $Today"
Write-Log "=========================================="

# Kontrola, či dnes už bola rozprávka vygenerovaná
if (-not $Force) {
    if (Test-Path $RozpravkyDir) {
        $todayStories = Get-ChildItem -Path $RozpravkyDir -Directory |
            Where-Object { $_.Name -like "$Today-*" }

        if ($todayStories) {
            Write-Log "Dnes už bola vygenerovaná rozprávka: $($todayStories.Name -join ', ')" "WARN"
            Write-Log "Použite parameter -Force pre vynútenie generovania." "WARN"

            # Zapísať do logu aj existujúce súbory
            foreach ($story in $todayStories) {
                $metadataFile = Join-Path $story.FullName "metadata.json"
                if (Test-Path $metadataFile) {
                    $metadata = Get-Content $metadataFile -Raw | ConvertFrom-Json
                    Write-Log "  Existujúca rozprávka: $($metadata.title) (stav: $($metadata.status))" "INFO"
                }
            }

            Write-Log "Denné generovanie preskočené."
            exit 0
        }
    }
}

Write-Log "Žiadna dnešná rozprávka. Spúšťam generovanie..."

# Spustenie generate-story.ps1
$generateScript = Join-Path $PSScriptRoot "generate-story.ps1"

if (-not (Test-Path $generateScript)) {
    Write-Log "Skript generate-story.ps1 nebol nájdený: $generateScript" "ERROR"
    exit 1
}

try {
    Write-Log "Spúšťam: $generateScript -Minutes $Minutes"

    # Spustenie a zachytenie výstupu
    $output = & $generateScript -Minutes $Minutes 2>&1
    $exitCode = $LASTEXITCODE

    # Zapísať výstup do logu
    $output | ForEach-Object {
        $line = $_ -replace '\e\[[0-9;]*m', ''  # Odstránenie ANSI farieb pre log
        Add-Content -Path $LogFile -Value $line -Encoding UTF8
    }

    if ($exitCode -ne 0) {
        Write-Log "Generovanie zlyhalo s kódom $exitCode" "ERROR"
        exit $exitCode
    }

    Write-Log "Generovanie úspešne dokončené." "SUCCESS"
} catch {
    Write-Log "Neočakávaná chyba: $_" "ERROR"
    Write-Log $_.ScriptStackTrace "ERROR"
    exit 1
}

# Vyčistenie starých logov (staršie ako 30 dní)
try {
    $cutoffDate = (Get-Date).AddDays(-30)
    $oldLogs = Get-ChildItem -Path $LogDir -Filter "daily-generate-*.log" |
        Where-Object { $_.LastWriteTime -lt $cutoffDate }

    if ($oldLogs) {
        $oldLogs | Remove-Item -Force
        Write-Log "Vymazaných $($oldLogs.Count) starých logov." "INFO"
    }
} catch {
    Write-Log "Chyba pri čistení starých logov: $_" "WARN"
}

Write-Log "=========================================="
Write-Log "Denné generovanie dokončené."
Write-Log "Log súbor: $LogFile"
Write-Log "=========================================="
