namespace Testcontainers.MongoDb;

/// <summary>
/// Provides the MongoDb connection string.
/// </summary>
internal sealed class MongoDbConnectionStringProvider : ContainerConnectionStringProvider<MongoDbContainer, MongoDbConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}