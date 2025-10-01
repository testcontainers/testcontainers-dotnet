using System.IO;
using System.Net.Http;

namespace Testcontainers.Mockaco;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
public sealed class MockacoBuilder : ContainerBuilder<MockacoBuilder, MockacoContainer, MockacoConfiguration>
{
    /// <summary>
    /// The default Docker image used for Mockaco.
    /// </summary>
    public const string MockacoImage = "natenho/mockaco:1.9.14";

    /// <summary>
    /// The default port exposed by the Mockaco container.
    /// </summary>
    public const ushort MockacoPort = 5000;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockacoBuilder" /> class
    /// with the default <see cref="MockacoConfiguration" />.
    /// </summary>
    public MockacoBuilder() : this(new MockacoConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MockacoBuilder" /> class
    /// with the provided <see cref="MockacoConfiguration" />.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private MockacoBuilder(MockacoConfiguration resourceConfiguration) : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <summary>
    /// Sets the path to the templates directory for the Mockaco container.
    /// </summary>
    /// <param name="templatesPath">The absolute path to the templates directory.</param>
    /// <returns>The updated <see cref="MockacoBuilder" /> instance.</returns>
    public MockacoBuilder WithTemplatesPath(string templatesPath)
    {
        return Merge(DockerResourceConfiguration, new MockacoConfiguration(templatesPath: templatesPath))
            .WithBindMount(Path.GetFullPath(templatesPath), "/app/Mocks", AccessMode.ReadWrite);
    }


    /// <inheritdoc />
    public override MockacoContainer Build()
    {
        Validate();
        return new MockacoContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override MockacoConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    protected override MockacoBuilder Init()
    {
        return base.Init()
            .WithImage(MockacoImage)
            .WithPortBinding(MockacoPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(request => request
                    .WithMethod(HttpMethod.Get)
                    .ForPort(MockacoPort)
                    .ForPath("_mockaco/ready")
                    .WithContent(() => new StringContent("Healthy"))));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.TemplatesPath,
                nameof(DockerResourceConfiguration.TemplatesPath))
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc />
    protected override MockacoBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MockacoConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MockacoBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MockacoConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MockacoBuilder Merge(MockacoConfiguration oldValue, MockacoConfiguration newValue)
    {
        return new MockacoBuilder(new MockacoConfiguration(oldValue, newValue));
    }
}