namespace Testcontainers.AwsEmulator;

/// <summary>
/// Represents the AWS Emulator credentials.
/// </summary>
/// <remarks>
/// The emulator generates the credentials during the startup and requires each
/// request to be signed with them. The credentials are valid for the emulator
/// instance only and cannot authenticate to AWS.
/// </remarks>
[PublicAPI]
public sealed class AwsEmulatorCredentials
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AwsEmulatorCredentials" /> class.
    /// </summary>
    /// <param name="accessKey">The access key.</param>
    /// <param name="secretKey">The secret key.</param>
    public AwsEmulatorCredentials(string accessKey, string secretKey)
    {
        AccessKey = accessKey;
        SecretKey = secretKey;
    }

    /// <summary>
    /// Gets the access key.
    /// </summary>
    public string AccessKey { get; }

    /// <summary>
    /// Gets the secret key.
    /// </summary>
    public string SecretKey { get; }
}