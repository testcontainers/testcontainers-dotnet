# This script checks modified files and returns a list of test projects to run.
# dotnet test ..

# How to test this script, run it with the required environment variables:

# 1. A file from root dir is modified, but not a .props file or a .cake file.
#    ALL_CHANGED_FILES="README.md" pwsh scripts/List-ChangedModules.ps1
#    The output should be an empty list.

# 2. A .cake file or a file in .cake-scripts dir modified.
#    ALL_CHANGED_FILES="build.cake credentials.cake" pwsh scripts/List-ChangedModules.ps1
#    The output should be all modules.

# 3. A .props file in root dir modified.
#    ALL_CHANGED_FILES="Directory.Build.props" pwsh scripts/List-ChangedModules.ps1
#    The output should be all modules.

# 4. A file in core Testcontainers dir is modified.
#    ALL_CHANGED_FILES="src/Testcontainers/Resource.cs" pwsh scripts/List-ChangedModules.ps1
#    The output should be all modules.

# 5. A file in any module dir is modified.
#    ALL_CHANGED_FILES="src/Testcontainers.ActiveMq/ArtemisBuilder.cs" pwsh scripts/List-ChangedModules.ps1
#    The output should be: @(Testcontainers.ActiveMq.Tests).

# 6. A file in any test dir is modified.
#    ALL_CHANGED_FILES="tests/Testcontainers.ClickHouse.Tests/ClickHouseContainerTest.cs" pwsh scripts/List-ChangedModules.ps1
#    The output should be: @(Testcontainers.ClickHouse.Tests).

# 7. Some files in a few modules is modified.
#    ALL_CHANGED_FILES="src/Testcontainers.ActiveMq/ArtemisBuilder.cs src/Testcontainers.ClickHouse/Testcontainers.ClickHouse.csproj" pwsh scripts/List-ChangedModules.ps1
#    The output should be: @(Testcontainers.ActiveMq.Tests, Testcontainers.ClickHouse.Tests).

# 8. A file in module and a file in core project is modified.
#    ALL_CHANGED_FILES="src/Testcontainers.ActiveMq/ArtemisBuilder.cs src/Testcontainers/Resource.cs" pwsh scripts/List-ChangedModules.ps1
#    The output should be all modules.

# 9. This script is modified.
#    ALL_CHANGED_FILES="scripts/List-ChangedModules.ps1" pwsh scripts/List-ChangedModules.ps1
#    The output should be an empty list.

# 10. A .github file is modified.
#    ALL_CHANGED_FILES=".github/release-drafter.yml" pwsh scripts/List-ChangedModules.ps1
#    The output should be an empty list.

# 11. A file in any module dir is modified + a file in a core TC dir is modified + an excluded file is modified.
#    ALL_CHANGED_FILES="src/Testcontainers.ActiveMq/ArtemisBuilder.cs src/Testcontainers/Resource.cs examples/Flyway/Directory.Build.props" pwsh scripts/List-ChangedModules.ps1
#    The output should be all modules.

# 12. A file in an excluded dir is modified.
#    ALL_CHANGED_FILES="examples/Flyway/Directory.Build.props" pwsh scripts/List-ChangedModules.ps1
#    The output should be an empty list.



$ErrorActionPreference = "Stop"

# ignore any files in this dirs
$excludedDirs = @(
    "examples/"
)
# if a file with extension like this was changed, we run tests for all modules
$wellKnownImportantFilesExt = @(
    ".cake",
    ".props"
)
# if a file inside this dirs was changed, we run tests for all modules
$wellKnownImportantDirs = @(
    "src/Testcontainers/"
)

# get a string from ALL_CHANGED_FILES env variable, split it, remove empty values, and remove duplicates
$allChangesFiles = $Env:ALL_CHANGED_FILES -Split '\s+' | Where-Object { $_ } | Select -Unique

# remove files in excluded dirs from $allChangesFiles
foreach ($excDir in $excludedDirs)
{
    $allChangesFiles = $allChangesFiles.Where({ -not $_.StartsWith($excDir) })
}

# if no files were changed, return empty list
if ($allChangesFiles.Length -eq 0)
{
    return @()
}

# 'src' directory resolved path
$srcDir = Resolve-Path -Path '../src' -RelativeBasePath $PSScriptRoot

# all child directories which contain modules plus main module directory
# excluding folder 'Templates'
$modulesDirs = Get-ChildItem -Path $srcDir -Include *.csproj -Exclude Testcontainers.ModuleName.csproj -Recurse

# Will produce list:
# Testcontainers
# Testcontainers.ActiveMq
# ...
$modulesDirsNames = 
    ( $modulesDirs
    | Select-Object -ExpandProperty Directory
    | Select-Object Name).Name

foreach ($impDir in $wellKnownImportantDirs)
{
    # if any file is from $wellKnownImportantDirs - we run all tests
    if ($allChangesFiles.Where({ $_.StartsWith($impDir) }).Count -gt 0)
    {
        return $modulesDirsNames | ForEach-Object { $_ + ".Tests" }
    }
}

foreach ($impExt in $wellKnownImportantFilesExt)
{
    # if any file has ext from $wellKnownImportantFilesExt - we run all tests
    if ($allChangesFiles.Where({ $_.EndsWith($impExt) }).Count -gt 0)
    {
        return $modulesDirsNames | ForEach-Object { $_ + ".Tests" }
    }
}

$runTestsOnModules = [System.Collections.Generic.List[string]]::new()
# also check tests dir ($modulesDirsNames will contain both src and tests)
$modulesDirsNames = $modulesDirsNames + ($modulesDirsNames | ForEach-Object { $_ + ".Tests" })
foreach ($changedFile in $allChangesFiles)
{
    foreach ($moduleDirName in $modulesDirsNames)
    {
        if ($changedFile.Contains($moduleDirName + "/"))
        {
            if ($moduleDirName.EndsWith(".Tests"))
            {
                $runTestsOnModules.Add($moduleDirName)
            }
            else
            {
                $runTestsOnModules.Add($moduleDirName + ".Tests")
            }
            break;
        }
    }
}

# sort, remove duplicates and return list of test projects
return $runTestsOnModules | Sort-Object | Select -Unique
