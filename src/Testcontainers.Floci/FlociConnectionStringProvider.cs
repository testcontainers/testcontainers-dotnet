namespace Testcontainers.Floci;

/// <summary>
/// Provides the Floci connection string.
/// </summary>
internal sealed class FlociConnectionStringProvider : ContainerConnectionStringProvider<FlociContainer, FlociConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}