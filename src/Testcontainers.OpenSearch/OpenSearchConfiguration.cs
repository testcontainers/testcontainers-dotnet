namespace Testcontainers.OpenSearch;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class OpenSearchConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchConfiguration" /> class.
    /// </summary>
    /// <param name="password">Password for default user 'admin'.</param>
    public OpenSearchConfiguration(string password = null)
    {
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
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
    }

    /// <summary>
    /// Gets the password for default user 'admin'.
    /// </summary>
    public string Password { get; }
}