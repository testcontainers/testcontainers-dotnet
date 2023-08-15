namespace Testcontainers.InfluxDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class InfluxDbBuilder : ContainerBuilder<InfluxDbBuilder, InfluxDbContainer, InfluxDbConfiguration>
{
    public const string InfluxDbImage = "influxdb:2.7";

    public const ushort InfluxDbPort = 8086;

    public const string DefaultUsername = "username";

    public const string DefaultPassword = "password";

    public const string DefaultOrganization = "organization";

    public const string DefaultBucket = "bucket";

    /// <summary>
    /// Initializes a new instance of the <see cref="InfluxDbBuilder" /> class.
    /// </summary>
    public InfluxDbBuilder()
        : this(new InfluxDbConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InfluxDbBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private InfluxDbBuilder(InfluxDbConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override InfluxDbConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the InfluxDb username.
    /// </summary>
    /// <param name="username">The InfluxDb username.</param>
    /// <returns>A configured instance of <see cref="InfluxDbBuilder" />.</returns>
    public InfluxDbBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new InfluxDbConfiguration(username: username))
            .WithEnvironment("DOCKER_INFLUXDB_INIT_USERNAME", username);
    }

    /// <summary>
    /// Sets the InfluxDb password.
    /// </summary>
    /// <param name="password">The InfluxDb password.</param>
    /// <returns>A configured instance of <see cref="InfluxDbBuilder" />.</returns>
    public InfluxDbBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new InfluxDbConfiguration(password: password))
            .WithEnvironment("DOCKER_INFLUXDB_INIT_PASSWORD", password);
    }

    /// <summary>
    /// Sets the InfluxDb organization.
    /// </summary>
    /// <param name="organization">The InfluxDb organization.</param>
    /// <returns>A configured instance of <see cref="InfluxDbBuilder" />.</returns>
    public InfluxDbBuilder WithOrganization(string organization)
    {
        return Merge(DockerResourceConfiguration, new InfluxDbConfiguration(organization: organization))
            .WithEnvironment("DOCKER_INFLUXDB_INIT_ORG", organization);
    }

    /// <summary>
    /// Sets the InfluxDb bucket.
    /// </summary>
    /// <param name="bucket">The InfluxDb bucket.</param>
    /// <returns>A configured instance of <see cref="InfluxDbBuilder" />.</returns>
    public InfluxDbBuilder WithBucket(string bucket)
    {
        return Merge(DockerResourceConfiguration, new InfluxDbConfiguration(bucket: bucket))
            .WithEnvironment("DOCKER_INFLUXDB_INIT_BUCKET", bucket);
    }

    /// <summary>
    /// Sets the InfluxDb admin token.
    /// </summary>
    /// <param name="adminToken">The InfluxDb admin token.</param>
    /// <returns>A configured instance of <see cref="InfluxDbBuilder" />.</returns>
    public InfluxDbBuilder WithAdminToken(string adminToken)
    {
        return Merge(DockerResourceConfiguration, new InfluxDbConfiguration(adminToken: adminToken))
            .WithEnvironment("DOCKER_INFLUXDB_INIT_ADMIN_TOKEN", adminToken);
    }

    /// <summary>
    /// Sets the InfluxDb retention.
    /// </summary>
    /// <param name="retention">The InfluxDb retention.</param>
    /// <returns>A configured instance of <see cref="InfluxDbBuilder" />.</returns>
    public InfluxDbBuilder WithRetention(string retention)
    {
        return Merge(DockerResourceConfiguration, new InfluxDbConfiguration(retention: retention))
            .WithEnvironment("DOCKER_INFLUXDB_INIT_RETENTION", retention);
    }

    /// <inheritdoc />
    public override InfluxDbContainer Build()
    {
        Validate();
        return new InfluxDbContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override InfluxDbBuilder Init()
    {
        return base.Init()
            .WithImage(InfluxDbImage)
            .WithPortBinding(InfluxDbPort, true)
            .WithEnvironment("DOCKER_INFLUXDB_INIT_MODE", "setup")
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            .WithOrganization(DefaultOrganization)
            .WithBucket(DefaultBucket)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPort(InfluxDbPort).ForPath("/ping").ForStatusCode(HttpStatusCode.NoContent)));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.Username, nameof(DockerResourceConfiguration.Username))
            .NotNull()
            .NotEmpty();

        _ = Guard.Argument(DockerResourceConfiguration.Password, nameof(DockerResourceConfiguration.Password))
            .NotNull()
            .NotEmpty();

        _ = Guard.Argument(DockerResourceConfiguration.Organization, nameof(DockerResourceConfiguration.Organization))
            .NotNull()
            .NotEmpty();

        _ = Guard.Argument(DockerResourceConfiguration.Bucket, nameof(DockerResourceConfiguration.Bucket))
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc />
    protected override InfluxDbBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new InfluxDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override InfluxDbBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new InfluxDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override InfluxDbBuilder Merge(InfluxDbConfiguration oldValue, InfluxDbConfiguration newValue)
    {
        return new InfluxDbBuilder(new InfluxDbConfiguration(oldValue, newValue));
    }
}