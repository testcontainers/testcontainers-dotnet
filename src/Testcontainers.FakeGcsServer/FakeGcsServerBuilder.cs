namespace Testcontainers.FakeGcsServer;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class FakeGcsServerBuilder : ContainerBuilder<FakeGcsServerBuilder, FakeGcsServerContainer, FakeGcsServerConfiguration>
{
    [Obsolete("This image tag is not recommended: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public const string FakeGcsServerImage = "fsouza/fake-gcs-server:1.47";

    public const ushort FakeGcsServerPort = 4443;

    public const string StartupScriptFilePath = "/testcontainers.sh";

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeGcsServerBuilder" /> class.
    /// </summary>
    [Obsolete("Use the constructor with the image argument instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public FakeGcsServerBuilder()
        : this(FakeGcsServerImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeGcsServerBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>fsouza/fake-gcs-server:1.47</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/fsouza/fake-gcs-server/tags" />.
    /// </remarks>
    public FakeGcsServerBuilder(string image)
        : this(new FakeGcsServerConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeGcsServerBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/fsouza/fake-gcs-server/tags" />.
    /// </remarks>
    public FakeGcsServerBuilder(IImage image)
        : this(new FakeGcsServerConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeGcsServerBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private FakeGcsServerBuilder(FakeGcsServerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override FakeGcsServerConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override FakeGcsServerContainer Build()
    {
        Validate();
        return new FakeGcsServerContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override FakeGcsServerBuilder Init()
    {
        return base.Init()
            .WithPortBinding(FakeGcsServerPort, true)
            .WithEntrypoint("/bin/sh", "-c")
            .WithCommand("while [ ! -f " + StartupScriptFilePath + " ]; do sleep 0.1; done; " + StartupScriptFilePath)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("server started"))
            .WithStartupCallback((container, ct) =>
            {
                const char lf = '\n';
                var startupScript = new StringBuilder();
                startupScript.Append("#!/bin/sh");
                startupScript.Append(lf);
                startupScript.Append("fake-gcs-server ");
                startupScript.Append("-backend memory ");
                startupScript.Append("-scheme http ");
                // If we do not remove the trailing slash, uploading an object will result in an
                // error: HttpStatusCode.NotFound. The HTTP request appears incorrect. The
                // container logs indicate the presence of an extra slash: `PUT //upload/storage/v1`.
                startupScript.Append("-external-url " + new UriBuilder(Uri.UriSchemeHttp, container.Hostname, container.GetMappedPublicPort(FakeGcsServerPort)).ToString().Trim('/'));
                return container.CopyAsync(Encoding.Default.GetBytes(startupScript.ToString()), StartupScriptFilePath, fileMode: Unix.FileMode755, ct: ct);
            });
    }

    /// <inheritdoc />
    protected override FakeGcsServerBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new FakeGcsServerConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override FakeGcsServerBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new FakeGcsServerConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override FakeGcsServerBuilder Merge(FakeGcsServerConfiguration oldValue, FakeGcsServerConfiguration newValue)
    {
        return new FakeGcsServerBuilder(new FakeGcsServerConfiguration(oldValue, newValue));
    }
}