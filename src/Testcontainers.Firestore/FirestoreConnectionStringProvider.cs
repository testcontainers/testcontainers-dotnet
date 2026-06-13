namespace Testcontainers.Firestore;

/// <summary>
/// Provides the Firestore connection string.
/// </summary>
internal sealed class FirestoreConnectionStringProvider : ContainerConnectionStringProvider<FirestoreContainer, FirestoreConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetEmulatorEndpoint();
    }
}