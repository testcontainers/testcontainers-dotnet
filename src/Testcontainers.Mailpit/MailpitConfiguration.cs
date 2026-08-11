namespace Testcontainers.Mailpit;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class MailpitConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MailpitConfiguration" /> class.
    /// </summary>
    /// <param name="smtpAuthCredentials">The username and password for SMTP authentication. The username must not contain a <c>:</c> character.</param>
    /// <param name="smtpAuthAllowInsecure">A value indicating whether insecure PLAIN and LOGIN SMTP authentication is allowed. Typically, STARTTLS is enforced for all SMTP authentication.</param>
    /// <param name="maxMessages">The maximum number of messages to store. Mailpit periodically deletes the oldest messages when the number of stored messages exceeds this value. Set to <c>0</c> to disable automatic deletion.</param>
    public MailpitConfiguration(
        NetworkCredential smtpAuthCredentials = null,
        bool smtpAuthAllowInsecure = true,
        uint maxMessages = 100)
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
    /// Gets the username and password for SMTP authentication. The username must not contain a <c>:</c> character.
    /// </summary>
    public NetworkCredential SmtpAuthCredentials { get; }

    /// <summary>
    /// Gets a value indicating whether insecure PLAIN and LOGIN SMTP authentication is allowed. Typically, STARTTLS is enforced for all SMTP authentication.
    /// </summary>
    public bool SmtpAuthAllowInsecure { get; }

    /// <summary>
    /// Gets the maximum number of messages to store. Mailpit periodically deletes the oldest messages when the number of stored messages exceeds this value. Set to <c>0</c> to disable automatic deletion.
    /// </summary>
    public uint MaxMessages { get; }
}