namespace Testcontainers.Mailpit;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class MailpitConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MailpitConfiguration" /> class.
    /// </summary>
    /// <param name="smtpAuthCredentials">Username and password for SMTP authentication. The username must not contain a <c>:</c> character.</param>
    /// <param name="smtpAuthAllowInsecure">Typically, STARTTLS is enforced for all SMTP authentication. This option allows insecure PLAIN & LOGIN SMTP authentication.</param>
    /// <param name="maxMessages">Maximum number of messages to store. Mailpit will periodically delete the oldest messages if greater than this. Set to 0 to disable auto-deletion.</param>
    public MailpitConfiguration(NetworkCredential smtpAuthCredentials = null, bool smtpAuthAllowInsecure = true, uint maxMessages = 100)
    {
        SmtpAuthCredentials = smtpAuthCredentials;
        SmtpAuthAllowInsecure = smtpAuthAllowInsecure;
        MaxMessages = maxMessages;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MailpitConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MailpitConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
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
        SmtpAuthCredentials = BuildConfiguration.Combine(oldValue.SmtpAuthCredentials, newValue.SmtpAuthCredentials);
        SmtpAuthAllowInsecure = BuildConfiguration.Combine(oldValue.SmtpAuthAllowInsecure, newValue.SmtpAuthAllowInsecure);
        MaxMessages = BuildConfiguration.Combine(oldValue.MaxMessages, newValue.MaxMessages);
    }

    /// <summary>
    /// Username and password for SMTP authentication. The username must not contain a <c>:</c> character.
    /// </summary>
    public NetworkCredential SmtpAuthCredentials { get; }

    /// <summary>
    /// Typically, STARTTLS is enforced for all SMTP authentication. This option allows insecure PLAIN & LOGIN SMTP authentication.
    /// </summary>
    public bool SmtpAuthAllowInsecure { get; }

    /// <summary>
    /// Maximum number of messages to store. Mailpit will periodically delete the oldest messages if greater than this. Set to 0 to disable auto-deletion.
    /// </summary>
    public uint MaxMessages { get; }
}
