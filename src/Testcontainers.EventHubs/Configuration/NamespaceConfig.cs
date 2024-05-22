namespace Testcontainers.EventHubs.Configuration
{
    public record NamespaceConfig
    {
        public string Type { get; set; }
        public string Name { get; set; }
        
        public List<Entity> Entities { get; set; } = [];
    }
}