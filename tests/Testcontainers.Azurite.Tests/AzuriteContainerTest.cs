namespace Testcontainers.Azurite;

public abstract class AzuriteContainerTest : IAsyncLifetime
{
    private readonly AzuriteContainer _azuriteContainer;

    private AzuriteContainerTest(AzuriteContainer azuriteContainer)
    {
        _azuriteContainer = azuriteContainer;
    }

    public Task InitializeAsync()
    {
        return _azuriteContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _azuriteContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task EstablishesBlobServiceConnection()
    {
        // Give
        var client = new BlobServiceClient(_azuriteContainer.GetConnectionString());

        // When
        var properties = await client.GetPropertiesAsync()
            .ConfigureAwait(false);

        // Then
        Assert.False(HasError(properties));
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task EstablishesQueueServiceConnection()
    {
        // Give
        var client = new QueueServiceClient(_azuriteContainer.GetConnectionString());

        // When
        var properties = await client.GetPropertiesAsync()
            .ConfigureAwait(false);

        // Then
        Assert.False(HasError(properties));
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task EstablishesTableServiceConnection()
    {
        // Give
        var client = new TableServiceClient(_azuriteContainer.GetConnectionString());

        // When
        var properties = await client.GetPropertiesAsync()
            .ConfigureAwait(false);

        // Then
        Assert.False(HasError(properties));
    }

    private static bool HasError<TResponseEntity>(NullableResponse<TResponseEntity> response)
    {
        using (var rawResponse = response.GetRawResponse())
        {
            return rawResponse.IsError;
        }
    }

    [UsedImplicitly]
    public sealed class AzuriteDefaultConfiguration : AzuriteContainerTest
    {
        public AzuriteDefaultConfiguration()
            : base(new AzuriteBuilder().Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class AzuriteInMemoryConfiguration : AzuriteContainerTest
    {
        public AzuriteInMemoryConfiguration()
            : base(new AzuriteBuilder().WithInMemoryPersistence().Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class AzuriteMemoryLimitConfiguration : AzuriteContainerTest
    {
        private const int MemoryLimitInMb = 64;

        private static readonly string[] LineEndings = { "\r\n", "\n" };

        public AzuriteMemoryLimitConfiguration()
            : base(new AzuriteBuilder().WithInMemoryPersistence(MemoryLimitInMb).Build())
        {
        }

        [Fact]
        public async Task MemoryLimitIsConfigured()
        {
            // Given
            var (stdout, _) = await _azuriteContainer.GetLogsAsync(timestampsEnabled: false)
                .ConfigureAwait(false);

            // When
            var firstLine = stdout.Split(LineEndings, StringSplitOptions.RemoveEmptyEntries).First();

            // Then
            Assert.StartsWith(string.Format(CultureInfo.InvariantCulture, "In-memory extent storage is enabled with a limit of {0:F2} MB", MemoryLimitInMb), firstLine);
        }
    }
}