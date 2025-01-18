namespace Testcontainers.EventHubs.Configuration;

public record Entity
{
    public string Name { get; set; }
    public int PartitionCount { get; set; }
    public List<ConsumerGroup> ConsumerGroups { get; set; } = [];
}