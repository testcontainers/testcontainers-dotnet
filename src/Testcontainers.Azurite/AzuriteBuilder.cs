namespace Testcontainers.Azurite
{
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
      this.DockerResourceConfiguration = this.Init().DockerResourceConfiguration;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="AzuriteBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private AzuriteBuilder(AzuriteConfiguration resourceConfiguration)
      : base(resourceConfiguration)
    {
      this.DockerResourceConfiguration = resourceConfiguration;
    }

    internal AzuriteConfiguration Configuration
    {
      get
      {
        return new AzuriteConfiguration(this.DockerResourceConfiguration);
      }
    }

    /// <inheritdoc />
    protected override AzuriteConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override AzuriteContainer Build()
    {
      var commandArgs = new List<string>();
      commandArgs.AddRange(this.GetExposedServices());
      commandArgs.AddRange(this.GetWorkspaceLocationDirectoryPath());
      commandArgs.AddRange(this.GetDebugModeEnabled());
      commandArgs.Add(this.GetSilentModeEnabled());
      commandArgs.Add(this.GetLooseModeEnabled());
      commandArgs.Add(this.GetSkipApiVersionCheckEnabled());
      commandArgs.Add(this.GetProductStyleUrlDisabled());
      commandArgs.AddRange(this.GetHttps());

      var builder = this
        .WithEntrypoint(this.GetExecutable())
        .WithCommand(commandArgs.Where(x => !string.IsNullOrEmpty(x)).ToArray());

      builder.Validate();

      return new AzuriteContainer(builder.DockerResourceConfiguration, TestcontainersSettings.Logger);
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
      return this.Merge(this.DockerResourceConfiguration, new AzuriteConfiguration(debugModeEnabled));
    }

    /// <summary>
    ///   Sets a value indicating whether Azurite silent mode is enabled or not.
    /// </summary>
    /// <remarks>
    ///   Default value is false.
    /// </remarks>
    public AzuriteBuilder WithSilentModeEnabled(bool silentModeEnabled)
    {
      return this.Merge(this.DockerResourceConfiguration, new AzuriteConfiguration(silentModeEnabled: silentModeEnabled));
    }

    /// <summary>
    ///   Sets a value indicating whether Azurite loose mode is enabled or not.
    /// </summary>
    /// <remarks>
    ///   Default value is false.
    /// </remarks>
    public AzuriteBuilder WithLooseModeEnabled(bool looseModeEnabled)
    {
      return this.Merge(this.DockerResourceConfiguration, new AzuriteConfiguration(looseModeEnabled: looseModeEnabled));
    }

    /// <summary>
    ///   Sets a value indicating whether Azurite skip API version check is enabled or not.
    /// </summary>
    /// <remarks>
    ///   Default value is false.
    /// </remarks>
    public AzuriteBuilder WithSkipApiVersionCheckEnabled(bool skipApiVersionCheckEnabled)
    {
      return this.Merge(this.DockerResourceConfiguration, new AzuriteConfiguration(skipApiVersionCheckEnabled: skipApiVersionCheckEnabled));
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
      return this.Merge(this.DockerResourceConfiguration, new AzuriteConfiguration(productStyleUrlDisabled: productStyleUrlDisabled));
    }

    /// <summary>
    ///   Sets a pfx file path and its password to enable https endpoints.
    /// </summary>
    public AzuriteBuilder WithHttpsDefinedByPfxFile(string pfxFilePath, string pfxFilePassword)
    {
      return this.Merge(this.DockerResourceConfiguration, new AzuriteConfiguration(certificate: pfxFilePath, password: pfxFilePassword));
    }

    /// <summary>
    ///   Sets a certificate and key pem file paths to enable https endpoints.
    /// </summary>
    public AzuriteBuilder WithHttpsDefinedByPemFiles(string certificateFilePath, string keyFilePath)
    {
      return this.Merge(this.DockerResourceConfiguration,
        new AzuriteConfiguration(certificate: certificateFilePath, key: keyFilePath));
    }

    /// <inheritdoc />
    protected override AzuriteBuilder Init()
    {
      return base
        .Init()
        .WithImage(DefaultImage)
        .WithPortBinding(DefaultBlobPort, true)
        .WithPortBinding(DefaultQueuePort, true)
        .WithPortBinding(DefaultTablePort, true)
        .WithWaitStrategy(Wait
          .ForUnixContainer()
          .UntilPortIsAvailable(DefaultBlobPort)
          .UntilPortIsAvailable(DefaultQueuePort)
          .UntilPortIsAvailable(DefaultTablePort));
    }

    /// <inheritdoc />
    protected override AzuriteBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
      return this.Merge(this.DockerResourceConfiguration, new AzuriteConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override AzuriteBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
      return this.Merge(this.DockerResourceConfiguration, new AzuriteConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override AzuriteBuilder Merge(AzuriteConfiguration oldValue, AzuriteConfiguration newValue)
    {
      return new AzuriteBuilder(new AzuriteConfiguration(oldValue, newValue));
    }

    private string GetExecutable()
    {
      return "azurite";
    }

    private string[] GetExposedServices()
    {
      const string defaultRemoteEndpoint = "0.0.0.0";

      var args = new List<string>
      {
        "--blobHost",
        defaultRemoteEndpoint,
        "--queueHost",
        defaultRemoteEndpoint,
        "--tableHost",
        defaultRemoteEndpoint,
      };

      return args.ToArray();
    }

    private string[] GetWorkspaceLocationDirectoryPath()
    {
      return new[] { "--location", DefaultWorkspaceDirectoryPath };
    }

    private string[] GetDebugModeEnabled()
    {
      var debugLogFilePath = Path.Combine(DefaultWorkspaceDirectoryPath, "debug.log");
      return this.DockerResourceConfiguration.DebugModeEnabled ?? false ? new[] { "--debug", debugLogFilePath } : Array.Empty<string>();
    }

    private string GetSilentModeEnabled()
    {
      return this.DockerResourceConfiguration.SilentModeEnabled ?? false ? "--silent" : null;
    }

    private string GetLooseModeEnabled()
    {
      return this.DockerResourceConfiguration.LooseModeEnabled ?? false ? "--loose" : null;
    }

    private string GetSkipApiVersionCheckEnabled()
    {
      return this.DockerResourceConfiguration.SkipApiVersionCheckEnabled ?? false ? "--skipApiVersionCheck" : null;
    }

    private string GetProductStyleUrlDisabled()
    {
      return this.DockerResourceConfiguration.ProductStyleUrlDisabled ?? false ? "--disableProductStyleUrl" : null;
    }

    private string[] GetHttps()
    {
      if (string.IsNullOrEmpty(this.DockerResourceConfiguration.Certificate))
      {
        return Array.Empty<string>();
      }

      var args = new List<string> { "--cert", this.DockerResourceConfiguration.Certificate };

      if (!string.IsNullOrEmpty(this.DockerResourceConfiguration.Key))
      {
        args.Add("--key");
        args.Add(this.DockerResourceConfiguration.Key);
      }

      if (!string.IsNullOrEmpty(this.DockerResourceConfiguration.Password))
      {
        args.Add("--pwd");
        args.Add(this.DockerResourceConfiguration.Password);
      }

      return args.ToArray();
    }
  }
}
