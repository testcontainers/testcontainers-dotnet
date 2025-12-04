namespace Testcontainers.Toxiproxy;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ToxiproxyBuilder : ContainerBuilder<ToxiproxyBuilder, ToxiproxyContainer, ToxiproxyConfiguration>
{
	public const string ToxiproxyImage = "ghcr.io/shopify/toxiproxy:2.12.0";

	public const ushort ToxiproxyControlPort = 8474;

	public const ushort FirstProxiedPort = 8666;

	public const ushort LastProxiedPort = 8666 + 32;

	/// <summary>
	/// Initializes a new instance of the <see cref="ToxiproxyBuilder" /> class.
	/// </summary>
	[Obsolete("Use the constructor with the image argument instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
	public ToxiproxyBuilder()
		: this(ToxiproxyImage)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ToxiproxyBuilder" /> class.
	/// </summary>
	/// <param name="image">Docker image tag. Available tags can be found here: <see href="https://github.com/Shopify/toxiproxy/pkgs/container/toxiproxy">https://github.com/Shopify/toxiproxy/pkgs/container/toxiproxy</see>.</param>
	public ToxiproxyBuilder(string image)
		: this(new ToxiproxyConfiguration())
	{
		DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ToxiproxyBuilder" /> class.
	/// </summary>
	/// <param name="image">Image instance to use in configuration.</param>
	public ToxiproxyBuilder(IImage image)
		: this(new ToxiproxyConfiguration())
	{
		DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
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

	/// <inheritdoc />
	protected override ToxiproxyBuilder Init()
	{
		const int count = LastProxiedPort - FirstProxiedPort;

		var toxiproxyBuilder = base.Init()
			.WithImage(ToxiproxyImage)
			.WithPortBinding(ToxiproxyControlPort, true)
			.WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
				request.ForPath("/version").ForPort(ToxiproxyControlPort)));

		// Allows up to 32 ports to be proxied (arbitrary value). The ports are
		// exposed here, but whether Toxiproxy listens on them is controlled at
		// runtime when configuring the proxy.
		return Enumerable.Range(FirstProxiedPort, count)
			.Aggregate(toxiproxyBuilder, (builder, port) =>
				builder.WithPortBinding(port, true));
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
		return new ToxiproxyBuilder(new ToxiproxyConfiguration(oldValue, newValue));
	}
}