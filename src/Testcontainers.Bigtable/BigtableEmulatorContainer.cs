namespace Testcontainers.Bigtable;

/// <inheritdoc cref="DockerContainer"/>
[PublicAPI]
public class BigtableEmulatorContainer: DockerContainer
{
  public readonly string ProjectId = "project-id";

  public readonly string InstanceId = "instance-id";
  public BigtableEmulatorContainer(IContainerConfiguration configuration, ILogger logger) : base(configuration, logger)
  {
  }

  public string GetEndpoint() => $"127.0.0.1:{GetMappedPublicPort(9000)}";
}
