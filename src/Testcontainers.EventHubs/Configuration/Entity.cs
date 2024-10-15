namespace Testcontainers.EventHubs.Configuration
{
    public record Entity
    {
        public string Name { get; set; }
        public string PartitionCount { get; set; }
        public List<ConsumerGroup> ConsumerGroups { get; set; } = [];
    }
}