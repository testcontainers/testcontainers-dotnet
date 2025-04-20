namespace Testcontainers.Firestore;

public sealed class FirestoreContainerTest : IAsyncLifetime
{
    private readonly FirestoreContainer _firestoreContainer = new FirestoreBuilder().Build();

    public async ValueTask InitializeAsync()
    {
        await _firestoreContainer.StartAsync();
    }

    public ValueTask DisposeAsync()
    {
        return _firestoreContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task GetSnapshotReturnsSetDocument()
    {
        // Given
        const string collection = "db";

        var projectId = Guid.NewGuid().ToString("D");

        var documentId = Guid.NewGuid().ToString("D");

        var documentData = new Dictionary<string, string>();
        documentData.Add("id", documentId);

        var firestoreDbBuilder = new FirestoreDbBuilder();
        firestoreDbBuilder.ProjectId = projectId;
        firestoreDbBuilder.Endpoint = _firestoreContainer.GetEmulatorEndpoint();
        firestoreDbBuilder.ChannelCredentials = ChannelCredentials.Insecure;

        var firestoreDb = await firestoreDbBuilder.BuildAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // When
        _ = await firestoreDb.Collection(collection).Document().SetAsync(documentData, cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var querySnapshot = await firestoreDb.Collection(collection).GetSnapshotAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(documentData, querySnapshot.Documents.Select(document => document.ConvertTo<Dictionary<string, string>>()).Single());
    }
}