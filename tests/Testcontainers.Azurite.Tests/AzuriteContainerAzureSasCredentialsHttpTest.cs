using Azure.Data.Tables.Sas;
using Azure.Storage.Sas;
using System;

namespace Testcontainers.Azurite;

public abstract class AzuriteContainerAzureSasCredentialsHttpTest : IAsyncLifetime
{
    private readonly AzuriteContainer _azuriteContainer = new AzuriteBuilder()
        .WithAccountCredentials(TestValues.AccountName, TestValues.AccountKey)
        .Build();

    private Uri BlobAccountSas;
    private Uri QueueAccountSas;
    private Uri TableAccountSas;

    public async Task InitializeAsync()
    {
        await _azuriteContainer.StartAsync();

        var blobClient = new BlobServiceClient(_azuriteContainer.GetConnectionString());
        BlobAccountSas = blobClient.GenerateAccountSasUri(AccountSasPermissions.All, DateTimeOffset.UtcNow.AddDays(1), AccountSasResourceTypes.All);

        var queueClient = new QueueServiceClient(_azuriteContainer.GetConnectionString());
        QueueAccountSas = queueClient.GenerateAccountSasUri(AccountSasPermissions.All, DateTimeOffset.UtcNow.AddDays(1), AccountSasResourceTypes.All);

        var tableClient = new TableServiceClient(_azuriteContainer.GetConnectionString());
        TableAccountSas = tableClient.GenerateSasUri(TableAccountSasPermissions.All, TableAccountSasResourceTypes.All, DateTimeOffset.UtcNow.AddDays(1));
    }

    public Task DisposeAsync() => _azuriteContainer.DisposeAsync().AsTask();

    private static bool HasError<TResponseEntity>(NullableResponse<TResponseEntity> response)
    {
        using (var rawResponse = response.GetRawResponse())
        {
            return rawResponse.IsError;
        }
    }

    public sealed class BlobService : AzuriteContainerAzureSasCredentialsHttpTest
    {
        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task EstablishesConnection()
        {
            // Give
            var client = new BlobServiceClient(
                new Uri(_azuriteContainer.GetBlobEndpoint()),
                new AzureSasCredential(BlobAccountSas.Query)
            );

            // When
            var properties = await client.GetPropertiesAsync()
                .ConfigureAwait(false);

            // Then
            Assert.False(HasError(properties));
        }
    }

    public sealed class QueueService : AzuriteContainerAzureSasCredentialsHttpTest
    {
        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task EstablishesConnection()
        {
            // Give
            var client = new QueueServiceClient(
                new Uri(_azuriteContainer.GetQueueEndpoint()),
                new AzureSasCredential(QueueAccountSas.Query)
            );

            // When
            var properties = await client.GetPropertiesAsync()
                .ConfigureAwait(false);

            // Then
            Assert.False(HasError(properties));
        }
    }

    public sealed class TableService : AzuriteContainerAzureSasCredentialsHttpTest
    {
        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task EstablishesConnection()
        {
            // Give
            var client = new TableServiceClient(
                new Uri(_azuriteContainer.GetTableEndpoint()),
                new AzureSasCredential(TableAccountSas.Query)
            );

            // When
            var properties = await client.GetPropertiesAsync()
                .ConfigureAwait(false);

            // Then
            Assert.False(HasError(properties));
        }
    }
}