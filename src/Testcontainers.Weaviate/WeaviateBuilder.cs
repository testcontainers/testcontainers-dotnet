// Copyright (c) Microsoft. All rights reserved.

namespace Testcontainers.Weaviate;

public sealed class WeaviateBuilder : ContainerBuilder<WeaviateBuilder, WeaviateContainer, WeaviateConfiguration>
{
    public const string WeaviateImage = "semitechnologies/weaviate:1.26.14";

    public const ushort WeaviateHttpPort = 8080;

    public const ushort WeaviateGrpcPort = 50051;

    public WeaviateBuilder() : this(new WeaviateConfiguration())
      => DockerResourceConfiguration = Init().DockerResourceConfiguration;

    private WeaviateBuilder(WeaviateConfiguration dockerResourceConfiguration) : base(dockerResourceConfiguration)
        => DockerResourceConfiguration = dockerResourceConfiguration;

    public override WeaviateContainer Build()
    {
        Validate();
        return new WeaviateContainer(DockerResourceConfiguration);
    }

    protected override WeaviateBuilder Init()
        => base.Init()
            .WithImage(WeaviateImage)
            .WithPortBinding(WeaviateHttpPort, true)
            .WithPortBinding(WeaviateGrpcPort, true)
            .WithEnvironment("AUTHENTICATION_ANONYMOUS_ACCESS_ENABLED", "true")
            .WithEnvironment("PERSISTENCE_DATA_PATH", "/var/lib/weaviate")
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilPortIsAvailable(WeaviateHttpPort)
                .UntilPortIsAvailable(WeaviateGrpcPort)
                .UntilHttpRequestIsSucceeded(r => r.ForPath("/v1/.well-known/ready").ForPort(WeaviateHttpPort)));

    protected override WeaviateBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        => Merge(DockerResourceConfiguration, new WeaviateConfiguration(resourceConfiguration));

    protected override WeaviateBuilder Merge(WeaviateConfiguration oldValue, WeaviateConfiguration newValue)
        => new(new WeaviateConfiguration(oldValue, newValue));

    protected override WeaviateConfiguration DockerResourceConfiguration { get; }

    protected override WeaviateBuilder Clone(IContainerConfiguration resourceConfiguration)
        => Merge(DockerResourceConfiguration, new WeaviateConfiguration(resourceConfiguration));
}
