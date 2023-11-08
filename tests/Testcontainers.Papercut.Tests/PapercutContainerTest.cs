using System.Diagnostics;
using System.Linq;
using System.Net.Mail;

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
        var client = new SmtpClient(_papercutContainer.Hostname,_papercutContainer.GetMappedPublicPort(25));
        
        //When
        client.Send("test@test.com","recipient@test.com","Test","A test message");
        
        //Then
        var result = await _papercutContainer.GetMessages();

        var startTimeout = Stopwatch.StartNew();
        while (result.TotalMessageCount < 1 && startTimeout.Elapsed.TotalSeconds < 5)
        {
            result = await _papercutContainer.GetMessages();
        }
        
        Assert.Equal(1, result.TotalMessageCount);
        var message = await _papercutContainer.GetMessage(result.Messages.Single().Id);
        Assert.Equal("Test",message.Subject);
        Assert.Equal("A test message\n",message.TextBody);
        Assert.Equal("test@test.com",message.From.Single().Address);
        Assert.Equal("recipient@test.com",message.To.Single().Address);
    }
}