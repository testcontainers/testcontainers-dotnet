namespace Testcontainers.AzureAppConfiguration;

public sealed class AzureAppConfigurationContainerTest : IAsyncLifetime
{
    private readonly AzureAppConfigurationContainer _azureAppConfigurationContainer = new AzureAppConfigurationBuilder().Build();

    public Task InitializeAsync()
    {
        return _azureAppConfigurationContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _azureAppConfigurationContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task GetConfigurationSettingReturnsSetConfigurationSetting()
    {
        // Given
        var client = new ConfigurationClient(_azureAppConfigurationContainer.GetConnectionString());

        // When
        await client.SetConfigurationSettingAsync(nameof(ConfigurationSetting.Key), nameof(ConfigurationSetting.Value))
            .ConfigureAwait(true);
        
        var response = await client.GetConfigurationSettingAsync(nameof(ConfigurationSetting.Key))
            .ConfigureAwait(true);

        // Then
        Assert.Equal(nameof(ConfigurationSetting.Value), response.Value.Value);
    }
}