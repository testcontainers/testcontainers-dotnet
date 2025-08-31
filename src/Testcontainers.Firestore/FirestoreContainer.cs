namespace Testcontainers.Firestore;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class FirestoreContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FirestoreContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public FirestoreContainer(FirestoreConfiguration configuration)
        : base(configuration) { }

    /// <summary>
    /// Gets the Firestore emulator endpoint.
    /// </summary>
    /// <returns>The Firestore emulator endpoint.</returns>
    public string GetEmulatorEndpoint()
    {
        return new UriBuilder(
            Uri.UriSchemeHttp,
            Hostname,
            GetMappedPublicPort(FirestoreBuilder.FirestorePort)
        ).ToString();
    }
}
