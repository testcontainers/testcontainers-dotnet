using Azure.Storage;
using System;

namespace Testcontainers.Azurite;

public abstract class AzuriteContainerSharedKeyHttpsPemTest : IAsyncLifetime
{
    private readonly AzuriteContainer _azuriteContainer = new AzuriteBuilder()
        .WithAccountCredentials(TestValues.AccountName, TestValues.AccountKey)
        .WithPemCertificate(Certificate.PEMPath, Certificate.PEMKeyPath)
        .Build();

    public Task InitializeAsync() => _azuriteContainer.StartAsync();

    public Task DisposeAsync() => _azuriteContainer.DisposeAsync().AsTask();

    private static bool HasError<TResponseEntity>(NullableResponse<TResponseEntity> response)
    {
        using (var rawResponse = response.GetRawResponse())
        {
            return rawResponse.IsError;
        }
    }

    public sealed class BlobService : AzuriteContainerSharedKeyHttpsPemTest
    {
        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task EstablishesConnection()
        {
            // Give
            var client = new BlobServiceClient(new Uri(_azuriteContainer.GetBlobEndpoint()), new StorageSharedKeyCredential(TestValues.AccountName, TestValues.AccountKey));

            // When
            var properties = await client.GetPropertiesAsync()
                .ConfigureAwait(false);

            // Then
            Assert.False(HasError(properties));
        }
    }

    public sealed class QueueService : AzuriteContainerSharedKeyHttpsPemTest
    {
        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task EstablishesConnection()
        {
            // Give
            var client = new QueueServiceClient(new Uri(_azuriteContainer.GetQueueEndpoint()), new StorageSharedKeyCredential(TestValues.AccountName, TestValues.AccountKey));

            // When
            var properties = await client.GetPropertiesAsync()
                .ConfigureAwait(false);

            // Then
            Assert.False(HasError(properties));
        }
    }

    public sealed class TableService : AzuriteContainerSharedKeyHttpsPemTest
    {
        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task EstablishesConnection()
        {
            // Give
            var client = new TableServiceClient(new Uri(_azuriteContainer.GetTableEndpoint()), new TableSharedKeyCredential(TestValues.AccountName, TestValues.AccountKey));

            // When
            var properties = await client.GetPropertiesAsync()
                .ConfigureAwait(false);

            // Then
            Assert.False(HasError(properties));
        }
    }
}