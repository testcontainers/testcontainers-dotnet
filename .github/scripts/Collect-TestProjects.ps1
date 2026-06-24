<#
    .SYNOPSIS
    Retrieves the GitHub Actions 'runs-on' configuration for each test project
    and ensures all test projects have a valid configuration.

    .DESCRIPTION
    Scans all test projects under 'tests', reads '.runs-on' for CI runner configuration,
    throws an error if missing, and outputs filtered projects as compressed JSON.
#>

# Get 'runs-on' configuration for each test project.
$testProjects = Get-ChildItem -Path 'tests' -Directory `
    | ? { $_.Name -Match '\.Tests$' } `
    | % { $testProject = $_.Name; Join-Path -Path $_.FullName -ChildPath '.runs-on' } `
    | % { If (Test-Path -LiteralPath $_) { Get-Content -LiteralPath $_ } Else { $Null } } `
    | % { [PSCustomObject]@{ 'name' = ($testProject -Replace '\.Tests$', ''); 'runs-on' = [string]$_ } }

# Validate that all projects have a '.runs-on' configuration.
$runsOnNotFound = $testProjects `
    | Where-Object 'runs-on' -Eq '' `
    | Select-Object -ExpandProperty 'name'

If ($runsOnNotFound) {
    Write-Error "Please add a '.runs-on' configuration file to the test project:`n  $($runsOnNotFound -Join "`n  ")"
    Exit 1
}

# Filter projects and output as compressed JSON.
$filteredTestProjects = $testProjects | & (Join-Path $PSScriptRoot 'Filter-TestProjects.ps1') | ConvertTo-Json -AsArray -Compress
$filteredTestProjects ?? "[]"
