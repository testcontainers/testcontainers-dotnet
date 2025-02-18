namespace Testcontainers.EventHubs;

[PublicAPI]
public record RootConfiguration(UserConfig UserConfig)
{
    public UserConfig UserConfig { get; } = UserConfig;
}

[PublicAPI]
public record UserConfig(IReadOnlyCollection<NamespaceConfig> NamespaceConfig, LoggingConfig LoggingConfig)
{
    public IReadOnlyCollection<NamespaceConfig> NamespaceConfig { get; } = NamespaceConfig;

    public LoggingConfig LoggingConfig { get; } = LoggingConfig;
}

[PublicAPI]
public record NamespaceConfig(string Type, string Name, IReadOnlyCollection<Entity> Entities)
{
    public string Type { get; } = Type;

    public string Name { get; } = Name;

    public IReadOnlyCollection<Entity> Entities { get; } = Entities;
}

[PublicAPI]
public record Entity(string Name, int PartitionCount, IReadOnlyCollection<ConsumerGroup> ConsumerGroups)
{
    public string Name { get; } = Name;

    public int PartitionCount { get; } = PartitionCount;

    public IReadOnlyCollection<ConsumerGroup> ConsumerGroups { get; } = ConsumerGroups;
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

    public EventHubsServiceConfiguration WithEntity(string name, int partitionCount, params string[] consumerGroupNames)
    {
        return WithEntity(name, partitionCount, new ReadOnlyCollection<string>(consumerGroupNames));
    }

    public EventHubsServiceConfiguration WithEntity(string name, int partitionCount, IEnumerable<string> consumerGroupNames)
    {
        var consumerGroups = new ReadOnlyCollection<ConsumerGroup>(consumerGroupNames.Select(consumerGroupName => new ConsumerGroup(consumerGroupName)).ToList());
        var entity = new Entity(name, partitionCount, consumerGroups);
        var entities = new ReadOnlyCollection<Entity>(_namespaceConfig.Entities.Append(entity).ToList());
        return new EventHubsServiceConfiguration(new NamespaceConfig(_namespaceConfig.Type, _namespaceConfig.Name, entities));
    }

    public bool Validate()
    {
        Predicate<Entity> isValidEntity = entity => entity.PartitionCount > 0 && entity.PartitionCount <= 32 && entity.ConsumerGroups.Count > 0 && entity.ConsumerGroups.Count <= 20;
        return _namespaceConfig.Entities.All(entity => isValidEntity(entity));
    }

    public string Build()
    {
        var rootConfiguration = new RootConfiguration(new UserConfig([_namespaceConfig], new LoggingConfig("file")));
        return JsonSerializer.Serialize(rootConfiguration);
    }
}