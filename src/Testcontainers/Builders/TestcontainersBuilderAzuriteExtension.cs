namespace DotNet.Testcontainers.Builders
{
  using System.Collections.ObjectModel;
  using DotNet.Testcontainers.Configurations.Modules.Databases;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <summary>
  /// This class applies the extended Testcontainer configurations for Azurite.
  /// </summary>
  [PublicAPI]
  public static class TestcontainersBuilderAzuriteExtension
  {
    public static ITestcontainersBuilder<AzuriteTestcontainer> WithAzurite(this ITestcontainersBuilder<AzuriteTestcontainer> builder, AzuriteTestcontainerConfiguration configuration)
    {
      return builder
        .WithImage(configuration.Image)
        .WithEnvironment(new ReadOnlyDictionary<string, string>(configuration.Environments))
        .WithPortBinding(configuration.BlobPort, AzuriteTestcontainerConfiguration.DefaultBlobPort)
        .WithPortBinding(configuration.QueuePort, AzuriteTestcontainerConfiguration.DefaultQueuePort)
        .WithPortBinding(configuration.TablePort, AzuriteTestcontainerConfiguration.DefaultTablePort)
        .WithOutputConsumer(configuration.OutputConsumer)
        .WithWaitStrategy(configuration.WaitStrategy)
        .ConfigureContainer(container =>
        {
          container.ContainerBlobPort = AzuriteTestcontainerConfiguration.DefaultBlobPort;
          container.ContainerQueuePort = AzuriteTestcontainerConfiguration.DefaultQueuePort;
          container.ContainerTablePort = AzuriteTestcontainerConfiguration.DefaultTablePort;
        });
    }
  }
}
