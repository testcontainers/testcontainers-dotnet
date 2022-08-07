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
      builder = builder
        .WithImage(configuration.Image)
        .WithEnvironment(new ReadOnlyDictionary<string, string>(configuration.Environments))
        .WithOutputConsumer(configuration.OutputConsumer)
        .WithWaitStrategy(configuration.WaitStrategy)
        .ConfigureContainer(container =>
        {
          container.ContainerBlobPort = configuration.RunBlobOnly || configuration.RunAllServices ? AzuriteTestcontainerConfiguration.DefaultBlobPort : 0;
          container.ContainerQueuePort = configuration.RunQueueOnly || configuration.RunAllServices ? AzuriteTestcontainerConfiguration.DefaultQueuePort : 0;
          container.ContainerTablePort = configuration.RunTableOnly || configuration.RunAllServices ? AzuriteTestcontainerConfiguration.DefaultTablePort : 0;
        });

      if (configuration.RunBlobOnly || configuration.RunAllServices)
      {
        builder = builder.WithPortBinding(configuration.BlobPort, AzuriteTestcontainerConfiguration.DefaultBlobPort);
      }

      if (configuration.RunQueueOnly || configuration.RunAllServices)
      {
        builder = builder.WithPortBinding(configuration.QueuePort, AzuriteTestcontainerConfiguration.DefaultQueuePort);
      }

      if (configuration.RunTableOnly || configuration.RunAllServices)
      {
        builder = builder.WithPortBinding(configuration.TablePort, AzuriteTestcontainerConfiguration.DefaultTablePort);
      }

      if (configuration.RunBlobOnly)
      {
        builder = builder.WithCommand("azurite-blob", "--blobHost", AzuriteTestcontainerConfiguration.DefaultBlobEndpoint);
      }

      if (configuration.RunQueueOnly)
      {
        builder = builder.WithCommand("azurite-queue", "--queueHost", AzuriteTestcontainerConfiguration.DefaultQueueEndpoint);
      }

      if (configuration.RunTableOnly)
      {
        builder = builder.WithCommand("azurite-table", "--tableHost", AzuriteTestcontainerConfiguration.DefaultTableEndpoint);
      }

      return builder;
    }
  }
}
