<#
    .SYNOPSIS
    Filters test projects for CI workflows.

    .DESCRIPTION
    Receives test project objects from the pipeline and returns them.
    Currently, all projects are passed through unchanged.
#>

$PROTECTED_BRANCHES = @(
    'main',
    'develop'
)

$GLOBAL_PATTERNS = @(
    '^\.github/scripts/',
    '^\.github/workflows/',
    '^build/',
    '^Directory\.Build\.props$',
    '^Directory\.Packages\.props$',
    '^src/Testcontainers/'
)

$DATABASE_MODULES = @(
    'Testcontainers.Cassandra',
    'Testcontainers.ClickHouse',
    'Testcontainers.CockroachDb',
    'Testcontainers.Db2',
    'Testcontainers.FirebirdSql',
    'Testcontainers.MariaDb',
    'Testcontainers.MsSql',
    'Testcontainers.MySql',
    'Testcontainers.Oracle',
    'Testcontainers.PostgreSql',
    'Testcontainers.Xunit',
    'Testcontainers.XunitV3'
)

$ORACLE_MODULES = @(
    'Testcontainers.Oracle',
    'Testcontainers.Oracle11',
    'Testcontainers.Oracle18',
    'Testcontainers.Oracle21',
    'Testcontainers.Oracle23'
)

$XUNIT_MODULES = @(
    'Testcontainers.Xunit',
    'Testcontainers.XunitV3'
)

function Should-RunTests {
    param ([string]$ModuleName)

    If ($script:branch -In $PROTECTED_BRANCHES) {
        Write-Host "Running '$ModuleName': protected branch '$script:branch'."
        return $true
    }

    ForEach ($pattern In $GLOBAL_PATTERNS) {
        If ($script:allChangedFiles | Where-Object { $_ -Match $pattern }) {
            Write-Host "Running '$ModuleName': global changes detected ($pattern)."
            return $true
        }
    }

    If ($script:allChangedFiles | Where-Object { $_ -Match "^(src|tests)/$ModuleName" }) {
        Write-Host "Running '$ModuleName': module-specific changes detected."
        return $true
    }

    If ($ModuleName -In $DATABASE_MODULES -And ($script:allChangedFiles | Where-Object { $_ -Match '^tests/Testcontainers\.Databases\.Tests' })) {
        Write-Host "Running '$ModuleName': database test changes detected."
        return $true
    }

    If ($ModuleName -In $ORACLE_MODULES -And ($script:allChangedFiles | Where-Object { $_ -Match '^(src|tests)/Testcontainers\.Oracle' })) {
        Write-Host "Running '$ModuleName': Oracle module changes detected."
        return $true
    }

    If ($ModuleName -In $XUNIT_MODULES -And ($script:allChangedFiles | Where-Object { $_ -Match '^(src|tests)/Testcontainers\.Xunit(V3)?' })) {
        Write-Host "Running '$ModuleName': xUnit module changes detected."
        return $true
    }

    Write-Host "Skipping '$ModuleName': no relevant changes detected."
    return $false
}

function Filter-TestProjects {
    [CmdletBinding()]
    Param (
        [Parameter(ValueFromPipeline = $true)]
        $TestProject
    )

    Begin {
        $script:branch = $env:GITHUB_REF_NAME
        $script:allChangedFiles = $env:ALL_CHANGED_FILES -Split "`n"
        $script:filteredModules = @()

        Write-Host "Filtering test projects for branch '$script:branch'."
        Write-Host "Analyzing $($script:allChangedFiles.Count) changed file(s)."
    }

    Process {
        If (Should-RunTests $TestProject.name) {
            $script:filteredModules += $TestProject
        }
    }

    End {
        Write-Host "Filtered $($script:filteredModules.Count) module(s) will run."
        $script:filteredModules
    }
}

$input | Filter-TestProjects
