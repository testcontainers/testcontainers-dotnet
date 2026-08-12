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

        using var mimePart = new TextPart();
        mimePart.Text = "This is just a test message, it doesn't have much going on.\n\nCheers,\n\nSender";

        using var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(from);
        mimeMessage.To.Add(to);
        mimeMessage.Subject = "Hey there from Mailpit!";
        mimeMessage.Body = mimePart;

        // When
        var messageId = await fixture.SendMessageAsync(mimeMessage, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var response = await fixture.ReadMessageAsync(messageId, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        var recipient = Assert.Single(response.To);
        Assert.Equal(mimeMessage.Subject, response.Subject);
        Assert.Equal(from.Address, response.From.Address);
        Assert.Equal(from.Name, response.From.Name);
        Assert.Equal(to.Address, recipient.Address);
        Assert.Equal(to.Name, recipient.Name);
    }

    public class MailpitFixture(IMessageSink messageSink)
        : ContainerFixture<MailpitBuilder, MailpitContainer>(messageSink)
    {
        [CanBeNull]
        protected virtual ICredentials Credentials
            => null;

        protected override MailpitBuilder Configure()
            => new MailpitBuilder(TestSession.GetImageFromDockerfile());

        public async Task<string> SendMessageAsync(MimeMessage message, CancellationToken cancellationToken = default)
        {
            using var smtpClient = new SmtpClient();
            smtpClient.ServerCertificateValidationCallback = (_, certificate, _, _) => certificate?.Issuer == "CN=localhost, O=Mailpit self-signed certificate";

            await smtpClient.ConnectAsync(Container.Hostname, Container.SmtpPort, SecureSocketOptions.Auto, cancellationToken)
                .ConfigureAwait(false);

            if (Credentials != null)
            {
                await smtpClient.AuthenticateAsync(Credentials, cancellationToken)
                    .ConfigureAwait(false);
            }

            var response = await smtpClient.SendAsync(message, cancellationToken)
                .ConfigureAwait(false);

            var messageId = MailpitRegexes.QueuedMessage().Match(response).Groups[1].Value;
            Assert.NotEmpty(messageId);

            return messageId;
        }

        public async Task<MailpitMessage> ReadMessageAsync(string messageId, CancellationToken cancellationToken = default)
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(Container.GetWebAddress());

            return await httpClient.GetFromJsonAsync<MailpitMessage>($"/api/v1/message/{messageId}", cancellationToken)
                .ConfigureAwait(false);
        }
    }

    [UsedImplicitly]
    public abstract class AuthenticationMailpitFixture(IMessageSink messageSink)
        : MailpitFixture(messageSink)
    {
        protected abstract bool AllowInsecure { get; }

        protected override NetworkCredential Credentials { get; }
            = new NetworkCredential("user", "p@ssw0rd");

        protected override MailpitBuilder Configure()
            => base.Configure().WithSmtpAuthCredentials(Credentials, AllowInsecure);
    }

    [UsedImplicitly]
    public sealed class AuthenticationSecureMailpitFixture(IMessageSink messageSink)
        : AuthenticationMailpitFixture(messageSink)
    {
        protected override bool AllowInsecure
            => false;
    }

    [UsedImplicitly]
    public sealed class AuthenticationInsecureMailpitFixture(IMessageSink messageSink)
        : AuthenticationMailpitFixture(messageSink)
    {
        protected override bool AllowInsecure
            => true;
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

    [UsedImplicitly]
    public sealed record MailpitMailbox
    {
        public MailpitMailbox(
            string address,
            string name)
        {
            Address = address;
            Name = name;
        }

        public string Address { get; }

        public string Name { get; }
    }

    [UsedImplicitly]
    public sealed record MailpitMessage
    {
        public MailpitMessage(
            string subject,
            MailpitMailbox from,
            MailpitMailbox[] to)
        {
            Subject = subject;
            From = from;
            To = to;
        }

        public string Subject { get; }

        public MailpitMailbox From { get; }

        public MailpitMailbox[] To { get; }
    }

    private static partial class MailpitRegexes
    {
        [GeneratedRegex("queued as (.+)")]
        public static partial Regex QueuedMessage();
    }
}