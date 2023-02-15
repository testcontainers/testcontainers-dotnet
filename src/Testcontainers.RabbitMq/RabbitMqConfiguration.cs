namespace Testcontainers.RabbitMq;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class RabbitMqConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqConfiguration" /> class.
    /// </summary>
    /// <param name="username">The RabbitMq username.</param>
    /// <param name="password">The RabbitMq password.</param>
    public RabbitMqConfiguration(
        string username = null,
        string password = null)
    {
        Username = username;
        Password = password;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public RabbitMqConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public RabbitMqConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public RabbitMqConfiguration(RabbitMqConfiguration resourceConfiguration)
        : this(new RabbitMqConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public RabbitMqConfiguration(RabbitMqConfiguration oldValue, RabbitMqConfiguration newValue)
        : base(oldValue, newValue)
    {
        Username = BuildConfiguration.Combine(oldValue.Username, newValue.Username);
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
    }

    /// <summary>
    /// Gets the RabbitMq username.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the RabbitMq password.
    /// </summary>
    public string Password { get; }
}