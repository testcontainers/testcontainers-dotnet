namespace Testcontainers.Bigtable;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public class BigtableBuilder: ContainerBuilder<BigtableBuilder, BigtableContainer, BigtableConfiguration>
{
  public const string GCloudCliDockerImage = "gcr.io/google.com/cloudsdktool/google-cloud-cli";

  public const ushort BigtablePort = 9000;

  public const string CMD = "gcloud beta emulators bigtable start --host-port 0.0.0.0:9000";

  /// <summary>
  /// Initializes a new instance of the <see cref="BigtableBuilder" /> class.
  /// </summary>
  public BigtableBuilder() : this(new BigtableConfiguration())
  {
    DockerResourceConfiguration = Init().DockerResourceConfiguration;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="BigtableBuilder" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  private BigtableBuilder(BigtableConfiguration resourceConfiguration) : base(resourceConfiguration)
  {
    DockerResourceConfiguration = resourceConfiguration;
  }

  protected override BigtableConfiguration DockerResourceConfiguration { get; }

  public override BigtableContainer Build()
  {
    Validate();
    return new BigtableContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
  }

  protected override BigtableBuilder Init()
  {
    return base.Init()
      .WithImage(GCloudCliDockerImage)
      .WithPortBinding(BigtablePort, true)
      .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("(?s).*running.*$").UntilPortIsAvailable(BigtablePort))
      .WithCommand("/bin/sh", "-c", CMD);
  }

  protected override BigtableBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
  {
    return Merge(DockerResourceConfiguration, new BigtableConfiguration(resourceConfiguration));
  }

  protected override BigtableBuilder Clone(IContainerConfiguration resourceConfiguration)
  {
    return Merge(DockerResourceConfiguration, new BigtableConfiguration(resourceConfiguration));
  }

  protected override BigtableBuilder Merge(BigtableConfiguration oldValue, BigtableConfiguration newValue)
  {
    return new BigtableBuilder(new BigtableConfiguration(oldValue, newValue));
  }
}
