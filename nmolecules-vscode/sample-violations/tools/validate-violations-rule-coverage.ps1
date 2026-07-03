param(
    [string]$WorkspaceRoot = ""
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $false

$workspacePath = if ([string]::IsNullOrWhiteSpace($WorkspaceRoot)) {
    Resolve-Path -LiteralPath (Join-Path $PSScriptRoot "..")
} else {
    Resolve-Path -LiteralPath $WorkspaceRoot
}
$repoRoot = Resolve-Path -LiteralPath (Join-Path $workspacePath "..\..\..")

$projects = @(
    "nmolecules-integrations/nmolecules-vscode/sample-violations/src/Banking.Violations.Domain/Banking.Violations.Domain.csproj",
    "nmolecules-integrations/nmolecules-vscode/sample-violations/src/Banking.Violations.Application/Banking.Violations.Application.csproj",
    "nmolecules-integrations/nmolecules-vscode/sample-violations/src/Banking.Violations.Infrastructure/Banking.Violations.Infrastructure.csproj",
    "nmolecules-integrations/nmolecules-vscode/sample-violations/src/Banking.Violations.RuleMatrix/Banking.Violations.RuleMatrix.csproj",
    "nmolecules-integrations/nmolecules-vscode/sample-violations/src/Banking.Violations.CqrsOnly/Banking.Violations.CqrsOnly.csproj",
    "nmolecules-integrations/nmolecules-vscode/sample-violations/src/Banking.Violations.MetadataMissing/Banking.Violations.MetadataMissing.csproj",
    "nmolecules-integrations/nmolecules-vscode/sample-violations/src/Banking.Violations.MetadataConsistency/Banking.Violations.MetadataConsistency.csproj"
)

$buildRuleIds = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
$buildArgs = @("build", "-t:Rebuild", "-v", "minimal")

foreach ($relativeProject in $projects) {
    $projectPath = Join-Path $repoRoot $relativeProject
    $output = & dotnet $buildArgs $projectPath 2>&1
    $matches = [regex]::Matches(($output | Out-String), "XMolecules[A-Za-z]+\d{4}")
    foreach ($match in $matches) {
        [void]$buildRuleIds.Add($match.Value)
    }
}

$ruleContent = & rg --line-number "XMolecules[A-Za-z]+\d{4}" (Join-Path $repoRoot "nmolecules-integrations/nmolecules-roslyn/src/nMolecules.Analyzers/nMolecules.Analyzers") | Out-String
$ruleMatches = [regex]::Matches($ruleContent, "XMolecules[A-Za-z]+\d{4}")
$allRuleIds = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
foreach ($match in $ruleMatches) {
    [void]$allRuleIds.Add($match.Value)
}

$missing = @($allRuleIds | Where-Object { -not $buildRuleIds.Contains($_) } | Sort-Object)

Write-Host "Violation workspace rule IDs: $($buildRuleIds.Count)"
Write-Host "Analyzer code rule IDs:       $($allRuleIds.Count)"
Write-Host "Missing IDs:                  $($missing.Count)"

if ($missing.Count -gt 0) {
    $missing | ForEach-Object { Write-Host "  $_" }
    exit 1
}

exit 0
