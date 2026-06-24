namespace Testcontainers.Sftp;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class SftpConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SftpConfiguration" /> class.
    /// </summary>
    /// <param name="username">The Sftp username.</param>
    /// <param name="password">The Sftp password.</param>
    /// <param name="uploadDirectory">The directory to which files are uploaded.</param>
    public SftpConfiguration(
        string username = null,
        string password = null,
        string uploadDirectory = null)
    {
        Username = username;
        Password = password;
        UploadDirectory = uploadDirectory;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SftpConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public SftpConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SftpConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public SftpConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SftpConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public SftpConfiguration(SftpConfiguration resourceConfiguration)
        : this(new SftpConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SftpConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public SftpConfiguration(SftpConfiguration oldValue, SftpConfiguration newValue)
        : base(oldValue, newValue)
    {
        Username = BuildConfiguration.Combine(oldValue.Username, newValue.Username);
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
        UploadDirectory = BuildConfiguration.Combine(oldValue.UploadDirectory, newValue.UploadDirectory);
    }

    /// <summary>
    /// Gets the Sftp username.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the Sftp password.
    /// </summary>
    public string Password { get; }

    /// <summary>
    /// Gets the directory to which files are uploaded.
    /// </summary>
    public string UploadDirectory { get; }
}