using System;

namespace TestContainers.Minio;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class MinioConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Minio UserName
    /// </summary>
    public string UserName { get; }
    /// <summary>
    /// Minio Password
    /// </summary>
    public string Password { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MinioConfiguration" /> class.
    /// </summary>
    /// <param name="userName">The Minio database.</param>
    /// <param name="password">The Minio username.</param>
    public MinioConfiguration(string userName = "ROOTNAME", string password = "ChangeMe2137") : base()
    {
        this.UserName = userName;
        this.Password = password;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MinioConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MinioConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MinioConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MinioConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MinioConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MinioConfiguration(MinioConfiguration resourceConfiguration)
        : this(new MinioConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MinioConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public MinioConfiguration(MinioConfiguration oldValue, MinioConfiguration newValue)
        : base(oldValue, newValue)
    {
        UserName = BuildConfiguration.Combine(oldValue.UserName, newValue.UserName);
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
    }
}