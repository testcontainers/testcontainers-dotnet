namespace Testcontainers.Azurite;

/// <summary>
/// Provides the Azurite connection string.
/// </summary>
internal sealed class AzuriteConnectionStringProvider : ContainerConnectionStringProvider<AzuriteContainer, AzuriteConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}