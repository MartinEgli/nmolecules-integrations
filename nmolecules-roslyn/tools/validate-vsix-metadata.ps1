[CmdletBinding()]
param()

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$manifestPath = Join-Path $repoRoot "src\nMolecules.Analyzers\nMolecules.Analyzers.Vsix\source.extension.vsixmanifest"
$vsixProjectPath = Join-Path $repoRoot "src\nMolecules.Analyzers\nMolecules.Analyzers.Vsix\nMolecules.Analyzers.Vsix.csproj"

if (-not (Test-Path $manifestPath)) {
    throw "VSIX manifest not found: $manifestPath"
}

if (-not (Test-Path $vsixProjectPath)) {
    throw "VSIX project not found: $vsixProjectPath"
}

[xml]$manifest = Get-Content $manifestPath
$namespace = New-Object System.Xml.XmlNamespaceManager($manifest.NameTable)
$namespace.AddNamespace("vsx", "http://schemas.microsoft.com/developer/vsx-schema/2011")
$namespace.AddNamespace("d", "http://schemas.microsoft.com/developer/vsx-schema-design/2011")

$expectedTargets = @(
    @{ Id = "Microsoft.VisualStudio.Community"; Version = "[17.0,18.0)" },
    @{ Id = "Microsoft.VisualStudio.Community"; Version = "[18.0,19.0)" },
    @{ Id = "Microsoft.VisualStudio.Pro"; Version = "[17.0,18.0)" },
    @{ Id = "Microsoft.VisualStudio.Pro"; Version = "[18.0,19.0)" },
    @{ Id = "Microsoft.VisualStudio.Enterprise"; Version = "[17.0,18.0)" },
    @{ Id = "Microsoft.VisualStudio.Enterprise"; Version = "[18.0,19.0)" }
)

$failures = New-Object System.Collections.Generic.List[string]

foreach ($target in $expectedTargets) {
    $node = $manifest.SelectSingleNode("//vsx:Installation/vsx:InstallationTarget[@Id='$($target.Id)' and @Version='$($target.Version)']", $namespace)
    if ($null -eq $node) {
        $failures.Add("Missing installation target: $($target.Id) $($target.Version)")
        continue
    }

    $archNode = $node.SelectSingleNode("vsx:ProductArchitecture", $namespace)
    if ($null -eq $archNode -or $archNode.InnerText -ne "amd64") {
        $failures.Add("Installation target must declare amd64 architecture: $($target.Id) $($target.Version)")
    }
}

$expectedAssets = @(
    @{ Type = "Microsoft.VisualStudio.MefComponent"; Project = "nMolecules.Analyzers" },
    @{ Type = "Microsoft.VisualStudio.Analyzer"; Project = "nMolecules.Analyzers" },
    @{ Type = "Microsoft.VisualStudio.MefComponent"; Project = "nMolecules.Analyzers.CodeFixes" },
    @{ Type = "Microsoft.VisualStudio.Analyzer"; Project = "nMolecules.Analyzers.CodeFixes" }
)

foreach ($asset in $expectedAssets) {
    $assetNode = $manifest.SelectSingleNode("//vsx:Assets/vsx:Asset[@Type='$($asset.Type)' and @d:ProjectName='$($asset.Project)']", $namespace)
    if ($null -eq $assetNode) {
        $failures.Add("Missing VSIX asset: $($asset.Type) for project $($asset.Project)")
    }
}

[xml]$vsixProject = Get-Content $vsixProjectPath
$projectReferences = $vsixProject.SelectNodes("//Project/ItemGroup/ProjectReference[@Include]") |
    ForEach-Object { $_.GetAttribute("Include") }

if ($projectReferences -notcontains "..\nMolecules.Analyzers\nMolecules.Analyzers.csproj") {
    $failures.Add("VSIX project is missing analyzer project reference.")
}

if ($projectReferences -notcontains "..\nMolecules.Analyzers.CodeFixes\nMolecules.Analyzers.CodeFixes.csproj") {
    $failures.Add("VSIX project is missing code-fixes project reference.")
}

if ($failures.Count -gt 0) {
    Write-Host "VSIX metadata validation failed:"
    foreach ($failure in $failures) {
        Write-Host "  - $failure"
    }

    throw "VSIX metadata validation failed."
}

Write-Host "VSIX metadata validation passed."
