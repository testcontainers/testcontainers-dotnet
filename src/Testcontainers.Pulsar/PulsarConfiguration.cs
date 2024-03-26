namespace Testcontainers.Pulsar;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class PulsarConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PulsarConfiguration" /> class.
    /// </summary>
    /// <param name="authenticationEnabled"></param>
    /// <param name="functionsWorkerEnabled"></param>
    public PulsarConfiguration(
        bool? authenticationEnabled = null,
        bool? functionsWorkerEnabled = null)
    {
        AuthenticationEnabled = authenticationEnabled;
        FunctionsWorkerEnabled = functionsWorkerEnabled;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PulsarConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public PulsarConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PulsarConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public PulsarConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PulsarConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public PulsarConfiguration(PulsarConfiguration resourceConfiguration)
        : this(new PulsarConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PulsarConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public PulsarConfiguration(PulsarConfiguration oldValue, PulsarConfiguration newValue)
        : base(oldValue, newValue)
    {
        AuthenticationEnabled = (oldValue.AuthenticationEnabled.HasValue && oldValue.AuthenticationEnabled.Value) || (newValue.AuthenticationEnabled.HasValue && newValue.AuthenticationEnabled.Value);
        FunctionsWorkerEnabled = (oldValue.FunctionsWorkerEnabled.HasValue && oldValue.FunctionsWorkerEnabled.Value) || (newValue.FunctionsWorkerEnabled.HasValue && newValue.FunctionsWorkerEnabled.Value);
    }

    /// <summary>
    /// Gets or sets the flag indicating whether authentication is enabled for the Pulsar container.
    /// </summary>
    /// <remarks>
    /// By default, authentication is disabled.
    /// Setting this property to true enables authentication for the Pulsar container.
    /// </remarks>
    public bool? AuthenticationEnabled { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the functions worker is enabled.
    /// </summary>
    /// <remarks>
    /// The functions worker is responsible for executing functions within Pulsar.
    /// By default, the functions worker is not enabled.
    /// </remarks>
    public bool? FunctionsWorkerEnabled { get; }
}