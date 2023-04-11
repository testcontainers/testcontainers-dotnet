namespace Testcontainers.Azurite;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class AzuriteBuilder : ContainerBuilder<AzuriteBuilder, AzuriteContainer, AzuriteConfiguration>
{
    public const string AzuriteImage = "mcr.microsoft.com/azure-storage/azurite:3.23.0";

    public const ushort BlobPort = 10000;

    public const ushort QueuePort = 10001;

    public const ushort TablePort = 10002;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteBuilder" /> class.
    /// </summary>
    public AzuriteBuilder()
        : this(new AzuriteConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
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
        return new AzuriteContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override AzuriteBuilder Init()
    {
        return base.Init()
            .WithImage(AzuriteImage)
            .WithPortBinding(BlobPort, true)
            .WithPortBinding(QueuePort, true)
            .WithPortBinding(TablePort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil()));
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
    
    /// <inheritdoc cref="IWaitUntil" />
    public class WaitUntil : IWaitUntil
    {
        private static readonly IList<string> EnabledServiceLogMessages = new List<string>();

        static WaitUntil()
        {
            EnabledServiceLogMessages.Add("Blob service is successfully listening");
            EnabledServiceLogMessages.Add("Queue service is successfully listening");
            EnabledServiceLogMessages.Add("Table service is successfully listening");
        }

        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            var (stdout, _) = await container.GetLogsAsync(timestampsEnabled: false)
                .ConfigureAwait(false);

            return EnabledServiceLogMessages.All(stdout.Contains);
        }
    }
}