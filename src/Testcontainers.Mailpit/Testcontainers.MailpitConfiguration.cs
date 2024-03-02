namespace Testcontainers.Mailpit;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class MailpitConfiguration : ContainerConfiguration
{
    public sealed class AuthCredentials
    {
        public string Username { get; }
        public string Password { get; }

        public AuthCredentials(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MailpitConfiguration" /> class.
    /// </summary>
    /// <param name="config">The Testcontainers.Mailpit config.</param>
    public MailpitConfiguration(
        List<AuthCredentials> smtpAuthCredentials = null,
        bool smtpAuthAllowInsecure = true,
        uint maxMessages = 100
    )
    {
        SmtpAuthCredentials = smtpAuthCredentials;
        SmtpAuthAllowInsecure = smtpAuthAllowInsecure;
        MaxMessages = maxMessages;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MailpitConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MailpitConfiguration(
        IResourceConfiguration<CreateContainerParameters> resourceConfiguration
    )
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MailpitConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MailpitConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MailpitConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MailpitConfiguration(MailpitConfiguration resourceConfiguration)
        : this(new MailpitConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MailpitConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public MailpitConfiguration(MailpitConfiguration oldValue, MailpitConfiguration newValue)
        : base(oldValue, newValue)
    {
        SmtpAuthCredentials = BuildConfiguration.Combine(
            oldValue.SmtpAuthCredentials,
            newValue.SmtpAuthCredentials
        );
        SmtpAuthAllowInsecure = BuildConfiguration.Combine(
            oldValue.SmtpAuthAllowInsecure,
            newValue.SmtpAuthAllowInsecure
        );
    }

    /// <summary>
    /// A list of usernames and passwords for SMTP authentication.
    /// See <a href="https://mailpit.axllent.org/docs/configuration/smtp-authentication/">Mailpit docs</a> for more information.
    /// </summary>
    public List<AuthCredentials> SmtpAuthCredentials { get; }

    /// <summary>
    /// Typically STARTTLS is enforced for all SMTP authentication. This option allows insecure PLAIN & LOGIN SMTP authentication.
    /// </summary>
    public bool SmtpAuthAllowInsecure { get; }

    /// <summary>
    /// Maximum number of messages to store. Mailpit will periodically delete the oldest messages if greater than this. Set to 0 to disable auto-deletion.
    /// </summary>
    public uint MaxMessages { get; }
}
