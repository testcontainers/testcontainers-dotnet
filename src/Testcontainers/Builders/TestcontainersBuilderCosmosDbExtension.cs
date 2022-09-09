namespace DotNet.Testcontainers.Builders
{
    using DotNet.Testcontainers.Configurations;
    using DotNet.Testcontainers.Containers;
    using JetBrains.Annotations;

    [PublicAPI]
    public static class TestcontainersBuilderCosmosDbExtension
    {
        public static ITestcontainersBuilder<CosmosDbTestcontainer> WithCosmosDb(
            this ITestcontainersBuilder<CosmosDbTestcontainer> builder, CosmosDbTestcontainerConfiguration configuration)
        {
          return builder.WithImage(configuration.Image)
            .WithPortBinding(configuration.DefaultPort, true)
            .WithExposedPort(configuration.DefaultPort)
            .WithWaitStrategy(configuration.WaitStrategy)
            .WithOutputConsumer(configuration.OutputConsumer)
            .WithEnvironment("AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE", "false")
            .WithEnvironment("AZURE_COSMOS_EMULATOR_PARTITION_COUNT", "1")
            .ConfigureContainer(testcontainer =>
            {
              testcontainer.ContainerPort = configuration.DefaultPort;
              testcontainer.Password = configuration.Password;
            });
        }
    }
}
