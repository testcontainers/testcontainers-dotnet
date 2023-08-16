namespace Testcontainers.InfluxDb;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class InfluxDbConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InfluxDbConfiguration" /> class.
    /// </summary>
    /// <param name="username">The InfluxDb username.</param>
    /// <param name="password">The InfluxDb password.</param>
    /// <param name="organization">The InfluxDb organization.</param>
    /// <param name="bucket">The InfluxDb bucket.</param>
    /// <param name="adminToken">The InfluxDb admin token.</param>
    /// <param name="retention">The InfluxDb retention.</param>
    public InfluxDbConfiguration(
        string username = null,
        string password = null,
        string organization = null,
        string bucket = null,
        string adminToken = null,
        string retention = null)
    {
        Username = username;
        Password = password;
        Organization = organization;
        Bucket = bucket;
        AdminToken = adminToken;
        Retention = retention;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InfluxDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public InfluxDbConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InfluxDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public InfluxDbConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InfluxDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public InfluxDbConfiguration(InfluxDbConfiguration resourceConfiguration)
        : this(new InfluxDbConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InfluxDbConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public InfluxDbConfiguration(InfluxDbConfiguration oldValue, InfluxDbConfiguration newValue)
        : base(oldValue, newValue)
    {
        Username = BuildConfiguration.Combine(oldValue.Username, newValue.Username);
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
        Organization = BuildConfiguration.Combine(oldValue.Organization, newValue.Organization);
        Bucket = BuildConfiguration.Combine(oldValue.Bucket, newValue.Bucket);
        AdminToken = BuildConfiguration.Combine(oldValue.AdminToken, newValue.AdminToken);
        Retention = BuildConfiguration.Combine(oldValue.Retention, newValue.Retention);
    }

    /// <summary>
    /// Gets the InfluxDb username.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the InfluxDb password.
    /// </summary>
    public string Password { get; }

    /// <summary>
    /// Gets the InfluxDb organization.
    /// </summary>
    public string Organization { get; }

    /// <summary>
    /// Gets the InfluxDb bucket.
    /// </summary>
    public string Bucket { get; }

    /// <summary>
    /// Gets the InfluxDb admin token.
    /// </summary>
    public string AdminToken { get; }

    /// <summary>
    /// Gets the InfluxDb retention.
    /// </summary>
    public string Retention { get; }
}