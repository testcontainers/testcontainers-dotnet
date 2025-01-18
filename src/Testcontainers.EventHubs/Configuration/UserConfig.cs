namespace Testcontainers.EventHubs.Configuration;

public record UserConfig
{
    public List<NamespaceConfig> NamespaceConfig { get; set; } = [];
    public LoggingConfig LoggingConfig { get; set; }
}