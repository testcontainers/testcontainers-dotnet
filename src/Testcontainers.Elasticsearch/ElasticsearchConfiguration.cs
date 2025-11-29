namespace Testcontainers.Elasticsearch;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class ElasticsearchConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchConfiguration" /> class.
    /// </summary>
    /// <param name="username">The Elasticsearch username.</param>
    /// <param name="password">The Elasticsearch password.</param>
    public ElasticsearchConfiguration(
        string username = null,
        string password = null)
    {
        Username = username;
        Password = password;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ElasticsearchConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ElasticsearchConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ElasticsearchConfiguration(ElasticsearchConfiguration resourceConfiguration)
        : this(new ElasticsearchConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public ElasticsearchConfiguration(ElasticsearchConfiguration oldValue, ElasticsearchConfiguration newValue)
        : base(oldValue, newValue)
    {
        Username = BuildConfiguration.Combine(oldValue.Username, newValue.Username);
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
    }

    /// <summary>
    /// Gets the Elasticsearch username.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the Elasticsearch password.
    /// </summary>
    public string Password { get; }

    /// <summary>
    /// Returns <c>true</c> if https connection to container is enabled, based on configuration environment variables.
    /// </summary>
    public bool HttpsEnabled
    {
        get
        {
            var hasSecurityEnabled = Environments
                .TryGetValue("xpack.security.enabled", out var securityEnabled);

            var hasHttpSslEnabled = Environments
                .TryGetValue("xpack.security.http.ssl.enabled", out var httpSslEnabled);

            var httpsDisabled =
                hasSecurityEnabled &&
                hasHttpSslEnabled &&
                "false".Equals(securityEnabled, StringComparison.OrdinalIgnoreCase) &&
                "false".Equals(httpSslEnabled, StringComparison.OrdinalIgnoreCase);

            return !httpsDisabled;
        }
    }
}