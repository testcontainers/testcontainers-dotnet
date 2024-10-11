using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Testcontainers.Mailpit;

public sealed class MailpitContainerTest : IAsyncLifetime
{
    private readonly MailpitContainer _mailpitContainer = new MailpitBuilder()
        .WithSmtpAuthCredentials(
            new List<MailpitConfiguration.AuthCredentials>([GetTestCredentials()])
        )
        .Build();

    public Task InitializeAsync()
    {
        return _mailpitContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _mailpitContainer.DisposeAsync().AsTask();
    }

    private static MailpitConfiguration.AuthCredentials GetTestCredentials() => new("test", "test");

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task MailSentAndApiReturnsSuccessful()
    {
        // Given
        const string to = "receiver@mailpit-testcontainers.com";
        const string from = "sender@mailpit-testcontainers.com";
        var message = new MailMessage(from, to)
        {
            Subject = "Hey there from Mailpit!",
            Body =
                "This is just a test message, it doesn't have much going on.\n\nCheers,\n\nSender"
        };
        var credentials = GetTestCredentials();
        var smtpClient = new SmtpClient(_mailpitContainer.Hostname, _mailpitContainer.SmtpPort)
        {
            Credentials = new NetworkCredential(credentials.Username, credentials.Password)
        };

        // When
        await smtpClient.SendMailAsync(message);

        // Then
        var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );

        var host = _mailpitContainer.Hostname;
        var port = _mailpitContainer.ApiPort;
        var queryParams = HttpUtility.ParseQueryString(string.Empty);
        queryParams.Add("query", $"to:\"{to}\"");
        var url = $"http://{host}:{port}/api/v1/search?{queryParams}";
        var response = await client.GetAsync(url);

        var jsonString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseBody = JsonConvert.DeserializeObject<JObject>(jsonString);

        Assert.Equal(1, responseBody["messages_count"]);
    }
}
