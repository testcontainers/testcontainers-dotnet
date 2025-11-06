<#
    .SYNOPSIS
    Filters test projects based on changed files to optimize CI workflow execution.

    .DESCRIPTION
    Analyzes changed files and determines which test projects need to run. Protected
    branches and global repository changes run all tests. Other branches run only
    affected modules.

    .EXAMPLE
    # Test protected branch (runs all tests):
    $env:GITHUB_REF_NAME="main"; $env:ALL_CHANGED_FILES="README.md"; $input | Filter-TestProjects

    .EXAMPLE
    # Test core library changes (runs all tests):
    $env:GITHUB_REF_NAME="1/merge"; $env:ALL_CHANGED_FILES="src/Testcontainers/Testcontainers.csproj"; $input | Filter-TestProjects

    .EXAMPLE
    # Test module-specific changes (runs only affected module):
    $env:GITHUB_REF_NAME="2/merge"; $env:ALL_CHANGED_FILES="src/Testcontainers.Redis/RedisBuilder.cs"; $input | Filter-TestProjects
#>

$PROTECTED_BRANCHES = @(
    "main",
    "develop"
)

$GLOBAL_PATTERNS = @(
    "^\.github/scripts/",
    "^\.github/workflows/",
    "^build/",
    "^Directory\.Build\.props$",
    "^Directory\.Packages\.props$",
    "^src/Testcontainers/"
)

$DATABASE_MODULES = @(
    "Testcontainers.Cassandra",
    "Testcontainers.ClickHouse",
    "Testcontainers.CockroachDb",
    "Testcontainers.Db2",
    "Testcontainers.FirebirdSql",
    "Testcontainers.MariaDb",
    "Testcontainers.MsSql",
    "Testcontainers.MySql",
    "Testcontainers.Oracle",
    "Testcontainers.PostgreSql",
    "Testcontainers.Xunit",
    "Testcontainers.XunitV3"
)

$ORACLE_MODULES = @(
    "Testcontainers.Oracle",
    "Testcontainers.Oracle11",
    "Testcontainers.Oracle18",
    "Testcontainers.Oracle21",
    "Testcontainers.Oracle23"
)

$XUNIT_MODULES = @(
    "Testcontainers.Xunit",
    "Testcontainers.XunitV3"
)

function Should-RunTests {
    param ([string]$ModuleName)

    # Rule 1: Protected branches always run all tests.
    # Ensures main/develop branches have full test coverage.
    If ($script:branch -In $PROTECTED_BRANCHES) {
        Write-Host "Running '$ModuleName': protected branch '$script:branch'."
        return $True
    }

    # Rule 2: Global changes affect all modules.
    ForEach ($pattern In $GLOBAL_PATTERNS) {
        If ($script:allChangedFiles | Where-Object { $_ -Match $pattern }) {
            Write-Host "Running '$ModuleName': global changes detected ($pattern)."
            return $True
        }
    }

    # Rule 3: Module-specific changes.
    If ($script:allChangedFiles | Where-Object { $_ -Match "^(src|tests)/$ModuleName" }) {
        Write-Host "Running '$ModuleName': module-specific changes detected."
        return $True
    }

    # Rule 4: Shared database tests for ADO.NET compatible modules.
    If ($ModuleName -In $DATABASE_MODULES -And ($script:allChangedFiles | Where-Object { $_ -Match '^tests/Testcontainers\.Databases\.Tests' })) {
        Write-Host "Running '$ModuleName': database test changes detected."
        return $True
    }

    # Rule 5: Oracle integration variants.
    If ($ModuleName -In $ORACLE_MODULES -And ($script:allChangedFiles | Where-Object { $_ -Match '^(src|tests)/Testcontainers\.Oracle' })) {
        Write-Host "Running '$ModuleName': Oracle module changes detected."
        return $True
    }

    # Rule 6: xUnit integration variants.
    If ($ModuleName -In $XUNIT_MODULES -And ($script:allChangedFiles | Where-Object { $_ -Match '^(src|tests)/Testcontainers\.Xunit(V3)?' })) {
        Write-Host "Running '$ModuleName': xUnit module changes detected."
        return $True
    }

    Write-Host "Skipping '$ModuleName': no relevant changes detected."
    return $False
}

function Filter-TestProjects {
    [CmdletBinding()]
    Param (
        [Parameter(ValueFromPipeline = $True)]
        $TestProject
    )

    Begin {
        $script:branch = $env:GITHUB_REF_NAME
        $script:allChangedFiles = $env:ALL_CHANGED_FILES -Split "[\s\t\n]"
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
