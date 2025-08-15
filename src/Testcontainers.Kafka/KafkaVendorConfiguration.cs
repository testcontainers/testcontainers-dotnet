namespace Testcontainers.Kafka;

/// <summary>
/// Represents the configuration for a Kafka vendor.
/// </summary>
internal abstract class KafkaVendorConfiguration
{
    /// <summary>
    /// Gets the Kafka vendor.
    /// </summary>
    public abstract KafkaVendor Vendor { get; }

    /// <summary>
    /// Gets the consensus protocol.
    /// </summary>
    public abstract ConsensusProtocol ConsensusProtocol { get; }

    /// <summary>
    /// Determines whether the specified Docker image belongs to this vendor.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    /// <returns><c>true</c> if the image belongs to this vendor; otherwise, <c>false</c>.</returns>
    public abstract bool IsImageFromVendor(IImage image);

    /// <summary>
    /// Validates the resource configuration.
    /// </summary>
    /// <param name="resourceConfiguration">The resource configuration.</param>
    public abstract void Validate(KafkaConfiguration resourceConfiguration);

    /// <summary>
    /// Creates the startup script.
    /// </summary>
    /// <param name="resourceConfiguration">The resource configuration.</param>
    /// <param name="container">The Kafka container.</param>
    /// <returns>The startup script as a string.</returns>
    public abstract string CreateStartupScript(KafkaConfiguration resourceConfiguration, KafkaContainer container);
}