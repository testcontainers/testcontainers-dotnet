namespace Testcontainers.Triton;

/// <summary>
/// Provides the Triton connection string.
/// </summary>
internal sealed class TritonConnectionStringProvider : ContainerConnectionStringProvider<TritonContainer, TritonConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetHttpEndpoint();
    }
}
