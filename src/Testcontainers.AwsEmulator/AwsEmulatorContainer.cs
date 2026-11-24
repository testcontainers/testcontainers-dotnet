namespace Testcontainers.AwsEmulator;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class AwsEmulatorContainer : DockerContainer
{
    private const string DefaultProfile = "[default]";

    /// <summary>
    /// Initializes a new instance of the <see cref="AwsEmulatorContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public AwsEmulatorContainer(AwsEmulatorConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the AWS Emulator connection string.
    /// </summary>
    /// <remarks>
    /// The emulator accepts HTTPS requests only. Configure the client to trust the
    /// certificate of <see cref="GetCertificateAsync" /> and to sign the requests
    /// with the credentials of <see cref="GetCredentialsAsync" />.
    /// </remarks>
    /// <returns>The AWS Emulator connection string.</returns>
    public string GetConnectionString()
    {
        return new UriBuilder(Uri.UriSchemeHttps, Hostname, GetMappedPublicPort(AwsEmulatorBuilder.AwsEmulatorPort)).ToString();
    }

    /// <summary>
    /// Gets the AWS Emulator certificate.
    /// </summary>
    /// <remarks>
    /// The emulator generates a self-signed certificate during the startup.
    /// Configure the client to trust it instead of accepting any certificate. The
    /// certificate is valid for <c>localhost</c>, <c>127.0.0.1</c> and <c>::1</c>.
    /// The validation fails if the Docker host is reached through another
    /// hostname or IP address.
    /// </remarks>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the certificate has been read, returning the AWS Emulator certificate.</returns>
    public async Task<X509Certificate2> GetCertificateAsync(CancellationToken ct = default)
    {
        var certificateBytes = await ReadFileAsync(AwsEmulatorBuilder.CertificateFilePath, ct)
            .ConfigureAwait(false);

#if NET5_0_OR_GREATER
        return X509Certificate2.CreateFromPem(Encoding.Default.GetString(certificateBytes));
#else
        return new X509Certificate2(certificateBytes);
#endif
    }

    /// <summary>
    /// Gets the AWS Emulator credentials.
    /// </summary>
    /// <remarks>
    /// The emulator generates the credentials during the startup and rejects
    /// requests that are not signed with them.
    /// </remarks>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the credentials have been read, returning the AWS Emulator credentials.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the credentials file does not contain the credentials of the default profile.</exception>
    public async Task<AwsEmulatorCredentials> GetCredentialsAsync(CancellationToken ct = default)
    {
        var credentialsBytes = await ReadFileAsync(AwsEmulatorBuilder.CredentialsFilePath, ct)
            .ConfigureAwait(false);

        return ParseCredentials(Encoding.Default.GetString(credentialsBytes));
    }

    /// <summary>
    /// Parses the credentials of the default profile.
    /// </summary>
    /// <remarks>
    /// The emulator writes the credentials in the format of the AWS
    /// shared credentials file. The method does not use the AWS
    /// configuration of the test host.
    /// </remarks>
    /// <param name="credentials">The content of the credentials file.</param>
    /// <returns>The AWS Emulator credentials.</returns>
    private static AwsEmulatorCredentials ParseCredentials(string credentials)
    {
        string accessKey = null;

        string secretKey = null;

        var isDefaultProfile = false;

        using var stringReader = new StringReader(credentials);

        string line;

        while ((line = stringReader.ReadLine()) != null)
        {
            line = line.Trim();

            if (line.StartsWith("[", StringComparison.Ordinal))
            {
                isDefaultProfile = DefaultProfile.Equals(line, StringComparison.Ordinal);
                continue;
            }

            var separatorIndex = line.IndexOf('=');

            if (!isDefaultProfile || separatorIndex < 0)
            {
                continue;
            }

            var key = line.Substring(0, separatorIndex).Trim();

            var value = line.Substring(separatorIndex + 1).Trim();

            switch (key)
            {
                case "aws_access_key_id":
                    accessKey = value;
                    break;
                case "aws_secret_access_key":
                    secretKey = value;
                    break;
            }
        }

        if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException($"The credentials file '{AwsEmulatorBuilder.CredentialsFilePath}' does not contain the access key and secret key of the default profile.");
        }

        return new AwsEmulatorCredentials(accessKey, secretKey);
    }
}