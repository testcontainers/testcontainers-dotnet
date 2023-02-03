namespace Testcontainers.MsSql;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class MsSqlBuilder : ContainerBuilder<MsSqlBuilder, MsSqlContainer, MsSqlConfiguration>
{
    public const string MsSqlImage = "2019-CU18-ubuntu-20.04";

    public const ushort MsSqlPort = 1433;

    /// <summary>
    /// Initializes a new instance of the <see cref="MsSqlBuilder" /> class.
    /// </summary>
    public MsSqlBuilder()
        : this(new MsSqlConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MsSqlBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private MsSqlBuilder(MsSqlConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override MsSqlConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the MsSql password.
    /// </summary>
    /// <param name="password">The MsSql password.</param>
    /// <returns>A configured instance of <see cref="MsSqlBuilder" />.</returns>
    public MsSqlBuilder WithPassword(string password)
    {
        return Clone(new MsSqlConfiguration(password: password)).WithEnvironment("MSSQL_SA_PASSWORD", password);
    }

    /// <inheritdoc />
    public override MsSqlContainer Build()
    {
        Validate();
        return new MsSqlContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override MsSqlBuilder Init()
    {
        return base.Init()
            .WithImage(MsSqlImage)
            .WithPortBinding(MsSqlPort, true)
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithDatabase("master")
            .WithUsername("sa")
            .WithPassword(Guid.NewGuid().ToString());
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
    protected override MsSqlBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MsSqlConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MsSqlBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MsSqlConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MsSqlBuilder Merge(MsSqlConfiguration oldValue, MsSqlConfiguration newValue)
    {
        return new MsSqlBuilder(new MsSqlConfiguration(oldValue, newValue));
    }

    /// <summary>
    /// Sets the MsSql database.
    /// </summary>
    /// <remarks>
    /// The Docker image does not allow to configure the database.
    /// </remarks>
    /// <param name="database">The MsSql database.</param>
    /// <returns>A configured instance of <see cref="MsSqlBuilder" />.</returns>
    private MsSqlBuilder WithDatabase(string database)
    {
        return Clone(new MsSqlConfiguration(database: database));
    }

    /// <summary>
    /// Sets the MsSql username.
    /// </summary>
    /// <remarks>
    /// The Docker image does not allow to configure the username.
    /// </remarks>
    /// <param name="username">The MsSql username.</param>
    /// <returns>A configured instance of <see cref="MsSqlBuilder" />.</returns>
    private MsSqlBuilder WithUsername(string username)
    {
        return Clone(new MsSqlConfiguration(username: username));
    }
}