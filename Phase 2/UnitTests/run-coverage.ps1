# Runs every Phase 2 unit test with coverage and prints line/branch coverage per source file.
# Usage (from anywhere):  powershell -ExecutionPolicy Bypass -File ".\UnitTests\run-coverage.ps1"
param(
    [double]$Threshold = 80
)

$root = Split-Path -Parent $PSScriptRoot
$solution = Get-ChildItem $root -Filter *.sln | Select-Object -First 1
$results = Join-Path $PSScriptRoot "TestResults"
if (Test-Path $results) { Remove-Item -Recurse -Force $results }

dotnet test $solution.FullName --collect:"XPlat Code Coverage" --results-directory $results
if ($LASTEXITCODE -ne 0) { Write-Host "Tests failed." -ForegroundColor Red; exit 1 }

# Merge line hits per source file across all cobertura reports.
$lines = @{}      # file -> @{ lineNumber -> covered(bool) }
$branches = @{}   # file -> @{ lineNumber -> @(covered, total) }

Get-ChildItem $results -Recurse -Filter coverage.cobertura.xml | ForEach-Object {
    [xml]$xml = Get-Content $_.FullName
    # File names are relative to the report's <source> folder; resolve them to full paths.
    $sourceRoot = @($xml.coverage.sources.source)[0]
    foreach ($class in $xml.coverage.packages.package.classes.class) {
        $file = if ([IO.Path]::IsPathRooted($class.filename)) { $class.filename } else { Join-Path $sourceRoot $class.filename }
        $file = [IO.Path]::GetFullPath($file)
        if (-not $lines.ContainsKey($file)) { $lines[$file] = @{}; $branches[$file] = @{} }
        foreach ($line in $class.lines.line) {
            $n = [int]$line.number
            $hit = [int]$line.hits -gt 0
            $lines[$file][$n] = ($lines[$file][$n] -or $hit)
            if ($line.branch -eq 'True' -and $line.'condition-coverage' -match '\((\d+)/(\d+)\)') {
                $prev = $branches[$file]["$n"]
                $covered = [int]$Matches[1]; $total = [int]$Matches[2]
                if (-not $prev -or $covered -gt $prev[0]) { $branches[$file]["$n"] = @($covered, $total) }
            }
        }
    }
}

$rows = foreach ($file in $lines.Keys | Sort-Object) {
    $total = $lines[$file].Count
    $covered = ($lines[$file].Values | Where-Object { $_ }).Count
    $bCovered = 0; $bTotal = 0
    foreach ($b in $branches[$file].Values) { $bCovered += $b[0]; $bTotal += $b[1] }
    [pscustomobject]@{
        File       = $file.Substring($root.Length + 1)
        Lines      = "$covered/$total"
        'Line %'   = [math]::Round(100 * $covered / [math]::Max($total, 1), 1)
        'Branch %' = if ($bTotal) { [math]::Round(100 * $bCovered / $bTotal, 1) } else { 'n/a' }
    }
}

$rows | Format-Table -AutoSize | Out-String -Width 220 | Write-Host

$allLines = ($lines.Values | ForEach-Object { $_.Count } | Measure-Object -Sum).Sum
$allCovered = ($lines.Values | ForEach-Object { ($_.Values | Where-Object { $_ }).Count } | Measure-Object -Sum).Sum
$allBranches = 0; $allBranchesCovered = 0
foreach ($f in $branches.Values) { foreach ($b in $f.Values) { $allBranchesCovered += $b[0]; $allBranches += $b[1] } }
Write-Host ("Overall line coverage: {0}% ({1}/{2} lines)" -f [math]::Round(100 * $allCovered / $allLines, 1), $allCovered, $allLines)
if ($allBranches) { Write-Host ("Overall branch coverage: {0}% ({1}/{2} branches)" -f [math]::Round(100 * $allBranchesCovered / $allBranches, 1), $allBranchesCovered, $allBranches) }

$below = $rows | Where-Object { $_.'Line %' -lt $Threshold }
if ($below) {
    Write-Host "Files below $Threshold%:" -ForegroundColor Red
    $below | ForEach-Object { Write-Host "  $($_.File) - $($_.'Line %')%" -ForegroundColor Red }
    exit 1
}
Write-Host "All files meet the $Threshold% threshold." -ForegroundColor Green
