namespace Testcontainers.Sftp;

public sealed class SftpContainerTest : IAsyncLifetime
{
    private readonly SftpContainer _sftpContainer = new SftpBuilder().Build();

    public async ValueTask InitializeAsync()
    {
        await _sftpContainer.StartAsync().ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _sftpContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task IsConnectedReturnsTrue()
    {
        // Given
        var host = _sftpContainer.Hostname;

        var port = _sftpContainer.GetMappedPublicPort(SftpBuilder.SftpPort);

        using var sftpClient = new SftpClient(
            host,
            port,
            SftpBuilder.DefaultUsername,
            SftpBuilder.DefaultPassword
        );

        // When
        await sftpClient.ConnectAsync(CancellationToken.None).ConfigureAwait(true);

        // Then
        Assert.True(sftpClient.IsConnected);
    }
}
