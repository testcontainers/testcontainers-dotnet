namespace Testcontainers.Seq;

/// <summary>
/// Provides the Seq connection string.
/// </summary>
internal sealed class SeqConnectionStringProvider : ContainerConnectionStringProvider<SeqContainer, SeqConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetEndpoint();
    }
}