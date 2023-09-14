namespace Testcontainers.Firestore;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class FirestoreConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FirestoreConfiguration" /> class.
    /// </summary>
    public FirestoreConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FirestoreConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public FirestoreConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FirestoreConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public FirestoreConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FirestoreConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public FirestoreConfiguration(FirestoreConfiguration resourceConfiguration)
        : this(new FirestoreConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FirestoreConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public FirestoreConfiguration(FirestoreConfiguration oldValue, FirestoreConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}