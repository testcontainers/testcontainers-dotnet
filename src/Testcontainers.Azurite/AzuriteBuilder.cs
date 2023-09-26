namespace Testcontainers.Azurite;

using System.IO;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class AzuriteBuilder : ContainerBuilder<AzuriteBuilder, AzuriteContainer, AzuriteConfiguration>
{
    public const string AzuriteImage = "mcr.microsoft.com/azure-storage/azurite:3.24.0";

    public const ushort BlobPort = 10000;

    public const ushort QueuePort = 10001;

    public const ushort TablePort = 10002;

    private readonly ISet<AzuriteService> _enabledServices = new HashSet<AzuriteService>();

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteBuilder" /> class.
    /// </summary>
    public AzuriteBuilder()
        : this(new AzuriteConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;

        _enabledServices.Add(AzuriteService.Blob);
        _enabledServices.Add(AzuriteService.Queue);
        _enabledServices.Add(AzuriteService.Table);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private AzuriteBuilder(AzuriteConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override AzuriteConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override AzuriteContainer Build()
    {
        Validate();

        var waitStrategy = Wait.ForUnixContainer();

        if (_enabledServices.Contains(AzuriteService.Blob))
        {
            waitStrategy = waitStrategy.UntilMessageIsLogged("Blob service is successfully listening");
        }

        if (_enabledServices.Contains(AzuriteService.Queue))
        {
            waitStrategy = waitStrategy.UntilMessageIsLogged("Queue service is successfully listening");
        }

        if (_enabledServices.Contains(AzuriteService.Table))
        {
            waitStrategy = waitStrategy.UntilMessageIsLogged("Table service is successfully listening");
        }

        var azuriteBuilder = DockerResourceConfiguration.WaitStrategies.Count() > 1 ? this : WithWaitStrategy(waitStrategy);
        return new AzuriteContainer(azuriteBuilder.DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    public AzuriteBuilder WithAccountCredentials(string accountName, string accountKey)
    {
        return Merge(DockerResourceConfiguration, new AzuriteConfiguration(accountName: accountName, accountKey: accountKey))
            .WithEnvironment("AZURITE_ACCOUNTS", $"{accountName}:{accountKey}");
    }

    public AzuriteBuilder WithPemCertificate(string certificatePath, string certificateKeyPath)
    {
        return Merge(DockerResourceConfiguration, new AzuriteConfiguration(certificatePath: certificatePath, certificateKeyPath: certificateKeyPath))
            .WithBindMount(Path.GetDirectoryName(GetType().Assembly.Location), "/internal")
            .WithCommand("--oauth", "basic")
            .WithCommand("--cert", CleanUpPaths(certificatePath), "--key", CleanUpPaths(certificateKeyPath));
    }

    /// <inheritdoc />
    protected override AzuriteBuilder Init()
    {
        return base.Init()
            .WithImage(AzuriteImage)
            .WithPortBinding(BlobPort, true)
            .WithPortBinding(QueuePort, true)
            .WithPortBinding(TablePort, true)
            // Default command, if not specified the command will be completely overwritten in case of a certificate.
            .WithCommand("azurite", "--blobHost", "0.0.0.0", "--queueHost", "0.0.0.0", "--tableHost", "0.0.0.0");
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

    private static string CleanUpPaths(string value)
    {
        return value.StartsWith("/internal/", StringComparison.OrdinalIgnoreCase) ? value : $"/internal/{value}";
    }
}