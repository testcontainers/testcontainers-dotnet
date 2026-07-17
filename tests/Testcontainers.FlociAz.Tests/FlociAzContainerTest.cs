namespace Testcontainers.FlociAz;

public sealed class FlociAzContainerTest : IAsyncLifetime
{
    private const string AzureService = "Service";

    private readonly FlociAzContainer _flociAzContainer = new FlociAzBuilder(TestSession.GetImageFromDockerfile()).Build();

    public async ValueTask InitializeAsync()
    {
        await _flociAzContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _flociAzContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "blob")]
    public async Task DownloadBlobReturnsUploadedBlob()
    {
        // Given
        var content = Guid.NewGuid().ToString("D");

        var client = new BlobServiceClient(_flociAzContainer.GetConnectionString());

        var containerClient = client.GetBlobContainerClient(Guid.NewGuid().ToString("D"));

        var blobClient = containerClient.GetBlobClient(Guid.NewGuid().ToString("D"));

        // When
        _ = await containerClient.CreateAsync(cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        _ = await blobClient.UploadAsync(BinaryData.FromString(content), cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var downloadResult = await blobClient.DownloadContentAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(content, downloadResult.Value.Content.ToString());
        Assert.Equal(_flociAzContainer.GetConnectionString(), _flociAzContainer.GetConnectionString(ConnectionMode.Host));
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "queue")]
    public async Task ReceiveMessageReturnsSentMessage()
    {
        // Given
        var message = Guid.NewGuid().ToString("D");

        var client = new QueueServiceClient(_flociAzContainer.GetConnectionString());

        var queueClient = client.GetQueueClient(Guid.NewGuid().ToString("D"));

        // When
        _ = await queueClient.CreateAsync(cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        _ = await queueClient.SendMessageAsync(message, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var receivedMessage = await queueClient.ReceiveMessageAsync(cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(message, receivedMessage.Value.MessageText);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "table")]
    public async Task GetEntityReturnsAddedEntity()
    {
        // Given
        var partitionKey = Guid.NewGuid().ToString("D");

        var rowKey = Guid.NewGuid().ToString("D");

        var client = new TableServiceClient(_flociAzContainer.GetConnectionString());

        var tableClient = client.GetTableClient("Table" + Guid.NewGuid().ToString("N"));

        // When
        _ = await tableClient.CreateAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        _ = await tableClient.AddEntityAsync(new TableEntity(partitionKey, rowKey), TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var entityResponse = await tableClient.GetEntityAsync<TableEntity>(partitionKey, rowKey, cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(rowKey, entityResponse.Value.RowKey);
    }
}
