using System.Collections.Generic;

namespace Testcontainers.Minio;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class LocalStackConfiguration : ContainerConfiguration
{
    public string ExternalServicePortStart { get; }
    public string ExternalServicePortEnd { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStackConfiguration" /> class.
    /// </summary>
    /// <param name="externalServicePortStart">The LocalStack services list.</param>
    /// <param name="externalServicePortEnd">The LocalStack services list.</param>
    public LocalStackConfiguration(string externalServicePortStart = null, string externalServicePortEnd = null)
    {
        ExternalServicePortStart = externalServicePortStart;
        ExternalServicePortEnd = externalServicePortEnd;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStackConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public LocalStackConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStackConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public LocalStackConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStackConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public LocalStackConfiguration(LocalStackConfiguration resourceConfiguration)
        : this(new LocalStackConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStackConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public LocalStackConfiguration(LocalStackConfiguration oldValue, LocalStackConfiguration newValue)
        : base(oldValue, newValue)
    {
        ExternalServicePortStart = BuildConfiguration.Combine(oldValue.ExternalServicePortStart, newValue.ExternalServicePortStart);
        ExternalServicePortEnd = BuildConfiguration.Combine(oldValue.ExternalServicePortEnd, newValue.ExternalServicePortEnd);
    }
}