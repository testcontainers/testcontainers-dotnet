namespace Testcontainers.Sftp;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class SftpContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SftpContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public SftpContainer(SftpConfiguration configuration)
        : base(configuration)
    {
    }
}