namespace Testcontainers.RustFs;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class RustFsConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RustFsConfiguration" /> class.
    /// </summary>
    /// <param name="username">The RustFS username.</param>
    /// <param name="password">The RustFS password.</param>
    public RustFsConfiguration(
        string username = null,
        string password = null)
    {
        Username = username;
        Password = password;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RustFsConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public RustFsConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RustFsConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public RustFsConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RustFsConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public RustFsConfiguration(RustFsConfiguration resourceConfiguration)
        : this(new RustFsConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RustFsConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public RustFsConfiguration(RustFsConfiguration oldValue, RustFsConfiguration newValue)
        : base(oldValue, newValue)
    {
        Username = BuildConfiguration.Combine(oldValue.Username, newValue.Username);
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
    }

    /// <summary>
    /// Gets the RustFS username.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the RustFS password.
    /// </summary>
    public string Password { get; }
}