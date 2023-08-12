namespace Testcontainers.ClickHouse;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ClickHouseBuilder : ContainerBuilder<ClickHouseBuilder, ClickHouseContainer, ClickHouseConfiguration>
{
    public const string ClickHouseImage = "clickhouse/clickhouse-server:23.6-alpine";

    public const ushort HttpPort = 8123;

    public const ushort NativePort = 9000;

    public const string DefaultDatabase = "default";

    public const string DefaultUsername = "clickhouse";

    public const string DefaultPassword = "clickhouse";

    /// <summary>
    /// Initializes a new instance of the <see cref="ClickHouseBuilder" /> class.
    /// </summary>
    public ClickHouseBuilder()
        : this(new ClickHouseConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClickHouseBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private ClickHouseBuilder(ClickHouseConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override ClickHouseConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the ClickHouse database.
    /// </summary>
    /// <param name="database">The ClickHouse database.</param>
    /// <returns>A configured instance of <see cref="ClickHouseBuilder" />.</returns>
    public ClickHouseBuilder WithDatabase(string database)
    {
        return Merge(DockerResourceConfiguration, new ClickHouseConfiguration(database: database))
            .WithEnvironment("CLICKHOUSE_DB", database);
    }

    /// <summary>
    /// Sets the ClickHouse username.
    /// </summary>
    /// <param name="username">The ClickHouse username.</param>
    /// <returns>A configured instance of <see cref="ClickHouseBuilder" />.</returns>
    public ClickHouseBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new ClickHouseConfiguration(username: username))
            .WithEnvironment("CLICKHOUSE_USER", username);
    }

    /// <summary>
    /// Sets the ClickHouse password.
    /// </summary>
    /// <param name="password">The ClickHouse password.</param>
    /// <returns>A configured instance of <see cref="ClickHouseBuilder" />.</returns>
    public ClickHouseBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new ClickHouseConfiguration(password: password))
            .WithEnvironment("CLICKHOUSE_PASSWORD", password);
    }

    /// <inheritdoc />
    public override ClickHouseContainer Build()
    {
        Validate();
        return new ClickHouseContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override ClickHouseBuilder Init()
    {
        return base.Init()
            .WithImage(ClickHouseImage)
            .WithPortBinding(HttpPort, true)
            .WithPortBinding(NativePort, true)
            .WithDatabase(DefaultDatabase)
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPort(HttpPort).ForResponseMessageMatching(IsNodeReadyAsync)));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.Password, nameof(DockerResourceConfiguration.Password))
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc />
    protected override ClickHouseBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ClickHouseConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ClickHouseBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ClickHouseConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ClickHouseBuilder Merge(ClickHouseConfiguration oldValue, ClickHouseConfiguration newValue)
    {
        return new ClickHouseBuilder(new ClickHouseConfiguration(oldValue, newValue));
    }

    private async Task<bool> IsNodeReadyAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync()
            .ConfigureAwait(false);

        return "Ok.\n".Equals(content, StringComparison.OrdinalIgnoreCase);
    }
}