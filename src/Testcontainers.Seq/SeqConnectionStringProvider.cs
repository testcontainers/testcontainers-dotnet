namespace Testcontainers.Seq;

/// <summary>
/// Provides the Redpanda connection string.
/// </summary>
internal sealed class SeqConnectionStringProvider : ContainerConnectionStringProvider<SeqContainer, SeqConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetEndpoint();
    }
}