namespace Testcontainers.DynamoDb;

/// <summary>
/// Provides the DynamoDb connection string.
/// </summary>
internal sealed class DynamoDbConnectionStringProvider : ContainerConnectionStringProvider<DynamoDbContainer, DynamoDbConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}