namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using Azure;
  using Azure.Data.Tables;
  using Azure.Storage.Blobs;
  using Azure.Storage.Queues;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public sealed class AzuriteTestcontainerTest
  {
    private static readonly string BlobServiceDataFileName = GetDataFilename("blob");
    private static readonly string QueueServiceDataFileName = GetDataFilename("queue");
    private static readonly string TableServiceDataFileName = GetDataFilename("table");

    private static string GetDataFilename(string service)
    {
      return $"__azurite_db_{service}__.json";
    }

    [Collection(nameof(Testcontainers))]
    public class AzuriteDefaultTestcontainerTest : IClassFixture<AzuriteFixture.AzuriteDefaultFixture>
    {
      private readonly AzuriteFixture.AzuriteDefaultFixture fixture;

      public AzuriteDefaultTestcontainerTest(AzuriteFixture.AzuriteDefaultFixture fixture)
      {
        this.fixture = fixture;
      }

      [Fact]
      public async Task ConnectionToAllServicesEstablished()
      {
        // Given
        var blobServiceClient = new BlobServiceClient(this.fixture.Container.ConnectionString);
        var queueServiceClient = new QueueServiceClient(this.fixture.Container.ConnectionString);
        var tableServiceClient = new TableServiceClient(this.fixture.Container.ConnectionString);

        // When
        var blobProperties = await blobServiceClient.GetPropertiesAsync();
        var queueProperties = await queueServiceClient.GetPropertiesAsync();
        var tableServiceProperties = await tableServiceClient.GetPropertiesAsync();
        var workspaceCommandResult = await this.fixture.Container.ExecAsync(new List<string> { "ls", AzuriteTestcontainerConfiguration.DefaultLocation });

        // Then
        Assert.True(blobProperties.GetRawResponse().Status is >= 200 and <= 299);
        Assert.True(queueProperties.GetRawResponse().Status is >= 200 and <= 299);
        Assert.True(tableServiceProperties.GetRawResponse().Status is >= 200 and <= 299);
        Assert.True(workspaceCommandResult.ExitCode == 0);
        Assert.Contains(BlobServiceDataFileName, workspaceCommandResult.Stdout);
        Assert.Contains(QueueServiceDataFileName, workspaceCommandResult.Stdout);
        Assert.Contains(TableServiceDataFileName, workspaceCommandResult.Stdout);
      }
    }

    [Collection(nameof(Testcontainers))]
    public class AzuriteWithCustomContainerPortsTestcontainerTest : IClassFixture<AzuriteFixture.AzuriteWithCustomContainerPortsFixture>
    {
      private readonly AzuriteFixture.AzuriteWithCustomContainerPortsFixture fixture;

      public AzuriteWithCustomContainerPortsTestcontainerTest(AzuriteFixture.AzuriteWithCustomContainerPortsFixture fixture)
      {
        this.fixture = fixture;
      }

      [Fact]
      public async Task ConnectionToAllServicesWithCustomContainerPortsEstablished()
      {
        // Given
        var blobServiceClient = new BlobServiceClient(this.fixture.Container.ConnectionString);
        var queueServiceClient = new QueueServiceClient(this.fixture.Container.ConnectionString);
        var tableServiceClient = new TableServiceClient(this.fixture.Container.ConnectionString);

        // When
        var blobProperties = await blobServiceClient.GetPropertiesAsync();
        var queueProperties = await queueServiceClient.GetPropertiesAsync();
        var tableServiceProperties = await tableServiceClient.GetPropertiesAsync();
        var workspaceCommandResult = await this.fixture.Container.ExecAsync(new List<string> { "ls", AzuriteTestcontainerConfiguration.DefaultLocation });

        // Then
        Assert.True(blobProperties.GetRawResponse().Status is >= 200 and <= 299);
        Assert.True(queueProperties.GetRawResponse().Status is >= 200 and <= 299);
        Assert.True(tableServiceProperties.GetRawResponse().Status is >= 200 and <= 299);
        Assert.Equal(this.fixture.CustomBlobContainerPort, this.fixture.Container.ContainerBlobPort);
        Assert.Equal(this.fixture.CustomQueueContainerPort, this.fixture.Container.ContainerQueuePort);
        Assert.Equal(this.fixture.CustomTableContainerPort, this.fixture.Container.ContainerTablePort);
        Assert.True(workspaceCommandResult.ExitCode == 0);
        Assert.Contains(BlobServiceDataFileName, workspaceCommandResult.Stdout);
        Assert.Contains(QueueServiceDataFileName, workspaceCommandResult.Stdout);
        Assert.Contains(TableServiceDataFileName, workspaceCommandResult.Stdout);
      }
    }

    [Collection(nameof(Testcontainers))]
    public class AzuriteWithBlobOnlyServiceTestcontainerTest : IClassFixture<AzuriteFixture.AzuriteWithBlobOnlyFixture>
    {
      private readonly AzuriteFixture.AzuriteWithBlobOnlyFixture fixture;

      public AzuriteWithBlobOnlyServiceTestcontainerTest(AzuriteFixture.AzuriteWithBlobOnlyFixture fixture)
      {
        this.fixture = fixture;
      }

      [Fact]
      public async Task ConnectionToBlobOnlyServiceEstablished()
      {
        // Given
        var blobServiceClient = new BlobServiceClient(this.fixture.Container.ConnectionString);
        var queueServiceClient = new QueueServiceClient(this.fixture.Container.ConnectionString);
        var tableServiceClient = new TableServiceClient(this.fixture.Container.ConnectionString);

        // When
        var blobProperties = await blobServiceClient.GetPropertiesAsync();
        var workspaceCommandResult = await this.fixture.Container.ExecAsync(new List<string> { "ls", AzuriteTestcontainerConfiguration.DefaultLocation });

        // Then
        Assert.True(blobProperties.GetRawResponse().Status is >= 200 and <= 299);
        await Assert.ThrowsAsync<RequestFailedException>(() => queueServiceClient.GetPropertiesAsync());
        await Assert.ThrowsAsync<RequestFailedException>(() => tableServiceClient.GetPropertiesAsync());
        Assert.True(workspaceCommandResult.ExitCode == 0);
        Assert.Contains(BlobServiceDataFileName, workspaceCommandResult.Stdout);
        Assert.DoesNotContain(QueueServiceDataFileName, workspaceCommandResult.Stdout);
        Assert.DoesNotContain(TableServiceDataFileName, workspaceCommandResult.Stdout);
      }
    }

    [Collection(nameof(Testcontainers))]
    public class AzuriteWithQueueOnlyServiceTestcontainerTest : IClassFixture<AzuriteFixture.AzuriteWithQueueOnlyFixture>
    {
      private readonly AzuriteFixture.AzuriteWithQueueOnlyFixture fixture;

      public AzuriteWithQueueOnlyServiceTestcontainerTest(AzuriteFixture.AzuriteWithQueueOnlyFixture fixture)
      {
        this.fixture = fixture;
      }

      [Fact]
      public async Task ConnectionToQueueOnlyServiceEstablished()
      {
        // Given
        var blobServiceClient = new BlobServiceClient(this.fixture.Container.ConnectionString);
        var queueServiceClient = new QueueServiceClient(this.fixture.Container.ConnectionString);
        var tableServiceClient = new TableServiceClient(this.fixture.Container.ConnectionString);

        // When
        var queueProperties = await queueServiceClient.GetPropertiesAsync();
        var workspaceCommandResult = await this.fixture.Container.ExecAsync(new List<string> { "ls", AzuriteTestcontainerConfiguration.DefaultLocation });

        // Then
        Assert.True(queueProperties.GetRawResponse().Status is >= 200 and <= 299);
        await Assert.ThrowsAsync<RequestFailedException>(() => blobServiceClient.GetPropertiesAsync());
        await Assert.ThrowsAsync<RequestFailedException>(() => tableServiceClient.GetPropertiesAsync());
        Assert.True(workspaceCommandResult.ExitCode == 0);
        Assert.DoesNotContain(BlobServiceDataFileName, workspaceCommandResult.Stdout);
        Assert.Contains(QueueServiceDataFileName, workspaceCommandResult.Stdout);
        Assert.DoesNotContain(TableServiceDataFileName, workspaceCommandResult.Stdout);
      }
    }

    [Collection(nameof(Testcontainers))]
    public class AzuriteWithTableOnlyServiceTestcontainerTest : IClassFixture<AzuriteFixture.AzuriteWithTableOnlyFixture>
    {
      private readonly AzuriteFixture.AzuriteWithTableOnlyFixture fixture;

      public AzuriteWithTableOnlyServiceTestcontainerTest(AzuriteFixture.AzuriteWithTableOnlyFixture fixture)
      {
        this.fixture = fixture;
      }

      [Fact]
      public async Task ConnectionToTableOnlyServiceEstablished()
      {
        // Given
        var blobServiceClient = new BlobServiceClient(this.fixture.Container.ConnectionString);
        var queueServiceClient = new QueueServiceClient(this.fixture.Container.ConnectionString);
        var tableServiceClient = new TableServiceClient(this.fixture.Container.ConnectionString);

        // When
        var tableServiceProperties = await tableServiceClient.GetPropertiesAsync();
        var workspaceCommandResult = await this.fixture.Container.ExecAsync(new List<string> { "ls", AzuriteTestcontainerConfiguration.DefaultLocation });

        // Then
        Assert.True(tableServiceProperties.GetRawResponse().Status is >= 200 and <= 299);
        await Assert.ThrowsAsync<RequestFailedException>(() => blobServiceClient.GetPropertiesAsync());
        await Assert.ThrowsAsync<RequestFailedException>(() => queueServiceClient.GetPropertiesAsync());
        Assert.True(workspaceCommandResult.ExitCode == 0);
        Assert.DoesNotContain(BlobServiceDataFileName, workspaceCommandResult.Stdout);
        Assert.DoesNotContain(QueueServiceDataFileName, workspaceCommandResult.Stdout);
        Assert.Contains(TableServiceDataFileName, workspaceCommandResult.Stdout);
      }
    }

    [Collection(nameof(Testcontainers))]
    public class AzuriteWithBoundLocationPathTestcontainerTest : IClassFixture<AzuriteFixture.AzuriteWithBoundLocationPathFixture>
    {
      private readonly AzuriteFixture.AzuriteWithBoundLocationPathFixture fixture;

      public AzuriteWithBoundLocationPathTestcontainerTest(AzuriteFixture.AzuriteWithBoundLocationPathFixture fixture)
      {
        this.fixture = fixture;
      }

      [Fact]
      public void BoundLocationPathShouldContainsDataFiles()
      {
        // Given

        // When
        var fileNames = this.fixture.DataDirectoryPath.GetFiles().Select(fileInfo => fileInfo.Name).ToList();

        // Then
        Assert.Contains(fileNames, fileName => fileName.Equals(BlobServiceDataFileName, StringComparison.InvariantCultureIgnoreCase));
        Assert.Contains(fileNames, fileName => fileName.Equals(QueueServiceDataFileName, StringComparison.InvariantCultureIgnoreCase));
        Assert.Contains(fileNames, fileName => fileName.Equals(TableServiceDataFileName, StringComparison.InvariantCultureIgnoreCase));
      }
    }
  }
}
