namespace Testcontainers.Azurite;

public abstract class AzuriteContainerTest : ContainerTest<AzuriteBuilder, AzuriteContainer>
{
    private static bool HasError<TResponseEntity>(NullableResponse<TResponseEntity> response)
    {
        using (var rawResponse = response.GetRawResponse())
        {
            return rawResponse.IsError;
        }
    }

    public sealed class BlobService : AzuriteContainerTest
    {
        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task EstablishesConnection()
        {
            // Give
            var client = new BlobServiceClient(Container.GetConnectionString());

            // When
            var properties = await client.GetPropertiesAsync()
                .ConfigureAwait(false);

            // Then
            Assert.False(HasError(properties));
        }
    }

    public sealed class QueueService : AzuriteContainerTest
    {
        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task EstablishesConnection()
        {
            // Give
            var client = new QueueServiceClient(Container.GetConnectionString());

            // When
            var properties = await client.GetPropertiesAsync()
                .ConfigureAwait(false);

            // Then
            Assert.False(HasError(properties));
        }
    }

    public sealed class TableService : AzuriteContainerTest
    {
        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task EstablishesConnection()
        {
            // Give
            var client = new TableServiceClient(Container.GetConnectionString());

            // When
            var properties = await client.GetPropertiesAsync()
                .ConfigureAwait(false);

            // Then
            Assert.False(HasError(properties));
        }
    }
}