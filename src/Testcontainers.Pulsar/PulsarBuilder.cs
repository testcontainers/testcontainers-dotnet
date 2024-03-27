namespace Testcontainers.Pulsar;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class PulsarBuilder : ContainerBuilder<PulsarBuilder, PulsarContainer, PulsarConfiguration>
{
    public const string PulsarImage = "apachepulsar/pulsar:3.2.0";

    public const ushort BrokerPort = 6650;
    public const ushort BrokerHttpPort = 8080;

    /// <summary>
    /// Initializes a new instance of the <see cref="PulsarBuilder" /> class.
    /// </summary>
    public PulsarBuilder()
        : this(new PulsarConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PulsarBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private PulsarBuilder(PulsarConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override PulsarConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override PulsarContainer Build()
    {
        Validate();
        return new PulsarContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override PulsarBuilder Init()
    {
        return base.Init()
            .WithImage(PulsarImage)
            .WithPortBinding(BrokerPort, true)
            .WithPortBinding(BrokerHttpPort, true)
            .WithEntrypoint("/bin/bash", "-c", "/pulsar/bin/apply-config-from-env.py /pulsar/conf/standalone.conf && bin/pulsar standalone --no-functions-worker -nss")
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilPortIsAvailable(BrokerPort)
                .UntilPortIsAvailable(BrokerHttpPort)
                .UntilHttpRequestIsSucceeded(request => request.ForPort(BrokerHttpPort).ForPath("/admin/v2/clusters").ForResponseMessageMatching(IsNodeReadyAsync)));
    }

    /// <inheritdoc />
    protected override PulsarBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new PulsarConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override PulsarBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new PulsarConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override PulsarBuilder Merge(PulsarConfiguration oldValue, PulsarConfiguration newValue)
    {
        return new PulsarBuilder(new PulsarConfiguration(oldValue, newValue));
    }

    private async Task<bool> IsNodeReadyAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync()
            .ConfigureAwait(false);

        return "[\"standalone\"]".Equals(content, StringComparison.OrdinalIgnoreCase);
    }
}