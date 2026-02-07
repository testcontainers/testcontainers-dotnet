namespace Testcontainers.Papercut;

public sealed class PapercutContainerTest : IAsyncLifetime
{
    private readonly PapercutContainer _papercutContainer = new PapercutBuilder(TestSession.GetImageFromDockerfile()).Build();

    public async ValueTask InitializeAsync()
    {
        await _papercutContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _papercutContainer.DisposeAsync();
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

        var messagesJson = await httpClient.GetStringAsync("/api/messages", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var jsonDocument = JsonDocument.Parse(messagesJson);
        var messages = jsonDocument.RootElement.GetProperty("messages").Deserialize<Message[]>();

        // Then
        Assert.Single(messages, message => subject.Equals(message.Subject));
        Assert.Equal(_papercutContainer.GetBaseAddress(), _papercutContainer.GetConnectionString());
    }

    private sealed record Message
    {
        [JsonConstructor]
        public Message(string id, string subject, string size, DateTime createdAt)
        {
            Subject = subject;
        }

        [JsonPropertyName("subject")]
        public string Subject { get; }
    }
}