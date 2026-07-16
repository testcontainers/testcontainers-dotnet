namespace Testcontainers.Mailpit;

public abstract partial class MailpitContainerTest(MailpitContainerTest.MailpitFixture fixture)
{
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task MailSentAndApiReturnsSuccessful()
    {
        // Given
        var from = new MailboxAddress("The Sender", "sender@mailpit-testcontainers.com");
        var to = new MailboxAddress("The Receiver", "receiver@mailpit-testcontainers.com");
        var message = new MimeMessage
        {
            From = { from },
            To = { to },
            Subject = "Hey there from Mailpit!",
            Body = new TextPart { Text = "This is just a test message, it doesn't have much going on.\n\nCheers,\n\nSender" },
        };

        // When
        var messageId = await fixture.SendMessageAsync(message, TestContext.Current.CancellationToken);

        // Then
        var response = await fixture.GetMessageAsync(messageId, TestContext.Current.CancellationToken);
        Assert.Equal(message.Subject, response.Subject);
        Assert.Equal(from.Address, response.From.Address);
        Assert.Equal(from.Name, response.From.Name);
        Assert.Equal(to.Address, response.To[0].Address);
        Assert.Equal(to.Name, response.To[0].Name);
    }

    public partial class MailpitFixture(IMessageSink messageSink)
        : ContainerFixture<MailpitBuilder, MailpitContainer>(messageSink)
    {
        protected override MailpitBuilder Configure()
            => new(TestSession.GetImageFromDockerfile());

        [GeneratedRegex("queued as (.+)")]
        private static partial Regex QueuedMessage();

        [CanBeNull]
        protected virtual ICredentials Credentials => null;

        public async Task<string> SendMessageAsync(MimeMessage message, CancellationToken cancellationToken)
        {
            using var smtpClient = new SmtpClient();
            smtpClient.ServerCertificateValidationCallback = (_, certificate, _, _) => certificate?.Issuer == "CN=localhost, O=Mailpit self-signed certificate";
            await smtpClient.ConnectAsync(Container.Hostname, Container.SmtpPort, SecureSocketOptions.Auto, cancellationToken);
            if (Credentials != null)
            {
                await smtpClient.AuthenticateAsync(Credentials, cancellationToken);
            }

            var result = await smtpClient.SendAsync(message, cancellationToken);

            var messageId = QueuedMessage().Match(result).Groups[1].Value;
            Assert.NotEmpty(messageId);
            return messageId;
        }

        public async Task<MailpitMessage> GetMessageAsync(string messageId, CancellationToken cancellationToken)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(Container.GetWebAddress());
            return await client.GetFromJsonAsync<MailpitMessage>($"/api/v1/message/{messageId}", cancellationToken);
        }

        public class MailpitMessage
        {
            public string Subject { get; init; }
            public Mailbox From { get; init; }
            public Mailbox[] To { get; init; } = [];

            public class Mailbox
            {
                public string Address { get; init; }
                public string Name { get; init; }
            }
        }
    }

    public abstract class AuthenticationMailpitFixture(IMessageSink messageSink) : MailpitFixture(messageSink)
    {
        protected override NetworkCredential Credentials { get; } = new NetworkCredential("user", "p@ssw0rd");

        protected abstract bool AllowInsecure { get; }

        protected override MailpitBuilder Configure()
            => base.Configure().WithSmtpAuthCredentials(Credentials, AllowInsecure);
    }

    [UsedImplicitly]
    public class AuthenticationSecureMailpitFixture(IMessageSink messageSink) : AuthenticationMailpitFixture(messageSink)
    {
        protected override bool AllowInsecure => false;
    }

    [UsedImplicitly]
    public class AuthenticationInsecureMailpitFixture(IMessageSink messageSink) : AuthenticationMailpitFixture(messageSink)
    {
        protected override bool AllowInsecure => true;
    }

    [UsedImplicitly]
    public sealed class DefaultConfiguration(MailpitFixture fixture)
        : MailpitContainerTest(fixture), IClassFixture<MailpitFixture>;

    [UsedImplicitly]
    public sealed class SmtpAuthSecure(AuthenticationSecureMailpitFixture fixture)
        : MailpitContainerTest(fixture), IClassFixture<AuthenticationSecureMailpitFixture>;

    [UsedImplicitly]
    public sealed class SmtpAuthInsecure(AuthenticationInsecureMailpitFixture fixture)
        : MailpitContainerTest(fixture), IClassFixture<AuthenticationInsecureMailpitFixture>;
}
