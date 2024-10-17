
namespace Testcontainers.Toxiproxy;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ToxiproxyBuilder : ContainerBuilder<ToxiproxyBuilder, ToxiproxyContainer, ToxiproxyConfiguration>
{
    public const string ToxiproxyImage = "ghcr.io/shopify/toxiproxy";
    public const ushort ControlPort = 8474;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToxiproxyBuilder" /> class.
    /// </summary>
    public ToxiproxyBuilder()
        : this(new ToxiproxyConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToxiproxyBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private ToxiproxyBuilder(ToxiproxyConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override ToxiproxyConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override ToxiproxyContainer Build()
    {
        Validate();
        return new ToxiproxyContainer(DockerResourceConfiguration);
    }

    /// <summary>
    /// Initialize the default Toxiproxy configuration with image, port, and wait strategy.
    /// </summary>
    /// <returns>A configured instance of <see cref="ToxiproxyBuilder" />.</returns>
    protected override ToxiproxyBuilder Init()
    {
        // Define a wait strategy that waits for the Toxiproxy CLI command `list` to complete successfully.


        return base.Init()
                .WithImage(ToxiproxyImage) // Set the Toxiproxy image.
                .WithPortBinding(ControlPort, true) // Bind the control port.
            ; // Use the defined wait strategy.
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        // Validate that the DockerResourceConfiguration is properly set.
        _ = Guard.Argument(DockerResourceConfiguration, nameof(DockerResourceConfiguration)).NotNull();
    }

    /// <inheritdoc />
    protected override ToxiproxyBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ToxiproxyConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ToxiproxyBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ToxiproxyConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ToxiproxyBuilder Merge(ToxiproxyConfiguration oldValue, ToxiproxyConfiguration newValue)
    {
        // Merge the old and new configurations into an immutable copy.
        var mergedConfiguration = new ToxiproxyConfiguration(oldValue, newValue);
        return new ToxiproxyBuilder(mergedConfiguration);
    }
}
