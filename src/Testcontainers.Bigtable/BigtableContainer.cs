namespace Testcontainers.Bigtable;

/// <inheritdoc cref="DockerContainer"/>
[PublicAPI]
public class BigtableContainer: DockerContainer
{
  public BigtableContainer(IContainerConfiguration configuration, ILogger logger) : base(configuration, logger)
  {
  }

  public string GetEndpoint() => $"127.0.0.1:{GetMappedPublicPort(9000)}";
}
