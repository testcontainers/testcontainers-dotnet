namespace Testcontainers.Gitlab;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class GitlabConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GitlabConfiguration" /> class.
    /// </summary>
    /// <param name="password">The Gitlab admin password.</param>
    public GitlabConfiguration(string password)
    {
        _password = password;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GitlabConfiguration" /> class.
    /// </summary>
    public GitlabConfiguration()
    {
        _password = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GitlabConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public GitlabConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GitlabConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public GitlabConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GitlabConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public GitlabConfiguration(GitlabConfiguration resourceConfiguration)
        : this(new GitlabConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GitlabConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public GitlabConfiguration(GitlabConfiguration oldValue, GitlabConfiguration newValue)
        : base(oldValue, newValue)
    {
        _password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
    }

    /// <summary>
    /// Gets the Gitlab admin password.
    /// </summary>
    public string Password => _password;

    private readonly string _password;
}