namespace Testcontainers.MongoDbAtlasLocal;

/// <summary>
/// Provides the MongoDb Atlas Local connection string.
/// </summary>
internal sealed class MongoDbAtlasLocalConnectionStringProvider : ContainerConnectionStringProvider<MongoDbAtlasLocalContainer, MongoDbAtlasLocalConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}