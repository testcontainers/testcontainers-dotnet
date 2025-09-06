namespace Testcontainers.Kafka;

/// <summary>
/// Represents the consensus protocols.
/// </summary>
public enum ConsensusProtocol
{
    /// <summary>
    /// Represents the KRaft consensus protocol.
    /// </summary>
    KRaft,

    /// <summary>
    /// Represents the ZooKeeper-based consensus protocol.
    /// </summary>
    ZooKeeper,
}
