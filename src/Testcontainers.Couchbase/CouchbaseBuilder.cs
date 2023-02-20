namespace Testcontainers.Couchbase;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class CouchbaseBuilder : ContainerBuilder<CouchbaseBuilder, CouchbaseContainer, CouchbaseConfiguration>
{
    public const string CouchbaseImage = "couchbase:7.1.3";

    public const ushort MgmtPort = 8091;

    public const ushort MgmtSslPort = 18091;

    public const ushort ViewPort = 8092;

    public const ushort ViewSslPort = 18092;

    public const ushort QueryPort = 8093;

    public const ushort QuerySslPort = 18093;

    public const ushort SearchPort = 8094;

    public const ushort SearchSslPort = 18094;

    public const ushort AnalyticsPort = 8095;

    public const ushort AnalyticsSslPort = 18095;

    public const ushort EventingPort = 8096;

    public const ushort EventingSslPort = 18096;

    public const ushort KvPort = 11210;

    public const ushort KvSslPort = 11207;

    private const string DefaultUsername = "Administrator";

    private const string DefaultPassword = "password";

    /// <summary>
    /// Initializes a new instance of the <see cref="CouchbaseBuilder" /> class.
    /// </summary>
    public CouchbaseBuilder()
        : this(new CouchbaseConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CouchbaseBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private CouchbaseBuilder(CouchbaseConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override CouchbaseConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override CouchbaseContainer Build()
    {
        Validate();
        return new CouchbaseContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override CouchbaseBuilder Init()
    {
        return base.Init()
            .WithImage(CouchbaseImage)
            .WithPortBinding(MgmtPort, true)
            // .WithPortBinding(MgmtSslPort, true)
            .WithPortBinding(ViewPort, true)
            // .WithPortBinding(ViewSslPort, true)
            .WithPortBinding(QueryPort, true)
            // .WithPortBinding(QuerySslPort, true)
            .WithPortBinding(SearchPort, true)
            // .WithPortBinding(SearchSslPort, true)
            .WithPortBinding(AnalyticsPort, true)
            // .WithPortBinding(AnalyticsSslPort, true)
            .WithPortBinding(EventingPort, true)
            // .WithPortBinding(EventingSslPort, true)
            .WithPortBinding(KvPort, true)
            // .WithPortBinding(KvSslPort, true)
            .WithStartupCallback(Foo)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(request
                    => request
                        .ForPath("/pools/default")
                        .ForPort(MgmtPort)
                        .WithBasicAuthentication(DefaultUsername, DefaultPassword))
                .UntilHttpRequestIsSucceeded(request
                    => request
                        .ForPath("/admin/ping")
                        .ForPort(QueryPort)
                        .WithBasicAuthentication(DefaultUsername, DefaultPassword))
                .UntilHttpRequestIsSucceeded(request
                    => request
                        .ForPath("/admin/ping")
                        .ForPort(AnalyticsPort)
                        .WithBasicAuthentication(DefaultUsername, DefaultPassword))
                .UntilHttpRequestIsSucceeded(request
                    => request
                        .ForPath("/api/v1/config")
                        .ForPort(EventingPort)
                        .WithBasicAuthentication(DefaultUsername, DefaultPassword)));
    }

    private Task Foo(IContainer container, CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }

    // /// <inheritdoc />
    // protected override void Validate()
    // {
    //     base.Validate();
    // }

    /// <inheritdoc />
    protected override CouchbaseBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new CouchbaseConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override CouchbaseBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new CouchbaseConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override CouchbaseBuilder Merge(CouchbaseConfiguration oldValue, CouchbaseConfiguration newValue)
    {
        return new CouchbaseBuilder(new CouchbaseConfiguration(oldValue, newValue));
    }
}