namespace Testcontainers.Firestore;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class FirestoreContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FirestoreContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public FirestoreContainer(FirestoreConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }

    public void SetEmulatorHost()
    {
        Environment.SetEnvironmentVariable("FIRESTORE_EMULATOR_HOST", $"{Hostname}:8080/");
    }
}