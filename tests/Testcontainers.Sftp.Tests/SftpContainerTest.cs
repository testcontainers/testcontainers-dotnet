
namespace Testcontainers.Sftp;

public sealed class SftpContainerTest : IAsyncLifetime
{
    private readonly SftpContainer _sftpContainer = new SftpBuilder().Build();

    public Task InitializeAsync()
    {
        return _sftpContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _sftpContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task IsConnectedReturnsTrue()
    {
        // Given
        var hostname = _sftpContainer.Hostname;
        var port = _sftpContainer.GetMappedPublicPort(SftpBuilder.SftpPort);
        var client = new SftpClient(hostname, port, SftpBuilder.DefaultUsername, SftpBuilder.DefaultPassword);

        // When
        await client.ConnectAsync(CancellationToken.None);

        // Then
        Assert.True(client.IsConnected);
    }
}