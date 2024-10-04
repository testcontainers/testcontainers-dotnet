namespace Testcontainers.EventHubs.Configuration
{
    public class ConfigurationBuilder
    {
        private const string DefaultNamespace = "emulatorns1";
        
        private readonly RootConfiguration _rootConfiguration = new RootConfiguration();
            
        private ConfigurationBuilder()
        {
            _rootConfiguration.UserConfig = new UserConfig
            {
                NamespaceConfig = new List<NamespaceConfig>()
                {
                    new NamespaceConfig
                    {
                        Type = "EventHub",
                        Name = DefaultNamespace
                    }
                },
                LoggingConfig = new LoggingConfig() { Type = "File" }
            };
        }

        public static ConfigurationBuilder Create()
        {
            return new ConfigurationBuilder();
        }
        
        public ConfigurationBuilder WithEventHub(string entityName, string partitionCount, IEnumerable<string> consumerGroups)
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
                ConsumerGroups = consumerGroups.Select(consumerGroupName => new ConsumerGroup { Name = consumerGroupName }).ToList()
            });

            return this;
        }

        public string Build()
        {
            return JsonSerializer.Serialize(_rootConfiguration);
        }
    }
}