namespace Testcontainers.OpenSearch;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class OpenSearchConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchConfiguration" /> class.
    /// </summary>
    /// <param name="tlsEnabled">A boolean value indicating whether TLS is enabled.</param>
    /// <param name="username">The OpenSearch username.</param>
    /// <param name="password">The OpenSearch password.</param>
    public OpenSearchConfiguration(
        bool? tlsEnabled = null,
        string username = null,
        string password = null)
    {
        TlsEnabled = tlsEnabled;
        Username = username;
        Password = password;
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
        TlsEnabled = BuildConfiguration.Combine(oldValue.TlsEnabled, newValue.TlsEnabled);
        Username = BuildConfiguration.Combine(oldValue.Username, newValue.Username);
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
    }

    /// <summary>
    /// Gets a value indicating whether TLS is enabled or not.
    /// </summary>
    public bool? TlsEnabled { get; }

    /// <summary>
    /// Gets the OpenSearch username.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the OpenSearch password.
    /// </summary>
    public string Password { get; }
}