
namespace Testcontainers.EventHubs;

[PublicAPI]
public record RootConfiguration(UserConfig UserConfig)
{
    public UserConfig UserConfig { get; } = UserConfig;
}

[PublicAPI]
public record UserConfig(
    IReadOnlyList<NamespaceConfig> NamespaceConfig,
    LoggingConfig LoggingConfig)
{
    public IReadOnlyList<NamespaceConfig> NamespaceConfig { get; } = NamespaceConfig;
    public LoggingConfig LoggingConfig { get; } = LoggingConfig;
}

[PublicAPI]
public record NamespaceConfig(
    string Type,
    string Name,
    IReadOnlyList<Entity> Entities)
{
    public string Type { get; } = Type;
    public string Name { get; } = Name;
    public IReadOnlyList<Entity> Entities { get; } = Entities;
}

[PublicAPI]
public record Entity(
    string Name,
    int PartitionCount,
    IReadOnlyList<ConsumerGroup> ConsumerGroups)
{
    public string Name { get; } = Name;
    public int PartitionCount { get; } = PartitionCount;
    public IReadOnlyList<ConsumerGroup> ConsumerGroups { get; } = ConsumerGroups;
}

[PublicAPI]
public record ConsumerGroup(string Name)
{
    public string Name { get; } = Name;
}

[PublicAPI]
public record LoggingConfig(string Type)
{
    public string Type { get; } = Type;
}

[PublicAPI]
public sealed class EventHubsServiceConfiguration
{
    private readonly NamespaceConfig _namespaceConfig;

    private EventHubsServiceConfiguration(NamespaceConfig namespaceConfig)
    {
        _namespaceConfig = namespaceConfig;
    }

    public static EventHubsServiceConfiguration Create()
    {
        var namespaceConfig = new NamespaceConfig("EventHub", "ns-1", Array.Empty<Entity>());
        return new EventHubsServiceConfiguration(namespaceConfig);
    }

    public EventHubsServiceConfiguration WithEntity(string name,
        int partitionCount,
        params string[] consumerGroups)
    {
        return WithEntity(name, partitionCount, consumerGroups.ToImmutableList());
    }

    public EventHubsServiceConfiguration WithEntity(string name,
        int partitionCount,
        IEnumerable<string> consumerGroups)
    {
        var entity = new Entity(name, partitionCount,
            consumerGroups.Select(consumerGroup => new ConsumerGroup(consumerGroup)).ToImmutableList());
        var entities = _namespaceConfig.Entities.Append(entity).ToImmutableList();
        return new EventHubsServiceConfiguration(new NamespaceConfig(_namespaceConfig.Type, _namespaceConfig.Name,
            entities));
    }

    public bool Validate()
    {
        return _namespaceConfig.Entities.All(entity =>
            entity.PartitionCount is > 0 and <= 32 && entity.ConsumerGroups.Count is > 0 and <= 20);
    }

    public string Build()
    {
        var rootConfiguration =
            new RootConfiguration(new UserConfig([_namespaceConfig], new LoggingConfig("file")));
        return JsonSerializer.Serialize(rootConfiguration);
    }
}