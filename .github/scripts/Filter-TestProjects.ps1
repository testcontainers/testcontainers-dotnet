<#
.SYNOPSIS
Filters test projects for CI workflows.

.DESCRIPTION
Receives test project objects from the pipeline and returns them.
Currently, all projects are passed through unchanged.
#>

Param (
    [Parameter(ValueFromPipeline)]
    $InputObject
)

Process {
    # Return test projects unchanged.
    $InputObject
}
