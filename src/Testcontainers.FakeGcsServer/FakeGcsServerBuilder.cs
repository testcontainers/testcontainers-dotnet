namespace Testcontainers.FakeGcsServer;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class FakeGcsServerBuilder : ContainerBuilder<FakeGcsServerBuilder, FakeGcsServerContainer, FakeGcsServerConfiguration>
{
    public const string FakeGcsServerImage = "fsouza/fake-gcs-server:1.47.5";
    public const ushort FakeGcsServerPort = 4443;
    public const string StartupScriptFilePath = "/testcontainers.sh";

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeGcsServerBuilder" /> class.
    /// </summary>
    public FakeGcsServerBuilder()
        : this(new FakeGcsServerConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeGcsServerBuilder" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    private FakeGcsServerBuilder(FakeGcsServerConfiguration dockerResourceConfiguration)
        : base(dockerResourceConfiguration)
    {
        DockerResourceConfiguration = dockerResourceConfiguration;
    }

    /// <inheritdoc />
    protected override FakeGcsServerConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override FakeGcsServerContainer Build()
    {
        Validate();
        return new FakeGcsServerContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override FakeGcsServerBuilder Init()
    {
        return base.Init()
            .WithImage(FakeGcsServerImage)
            .WithPortBinding(FakeGcsServerPort, true)
            .WithEntrypoint("/bin/sh", "-c")
            .WithCommand($"while [ ! -f {StartupScriptFilePath} ]; do sleep 0.1; done; sh {StartupScriptFilePath}")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(new Regex("server started at.*", RegexOptions.IgnoreCase)))
            .WithStartupCallback((container, ct) =>
            {
                const char lf = '\n';
                var startupScript = new StringBuilder();
                startupScript.Append("#!/bin/bash");
                startupScript.Append(lf);
                startupScript.Append($"fake-gcs-server -backend memory -scheme http -port {FakeGcsServerPort} -external-url {new UriBuilder(Uri.UriSchemeHttp, container.Hostname, container.GetMappedPublicPort(FakeGcsServerPort))}");
                startupScript.Append(lf);
                return container.CopyAsync(Encoding.Default.GetBytes(startupScript.ToString()), StartupScriptFilePath, Unix.FileMode755, ct);
            });
    }

    /// <inheritdoc />
    protected override FakeGcsServerBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new FakeGcsServerConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override FakeGcsServerBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new FakeGcsServerConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override FakeGcsServerBuilder Merge(FakeGcsServerConfiguration oldValue, FakeGcsServerConfiguration newValue)
    {
        return new FakeGcsServerBuilder(new FakeGcsServerConfiguration(oldValue, newValue));
    }
}