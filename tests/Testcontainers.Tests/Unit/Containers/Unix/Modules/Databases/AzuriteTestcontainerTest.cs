namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Threading.Tasks;
  using Azure;
  using Azure.Data.Tables;
  using Azure.Storage.Blobs;
  using Azure.Storage.Queues;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public static class AzuriteTestcontainerTest
  {
    private static readonly string BlobServiceDataFileName = GetDataFileName(AzuriteTestcontainerConfiguration.AzuriteServices.Blob);

    private static readonly string QueueServiceDataFileName = GetDataFileName(AzuriteTestcontainerConfiguration.AzuriteServices.Queue);

    private static readonly string TableServiceDataFileName = GetDataFileName(AzuriteTestcontainerConfiguration.AzuriteServices.Table);

    private static string GetDataFileName(AzuriteTestcontainerConfiguration.AzuriteServices services)
    {
      return $"__azurite_db_{services.ToString().ToLowerInvariant()}__.json";
    }

    private static bool HasError<T>(Response<T> response)
    {
      using (var rawResponse = response.GetRawResponse())
      {
        return rawResponse.IsError;
      }
    }

    public sealed class DisabledService
    {
      public static IEnumerable<object[]> DisabledServices { get; }
        = new[]
        {
          new[] { new AzuriteTestcontainerConfiguration { BlobServiceOnlyEnabled = false } },
          new[] { new AzuriteTestcontainerConfiguration { QueueServiceOnlyEnabled = false } },
          new[] { new AzuriteTestcontainerConfiguration { TableServiceOnlyEnabled = false } },
        };

      [Theory]
      [MemberData(nameof(DisabledServices))]
      public void ShouldEnableAllServices(AzuriteTestcontainerConfiguration configuration)
      {
        Assert.True(configuration.AllServicesEnabled);
      }
    }

    [Collection(nameof(Testcontainers))]
    public sealed class AllServicesEnabled : IClassFixture<AzuriteFixture.AzuriteDefaultFixture>, IClassFixture<AzuriteFixture.AzuriteWithCustomContainerPortsFixture>
    {
      private readonly AzuriteFixture.AzuriteDefaultFixture commonContainerPorts;

      private readonly AzuriteFixture.AzuriteDefaultFixture customContainerPorts;

      public AllServicesEnabled(AzuriteFixture.AzuriteDefaultFixture commonContainerPorts, AzuriteFixture.AzuriteWithCustomContainerPortsFixture customContainerPorts)
      {
        this.commonContainerPorts = commonContainerPorts;
        this.customContainerPorts = customContainerPorts;
      }

      private AllServicesEnabled(AzuriteFixture.AzuriteDefaultFixture commonContainerPorts)
      {
        _ = commonContainerPorts;
      }

      private AllServicesEnabled(AzuriteFixture.AzuriteWithCustomContainerPortsFixture customContainerPorts)
      {
        _ = customContainerPorts;
      }

      [Fact]
      public async Task ShouldEstablishServiceConnection()
      {
        var exception = await Record.ExceptionAsync(() => Task.WhenAll(EstablishConnection(this.commonContainerPorts), EstablishConnection(this.customContainerPorts)))
          .ConfigureAwait(false);

        Assert.Null(exception);
      }

      private static async Task EstablishConnection(AzuriteFixture.AzuriteDefaultFixture azurite)
      {
        // Given
        var blobServiceClient = new BlobServiceClient(azurite.Container.ConnectionString);

        var queueServiceClient = new QueueServiceClient(azurite.Container.ConnectionString);

        var tableServiceClient = new TableServiceClient(azurite.Container.ConnectionString);

        // When
        var blobProperties = await blobServiceClient.GetPropertiesAsync()
          .ConfigureAwait(false);

        var queueProperties = await queueServiceClient.GetPropertiesAsync()
          .ConfigureAwait(false);

        var tableProperties = await tableServiceClient.GetPropertiesAsync()
          .ConfigureAwait(false);

        var execResult = await azurite.Container.ExecAsync(new List<string> { "ls", AzuriteTestcontainerConfiguration.DefaultWorkspaceDirectoryPath })
          .ConfigureAwait(false);

        // Then
        Assert.False(HasError(blobProperties));
        Assert.False(HasError(queueProperties));
        Assert.False(HasError(tableProperties));
        Assert.Equal(0, execResult.ExitCode);
        Assert.Equal(azurite.Configuration.BlobContainerPort, azurite.Container.BlobContainerPort);
        Assert.Equal(azurite.Configuration.QueueContainerPort, azurite.Container.QueueContainerPort);
        Assert.Equal(azurite.Configuration.TableContainerPort, azurite.Container.TableContainerPort);
        Assert.Contains(BlobServiceDataFileName, execResult.Stdout);
        Assert.Contains(QueueServiceDataFileName, execResult.Stdout);
        Assert.Contains(TableServiceDataFileName, execResult.Stdout);
      }
    }

    [Collection(nameof(Testcontainers))]
    public sealed class BlobServiceEnabled : IClassFixture<AzuriteFixture.AzuriteWithBlobOnlyFixture>
    {
      private readonly AzuriteFixture.AzuriteDefaultFixture azurite;

      public BlobServiceEnabled(AzuriteFixture.AzuriteWithBlobOnlyFixture azurite)
      {
        this.azurite = azurite;
      }

      [Fact]
      public async Task ShouldEstablishServiceConnection()
      {
        // Given
        var blobServiceClient = new BlobServiceClient(this.azurite.Container.ConnectionString);

        var queueServiceClient = new QueueServiceClient(this.azurite.Container.ConnectionString);

        var tableServiceClient = new TableServiceClient(this.azurite.Container.ConnectionString);

        // When
        var blobProperties = await blobServiceClient.GetPropertiesAsync()
          .ConfigureAwait(false);

        var execResult = await this.azurite.Container.ExecAsync(new List<string> { "ls", AzuriteTestcontainerConfiguration.DefaultWorkspaceDirectoryPath })
          .ConfigureAwait(false);

        // Then
        Assert.False(HasError(blobProperties));
        Assert.Equal(0, execResult.ExitCode);
        Assert.Contains(BlobServiceDataFileName, execResult.Stdout);

        Assert.DoesNotContain(QueueServiceDataFileName, execResult.Stdout);
        Assert.DoesNotContain(TableServiceDataFileName, execResult.Stdout);

        await Assert.ThrowsAsync<RequestFailedException>(() => queueServiceClient.GetPropertiesAsync())
          .ConfigureAwait(false);

        await Assert.ThrowsAsync<RequestFailedException>(() => tableServiceClient.GetPropertiesAsync())
          .ConfigureAwait(false);
      }
    }

    [Collection(nameof(Testcontainers))]
    public sealed class QueueServiceEnabled : IClassFixture<AzuriteFixture.AzuriteWithQueueOnlyFixture>
    {
      private readonly AzuriteFixture.AzuriteDefaultFixture azurite;

      public QueueServiceEnabled(AzuriteFixture.AzuriteWithQueueOnlyFixture azurite)
      {
        this.azurite = azurite;
      }

      [Fact]
      public async Task ShouldEstablishServiceConnection()
      {
        // Given
        var blobServiceClient = new BlobServiceClient(this.azurite.Container.ConnectionString);

        var queueServiceClient = new QueueServiceClient(this.azurite.Container.ConnectionString);

        var tableServiceClient = new TableServiceClient(this.azurite.Container.ConnectionString);

        // When
        var queueProperties = await queueServiceClient.GetPropertiesAsync()
          .ConfigureAwait(false);

        var execResult = await this.azurite.Container.ExecAsync(new List<string> { "ls", AzuriteTestcontainerConfiguration.DefaultWorkspaceDirectoryPath })
          .ConfigureAwait(false);

        // Then
        Assert.False(HasError(queueProperties));
        Assert.Equal(0, execResult.ExitCode);
        Assert.Contains(QueueServiceDataFileName, execResult.Stdout);

        Assert.DoesNotContain(BlobServiceDataFileName, execResult.Stdout);
        Assert.DoesNotContain(TableServiceDataFileName, execResult.Stdout);

        await Assert.ThrowsAsync<RequestFailedException>(() => blobServiceClient.GetPropertiesAsync())
          .ConfigureAwait(false);

        await Assert.ThrowsAsync<RequestFailedException>(() => tableServiceClient.GetPropertiesAsync())
          .ConfigureAwait(false);
      }
    }

    [Collection(nameof(Testcontainers))]
    public sealed class TableServiceEnabled : IClassFixture<AzuriteFixture.AzuriteWithTableOnlyFixture>
    {
      private readonly AzuriteFixture.AzuriteDefaultFixture azurite;

      public TableServiceEnabled(AzuriteFixture.AzuriteWithTableOnlyFixture azurite)
      {
        this.azurite = azurite;
      }

      [Fact]
      public async Task ShouldEstablishServiceConnection()
      {
        // Given
        var blobServiceClient = new BlobServiceClient(this.azurite.Container.ConnectionString);

        var queueServiceClient = new QueueServiceClient(this.azurite.Container.ConnectionString);

        var tableServiceClient = new TableServiceClient(this.azurite.Container.ConnectionString);

        // When
        var tableProperties = await tableServiceClient.GetPropertiesAsync()
          .ConfigureAwait(false);

        var execResult = await this.azurite.Container.ExecAsync(new List<string> { "ls", AzuriteTestcontainerConfiguration.DefaultWorkspaceDirectoryPath })
          .ConfigureAwait(false);

        // Then
        Assert.False(HasError(tableProperties));
        Assert.Equal(0, execResult.ExitCode);
        Assert.Contains(TableServiceDataFileName, execResult.Stdout);

        Assert.DoesNotContain(BlobServiceDataFileName, execResult.Stdout);
        Assert.DoesNotContain(QueueServiceDataFileName, execResult.Stdout);

        await Assert.ThrowsAsync<RequestFailedException>(() => blobServiceClient.GetPropertiesAsync())
          .ConfigureAwait(false);

        await Assert.ThrowsAsync<RequestFailedException>(() => queueServiceClient.GetPropertiesAsync())
          .ConfigureAwait(false);
      }
    }

    [Collection(nameof(Testcontainers))]
    public sealed class CustomLocation : IClassFixture<AzuriteFixture.AzuriteWithCustomWorkspaceFixture>
    {
      private readonly IEnumerable<string> dataFiles;

      public CustomLocation(AzuriteFixture.AzuriteWithCustomWorkspaceFixture azurite)
      {
        if (Directory.Exists(azurite.Configuration.Location))
        {
          this.dataFiles = Directory.EnumerateFiles(azurite.Configuration.Location, "*", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileName)
            .ToArray();
        }
        else
        {
          this.dataFiles = Array.Empty<string>();
        }
      }

      [Fact]
      public void ShouldGetDataFiles()
      {
        Assert.Contains(BlobServiceDataFileName, this.dataFiles);
        Assert.Contains(QueueServiceDataFileName, this.dataFiles);
        Assert.Contains(TableServiceDataFileName, this.dataFiles);
      }
    }
  }
}
