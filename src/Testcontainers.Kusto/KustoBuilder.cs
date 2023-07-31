namespace Testcontainers.Kusto;

using System.Threading.Tasks;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
/// <remarks>
/// Builds a container running the "Azure Data Explorer Kusto emulator":
/// https://learn.microsoft.com/en-us/azure/data-explorer/kusto-emulator-overview
/// </remarks>
[PublicAPI]
public sealed class KustoBuilder : ContainerBuilder<KustoBuilder, KustoContainer, KustoConfiguration>
{
    public const string KustoImage = "mcr.microsoft.com/azuredataexplorer/kustainer-linux:latest";

    public const ushort KustoPort = 8080;

    /// <summary>
    /// Initializes a new instance of the <see cref="KustoBuilder" /> class.
    /// </summary>
    public KustoBuilder()
        : this(new KustoConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KustoBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private KustoBuilder(KustoConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    // /// <inheritdoc />
    protected override KustoConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override KustoContainer Build()
    {
        Validate();
        return new KustoContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override KustoBuilder Init()
    {
        return base.Init()
            .WithImage(KustoImage)
            .WithPortBinding(KustoPort, true)
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil()));
    }

    /// <inheritdoc />
    protected override KustoBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new KustoConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override KustoBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new KustoConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override KustoBuilder Merge(KustoConfiguration oldValue, KustoConfiguration newValue)
    {
        return new KustoBuilder(new KustoConfiguration(oldValue, newValue));
    }

    /// <inheritdoc cref="IWaitUntil" />
    private sealed class WaitUntil : IWaitUntil
    {
        private readonly string[] _command =
        {
            "curl",
            "-X",
            "POST",
            "-i",
            "-H",
            "Content-Type: application/json",
            "-d",
            "{\"csl\":\".show cluster\"}",
            "http://localhost:8080/v1/rest/mgmt"
        };

        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            var execResult = await container.ExecAsync(_command)
                .ConfigureAwait(false);

            return execResult.ExitCode == 0 &&
                execResult.Stdout.Contains("200 OK");
        }
    }
}