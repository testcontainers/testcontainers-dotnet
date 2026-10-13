namespace Testcontainers.MongoDbAtlasLocal;

internal static class AtlasSearch
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(250);

    private static readonly TimeSpan PollTimeout = TimeSpan.FromMinutes(1);

    public static async Task WaitUntilSearchIndexIsQueryableAsync(IMongoCollection<BsonDocument> collection, string indexName, CancellationToken ct)
    {
        // Atlas Search builds the index asynchronously, it becomes queryable shortly after it has been created.
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeoutCts.CancelAfter(PollTimeout);

        while (true)
        {
            using var cursor = await collection.SearchIndexes.ListAsync(indexName, cancellationToken: timeoutCts.Token)
                .ConfigureAwait(false);

            var searchIndexes = await cursor.ToListAsync(timeoutCts.Token)
                .ConfigureAwait(false);

            if (searchIndexes.Any(searchIndex => searchIndex.TryGetValue("queryable", out var queryable) && queryable.ToBoolean()))
            {
                return;
            }

            await Task.Delay(PollInterval, timeoutCts.Token)
                .ConfigureAwait(false);
        }
    }

    public static async Task<string[]> WaitUntilSearchReturnsAsync(IMongoCollection<BsonDocument> collection, string indexName, SearchDefinition<BsonDocument> searchDefinition, int expectedCount, CancellationToken ct)
    {
        // mongot replicates documents from mongod asynchronously, a queryable index may not contain all documents yet.
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeoutCts.CancelAfter(PollTimeout);

        while (true)
        {
            var documents = await collection.Aggregate()
                .Search(searchDefinition, indexName: indexName)
                .ToListAsync(timeoutCts.Token)
                .ConfigureAwait(false);

            if (documents.Count >= expectedCount)
            {
                return documents.Select(document => document["title"].AsString).ToArray();
            }

            await Task.Delay(PollInterval, timeoutCts.Token)
                .ConfigureAwait(false);
        }
    }
}