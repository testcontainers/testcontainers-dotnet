namespace DotNet.Testcontainers.Builders
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.IO;
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

      if (!string.IsNullOrWhiteSpace(configuration.Location))
      {
        builder = builder.WithBindMount(configuration.Location, AzuriteTestcontainerConfiguration.DefaultLocation);
      }

      builder = builder
        .WithCommand(GetMainCommand(configuration))
        .WithCommand(GetServiceEndpointArgs(configuration))
        .WithCommand(GetWorkspaceLocation())
        .WithCommand(GetSilentMode(configuration))
        .WithCommand(GetLooseMode(configuration))
        .WithCommand(GetSkipApiVersionCheck(configuration))
        .WithCommand(GetDisableProductStyleUrl(configuration))
        .WithCommand(GetDebugLog(configuration));

      return builder;
    }

    private static string[] GetDebugLog(AzuriteTestcontainerConfiguration configuration)
    {
      var debugLogFilePath = Path.Combine(AzuriteTestcontainerConfiguration.DefaultLocation, "debug.log");
      return configuration.DisableProductStyleUrl ? new[] { "--debug", debugLogFilePath } : null;
    }

    private static string GetDisableProductStyleUrl(AzuriteTestcontainerConfiguration configuration)
    {
      return configuration.DisableProductStyleUrl ? "--disableProductStyleUrl" : null;
    }

    private static string GetSkipApiVersionCheck(AzuriteTestcontainerConfiguration configuration)
    {
      return configuration.SkipApiVersionCheck ? "--skipApiVersionCheck" : null;
    }

    private static string GetLooseMode(AzuriteTestcontainerConfiguration configuration)
    {
      return configuration.LooseMode ? "--loose" : null;
    }

    private static string GetSilentMode(AzuriteTestcontainerConfiguration configuration)
    {
      return configuration.Silent ? "--silent" : null;
    }

    private static string[] GetWorkspaceLocation()
    {
      return new[] { "--location", AzuriteTestcontainerConfiguration.DefaultLocation };
    }

    private static string[] GetServiceEndpointArgs(AzuriteTestcontainerConfiguration configuration)
    {
      var args = new List<string>();

      if (configuration.RunBlobOnly || configuration.RunAllServices)
      {
        args.Add("--blobHost");
        args.Add(AzuriteTestcontainerConfiguration.DefaultBlobEndpoint);
      }

      if (configuration.RunQueueOnly || configuration.RunAllServices)
      {
        args.Add("--queueHost");
        args.Add(AzuriteTestcontainerConfiguration.DefaultQueueEndpoint);
      }

      if (configuration.RunTableOnly || configuration.RunAllServices)
      {
        args.Add("--tableHost");
        args.Add(AzuriteTestcontainerConfiguration.DefaultTableEndpoint);
      }

      return args.Count > 0 ? args.ToArray() : null;
    }

    private static string GetMainCommand(AzuriteTestcontainerConfiguration configuration)
    {
      if (configuration.RunBlobOnly)
      {
        return "azurite-blob";
      }

      if (configuration.RunQueueOnly)
      {
        return "azurite-queue";
      }

      if (configuration.RunTableOnly)
      {
        return "azurite-table";
      }

      return "azurite";
    }
  }
}
