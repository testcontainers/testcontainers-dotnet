using DotNet.Testcontainers.Images;

namespace Testcontainers.Kafka.Vendors;

internal interface IKafkaVendor
{
    KafkaImageVendor ImageVendor { get; }

    /// <summary>
    /// Default consensus protocol to be used if this vendor if end user has not specified other consensus protocol
    /// </summary>
    KafkaConsensusProtocol DefaultConsensusProtocol { get; }

    bool ImageBelongsToVendor(IImage image);
    void ValidateConfigurationAndThrow(KafkaConfiguration configuration);
    string GetStartupScript(StartupScriptContext context);
}