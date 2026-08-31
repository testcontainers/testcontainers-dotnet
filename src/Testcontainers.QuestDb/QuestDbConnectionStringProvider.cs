namespace Testcontainers.QuestDb;

/// <summary>
/// Provides the QuestDb connection string.
/// </summary>
internal sealed class QuestDbConnectionStringProvider : ContainerConnectionStringProvider<QuestDbContainer, QuestDbConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}