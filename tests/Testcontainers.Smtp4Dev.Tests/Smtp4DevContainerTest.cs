using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mail;
using TestContainers.Smtp4Dev;

namespace Testcontainers.Smtp4Dev;

public sealed class Smtp4DevContainerTest : IAsyncLifetime
{
    private readonly Smtp4DevContainer _smtp4DevContainer = new Smtp4DevBuilder().Build();

    public Task InitializeAsync()
    {
        return _smtp4DevContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _smtp4DevContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task IsConnectedReturnsTrue()
    {
        // Given
        var host = _smtp4DevContainer.Hostname;

        var smtpPort = _smtp4DevContainer.GetMappedPublicPort(Smtp4DevBuilder.SmtpPort);
        var httpPort = _smtp4DevContainer.GetMappedPublicPort(Smtp4DevBuilder.WebInterfacePort);

        const string senderEmail = "sender@example.com";
        const string receiverEmail = "receiver@example.com";
        const string subject = "Test mail";
        const string body = "This is a test mail";

        using var smtpClient = new SmtpClient(host, smtpPort);
        using var httpClient = new HttpClient();

        // When
        await smtpClient.SendMailAsync(new MailMessage
        {
            From = new MailAddress(senderEmail),
            To = { new MailAddress(receiverEmail) },
            Subject = subject,
            Body = body,
        });

        // Then
        var result = await httpClient.GetFromJsonAsync<PagedMessageResult>($"http://{host}:{httpPort}/api/Messages");

        Assert.Contains(result.Results, message => message.From == senderEmail &&
                                                   message.To.Length == 1 && message.To[0] == receiverEmail &&
                                                   message.Subject == subject);
    }

    public record PagedMessageResult(List<Message> Results);

    public record Message(string From, string[] To, string Subject);
}