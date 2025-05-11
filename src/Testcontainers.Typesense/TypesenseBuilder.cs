namespace Testcontainers.Typesense;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class TypesenseBuilder : ContainerBuilder<TypesenseBuilder, TypesenseContainer, TypesenseConfiguration>
{
    public const string TypesenseImage = "typesense/typesense:28.0";

    public const ushort DefaultPort = 8108;

    public const string DefaultApiKey = "iXy3xHM!KRogYBLg&nRg0V";

    public const bool DefaultCors = true;

    public const string DefaultVolume = "/data";

    /// <summary>
    /// Initializes a new instance of the <see cref="TypesenseBuilder" /> class.
    /// </summary>
    public TypesenseBuilder()
        : this(new TypesenseConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypesenseBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private TypesenseBuilder(TypesenseConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override TypesenseConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the Api key.
    /// </summary>
    /// <param name="apiKey">The Api key.</param>
    /// <returns>A configured instance of <see cref="TypesenseBuilder" />.</returns>
    public TypesenseBuilder WithApiKey(string apiKey)
    {
        return Merge(DockerResourceConfiguration, new TypesenseConfiguration(apiKey: apiKey))
          .WithCommand("--api-key", apiKey);
    }

    /// <summary>
    /// Enables Cors.
    /// </summary>
    /// <returns>A configured instance of <see cref="TypesenseBuilder" />.</returns>
    public TypesenseBuilder EnableCors(bool enableCors)
    {
        return Merge(DockerResourceConfiguration, new TypesenseConfiguration(enableCors: enableCors))
          .WithCommand("--enable-cors");
    }

    /// <summary>
    /// Enables Cors.
    /// </summary>
    /// <returns>A configured instance of <see cref="TypesenseBuilder" />.</returns>
    public TypesenseBuilder WithDataDirectory(string volume)
    {
        return Merge(DockerResourceConfiguration, new TypesenseConfiguration(volume: volume))
          .WithCommand("--data-dir", volume);
    }

    /// <inheritdoc />
    public override TypesenseContainer Build()
    {
        Validate();
        return new TypesenseContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override TypesenseBuilder Init()
    {
        return base.Init()
            .WithImage(TypesenseImage)
            .WithPortBinding(DefaultPort, true)
            .WithApiKey(DefaultApiKey)
            .EnableCors(DefaultCors)
            .WithTmpfsMount(DefaultVolume)
            .WithDataDirectory(DefaultVolume)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Peer refresh succeeded"));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.ApiKey, nameof(DockerResourceConfiguration.ApiKey))
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc />
    protected override TypesenseBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new TypesenseConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override TypesenseBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new TypesenseConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override TypesenseBuilder Merge(TypesenseConfiguration oldValue, TypesenseConfiguration newValue)
    {
        return new TypesenseBuilder(new TypesenseConfiguration(oldValue, newValue));
    }
}
