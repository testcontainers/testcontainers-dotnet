namespace Testcontainers.ZooKeeper;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ZooKeeperBuilder : ContainerBuilder<ZooKeeperBuilder, ZooKeeperContainer, ZooKeeperConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string ZooKeeperImage = "zookeeper:3.9.5";

    public const ushort ZooKeeperPort = 2181;

    public const ushort ZooKeeperAdminServerPort = 8080;

    /// <summary>
    /// Initializes a new instance of the <see cref="ZooKeeperBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    [ExcludeFromCodeCoverage]
    public ZooKeeperBuilder()
        : this(ZooKeeperImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZooKeeperBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>zookeeper:3.9.5</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/_/zookeeper/tags" />.
    /// </remarks>
    public ZooKeeperBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZooKeeperBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/_/zookeeper/tags" />.
    /// </remarks>
    public ZooKeeperBuilder(IImage image)
        : this(new ZooKeeperConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZooKeeperBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private ZooKeeperBuilder(ZooKeeperConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override ZooKeeperConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override ZooKeeperContainer Build()
    {
        Validate();
        return new ZooKeeperContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override ZooKeeperBuilder Init()
    {
        return base.Init()
            .WithPortBinding(ZooKeeperPort, true)
            .WithPortBinding(ZooKeeperAdminServerPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/commands/ruok").ForPort(ZooKeeperAdminServerPort).ForResponseMessageMatching(IsNodeReadyAsync)));
    }

    /// <inheritdoc />
    protected override ZooKeeperBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ZooKeeperConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ZooKeeperBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ZooKeeperConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ZooKeeperBuilder Merge(ZooKeeperConfiguration oldValue, ZooKeeperConfiguration newValue)
    {
        return new ZooKeeperBuilder(new ZooKeeperConfiguration(oldValue, newValue));
    }

    /// <summary>
    /// Determines whether the ZooKeeper node is ready by checking the AdminServer's
    /// <c>ruok</c> command response.
    /// </summary>
    /// <param name="response">The HTTP response message of the <c>ruok</c> command.</param>
    /// <returns>A task that completes with <c>true</c> when the node is ready; otherwise, <c>false</c>.</returns>
    private static async Task<bool> IsNodeReadyAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync()
            .ConfigureAwait(false);

        return content.Contains("\"error\" : null");
    }
}
