namespace Testcontainers.Oracle;

/// <summary>
/// Provides the Oracle connection string.
/// </summary>
internal sealed class OracleConnectionStringProvider : ContainerConnectionStringProvider<OracleContainer, OracleConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}