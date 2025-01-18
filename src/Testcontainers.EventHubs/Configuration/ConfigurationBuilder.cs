namespace Testcontainers.EventHubs.Configuration;

public class ConfigurationBuilder
{
    private const string DefaultNamespace = "emulatorNs1";
    private const string DefaultLoggingType = "file";
        
    private readonly RootConfiguration _rootConfiguration = new RootConfiguration();
            
    private ConfigurationBuilder()
    {
        _rootConfiguration.UserConfig = new UserConfig
        {
            NamespaceConfig =
            [
                new NamespaceConfig
                {
                    Type = "EventHub",
                    Name = DefaultNamespace
                },
            ],
            LoggingConfig = new LoggingConfig() { Type = DefaultLoggingType },
        };
    }

    public static ConfigurationBuilder Create()
    {
        return new ConfigurationBuilder();
    }
        
    public ConfigurationBuilder WithEventHub(string entityName, int partitionCount, IEnumerable<string> consumerGroups)
    {
        var namespaceConfig = _rootConfiguration.UserConfig.NamespaceConfig.FirstOrDefault(x => x.Name == DefaultNamespace);
        if (namespaceConfig == null)
        {
            throw new InvalidOperationException($"Default Namespace '{DefaultNamespace}' not found.");
        }

        namespaceConfig.Entities.Add(new Entity
        {
            Name = entityName,
            PartitionCount = partitionCount,
            ConsumerGroups = consumerGroups.Select(consumerGroupName => new ConsumerGroup { Name = consumerGroupName }).ToList(),
        });

        return this;
    }

    public bool Validate()
    {
        return _rootConfiguration.UserConfig.NamespaceConfig.Count == 1 &&
               _rootConfiguration.UserConfig.NamespaceConfig.First().Entities.Count > 0 &&
               _rootConfiguration.UserConfig.NamespaceConfig.First().Entities.Count <= 10 &&
               _rootConfiguration.UserConfig.NamespaceConfig.First().Entities.All(entity =>
                   entity.PartitionCount is > 0 and <= 32 &&
                   entity.ConsumerGroups.Count is > 0 and <= 20);
    }

    public string Build()
    {
        return JsonSerializer.Serialize(_rootConfiguration);
    }
}