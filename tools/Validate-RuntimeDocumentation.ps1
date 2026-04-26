param(
    [string[]] $Files,
    [switch] $Strict
)

$ErrorActionPreference = "Stop"

function Get-TargetFiles {
    param(
        [string[]] $ExplicitFiles
    )

    if ($ExplicitFiles -and $ExplicitFiles.Count -gt 0) {
        return $ExplicitFiles | Where-Object {
            $_ -like "Assets/_Project/Scripts/Runtime/*.cs" -or
            $_ -like "Assets/_Project/Scripts/Runtime/*/*.cs" -or
            $_ -like "Assets/_Project/Scripts/Runtime/*/*/*.cs"
        }
    }

    $stagedFiles = git diff --cached --name-only --diff-filter=ACMR
    return $stagedFiles | Where-Object { $_ -like "Assets/_Project/Scripts/Runtime/*.cs" -or $_ -like "Assets/_Project/Scripts/Runtime/*" }
}

function Test-ClassSummary {
    param(
        [string[]] $Lines,
        [string] $RelativePath,
        [System.Collections.ArrayList] $Warnings
    )

    for ($i = 0; $i -lt $Lines.Length; $i++) {
        if ($Lines[$i] -match '^\s*public\s+class\s+\w+' -or $Lines[$i] -match '^\s*public\s+sealed\s+class\s+\w+') {
            for ($j = [Math]::Max(0, $i - 5); $j -lt $i; $j++) {
                if ($Lines[$j] -match '^\s*///\s*<summary>') {
                    return
                }
            }

            [void] $Warnings.Add([pscustomobject]@{
                File    = $RelativePath
                Line    = $i + 1
                Rule    = "summary"
                Message = "Falta <summary> de clase"
            })
            return
        }
    }
}

function Test-SerializeFieldTooltips {
    param(
        [string[]] $Lines,
        [string] $RelativePath,
        [System.Collections.ArrayList] $Warnings
    )

    for ($i = 0; $i -lt $Lines.Length; $i++) {
        if ($Lines[$i] -match '\[SerializeField\]') {
            $hasTooltip = $false

            for ($j = [Math]::Max(0, $i - 3); $j -lt $i; $j++) {
                if ($Lines[$j] -match '\[Tooltip\(') {
                    $hasTooltip = $true
                    break
                }
            }

            if (-not $hasTooltip) {
                [void] $Warnings.Add([pscustomobject]@{
                    File    = $RelativePath
                    Line    = $i + 1
                    Rule    = "tooltip"
                    Message = "Campo [SerializeField] sin [Tooltip]"
                })
            }
        }
    }
}

$filesToCheck = Get-TargetFiles -ExplicitFiles $Files
$filesToCheck = $filesToCheck | Where-Object { $_ -like "*.cs" } | Sort-Object -Unique

if (-not $filesToCheck -or $filesToCheck.Count -eq 0) {
    Write-Host "Validacion de documentacion: no hay scripts de Runtime para revisar." -ForegroundColor DarkGray
    exit 0
}

$warnings = [System.Collections.ArrayList]::new()

foreach ($relativePath in $filesToCheck) {
    if (-not (Test-Path $relativePath)) {
        continue
    }

    $lines = Get-Content -Path $relativePath

    Test-ClassSummary -Lines $lines -RelativePath $relativePath -Warnings $warnings
    Test-SerializeFieldTooltips -Lines $lines -RelativePath $relativePath -Warnings $warnings
}

if ($warnings.Count -eq 0) {
    Write-Host ""
    Write-Host "[OK] Documentacion de Runtime sin avisos en los archivos revisados." -ForegroundColor Green
    Write-Host ""
    exit 0
}

$summaryCount = ($warnings | Where-Object { $_.Rule -eq "summary" }).Count
$tooltipCount = ($warnings | Where-Object { $_.Rule -eq "tooltip" }).Count

$header = if ($Strict) { "[ERROR]" } else { "[AVISO]" }
$headerColor = if ($Strict) { "Red" } else { "Yellow" }

Write-Host ""
Write-Host "$header Documentacion de Runtime: $($warnings.Count) hallazgo(s)" -ForegroundColor $headerColor
Write-Host "        (summary de clase: $summaryCount | tooltip: $tooltipCount)" -ForegroundColor DarkYellow
Write-Host ""

$grouped = $warnings | Group-Object -Property File
foreach ($group in $grouped) {
    Write-Host "  $($group.Name)" -ForegroundColor Cyan
    foreach ($w in $group.Group) {
        Write-Host ("    linea {0,-5} {1,-10} {2}" -f $w.Line, $w.Rule, $w.Message) -ForegroundColor Gray
    }
}

Write-Host ""

if ($Strict) {
    Write-Host "  Modo estricto activo: los hallazgos bloquean la ejecucion." -ForegroundColor Red
    Write-Host "  Agrega <summary> y [Tooltip] segun las convenciones de AGENT.md." -ForegroundColor DarkGray
    Write-Host ""
    exit 1
}

Write-Host "  Commit permitido. Estas recomendaciones no bloquean el flujo local." -ForegroundColor DarkGray
Write-Host "  Para revisar manualmente:  pwsh -File .\tools\Validate-RuntimeDocumentation.ps1" -ForegroundColor DarkGray
Write-Host ""

exit 0
