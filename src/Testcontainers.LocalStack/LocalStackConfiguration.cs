using System.Collections.Generic;

namespace Testcontainers.Minio;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class LocalStackConfiguration : ContainerConfiguration
{
    public IEnumerable<AwsService> Services { get; }
    public string DefaultRegion { get; }
    public string ExternalServicePortStart { get; }
    public string ExternalServicePortEnd { get; }
    
    public string UseSsl { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStackConfiguration" /> class.
    /// </summary>
    /// <param name="services">The LocalStack services list.</param>
    /// <param name="defaultRegion">The LocalStack services list.</param>
    /// <param name="externalServicePortStart">The LocalStack services list.</param>
    /// <param name="externalServicePortEnd">The LocalStack services list.</param>
    /// <param name="useSsl">The LocalStack use ssl param.</param>
    public LocalStackConfiguration(IEnumerable<AwsService> services = null, string defaultRegion = null, string externalServicePortStart = null, string externalServicePortEnd = null, string useSsl = null)
    {
        Services = services;
        DefaultRegion = defaultRegion;
        ExternalServicePortStart = externalServicePortStart;
        ExternalServicePortEnd = externalServicePortEnd;
        UseSsl = useSsl;
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
        Services = BuildConfiguration.Combine(oldValue.Services, newValue.Services);
        DefaultRegion = BuildConfiguration.Combine(oldValue.DefaultRegion, newValue.DefaultRegion);
        ExternalServicePortStart = BuildConfiguration.Combine(oldValue.ExternalServicePortStart, newValue.ExternalServicePortStart);
        ExternalServicePortEnd = BuildConfiguration.Combine(oldValue.ExternalServicePortEnd, newValue.ExternalServicePortEnd);
    }
}