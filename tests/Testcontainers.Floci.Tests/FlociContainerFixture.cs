using System.Threading.Tasks;
using Xunit;

namespace Testcontainers.Floci.Tests;

[CollectionDefinition("Floci")]
public class FlociCollection : ICollectionFixture<FlociContainerFixture> { }

public sealed class FlociContainerFixture : IAsyncLifetime
{
    public FlociContainer Container { get; } = new FlociBuilder("floci/floci:1.5.13").Build();

    public ValueTask InitializeAsync() => new(Container.StartAsync());

    public ValueTask DisposeAsync() => Container.DisposeAsync();
}
