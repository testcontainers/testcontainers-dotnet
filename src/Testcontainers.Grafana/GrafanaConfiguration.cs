namespace Testcontainers.Grafana;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class GrafanaConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GrafanaConfiguration" /> class.
    /// </summary>
    /// <param name="username">The Grafana username.</param>
    /// <param name="password">The Grafana password.</param>
    public GrafanaConfiguration(
        string username = null,
        string password = null)
    {
        Username = username;
        Password = password;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GrafanaConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public GrafanaConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GrafanaConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public GrafanaConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GrafanaConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public GrafanaConfiguration(GrafanaConfiguration resourceConfiguration)
        : this(new GrafanaConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GrafanaConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public GrafanaConfiguration(GrafanaConfiguration oldValue, GrafanaConfiguration newValue)
        : base(oldValue, newValue)
    {
        Username = newValue.Username ?? oldValue.Username;
        Password = newValue.Password ?? oldValue.Password;
    }

    /// <summary>
    /// Gets the Grafana username.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the Grafana password.
    /// </summary>
    public string Password { get; }
}