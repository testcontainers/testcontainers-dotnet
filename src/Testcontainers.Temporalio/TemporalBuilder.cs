namespace Testcontainers.Temporalio;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class TemporalBuilder : ContainerBuilder<TemporalBuilder, TemporalContainer, TemporalConfiguration>
{
    public const int TemporalGrpcPort = 7233;

    public const int TemporalHttpPort = 8233;

    /// <summary>
    /// Initializes a new instance of the <see cref="TemporalBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>temporalio/temporal:1.5.1</c>, <c>temporalio/temporal:latest</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/temporalio/temporal/tags" />.
    /// </remarks>
    public TemporalBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemporalBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/temporalio/temporal/tags" />.
    /// </remarks>
    public TemporalBuilder(IImage image)
        : this(new TemporalConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemporalBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private TemporalBuilder(TemporalConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override TemporalConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Adds a namespace to pre-register at startup. The "default" namespace is always
    /// registered regardless. Multiple namespaces can be added by chaining calls.
    /// </summary>
    /// <param name="namespace">The namespace name to pre-register.</param>
    /// <returns>A configured instance of <see cref="TemporalBuilder" />.</returns>
    public TemporalBuilder WithNamespace(string @namespace)
    {
        return Merge(DockerResourceConfiguration, new TemporalConfiguration(namespaces: [@namespace]));
    }

    /// <summary>
    /// Registers a custom search attribute. Type must be one of:
    /// Text, Keyword, Int, Double, Bool, Datetime, KeywordList.
    /// </summary>
    /// <param name="name">The search attribute name.</param>
    /// <param name="type">The search attribute type.</param>
    /// <returns>A configured instance of <see cref="TemporalBuilder" />.</returns>
    public TemporalBuilder WithSearchAttribute(string name, string type)
    {
        return Merge(DockerResourceConfiguration, new TemporalConfiguration(searchAttributes: [name + "=" + type]));
    }

    /// <summary>
    /// Sets a dynamic configuration value. Keys must be identifiers, and values must be
    /// JSON values (e.g., key <c>frontend.enableUpdateWorkflowExecution</c> with value <c>true</c>).
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <param name="jsonValue">The configuration value as a JSON literal.</param>
    /// <returns>A configured instance of <see cref="TemporalBuilder" />.</returns>
    public TemporalBuilder WithDynamicConfigValue(string key, string jsonValue)
    {
        return Merge(DockerResourceConfiguration, new TemporalConfiguration(dynamicConfigValues: [key + "=" + jsonValue]));
    }

    /// <summary>
    /// Sets the path to a database file inside the container for persistent Temporal state.
    /// By default, Workflow Executions are lost when the container is removed.
    /// </summary>
    /// <param name="path">The path to the database file inside the container.</param>
    /// <returns>A configured instance of <see cref="TemporalBuilder" />.</returns>
    public TemporalBuilder WithDbFilename(string path)
    {
        return Merge(DockerResourceConfiguration, new TemporalConfiguration(dbFilename: path));
    }

    /// <inheritdoc />
    public override TemporalContainer Build()
    {
        Validate();

        var command = new List<string> { "server", "start-dev", "--ip", "0.0.0.0" };

        if (DockerResourceConfiguration.Namespaces != null && DockerResourceConfiguration.Namespaces.Any())
        {
            foreach (var ns in DockerResourceConfiguration.Namespaces)
            {
                command.Add("--namespace");
                command.Add(ns);
            }
        }

        if (DockerResourceConfiguration.SearchAttributes != null && DockerResourceConfiguration.SearchAttributes.Any())
        {
            foreach (var attr in DockerResourceConfiguration.SearchAttributes)
            {
                command.Add("--search-attribute");
                command.Add(attr);
            }
        }

        if (DockerResourceConfiguration.DynamicConfigValues != null && DockerResourceConfiguration.DynamicConfigValues.Any())
        {
            foreach (var value in DockerResourceConfiguration.DynamicConfigValues)
            {
                command.Add("--dynamic-config-value");
                command.Add(value);
            }
        }

        if (!string.IsNullOrEmpty(DockerResourceConfiguration.DbFilename))
        {
            command.Add("--db-filename");
            command.Add(DockerResourceConfiguration.DbFilename);
        }

        var temporalBuilder = WithCommand(command.ToArray());
        return new TemporalContainer(temporalBuilder.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override TemporalBuilder Init()
    {
        return base.Init()
            .WithPortBinding(TemporalGrpcPort, true)
            .WithPortBinding(TemporalHttpPort, true)
            .WithConnectionStringProvider(new TemporalConnectionStringProvider())
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilInternalTcpPortIsAvailable(TemporalGrpcPort)
                .UntilInternalTcpPortIsAvailable(TemporalHttpPort)
                .UntilHttpRequestIsSucceeded(request =>
                    request.ForPath("/api/v1/namespaces").ForPort(TemporalHttpPort)));
    }

    /// <inheritdoc />
    protected override TemporalBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new TemporalConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override TemporalBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new TemporalConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override TemporalBuilder Merge(TemporalConfiguration oldValue, TemporalConfiguration newValue)
    {
        return new TemporalBuilder(new TemporalConfiguration(oldValue, newValue));
    }
}
