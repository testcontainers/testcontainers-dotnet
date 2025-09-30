namespace Testcontainers.Kafka;

/// <summary>
/// Represents the vendor or distribution source of Kafka.
/// </summary>
public enum KafkaVendor
{
    /// <summary>
    /// Apache Software Foundation, open-source, community-maintained Apache
    /// Kafka.
    /// </summary>
    ApacheSoftwareFoundation,

    /// <summary>
    /// Confluent, Inc., commercial Confluent Platform with Apache Kafka and
    /// extra features.
    /// </summary>
    Confluent,
}