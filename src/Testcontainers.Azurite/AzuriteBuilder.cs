namespace Testcontainers.Azurite;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class AzuriteBuilder : ContainerBuilder<AzuriteBuilder, AzuriteContainer, AzuriteConfiguration>
{
  /// <summary>
  ///   Default Azurite docker image.
  /// </summary>
  public const string DefaultImage = "mcr.microsoft.com/azure-storage/azurite:3.18.0";

  /// <summary>
  ///   Default Azurite Blob service port.
  /// </summary>
  public const ushort DefaultBlobPort = 10000;

  /// <summary>
  ///   Default Azurite Queue service port.
  /// </summary>
  public const ushort DefaultQueuePort = 10001;

  /// <summary>
  ///   Default Azurite Table service port.
  /// </summary>
  public const ushort DefaultTablePort = 10002;

  /// <summary>
  ///   Default Azurite workspace directory path '/data/'.
  /// </summary>
  public const string DefaultWorkspaceDirectoryPath = "/data/";

  /// <summary>
  ///   Initializes a new instance of the <see cref="AzuriteBuilder" /> class.
  /// </summary>
  public AzuriteBuilder()
    : this(new AzuriteConfiguration())
  {
    DockerResourceConfiguration = Init().DockerResourceConfiguration;
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="AzuriteBuilder" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  private AzuriteBuilder(AzuriteConfiguration resourceConfiguration)
    : base(resourceConfiguration)
  {
    DockerResourceConfiguration = resourceConfiguration;
  }

  internal AzuriteConfiguration Configuration => new(DockerResourceConfiguration);

  /// <inheritdoc />
  protected override AzuriteConfiguration DockerResourceConfiguration { get; }

  private bool AllServicesEnabled => (DockerResourceConfiguration.AzuriteServices ?? AzuriteServices.All) == AzuriteServices.All;

  private bool BlobServiceOnlyEnabled => DockerResourceConfiguration.AzuriteServices is AzuriteServices.Blob;

  private bool QueueServiceOnlyEnabled => DockerResourceConfiguration.AzuriteServices is AzuriteServices.Queue;

  private bool TableServiceOnlyEnabled => DockerResourceConfiguration.AzuriteServices is AzuriteServices.Table;

  /// <inheritdoc />
  public override AzuriteContainer Build()
  {
    var builder = WithEntrypoint(GetExecutable())
      .WithCommand(GetEnabledServices())
      .WithCommand(GetWorkspaceLocationDirectoryPath())
      .WithCommand(GetDebugModeEnabled())
      .WithCommand(GetSilentModeEnabled())
      .WithCommand(GetLooseModeEnabled())
      .WithCommand(GetSkipApiVersionCheckEnabled())
      .WithCommand(GetProductStyleUrlDisabled())
      .AddWorkspaceBinding()
      .AddBlobPortAndBinding()
      .AddQueuePortAndBinding()
      .AddTablePortAndBinding()
      .WithWaitStrategy(UntilAzuriteIsAvailable());

    builder.Validate();

    return new AzuriteContainer(builder.DockerResourceConfiguration, TestcontainersSettings.Logger);
  }

  /// <summary>
  ///   Sets a value indicating which Azurite service is turned on.
  /// </summary>
  /// <remarks>
  ///   Default value is all services.
  /// </remarks>
  public AzuriteBuilder WithServices(AzuriteServices azuriteServices)
  {
    return Merge(DockerResourceConfiguration, new AzuriteConfiguration(azuriteServices));
  }

  /// <summary>
  ///   Sets the Azurite container blob port.
  /// </summary>
  public AzuriteBuilder WithBlobPort(int blobPort)
  {
    return Merge(DockerResourceConfiguration, new AzuriteConfiguration(blobPort: blobPort));
  }

  /// <summary>
  ///   Sets the host random blob port to bind.
  /// </summary>
  /// <remarks>
  ///   Bound to the container blob port.
  /// </remarks>
  public AzuriteBuilder WithBlobPortBinding()
  {
    return WithBlobPortBinding(0);
  }

  /// <summary>
  ///   Sets the host blob port to bind.
  /// </summary>
  /// <remarks>
  ///   Bound to the container blob port.
  /// </remarks>
  public AzuriteBuilder WithBlobPortBinding(int blobPortBinding)
  {
    return Merge(DockerResourceConfiguration, new AzuriteConfiguration(blobPortBinding: blobPortBinding));
  }

  /// <summary>
  ///   Cancel host blob port binding.
  /// </summary>
  public AzuriteBuilder WithoutBlobPortBinding()
  {
    return WithBlobPortBinding(-1);
  }

  /// <summary>
  ///   Sets the Azurite container queue port.
  /// </summary>
  public AzuriteBuilder WithQueuePort(int queuePort)
  {
    return Merge(DockerResourceConfiguration, new AzuriteConfiguration(queuePort: queuePort));
  }

  /// <summary>
  ///   Sets the host random queue port to bind.
  /// </summary>
  /// <remarks>
  ///   Bound to the container queue port.
  /// </remarks>
  public AzuriteBuilder WithQueuePortBinding()
  {
    return WithQueuePortBinding(0);
  }

  /// <summary>
  ///   Sets the host queue port to bind.
  /// </summary>
  /// <remarks>
  ///   Bound to the container queue port.
  /// </remarks>
  public AzuriteBuilder WithQueuePortBinding(int queuePortBinding)
  {
    return Merge(DockerResourceConfiguration, new AzuriteConfiguration(queuePortBinding: queuePortBinding));
  }

  /// <summary>
  ///   Cancel host queue port binding.
  /// </summary>
  public AzuriteBuilder WithoutQueuePortBinding()
  {
    return WithQueuePortBinding(-1);
  }

  /// <summary>
  ///   Sets the Azurite container table port.
  /// </summary>
  public AzuriteBuilder WithTablePort(int tablePort)
  {
    return Merge(DockerResourceConfiguration, new AzuriteConfiguration(tablePort: tablePort));
  }

  /// <summary>
  ///   Sets the host random table port to bind.
  /// </summary>
  /// <remarks>
  ///   Bound to the container table port.
  /// </remarks>
  public AzuriteBuilder WithTablePortBinding()
  {
    return WithTablePortBinding(0);
  }

  /// <summary>
  ///   Sets the host table port to bind.
  /// </summary>
  /// <remarks>
  ///   Bound to the container table port.
  /// </remarks>
  public AzuriteBuilder WithTablePortBinding(int tablePortBinding)
  {
    return Merge(DockerResourceConfiguration, new AzuriteConfiguration(tablePortBinding: tablePortBinding));
  }

  /// <summary>
  ///   Cancel host table port binding.
  /// </summary>
  public AzuriteBuilder WithoutTablePortBinding()
  {
    return WithTablePortBinding(-1);
  }

  /// <summary>
  ///   Sets the Azurite workspace directory path.
  /// </summary>
  /// <remarks>
  ///   Corresponds to the default workspace directory path.
  /// </remarks>
  public AzuriteBuilder WithWorkspaceLocation(string workspaceLocation)
  {
    return Merge(DockerResourceConfiguration, new AzuriteConfiguration(workspaceLocation: workspaceLocation));
  }

  /// <summary>
  ///   Sets the directory path where to bind the Azurite workspace directory.
  /// </summary>
  public AzuriteBuilder WithWorkspaceLocationBinding(string workspaceLocationBinding)
  {
    return Merge(DockerResourceConfiguration, new AzuriteConfiguration(workspaceLocationBinding: workspaceLocationBinding));
  }

  /// <summary>
  ///   Sets a value indicating whether Azurite debug mode is enabled or not.
  /// </summary>
  /// <remarks>
  ///   Writes logs to the workspace directory path.
  ///   Default value is false.
  /// </remarks>
  public AzuriteBuilder WithDebugModeEnabled(bool debugModeEnabled)
  {
    return Merge(DockerResourceConfiguration, new AzuriteConfiguration(debugModeEnabled: debugModeEnabled));
  }

  /// <summary>
  ///   Sets a value indicating whether Azurite silent mode is enabled or not.
  /// </summary>
  /// <remarks>
  ///   Default value is false.
  /// </remarks>
  public AzuriteBuilder WithSilentModeEnabled(bool silentModeEnabled)
  {
    return Merge(DockerResourceConfiguration, new AzuriteConfiguration(silentModeEnabled: silentModeEnabled));
  }

  /// <summary>
  ///   Sets a value indicating whether Azurite loose mode is enabled or not.
  /// </summary>
  /// <remarks>
  ///   Default value is false.
  /// </remarks>
  public AzuriteBuilder WithLooseModeEnabled(bool looseModeEnabled)
  {
    return Merge(DockerResourceConfiguration, new AzuriteConfiguration(looseModeEnabled: looseModeEnabled));
  }

  /// <summary>
  ///   Sets a value indicating whether Azurite skip API version check is enabled or not.
  /// </summary>
  /// <remarks>
  ///   Default value is false.
  /// </remarks>
  public AzuriteBuilder WithSkipApiVersionCheckEnabled(bool skipApiVersionCheckEnabled)
  {
    return Merge(DockerResourceConfiguration, new AzuriteConfiguration(skipApiVersionCheckEnabled: skipApiVersionCheckEnabled));
  }

  /// <summary>
  ///   Sets a value indicating whether Azurite product style URL is enabled or not.
  /// </summary>
  /// <remarks>
  ///   Parses storage account name from the URI path, instead of the URI host.
  ///   Default value is false.
  /// </remarks>
  public AzuriteBuilder WithProductStyleUrlDisabled(bool productStyleUrlDisabled)
  {
    return Merge(DockerResourceConfiguration, new AzuriteConfiguration(productStyleUrlDisabled: productStyleUrlDisabled));
  }

  /// <inheritdoc />
  protected override AzuriteBuilder Init()
  {
    return base.Init()
      .WithImage(DefaultImage)
      .WithServices(AzuriteServices.All)
      .WithBlobPort(DefaultBlobPort)
      .WithQueuePort(DefaultQueuePort)
      .WithTablePort(DefaultTablePort)
      .WithWorkspaceLocation(DefaultWorkspaceDirectoryPath);
  }

  /// <inheritdoc />
  protected override AzuriteBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
  {
    return Merge(DockerResourceConfiguration, new AzuriteConfiguration(resourceConfiguration));
  }

  /// <inheritdoc />
  protected override AzuriteBuilder Clone(IContainerConfiguration resourceConfiguration)
  {
    return Merge(DockerResourceConfiguration, new AzuriteConfiguration(resourceConfiguration));
  }

  /// <inheritdoc />
  protected override AzuriteBuilder Merge(AzuriteConfiguration oldValue, AzuriteConfiguration newValue)
  {
    return new AzuriteBuilder(new AzuriteConfiguration(oldValue, newValue));
  }

  private AzuriteBuilder AddBlobPortAndBinding()
  {
    if (!AllServicesEnabled && !BlobServiceOnlyEnabled)
    {
      return this;
    }

    var exposedPort = DockerResourceConfiguration.BlobPort ?? DefaultBlobPort;
    var bindedPort = DockerResourceConfiguration.BlobPortBinding ?? 0;
    return ExposeAndBindPort(exposedPort, bindedPort);
  }

  private AzuriteBuilder AddQueuePortAndBinding()
  {
    if (!AllServicesEnabled && !QueueServiceOnlyEnabled)
    {
      return this;
    }

    var exposedPort = DockerResourceConfiguration.QueuePort ?? DefaultQueuePort;
    var bindedPort = DockerResourceConfiguration.QueuePortBinding ?? 0;
    return ExposeAndBindPort(exposedPort, bindedPort);
  }

  private AzuriteBuilder AddTablePortAndBinding()
  {
    if (!AllServicesEnabled && !TableServiceOnlyEnabled)
    {
      return this;
    }

    var exposedPort = DockerResourceConfiguration.TablePort ?? DefaultTablePort;
    var bindedPort = DockerResourceConfiguration.TablePortBinding ?? 0;
    return ExposeAndBindPort(exposedPort, bindedPort);
  }

  private AzuriteBuilder AddWorkspaceBinding()
  {
    if (string.IsNullOrEmpty(DockerResourceConfiguration.WorkspaceLocationBinding))
    {
      return this;
    }

    var location = string.IsNullOrEmpty(DockerResourceConfiguration.WorkspaceLocation)
      ? DefaultWorkspaceDirectoryPath
      : DockerResourceConfiguration.WorkspaceLocation;

    return WithBindMount(DockerResourceConfiguration.WorkspaceLocationBinding, location);
  }

  private string GetExecutable()
  {
    if (BlobServiceOnlyEnabled)
    {
      return "azurite-blob";
    }

    if (QueueServiceOnlyEnabled)
    {
      return "azurite-queue";
    }

    if (TableServiceOnlyEnabled)
    {
      return "azurite-table";
    }

    return "azurite";
  }

  private string[] GetEnabledServices()
  {
    const string defaultRemoteEndpoint = "0.0.0.0";

    IList<string> args = new List<string>();

    if (AllServicesEnabled || BlobServiceOnlyEnabled)
    {
      args.Add("--blobHost");
      args.Add(defaultRemoteEndpoint);
      args.Add("--blobPort");
      args.Add((DockerResourceConfiguration.BlobPort ?? DefaultBlobPort).ToString(CultureInfo.InvariantCulture));
    }

    if (AllServicesEnabled || QueueServiceOnlyEnabled)
    {
      args.Add("--queueHost");
      args.Add(defaultRemoteEndpoint);
      args.Add("--queuePort");
      args.Add((DockerResourceConfiguration.QueuePort ?? DefaultQueuePort).ToString(CultureInfo.InvariantCulture));
    }

    if (AllServicesEnabled || TableServiceOnlyEnabled)
    {
      args.Add("--tableHost");
      args.Add(defaultRemoteEndpoint);
      args.Add("--tablePort");
      args.Add((DockerResourceConfiguration.TablePort ?? DefaultTablePort).ToString(CultureInfo.InvariantCulture));
    }

    return args.ToArray();
  }

  private string[] GetWorkspaceLocationDirectoryPath()
  {
    return string.IsNullOrEmpty(DockerResourceConfiguration.WorkspaceLocation)
      ? Array.Empty<string>()
      : new[] {"--location", DockerResourceConfiguration.WorkspaceLocation};
  }

  private string[] GetDebugModeEnabled()
  {
    var location = string.IsNullOrEmpty(DockerResourceConfiguration.WorkspaceLocation)
      ? DefaultWorkspaceDirectoryPath
      : DockerResourceConfiguration.WorkspaceLocation;
    var debugLogFilePath = Path.Combine(location, "debug.log");
    return DockerResourceConfiguration.DebugModeEnabled ?? false ? new[] {"--debug", debugLogFilePath} : Array.Empty<string>();
  }

  private string GetSilentModeEnabled()
  {
    return DockerResourceConfiguration.SilentModeEnabled ?? false ? "--silent" : null;
  }

  private string GetLooseModeEnabled()
  {
    return DockerResourceConfiguration.LooseModeEnabled ?? false ? "--loose" : null;
  }

  private string GetSkipApiVersionCheckEnabled()
  {
    return DockerResourceConfiguration.SkipApiVersionCheckEnabled ?? false ? "--skipApiVersionCheck" : null;
  }

  private string GetProductStyleUrlDisabled()
  {
    return DockerResourceConfiguration.ProductStyleUrlDisabled ?? false ? "--disableProductStyleUrl" : null;
  }

  private IWaitForContainerOS UntilAzuriteIsAvailable()
  {
    var waitStrategy = Wait.ForUnixContainer();
    waitStrategy = AllServicesEnabled || BlobServiceOnlyEnabled
      ? waitStrategy.UntilPortIsAvailable(DockerResourceConfiguration.BlobPort ?? DefaultBlobPort)
      : waitStrategy;
    waitStrategy = AllServicesEnabled || QueueServiceOnlyEnabled
      ? waitStrategy.UntilPortIsAvailable(DockerResourceConfiguration.QueuePort ?? DefaultQueuePort)
      : waitStrategy;
    waitStrategy = AllServicesEnabled || TableServiceOnlyEnabled
      ? waitStrategy.UntilPortIsAvailable(DockerResourceConfiguration.TablePort ?? DefaultTablePort)
      : waitStrategy;
    return waitStrategy;
  }

  private AzuriteBuilder ExposeAndBindPort(int exposedPort, int bindedPort)
  {
    var builder = WithExposedPort(exposedPort);
    return bindedPort switch
    {
      < 0 => builder,
      0 => builder.WithPortBinding(exposedPort, true),
      _ => builder.WithPortBinding(bindedPort, exposedPort),
    };
  }
}