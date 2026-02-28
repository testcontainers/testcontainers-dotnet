namespace Testcontainers.FakeGcsServer;

/// <summary>
/// Provides the FakeGcsServer connection string.
/// </summary>
internal sealed class FakeGcsServerConnectionStringProvider : ContainerConnectionStringProvider<FakeGcsServerContainer, FakeGcsServerConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}