namespace Testcontainers.Redpanda;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class RedpandaBuilder : ContainerBuilder<RedpandaBuilder, RedpandaContainer, RedpandaConfiguration>
{
    public const string RedpandaImage = "docker.redpanda.com/vectorized/redpanda:v22.2.1";

    public const ushort SchemaRegistryPort = 8081;

    public const ushort BrokerPort = 9092;

    public const string StarterScript = "/testcontainers.sh";

    /// <summary>
    /// Initializes a new instance of the <see cref="RedpandaBuilder" /> class.
    /// </summary>
    public RedpandaBuilder()
        : this(new RedpandaConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedpandaBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private RedpandaBuilder(RedpandaConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override RedpandaConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override RedpandaContainer Build()
    {
        Validate();
        return new RedpandaContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override RedpandaBuilder Init()
    {
        return base.Init()
            .WithImage(RedpandaImage)
            .WithPortBinding(SchemaRegistryPort, true)
            .WithPortBinding(BrokerPort, true)
            .WithEntrypoint("/bin/sh", "-c")
            .WithCommand("while [ ! -f " + StarterScript + " ]; do sleep 0.1; done; " + StarterScript)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Started Kafka API server"))
            .WithStartupCallback((container, ct) =>
            {
                var cmd = "#!/bin/bash\n";
                cmd += "/usr/bin/rpk redpanda start --mode dev-container ";
                cmd += "--kafka-addr PLAINTEXT://0.0.0.0:29092,OUTSIDE://0.0.0.0:9092 ";
                cmd += "--advertise-kafka-addr PLAINTEXT://127.0.0.1:29092,OUTSIDE://" + container.Hostname + ":" + container.GetMappedPublicPort(BrokerPort);
                return container.CopyFileAsync(StarterScript, Encoding.Default.GetBytes(cmd), 493, ct: ct);
            });
    }

    /// <inheritdoc />
    protected override RedpandaBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new RedpandaConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override RedpandaBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new RedpandaConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override RedpandaBuilder Merge(RedpandaConfiguration oldValue, RedpandaConfiguration newValue)
    {
        return new RedpandaBuilder(new RedpandaConfiguration(oldValue, newValue));
    }
}