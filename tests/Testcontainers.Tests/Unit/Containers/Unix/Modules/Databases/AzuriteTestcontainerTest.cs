namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Threading.Tasks;
  using Azure;
  using Azure.Data.Tables;
  using Azure.Storage.Blobs;
  using Azure.Storage.Queues;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class AzuriteTestcontainerTest : IAsyncLifetime
  {
    // We cannot use `Path.GetTempPath()` on macOS, see: https://github.com/common-workflow-language/cwltool/issues/328.
    private static readonly string TempDir = Environment.GetEnvironmentVariable("AGENT_TEMPDIRECTORY") ?? Directory.GetCurrentDirectory();
    private static readonly string BlobServiceDataFileName = GetDataFilename("blob");
    private static readonly string QueueServiceDataFileName = GetDataFilename("queue");
    private static readonly string TableServiceDataFileName = GetDataFilename("table");

    private AzuriteTestcontainer container;
    private string testDir;

    public Task InitializeAsync()
    {
      return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
      if (this.testDir != null && Directory.Exists(this.testDir))
      {
        Directory.Delete(this.testDir, true);
        this.testDir = null;
      }

      return this.container == null ? Task.CompletedTask : this.container.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task ConnectionToAllServicesEstablished()
    {
      // Given
      await this.StartAzuriteContainer();

      var blobServiceClient = new BlobServiceClient(this.container.ConnectionString);
      var queueServiceClient = new QueueServiceClient(this.container.ConnectionString);
      var tableServiceClient = new TableServiceClient(this.container.ConnectionString);

      // When
      var blobProperties = await blobServiceClient.GetPropertiesAsync();
      var queueProperties = await queueServiceClient.GetPropertiesAsync();
      var tableServiceProperties = await tableServiceClient.GetPropertiesAsync();
      var workspaceCommandResult = await this.container.ExecAsync(new List<string> {"ls", AzuriteTestcontainerConfiguration.DefaultLocation});

      // Then
      Assert.True(blobProperties.GetRawResponse().Status is >= 200 and <= 299);
      Assert.True(queueProperties.GetRawResponse().Status is >= 200 and <= 299);
      Assert.True(tableServiceProperties.GetRawResponse().Status is >= 200 and <= 299);
      Assert.True(workspaceCommandResult.ExitCode == 0);
      Assert.Contains(BlobServiceDataFileName, workspaceCommandResult.Stdout);
      Assert.Contains(QueueServiceDataFileName, workspaceCommandResult.Stdout);
      Assert.Contains(TableServiceDataFileName, workspaceCommandResult.Stdout);
    }

    [Fact]
    public async Task ConnectionToAllServicesWithCustomContainerPortsEstablished()
    {
      const int blobContainerPort = 7777;
      const int queueContainerPort = 8888;
      const int tableContainerPort = 9999;

      // Given
      await this.StartAzuriteContainer(configurations =>
      {
        configurations.BlobContainerPort = blobContainerPort;
        configurations.QueueContainerPort = queueContainerPort;
        configurations.TableContainerPort = tableContainerPort;
      });

      var blobServiceClient = new BlobServiceClient(this.container.ConnectionString);
      var queueServiceClient = new QueueServiceClient(this.container.ConnectionString);
      var tableServiceClient = new TableServiceClient(this.container.ConnectionString);

      // When
      var blobProperties = await blobServiceClient.GetPropertiesAsync();
      var queueProperties = await queueServiceClient.GetPropertiesAsync();
      var tableServiceProperties = await tableServiceClient.GetPropertiesAsync();
      var workspaceCommandResult = await this.container.ExecAsync(new List<string> { "ls", AzuriteTestcontainerConfiguration.DefaultLocation });

      // Then
      Assert.True(blobProperties.GetRawResponse().Status is >= 200 and <= 299);
      Assert.True(queueProperties.GetRawResponse().Status is >= 200 and <= 299);
      Assert.True(tableServiceProperties.GetRawResponse().Status is >= 200 and <= 299);
      Assert.Equal(blobContainerPort, this.container.ContainerBlobPort);
      Assert.Equal(queueContainerPort, this.container.ContainerQueuePort);
      Assert.Equal(tableContainerPort, this.container.ContainerTablePort);
      Assert.True(workspaceCommandResult.ExitCode == 0);
      Assert.Contains(BlobServiceDataFileName, workspaceCommandResult.Stdout);
      Assert.Contains(QueueServiceDataFileName, workspaceCommandResult.Stdout);
      Assert.Contains(TableServiceDataFileName, workspaceCommandResult.Stdout);
    }

    [Fact]
    public async Task ConnectionToBlobOnlyServiceEstablished()
    {
      // Given
      await this.StartAzuriteContainer(config =>
      {
        config.BlobServiceOnlyEnabled = true;
      });

      var blobServiceClient = new BlobServiceClient(this.container.ConnectionString);
      var queueServiceClient = new QueueServiceClient(this.container.ConnectionString);
      var tableServiceClient = new TableServiceClient(this.container.ConnectionString);

      // When
      var blobProperties = await blobServiceClient.GetPropertiesAsync();
      var workspaceCommandResult = await this.container.ExecAsync(new List<string> { "ls", AzuriteTestcontainerConfiguration.DefaultLocation });

      // Then
      Assert.True(blobProperties.GetRawResponse().Status is >= 200 and <= 299);
      await Assert.ThrowsAsync<RequestFailedException>(() => queueServiceClient.GetPropertiesAsync());
      await Assert.ThrowsAsync<RequestFailedException>(() => tableServiceClient.GetPropertiesAsync());
      Assert.True(workspaceCommandResult.ExitCode == 0);
      Assert.Contains(BlobServiceDataFileName, workspaceCommandResult.Stdout);
      Assert.DoesNotContain(QueueServiceDataFileName, workspaceCommandResult.Stdout);
      Assert.DoesNotContain(TableServiceDataFileName, workspaceCommandResult.Stdout);
    }

    [Fact]
    public async Task ConnectionToQueueOnlyServiceEstablished()
    {
      // Given
      await this.StartAzuriteContainer(config =>
      {
        config.QueueServiceOnlyEnabled = true;
      });

      var blobServiceClient = new BlobServiceClient(this.container.ConnectionString);
      var queueServiceClient = new QueueServiceClient(this.container.ConnectionString);
      var tableServiceClient = new TableServiceClient(this.container.ConnectionString);

      // When
      var queueProperties = await queueServiceClient.GetPropertiesAsync();
      var workspaceCommandResult = await this.container.ExecAsync(new List<string> { "ls", AzuriteTestcontainerConfiguration.DefaultLocation });

      // Then
      Assert.True(queueProperties.GetRawResponse().Status is >= 200 and <= 299);
      await Assert.ThrowsAsync<RequestFailedException>(() => blobServiceClient.GetPropertiesAsync());
      await Assert.ThrowsAsync<RequestFailedException>(() => tableServiceClient.GetPropertiesAsync());
      Assert.True(workspaceCommandResult.ExitCode == 0);
      Assert.DoesNotContain(BlobServiceDataFileName, workspaceCommandResult.Stdout);
      Assert.Contains(QueueServiceDataFileName, workspaceCommandResult.Stdout);
      Assert.DoesNotContain(TableServiceDataFileName, workspaceCommandResult.Stdout);
    }

    [Fact]
    public async Task ConnectionToTableOnlyServiceEstablished()
    {
      // Given
      await this.StartAzuriteContainer(config =>
      {
        config.TableServiceOnlyEnabled = true;
      });

      var blobServiceClient = new BlobServiceClient(this.container.ConnectionString);
      var queueServiceClient = new QueueServiceClient(this.container.ConnectionString);
      var tableServiceClient = new TableServiceClient(this.container.ConnectionString);

      // When
      var tableServiceProperties = await tableServiceClient.GetPropertiesAsync();
      var workspaceCommandResult = await this.container.ExecAsync(new List<string> { "ls", AzuriteTestcontainerConfiguration.DefaultLocation });

      // Then
      Assert.True(tableServiceProperties.GetRawResponse().Status is >= 200 and <= 299);
      await Assert.ThrowsAsync<RequestFailedException>(() => blobServiceClient.GetPropertiesAsync());
      await Assert.ThrowsAsync<RequestFailedException>(() => queueServiceClient.GetPropertiesAsync());
      Assert.True(workspaceCommandResult.ExitCode == 0);
      Assert.DoesNotContain(BlobServiceDataFileName, workspaceCommandResult.Stdout);
      Assert.DoesNotContain(QueueServiceDataFileName, workspaceCommandResult.Stdout);
      Assert.Contains(TableServiceDataFileName, workspaceCommandResult.Stdout);
    }

    [Fact]
    public async Task BindedLocationPathShouldContainsDataFiles()
    {
      // Given
      this.testDir = Path.Combine(TempDir, Guid.NewGuid().ToString("N"));
      Directory.CreateDirectory(this.testDir);

      // When
      await this.StartAzuriteContainer(config =>
      {
        config.Location = this.testDir;
      });

      // Then
      var files = Directory.GetFiles(this.testDir); 
      Assert.Contains(files, file => file.EndsWith(BlobServiceDataFileName, StringComparison.InvariantCultureIgnoreCase));
      Assert.Contains(files, file => file.EndsWith(QueueServiceDataFileName, StringComparison.InvariantCultureIgnoreCase));
      Assert.Contains(files, file => file.EndsWith(TableServiceDataFileName, StringComparison.InvariantCultureIgnoreCase));
    }

    private static string GetDataFilename(string service)
    {
      return $"__azurite_db_{service}__.json";
    }

    private async Task StartAzuriteContainer(Action<AzuriteTestcontainerConfiguration> configure = null)
    {
      var configuration = new AzuriteTestcontainerConfiguration();
      configure?.Invoke(configuration);
      this.container = new TestcontainersBuilder<AzuriteTestcontainer>()
        .WithAzurite(configuration)
        .Build();
      await this.container.StartAsync();
    }
  }
}
