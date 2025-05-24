namespace Testcontainers.OpenSearch;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class OpenSearchConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchConfiguration" /> class.
    /// </summary>
    /// <param name="password">Password for default user 'admin'.</param>
    public OpenSearchConfiguration(string password = null, bool? disabledSecurity = null) : base()
    {
        Username = OpenSearchBuilder.DefaultUsername;
        Password = password;
        DisabledSecurity = disabledSecurity;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public OpenSearchConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public OpenSearchConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public OpenSearchConfiguration(OpenSearchConfiguration resourceConfiguration)
        : this(new OpenSearchConfiguration(), resourceConfiguration)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public OpenSearchConfiguration(OpenSearchConfiguration oldValue, OpenSearchConfiguration newValue)
        : base(oldValue, newValue)
    {
        Username = BuildConfiguration.Combine(oldValue.Username, newValue.Username);
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
        DisabledSecurity = BuildConfiguration.Combine(oldValue.DisabledSecurity, newValue.DisabledSecurity);
    }

    /// <summary>
    /// Gets the password for default user 'admin'.
    /// </summary>
    public string Password { get; }

    /// <summary>
    /// Gets the default username 'admin'.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the status of security plugin.
    /// Returns 'true' if security plugin is disabled and connection should go over 'http' protocol and 'false' otherwise.
    /// </summary>
    public bool? DisabledSecurity { get; }
}