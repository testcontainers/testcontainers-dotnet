namespace Testcontainers.Bigtable;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public class BigtableEmulatorBuilder: ContainerBuilder<BigtableEmulatorBuilder, BigtableEmulatorContainer, BigtableEmulatorConfiguration>
{
  public const string GCloudCliDockerImage = "gcr.io/google.com/cloudsdktool/google-cloud-cli";

  public const ushort BigtablePort = 9000;

  public const string CMD = "gcloud beta emulators bigtable start --host-port 0.0.0.0:9000";

  /// <summary>
  /// Initializes a new instance of the <see cref="BigtableEmulatorBuilder" /> class.
  /// </summary>
  public BigtableEmulatorBuilder() : this(new BigtableEmulatorConfiguration())
  {
    DockerResourceConfiguration = Init().DockerResourceConfiguration;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="BigtableEmulatorBuilder" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  private BigtableEmulatorBuilder(BigtableEmulatorConfiguration resourceConfiguration) : base(resourceConfiguration)
  {
    DockerResourceConfiguration = resourceConfiguration;
  }

  protected override BigtableEmulatorConfiguration DockerResourceConfiguration { get; }

  public override BigtableEmulatorContainer Build()
  {
    Validate();
    return new BigtableEmulatorContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
  }

  protected override BigtableEmulatorBuilder Init()
  {
    return base.Init()
      .WithImage(GCloudCliDockerImage)
      .WithPortBinding(BigtablePort, true)
      .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("(?s).*running.*$").UntilPortIsAvailable(BigtablePort))
      .WithCommand("/bin/sh", "-c", CMD);
  }

  protected override BigtableEmulatorBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
  {
    return Merge(DockerResourceConfiguration, new BigtableEmulatorConfiguration(resourceConfiguration));
  }

  protected override BigtableEmulatorBuilder Clone(IContainerConfiguration resourceConfiguration)
  {
    return Merge(DockerResourceConfiguration, new BigtableEmulatorConfiguration(resourceConfiguration));
  }

  protected override BigtableEmulatorBuilder Merge(BigtableEmulatorConfiguration oldValue, BigtableEmulatorConfiguration newValue)
  {
    return new BigtableEmulatorBuilder(new BigtableEmulatorConfiguration(oldValue, newValue));
  }
}
