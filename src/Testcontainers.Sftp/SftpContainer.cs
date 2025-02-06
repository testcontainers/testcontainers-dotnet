namespace Testcontainers.Sftp;

[PublicAPI]
public sealed class SftpContainer : DockerContainer
{
    private readonly SftpConfiguration _configuration;

    public SftpContainer(SftpConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }
}