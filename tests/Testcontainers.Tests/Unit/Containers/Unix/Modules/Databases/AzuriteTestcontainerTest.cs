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
  using DotNet.Testcontainers.Configurations.Modules.Databases;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class AzuriteTestcontainerTest : IAsyncLifetime
  {
    // We cannot use `Path.GetTempPath()` on macOS, see: https://github.com/common-workflow-language/cwltool/issues/328.
    private static readonly string TempDir = Environment.GetEnvironmentVariable("AGENT_TEMPDIRECTORY") ?? Directory.GetCurrentDirectory();

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
      Assert.Contains(GetDataFilename("blob"), workspaceCommandResult.Stdout);
      Assert.Contains(GetDataFilename("queue"), workspaceCommandResult.Stdout);
      Assert.Contains(GetDataFilename("table"), workspaceCommandResult.Stdout);
    }

    [Fact]
    public async Task ConnectionToBlobOnlyServiceEstablished()
    {
      // Given
      await this.StartAzuriteContainer(config =>
      {
        config.RunBlobOnly = true;
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
      Assert.Contains(GetDataFilename("blob"), workspaceCommandResult.Stdout);
      Assert.DoesNotContain(GetDataFilename("queue"), workspaceCommandResult.Stdout);
      Assert.DoesNotContain(GetDataFilename("table"), workspaceCommandResult.Stdout);
    }

    [Fact]
    public async Task ConnectionToQueueOnlyServiceEstablished()
    {
      // Given
      await this.StartAzuriteContainer(config =>
      {
        config.RunQueueOnly = true;
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
      Assert.DoesNotContain(GetDataFilename("blob"), workspaceCommandResult.Stdout);
      Assert.Contains(GetDataFilename("queue"), workspaceCommandResult.Stdout);
      Assert.DoesNotContain(GetDataFilename("table"), workspaceCommandResult.Stdout);
    }

    [Fact]
    public async Task ConnectionToTableOnlyServiceEstablished()
    {
      // Given
      await this.StartAzuriteContainer(config =>
      {
        config.RunTableOnly = true;
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
      Assert.DoesNotContain(GetDataFilename("blob"), workspaceCommandResult.Stdout);
      Assert.DoesNotContain(GetDataFilename("queue"), workspaceCommandResult.Stdout);
      Assert.Contains(GetDataFilename("table"), workspaceCommandResult.Stdout);
    }

    [Fact]
    public async Task BindedLocationPathShouldContainsDataFiles()
    {
      // Given
      this.testDir = Path.Combine(TempDir, Guid.NewGuid().ToString("N"));
      Directory.CreateDirectory(this.testDir);
      await this.StartAzuriteContainer(config =>
      {
        config.Location = this.testDir;
      });

      // When

      // Then
      var files = Directory.GetFiles(this.testDir); 
      Assert.Contains(files, file => file.EndsWith(GetDataFilename("blob"), StringComparison.InvariantCultureIgnoreCase));
      Assert.Contains(files, file => file.EndsWith(GetDataFilename("queue"), StringComparison.InvariantCultureIgnoreCase));
      Assert.Contains(files, file => file.EndsWith(GetDataFilename("table"), StringComparison.InvariantCultureIgnoreCase));
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

    private static string GetDataFilename(string service)
    {
      return $"__azurite_db_{service}__.json";
    }
  }
}
