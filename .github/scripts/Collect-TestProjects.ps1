# Retrieves the GH workflow 'runs-on' configuration for each test project.
# During runtime, the strategy is passed to the CI job, allowing test projects to
# run parallel on individual runners.
$testProjects = Get-ChildItem -Path 'tests' -Directory `
    | % { $testProject = $_.Name; Join-Path -Path $_.FullName -ChildPath '.runs-on' } `
    | % { If (Test-Path -LiteralPath $_) { Get-Content -LiteralPath $_ } Else { $Null } } `
    | % { [PSCustomObject]@{ 'name' = ($testProject -Replace '\.Tests$', ''); 'runs-on' = [string]$_ } }

# Checks if any test project does not contain a valid '.runs-on' configuration.
# If a project is missing this configuration, an error is thrown to prevent
# developers from forgetting to add the configuration.
$runsOnNotFound = $testProjects `
    | Where-Object 'runs-on' -Eq '' `
    | Select-Object -ExpandProperty 'name'

If ($runsOnNotFound)
{
    Write-Error "Please add a '.runs-on' configuration file to the test project:`n  $($runsOnNotFound -Join "`n  ")"
    Exit 1
}

$testProjects | ConvertTo-Json -Compress