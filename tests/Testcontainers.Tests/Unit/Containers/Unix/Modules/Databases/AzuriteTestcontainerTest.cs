namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
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
    private AzuriteTestcontainer container;

    public Task InitializeAsync()
    {
      return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
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

      // Then
      Assert.True(blobProperties.GetRawResponse().Status is >= 200 and <= 299);
      Assert.True(queueProperties.GetRawResponse().Status is >= 200 and <= 299);
      Assert.True(tableServiceProperties.GetRawResponse().Status is >= 200 and <= 299);
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

      // Then
      Assert.True(blobProperties.GetRawResponse().Status is >= 200 and <= 299);
      await Assert.ThrowsAsync<RequestFailedException>(() => queueServiceClient.GetPropertiesAsync());
      await Assert.ThrowsAsync<RequestFailedException>(() => tableServiceClient.GetPropertiesAsync());
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

      // Then
      Assert.True(queueProperties.GetRawResponse().Status is >= 200 and <= 299);
      await Assert.ThrowsAsync<RequestFailedException>(() => blobServiceClient.GetPropertiesAsync());
      await Assert.ThrowsAsync<RequestFailedException>(() => tableServiceClient.GetPropertiesAsync());
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

      // Then
      Assert.True(tableServiceProperties.GetRawResponse().Status is >= 200 and <= 299);
      await Assert.ThrowsAsync<RequestFailedException>(() => blobServiceClient.GetPropertiesAsync());
      await Assert.ThrowsAsync<RequestFailedException>(() => queueServiceClient.GetPropertiesAsync());
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
