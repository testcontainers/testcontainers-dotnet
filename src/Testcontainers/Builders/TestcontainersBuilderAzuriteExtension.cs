namespace DotNet.Testcontainers.Builders
{
  using System.Collections.Generic;
  using System.IO;
  using DotNet.Testcontainers.Configurations;
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
        .WithWaitStrategy(configuration.WaitStrategy)
        .ConfigureContainer(container =>
        {
          container.ContainerBlobPort = configuration.BlobServiceOnlyEnabled || configuration.AllServicesEnabled ? configuration.BlobContainerPort : 0;
          container.ContainerQueuePort = configuration.QueueServiceOnlyEnabled || configuration.AllServicesEnabled ? configuration.QueueContainerPort : 0;
          container.ContainerTablePort = configuration.TableServiceOnlyEnabled || configuration.AllServicesEnabled ? configuration.TableContainerPort : 0;
        });

      if (configuration.BlobServiceOnlyEnabled || configuration.AllServicesEnabled)
      {
        builder = builder
          .WithExposedPort(configuration.BlobContainerPort)
          .WithPortBinding(configuration.BlobPort, configuration.BlobContainerPort);
      }

      if (configuration.QueueServiceOnlyEnabled || configuration.AllServicesEnabled)
      {
        builder = builder
          .WithExposedPort(configuration.QueueContainerPort)
          .WithPortBinding(configuration.QueuePort, configuration.QueueContainerPort);
      }

      if (configuration.TableServiceOnlyEnabled || configuration.AllServicesEnabled)
      {
        builder = builder
          .WithExposedPort(configuration.TableContainerPort)
          .WithPortBinding(configuration.TablePort, configuration.TableContainerPort);
      }

      if (!string.IsNullOrWhiteSpace(configuration.Location))
      {
        builder = builder.WithBindMount(configuration.Location, AzuriteTestcontainerConfiguration.DefaultLocation);
      }

      return builder
        .WithCommand(GetMainCommand(configuration))
        .WithCommand(GetServiceEndpointArgs(configuration))
        .WithCommand(GetWorkspaceLocation())
        .WithCommand(GetSilentMode(configuration))
        .WithCommand(GetLooseMode(configuration))
        .WithCommand(GetSkipApiVersionCheck(configuration))
        .WithCommand(GetDisableProductStyleUrl(configuration))
        .WithCommand(GetDebugLog(configuration));
    }

    private static string[] GetDebugLog(AzuriteTestcontainerConfiguration configuration)
    {
      var debugLogFilePath = Path.Combine(AzuriteTestcontainerConfiguration.DefaultLocation, "debug.log");
      return configuration.DebugEnabled ? new[] { "--debug", debugLogFilePath } : null;
    }

    private static string GetDisableProductStyleUrl(AzuriteTestcontainerConfiguration configuration)
    {
      return configuration.ProductStyleUrlDisabled ? "--disableProductStyleUrl" : null;
    }

    private static string GetSkipApiVersionCheck(AzuriteTestcontainerConfiguration configuration)
    {
      return configuration.SkipApiVersionCheckEnabled ? "--skipApiVersionCheck" : null;
    }

    private static string GetLooseMode(AzuriteTestcontainerConfiguration configuration)
    {
      return configuration.LooseModeEnabled ? "--loose" : null;
    }

    private static string GetSilentMode(AzuriteTestcontainerConfiguration configuration)
    {
      return configuration.SilentModeEnabled ? "--silent" : null;
    }

    private static string[] GetWorkspaceLocation()
    {
      return new[] { "--location", AzuriteTestcontainerConfiguration.DefaultLocation };
    }

    private static string[] GetServiceEndpointArgs(AzuriteTestcontainerConfiguration configuration)
    {
      var args = new List<string>();

      if (configuration.BlobServiceOnlyEnabled || configuration.AllServicesEnabled)
      {
        args.Add("--blobHost");
        args.Add(AzuriteTestcontainerConfiguration.DefaultBlobEndpoint);
        args.Add("--blobPort");
        args.Add(configuration.BlobContainerPort.ToString());
      }

      if (configuration.QueueServiceOnlyEnabled || configuration.AllServicesEnabled)
      {
        args.Add("--queueHost");
        args.Add(AzuriteTestcontainerConfiguration.DefaultQueueEndpoint);
        args.Add("--queuePort");
        args.Add(configuration.QueueContainerPort.ToString());
      }

      if (configuration.TableServiceOnlyEnabled || configuration.AllServicesEnabled)
      {
        args.Add("--tableHost");
        args.Add(AzuriteTestcontainerConfiguration.DefaultTableEndpoint);
        args.Add("--tablePort");
        args.Add(configuration.TableContainerPort.ToString());
      }

      return args.ToArray();
    }

    private static string GetMainCommand(AzuriteTestcontainerConfiguration configuration)
    {
      if (configuration.BlobServiceOnlyEnabled)
      {
        return "azurite-blob";
      }

      if (configuration.QueueServiceOnlyEnabled)
      {
        return "azurite-queue";
      }

      if (configuration.TableServiceOnlyEnabled)
      {
        return "azurite-table";
      }

      return "azurite";
    }
  }
}
