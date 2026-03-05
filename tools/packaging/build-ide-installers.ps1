[CmdletBinding()]
param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    [string]$OutputRoot = "",
    [switch]$SkipAnalyzerTests,
    [switch]$SkipVsCodeTests,
    [switch]$SkipVisualStudioVsix,
    [switch]$SkipVsCodePackage
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true

function Write-Step([string]$Message) {
    Write-Host ""
    Write-Host "==> $Message"
}

function Resolve-CommandPath([string]$Name) {
    $command = Get-Command $Name -ErrorAction SilentlyContinue
    if ($null -eq $command) {
        return $null
    }

    return $command.Source
}

function Invoke-Checked([string]$Description, [scriptblock]$Action) {
    & $Action
    if ($LASTEXITCODE -ne 0) {
        throw "$Description failed with exit code $LASTEXITCODE."
    }
}

function Get-VsixArtifactPath([string]$RootPath, [string]$ConfigurationName) {
    $candidate = Get-ChildItem -Path $RootPath -Filter *.vsix -Recurse |
        Where-Object { $_.FullName -like "*\bin\$ConfigurationName\*" } |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 1

    if ($null -eq $candidate) {
        throw "No VSIX artifact found in '$RootPath' for configuration '$ConfigurationName'."
    }

    return $candidate.FullName
}

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = (Resolve-Path (Join-Path $scriptRoot "..\..")).Path
$roslynRoot = Join-Path $repoRoot "nmolecules-roslyn"
$vscodeRoot = Join-Path $repoRoot "nmolecules-vscode"

if ([string]::IsNullOrWhiteSpace($OutputRoot)) {
    $OutputRoot = Join-Path $repoRoot "artifacts\installers"
}

$outputRootPath = (Resolve-Path -Path (New-Item -ItemType Directory -Force -Path $OutputRoot).FullName).Path
$visualStudioOutput = Join-Path $outputRootPath "visual-studio"
$vsCodeOutput = Join-Path $outputRootPath "vscode"

New-Item -ItemType Directory -Force -Path $visualStudioOutput | Out-Null
New-Item -ItemType Directory -Force -Path $vsCodeOutput | Out-Null

$visualStudioVsixPath = Join-Path $visualStudioOutput "nMolecules.Analyzers.VisualStudio.vsix"
$vsCodeVsixPath = Join-Path $vsCodeOutput "nMolecules.VSCode.vsix"

Write-Step "Running static VSIX metadata validation"
Invoke-Checked "VSIX metadata validation" {
    & pwsh -NoProfile -File (Join-Path $roslynRoot "tools\validate-vsix-metadata.ps1")
}

if (-not $SkipAnalyzerTests) {
    Write-Step "Running analyzer test suite"
    Invoke-Checked "Analyzer tests" {
        & dotnet test (Join-Path $roslynRoot "test\nMolecules.Analyzers.Test\nMolecules.Analyzers.Test.csproj") -v minimal
    }
}

if (-not $SkipVsCodeTests) {
    Write-Step "Running VS Code extension tests"
    Invoke-Checked "VS Code extension tests" {
        & npm test --prefix $vscodeRoot
    }
}

if (-not $SkipVisualStudioVsix) {
    Write-Step "Building Visual Studio VSIX package"
    $vsixProjectPath = Join-Path $roslynRoot "src\nMolecules.Analyzers\nMolecules.Analyzers.Vsix\nMolecules.Analyzers.Vsix.csproj"
    $msbuildPath = Resolve-CommandPath "msbuild.exe"

    if ($null -eq $msbuildPath) {
        throw "msbuild.exe not found. Install Visual Studio Build Tools or rerun with -SkipVisualStudioVsix."
    }

    Invoke-Checked "Visual Studio VSIX build" {
        & $msbuildPath $vsixProjectPath /restore /p:Configuration=$Configuration /p:DeployExtension=false
    }

    $builtVsixPath = Get-VsixArtifactPath -RootPath (Split-Path -Parent $vsixProjectPath) -ConfigurationName $Configuration
    Copy-Item -Path $builtVsixPath -Destination $visualStudioVsixPath -Force
    Write-Host "Visual Studio package: $visualStudioVsixPath"
}
else {
    Write-Step "Skipping Visual Studio VSIX package build"
}

if (-not $SkipVsCodePackage) {
    Write-Step "Packaging VS Code extension VSIX"
    if (Test-Path $vsCodeVsixPath) {
        Remove-Item -Path $vsCodeVsixPath -Force
    }

    Push-Location $vscodeRoot
    try {
        Invoke-Checked "VS Code VSIX package build" {
            & npm exec --yes --package @vscode/vsce -- vsce package --out $vsCodeVsixPath
        }
    }
    finally {
        Pop-Location
    }

    Write-Host "VS Code package: $vsCodeVsixPath"
}
else {
    Write-Step "Skipping VS Code VSIX package build"
}

Write-Step "Publishing Visual Studio setup executable"
$visualStudioSetupProject = Join-Path $repoRoot "tools\installers\nMolecules.Setup.VisualStudio\nMolecules.Setup.VisualStudio.csproj"
$visualStudioSetupPublish = Join-Path $visualStudioOutput "setup"
New-Item -ItemType Directory -Force -Path $visualStudioSetupPublish | Out-Null

Invoke-Checked "Visual Studio setup publish" {
    & dotnet publish $visualStudioSetupProject -c $Configuration -r win-x64 --self-contained false /p:PublishSingleFile=true /p:DebugType=none -o $visualStudioSetupPublish
}

if (Test-Path $visualStudioVsixPath) {
    Copy-Item -Path $visualStudioVsixPath -Destination (Join-Path $visualStudioSetupPublish (Split-Path -Leaf $visualStudioVsixPath)) -Force
}

$visualStudioCmdPath = Join-Path $visualStudioSetupPublish "install-visual-studio-extension.cmd"
$visualStudioCmd = @"
@echo off
set SCRIPT_DIR=%~dp0
"%SCRIPT_DIR%nMolecules.Setup.VisualStudio.exe" --vsix "%SCRIPT_DIR%nMolecules.Analyzers.VisualStudio.vsix"
"@
Set-Content -Path $visualStudioCmdPath -Value $visualStudioCmd -Encoding ASCII

Write-Step "Publishing VS Code setup executable"
$vsCodeSetupProject = Join-Path $repoRoot "tools\installers\nMolecules.Setup.VSCode\nMolecules.Setup.VSCode.csproj"
$vsCodeSetupPublish = Join-Path $vsCodeOutput "setup"
New-Item -ItemType Directory -Force -Path $vsCodeSetupPublish | Out-Null

Invoke-Checked "VS Code setup publish" {
    & dotnet publish $vsCodeSetupProject -c $Configuration -r win-x64 --self-contained false /p:PublishSingleFile=true /p:DebugType=none -o $vsCodeSetupPublish
}

if (Test-Path $vsCodeVsixPath) {
    Copy-Item -Path $vsCodeVsixPath -Destination (Join-Path $vsCodeSetupPublish (Split-Path -Leaf $vsCodeVsixPath)) -Force
}

$vsCodeCmdPath = Join-Path $vsCodeSetupPublish "install-vscode-extension.cmd"
$vsCodeCmd = @"
@echo off
set SCRIPT_DIR=%~dp0
"%SCRIPT_DIR%nMolecules.Setup.VSCode.exe" --vsix "%SCRIPT_DIR%nMolecules.VSCode.vsix"
"@
Set-Content -Path $vsCodeCmdPath -Value $vsCodeCmd -Encoding ASCII

Write-Step "Build complete"
Write-Host "Artifacts root: $outputRootPath"
Write-Host "Visual Studio setup: $visualStudioSetupPublish"
Write-Host "VS Code setup: $vsCodeSetupPublish"
