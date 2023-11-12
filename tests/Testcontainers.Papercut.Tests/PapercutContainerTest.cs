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
    public async Task SendingAnEmail()
    {
        //Given
        var client = new SmtpClient(_papercutContainer.Hostname,_papercutContainer.PublicSmtpPort);
        
        //When
        client.Send("test@test.com","recipient@test.com","Test","A test message");
        
        //Then
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_papercutContainer.WebUrl);
        var messageText = await httpClient.GetStringAsync("/api/messages");
        var result = JsonConvert.DeserializeObject<PapercutMessageCollection>(messageText);

        var startTimeout = Stopwatch.StartNew();
        while (result.TotalMessageCount < 1 && startTimeout.Elapsed.TotalSeconds < 5)
        {
            messageText = await httpClient.GetStringAsync("/api/messages");
            result = JsonConvert.DeserializeObject<PapercutMessageCollection>(messageText);
        }
        
        Assert.Equal(1, result.TotalMessageCount);
        messageText = await httpClient.GetStringAsync($"/api/messages/{result.Messages.Single().Id}");
        var message = JsonConvert.DeserializeObject<PapercutMessage>(messageText);
        Assert.Equal("Test",message.Subject);
        Assert.Equal("A test message\n",message.TextBody);
        Assert.Equal("test@test.com",message.From.Single().Address);
        Assert.Equal("recipient@test.com",message.To.Single().Address);
    }
}