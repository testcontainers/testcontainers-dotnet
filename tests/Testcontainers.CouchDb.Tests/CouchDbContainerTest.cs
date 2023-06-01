namespace Testcontainers.CouchDb;

public sealed class CouchDbContainerTest : ContainerTest<CouchDbBuilder, CouchDbContainer>
{
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task PutDatabaseReturnsHttpStatusCodeCreated()
    {
        // Given
        using var client = new MyCouchClient(Container.GetConnectionString(), "db");

        // When
        var database = await client.Database.PutAsync()
            .ConfigureAwait(false);

        // Then
        Assert.Equal(HttpStatusCode.Created, database.StatusCode);
    }
}