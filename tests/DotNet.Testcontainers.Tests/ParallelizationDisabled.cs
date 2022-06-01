namespace DotNet.Testcontainers.Tests;

using Xunit;

[CollectionDefinition(nameof(ParallelizationDisabled), DisableParallelization = true)]
public class ParallelizationDisabled
{

}
