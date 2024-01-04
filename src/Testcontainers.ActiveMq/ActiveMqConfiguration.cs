namespace Testcontainers.ActiveMq;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class ActiveMqConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActiveMqConfiguration" /> class.
    /// </summary>
    /// <param name="username">The ActiveMq username.</param>
    /// <param name="password">The ActiveMq password.</param>
    public ActiveMqConfiguration(
        string username = null,
        string password = null)
    {
        Username = username;
        Password = password;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActiveMqConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ActiveMqConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActiveMqConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ActiveMqConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActiveMqConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ActiveMqConfiguration(ActiveMqConfiguration resourceConfiguration)
        : this(new ActiveMqConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActiveMqConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public ActiveMqConfiguration(ActiveMqConfiguration oldValue, ActiveMqConfiguration newValue)
        : base(oldValue, newValue)
    {
        Username = BuildConfiguration.Combine(oldValue.Username, newValue.Username);
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
    }

    /// <summary>
    /// Gets the ActiveMq username.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the ActiveMq password.
    /// </summary>
    public string Password { get; }
}