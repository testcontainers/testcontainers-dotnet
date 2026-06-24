namespace Testcontainers.Mosquitto;

/// <summary>
/// Provides the Mosquitto connection string.
/// </summary>
internal sealed class MosquittoConnectionStringProvider : ContainerConnectionStringProvider<MosquittoContainer, MosquittoConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}