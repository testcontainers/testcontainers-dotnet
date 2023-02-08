namespace Testcontainers.Minio;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class MinioConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MinioConfiguration" /> class.
    /// </summary>
    /// <param name="username">The Minio database.</param>
    /// <param name="password">The Minio username.</param>
    public MinioConfiguration(
        string username = null,
        string password = null)
    {
        Username = username;
        Password = password;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MinioConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MinioConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MinioConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MinioConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MinioConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MinioConfiguration(MinioConfiguration resourceConfiguration)
        : this(new MinioConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MinioConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public MinioConfiguration(MinioConfiguration oldValue, MinioConfiguration newValue)
        : base(oldValue, newValue)
    {
        Username = BuildConfiguration.Combine(oldValue.Username, newValue.Username);
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
    }

    /// <summary>
    /// Gets the Minio username.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the Minio password.
    /// </summary>
    public string Password { get; }
}