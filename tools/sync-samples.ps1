<#
.SYNOPSIS
    建置並執行 samples/ 下的所有範例，再把程式碼與執行輸出同步回各篇文章。

.DESCRIPTION
    文章中以下列標記包住的區塊由本腳本維護，請勿手動修改區塊內容：

        <!-- sample:Visitor/Program.cs -->   ...程式碼區塊...   <!-- /sample -->
        <!-- output:Visitor -->              ...輸出區塊...     <!-- /output -->

    修改範例的流程：編輯 samples/<Pattern>/Program.cs，然後執行本腳本。

.PARAMETER Check
    只檢查、不修改任何檔案。範例輸出或文章內容與現況不一致時以非零代碼結束（供 CI 使用）。

.PARAMETER BuildArgs
    額外傳給 dotnet build 的參數，例如 -BuildArgs '-p:SampleTargetFramework=net10.0'。

.EXAMPLE
    pwsh tools/sync-samples.ps1
    pwsh tools/sync-samples.ps1 -Check
#>
[CmdletBinding()]
param(
    [switch]$Check,
    [string[]]$BuildArgs = @()
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$samplesDir = Join-Path $root 'samples'
$utf8 = [System.Text.UTF8Encoding]::new($false)
[Console]::OutputEncoding = $utf8

# 固定使用不因作業系統語系而變的文化特性，數字與金額的格式在每台機器上才會一致
$env:DOTNET_SYSTEM_GLOBALIZATION_INVARIANT = '1'

function Read-Text([string]$path) {
    return [System.IO.File]::ReadAllText($path, $utf8)
}

function Write-Text([string]$path, [string]$text) {
    [System.IO.File]::WriteAllText($path, $text, $utf8)
}

function ConvertTo-Lf([string]$text) {
    return $text.Replace("`r`n", "`n")
}

$problems = [System.Collections.Generic.List[string]]::new()

# ---- 0. 確認已安裝範例所需的 .NET SDK ----
# 沒有以 -BuildArgs 覆寫目標 Framework 時，先檢查 SDK 版本，避免只看到難懂的 NETSDK1045 錯誤
if (-not ($BuildArgs -match 'SampleTargetFramework')) {
    $props = Read-Text (Join-Path $samplesDir 'Directory.Build.props')
    if ($props -match '>net(\d+)\.0</SampleTargetFramework>') {
        $required = [int]$Matches[1]
        $installed = (& dotnet --list-sdks |
                ForEach-Object { [int]($_ -split '\.')[0] } |
                Measure-Object -Maximum).Maximum

        if ($installed -lt $required) {
            Write-Host "samples/ 預設以 .NET $required 建置，但這台電腦安裝的 SDK 最高只到 .NET $installed。" -ForegroundColor Red
            Write-Host "請先安裝 .NET $required SDK：https://dotnet.microsoft.com/download"
            Write-Host ''
            Write-Host "如果只是想先在 .NET $installed 上執行單一範例，可以這樣覆寫目標 Framework："
            Write-Host "  dotnet run --project samples/Strategy --property:SampleTargetFramework=net$installed.0"
            Write-Host '（Visitor 使用 C# 15 的 closed 與 union，必須以 .NET 11 SDK 建置，所以本腳本無法在較舊的 SDK 上完整執行。）'
            exit 1
        }
    }
}

# ---- 1. 建置 ----
Write-Host '== Building samples' -ForegroundColor Cyan
& dotnet build (Join-Path $samplesDir 'DesignPatterns.slnx') -c Release -nologo -v q @BuildArgs
if ($LASTEXITCODE -ne 0) {
    throw 'Build failed.'
}

# ---- 2. 執行每個範例並比對輸出 ----
Write-Host '== Running samples' -ForegroundColor Cyan
$projects = Get-ChildItem $samplesDir -Directory |
    Where-Object { Test-Path (Join-Path $_.FullName "$($_.Name).csproj") } |
    Sort-Object Name

foreach ($project in $projects) {
    $name = $project.Name
    $dll = Get-ChildItem (Join-Path $project.FullName 'bin/Release') -Recurse -Filter "$name.dll" |
        Sort-Object LastWriteTimeUtc -Descending |
        Select-Object -First 1
    if (-not $dll) {
        throw "No build output found for $name."
    }

    $lines = & dotnet $dll.FullName 2>&1 | ForEach-Object { "$_" }
    if ($LASTEXITCODE -ne 0) {
        throw "$name exited with code $LASTEXITCODE.`n$($lines -join "`n")"
    }

    $actual = (ConvertTo-Lf ($lines -join "`n")).TrimEnd() + "`n"
    $expectedPath = Join-Path $project.FullName 'expected-output.txt'
    $expected = if (Test-Path $expectedPath) { ConvertTo-Lf (Read-Text $expectedPath) } else { $null }

    if ($actual -ceq $expected) {
        Write-Host "  ok       $name"
    }
    elseif ($Check) {
        Write-Host "  MISMATCH $name" -ForegroundColor Red
        $problems.Add("samples/$name/expected-output.txt does not match the program's actual output")
    }
    else {
        Write-Text $expectedPath $actual
        Write-Host "  updated  $name" -ForegroundColor Yellow
    }
}

# ---- 3. 把程式碼與輸出同步回文章 ----
Write-Host '== Syncing articles' -ForegroundColor Cyan
$samplePattern = '(?s)(?<open><!-- sample:(?<path>[^ ]+) -->\r?\n)```csharp\r?\n.*?\r?\n```(?<close>\r?\n<!-- /sample -->)'
$outputPattern = '(?s)(?<open><!-- output:(?<name>[^ ]+) -->\r?\n)```text\r?\n.*?\r?\n```(?<close>\r?\n<!-- /output -->)'

foreach ($article in Get-ChildItem $root -Filter '*.md' | Sort-Object Name) {
    $original = Read-Text $article.FullName
    $newline = if ($original.Contains("`r`n")) { "`r`n" } else { "`n" }

    $evaluator = {
        param($match, $fence, $relativePath)
        $sourcePath = Join-Path $samplesDir $relativePath
        if (-not (Test-Path $sourcePath)) {
            throw "$($article.Name) refers to a missing file: samples/$relativePath"
        }
        $body = (ConvertTo-Lf (Read-Text $sourcePath)).TrimEnd().Replace("`n", $newline)
        return $match.Groups['open'].Value + '```' + $fence + $newline + $body + $newline + '```' + $match.Groups['close'].Value
    }

    $updated = [regex]::Replace($original, $samplePattern, [System.Text.RegularExpressions.MatchEvaluator] {
            param($match)
            & $evaluator $match 'csharp' $match.Groups['path'].Value
        })
    $updated = [regex]::Replace($updated, $outputPattern, [System.Text.RegularExpressions.MatchEvaluator] {
            param($match)
            & $evaluator $match 'text' (Join-Path $match.Groups['name'].Value 'expected-output.txt')
        })

    if ($updated -ceq $original) {
        continue
    }

    if ($Check) {
        Write-Host "  OUT OF DATE $($article.Name)" -ForegroundColor Red
        $problems.Add("$($article.Name) is out of date with samples/")
    }
    else {
        Write-Text $article.FullName $updated
        Write-Host "  updated  $($article.Name)" -ForegroundColor Yellow
    }
}

# ---- 4. 結果 ----
if ($problems.Count -gt 0) {
    Write-Host ''
    $problems | ForEach-Object { Write-Host "ERROR: $_" -ForegroundColor Red }
    Write-Host 'Run "pwsh tools/sync-samples.ps1" (without -Check) and commit the result.' -ForegroundColor Red
    exit 1
}

Write-Host 'All samples and articles are in sync.' -ForegroundColor Green
