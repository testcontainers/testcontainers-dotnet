namespace Testcontainers.Papercut;

public sealed class PapercutContainerTest : IAsyncLifetime
{
    private readonly PapercutContainer _papercutContainer = new PapercutBuilder().Build();

    public Task InitializeAsync()
    {
        return _papercutContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _papercutContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ReceivesSentMessage()
    {
        // Given
        const string subject = "Test";

        using var smtpClient = new SmtpClient(_papercutContainer.Hostname, _papercutContainer.SmtpPort);

        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_papercutContainer.GetBaseAddress());

        // When
        smtpClient.Send("from@example.com", "to@example.com", subject, "A test message");

        var messagesJson = await httpClient.GetStringAsync("/api/messages")
            .ConfigureAwait(true);

        var jsonDocument = JsonDocument.Parse(messagesJson);
        var messages = jsonDocument.RootElement.GetProperty("messages").Deserialize<Message[]>();

        // Then
        Assert.Single(messages, message => subject.Equals(message.Subject));
    }

    private readonly struct Message
    {
        [JsonConstructor]
        public Message(string id, string subject, string size, DateTime createdAt)
        {
            Id = id;
            Subject = subject;
            Size = size;
            CreatedAt = createdAt;
        }

        [JsonPropertyName("id")]
        public string Id { get; }

        [JsonPropertyName("subject")]
        public string Subject { get; }

        [JsonPropertyName("size")]
        public string Size { get; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; }
    }
}