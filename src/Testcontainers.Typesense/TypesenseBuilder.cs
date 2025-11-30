namespace Testcontainers.Typesense;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class TypesenseBuilder : ContainerBuilder<TypesenseBuilder, TypesenseContainer, TypesenseConfiguration>
{
    public const string TypesenseImage = "typesense/typesense:28.0";

    public const ushort TypesensePort = 8108;

    public const string DefaultDataDirectory = "/tmp";

    public const string DefaultApiKey = "testcontainers";

    /// <summary>
    /// Initializes a new instance of the <see cref="TypesenseBuilder" /> class.
    /// </summary>
    [Obsolete("Use constructor with image as a parameter instead.")]
    public TypesenseBuilder()
        : this(new TypesenseConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(TypesenseImage).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypesenseBuilder" /> class.
    /// </summary>
    /// <param name="image">Docker image tag. Available tags can be found here: <see href="https://hub.docker.com/r/typesense/typesense/tags">https://hub.docker.com/r/typesense/typesense/tags</see>.</param>
    public TypesenseBuilder(string image)
        : this(new TypesenseConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypesenseBuilder" /> class.
    /// </summary>
    /// <param name="image">Image instance to use in configuration.</param>
    public TypesenseBuilder(IImage image)
        : this(new TypesenseConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
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
    /// Sets the data directory.
    /// </summary>
    /// <param name="dataDirectoryPath">The data directory path.</param>
    /// <returns>A configured instance of <see cref="TypesenseBuilder" />.</returns>
    public TypesenseBuilder WithDataDirectory(string dataDirectoryPath)
    {
        return WithEnvironment("TYPESENSE_DATA_DIR", dataDirectoryPath);
    }

    /// <summary>
    /// Sets the API key.
    /// </summary>
    /// <param name="apiKey">The API key.</param>
    /// <returns>A configured instance of <see cref="TypesenseBuilder" />.</returns>
    public TypesenseBuilder WithApiKey(string apiKey)
    {
        return WithEnvironment("TYPESENSE_API_KEY", apiKey);
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
            .WithPortBinding(TypesensePort, true)
            .WithDataDirectory(DefaultDataDirectory)
            .WithApiKey(DefaultApiKey)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPort(TypesensePort).ForPath("/health").ForResponseMessageMatching(IsNodeReadyAsync)));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.Environments["TYPESENSE_DATA_DIR"], "DataDirectory")
            .NotNull()
            .NotEmpty();

        _ = Guard.Argument(DockerResourceConfiguration.Environments["TYPESENSE_API_KEY"], "ApiKey")
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

    private static async Task<bool> IsNodeReadyAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync()
            .ConfigureAwait(false);

        return "{\"ok\":true}".Equals(content, StringComparison.OrdinalIgnoreCase);
    }
}