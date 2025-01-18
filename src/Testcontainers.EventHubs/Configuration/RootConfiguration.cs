namespace Testcontainers.EventHubs.Configuration;

/// <summary>
/// Azure Event Hubs emulator JSON configuration.
/// <seealso href="https://raw.githubusercontent.com/Azure/azure-event-hubs-emulator-installer/refs/heads/main/EventHub-Emulator/Schema/Config-schema.json"/>
/// </summary>
public record RootConfiguration
{
    public UserConfig UserConfig { get; set; }
}