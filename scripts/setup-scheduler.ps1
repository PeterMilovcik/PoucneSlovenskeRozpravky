<#
.SYNOPSIS
    Nastavenie Windows Task Scheduler pre denné generovanie rozprávok.
.DESCRIPTION
    Vytvorí úlohu v Task Scheduler, ktorá spúšťa daily-generate.ps1 každý deň.
    - Názov úlohy: "PoucneRozpravky-DailyGeneration"
    - Trigger: Denne v konfigurovateľnom čase (predvolené 08:00)
    - Akcia: Spustenie daily-generate.ps1
    - Vyžaduje: Administrátorské oprávnenia
.PARAMETER Time
    Čas spustenia vo formáte HH:mm (predvolené: "08:00").
.PARAMETER Remove
    Odstrániť existujúcu naplánovanú úlohu.
.EXAMPLE
    .\setup-scheduler.ps1
    .\setup-scheduler.ps1 -Time "06:30"
    .\setup-scheduler.ps1 -Remove
#>
param(
    [Parameter(HelpMessage = "Čas denného spustenia vo formáte HH:mm")]
    [ValidatePattern("^[0-2][0-9]:[0-5][0-9]$")]
    [string]$Time = "08:00",

    [Parameter(HelpMessage = "Odstrániť existujúcu naplánovanú úlohu")]
    [switch]$Remove
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$TaskName = "PoucneRozpravky-DailyGeneration"
$TaskDescription = "Denné automatické generovanie poučnej slovenskej rozprávky"
$ProjectRoot = Split-Path -Parent $PSScriptRoot
$DailyScript = Join-Path $PSScriptRoot "daily-generate.ps1"

function Test-Administrator {
    $currentPrincipal = New-Object Security.Principal.WindowsPrincipal(
        [Security.Principal.WindowsIdentity]::GetCurrent()
    )
    return $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

# Hlavička
Write-Host ""
Write-Host "╔══════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║    ⏰ Task Scheduler - Poučné Slovenské Rozprávky ⏰     ║" -ForegroundColor Cyan
Write-Host "╚══════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# Kontrola administrátorských oprávnení
if (-not (Test-Administrator)) {
    Write-Host "  ❌ Tento skript vyžaduje administrátorské oprávnenia." -ForegroundColor Red
    Write-Host ""
    Write-Host "  Spustite PowerShell ako Administrátor a skúste znova:" -ForegroundColor Yellow
    Write-Host "  Start-Process pwsh -Verb RunAs -ArgumentList '-File', '$($MyInvocation.MyCommand.Path)'" -ForegroundColor DarkGray
    Write-Host ""
    exit 1
}

# Odstránenie úlohy
if ($Remove) {
    Write-Host "  Odstraňujem úlohu '$TaskName'..." -ForegroundColor Yellow

    try {
        $existingTask = Get-ScheduledTask -TaskName $TaskName -ErrorAction SilentlyContinue
        if ($existingTask) {
            Unregister-ScheduledTask -TaskName $TaskName -Confirm:$false
            Write-Host "  ✅ Úloha '$TaskName' bola úspešne odstránená." -ForegroundColor Green
        } else {
            Write-Host "  ℹ️  Úloha '$TaskName' neexistuje." -ForegroundColor Blue
        }
    } catch {
        Write-Host "  ❌ Chyba pri odstraňovaní úlohy: $_" -ForegroundColor Red
        exit 1
    }

    Write-Host ""
    exit 0
}

# Kontrola existencie skriptu
if (-not (Test-Path $DailyScript)) {
    Write-Host "  ❌ Skript daily-generate.ps1 nebol nájdený:" -ForegroundColor Red
    Write-Host "     $DailyScript" -ForegroundColor DarkGray
    exit 1
}

# Kontrola existujúcej úlohy
$existingTask = Get-ScheduledTask -TaskName $TaskName -ErrorAction SilentlyContinue
if ($existingTask) {
    Write-Host "  ⚠️  Úloha '$TaskName' už existuje." -ForegroundColor Yellow
    Write-Host "     Stav: $($existingTask.State)" -ForegroundColor DarkGray

    $taskTriggers = $existingTask.Triggers
    if ($taskTriggers) {
        Write-Host "     Trigger: $($taskTriggers[0].StartBoundary)" -ForegroundColor DarkGray
    }

    Write-Host ""
    Write-Host "  Odstraňujem starú úlohu a vytváram novú..." -ForegroundColor Yellow
    Unregister-ScheduledTask -TaskName $TaskName -Confirm:$false
}

# Vytvorenie novej úlohy
Write-Host "  Vytváram naplánovanú úlohu..." -ForegroundColor White
Write-Host "    Názov:   $TaskName" -ForegroundColor DarkGray
Write-Host "    Čas:     $Time (denne)" -ForegroundColor DarkGray
Write-Host "    Skript:  $DailyScript" -ForegroundColor DarkGray
Write-Host ""

try {
    # Nájdenie pwsh.exe
    $pwshPath = (Get-Command pwsh -ErrorAction SilentlyContinue).Source
    if (-not $pwshPath) {
        $pwshPath = (Get-Command powershell -ErrorAction SilentlyContinue).Source
    }
    if (-not $pwshPath) {
        Write-Host "  ❌ PowerShell executable nebol nájdený." -ForegroundColor Red
        exit 1
    }

    # Trigger - denne v zadanom čase
    $triggerTime = [DateTime]::ParseExact($Time, "HH:mm", $null)
    $trigger = New-ScheduledTaskTrigger -Daily -At $triggerTime

    # Akcia - spustenie daily-generate.ps1
    $action = New-ScheduledTaskAction `
        -Execute $pwshPath `
        -Argument "-NoProfile -NonInteractive -ExecutionPolicy Bypass -File `"$DailyScript`"" `
        -WorkingDirectory $ProjectRoot

    # Nastavenia úlohy
    $settings = New-ScheduledTaskSettingsSet `
        -AllowStartIfOnBatteries `
        -DontStopIfGoingOnBatteries `
        -StartWhenAvailable `
        -ExecutionTimeLimit (New-TimeSpan -Hours 2) `
        -RestartCount 3 `
        -RestartInterval (New-TimeSpan -Minutes 5)

    # Registrácia úlohy
    Register-ScheduledTask `
        -TaskName $TaskName `
        -Description $TaskDescription `
        -Trigger $trigger `
        -Action $action `
        -Settings $settings `
        -RunLevel Highest | Out-Null

    Write-Host "  ✅ Úloha '$TaskName' bola úspešne vytvorená!" -ForegroundColor Green
    Write-Host ""

    # Overenie
    $createdTask = Get-ScheduledTask -TaskName $TaskName
    Write-Host "  📋 Detaily úlohy:" -ForegroundColor Cyan
    Write-Host "     Názov:        $($createdTask.TaskName)" -ForegroundColor White
    Write-Host "     Stav:         $($createdTask.State)" -ForegroundColor White
    Write-Host "     Spustenie:    Denne o $Time" -ForegroundColor White
    Write-Host "     Pracovný dir: $ProjectRoot" -ForegroundColor White
    Write-Host "     PowerShell:   $pwshPath" -ForegroundColor White
    Write-Host ""
    Write-Host "  💡 Správa úlohy:" -ForegroundColor Yellow
    Write-Host "     Zobraziť:    Get-ScheduledTask -TaskName '$TaskName'" -ForegroundColor DarkGray
    Write-Host "     Spustiť:     Start-ScheduledTask -TaskName '$TaskName'" -ForegroundColor DarkGray
    Write-Host "     Odstrániť:   .\setup-scheduler.ps1 -Remove" -ForegroundColor DarkGray
    Write-Host ""
} catch {
    Write-Host "  ❌ Chyba pri vytváraní úlohy: $_" -ForegroundColor Red
    Write-Host "     $($_.ScriptStackTrace)" -ForegroundColor DarkGray
    exit 1
}
