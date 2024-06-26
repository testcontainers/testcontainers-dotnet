using System.Net.Sockets;
using System.Text;

namespace Testcontainers.ClamAv.Tests;

public sealed class ClamAvContainerTest : IAsyncLifetime
{
    private readonly ClamAvContainer _clamAvContainer = new ClamAvBuilder().Build();

    public Task InitializeAsync()
    {
        return _clamAvContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _clamAvContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void PingCommandReturnsPong()
    {
        string response = RunCommand("zPING\0"u8.ToArray());
        Assert.Equal("PONG\0", response);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void VersionCommandReturnsVersionString()
    {
        string response = RunCommand("zVERSION\0"u8.ToArray());

        // ClamAV 1.0.6/27266/Sun May  5 08:28:11 2024
        // where 27266 is the signatures version and it is followed by the date of the signatures
        Assert.StartsWith("ClamAV ", response);
    }

    private string RunCommand(byte[] command)
    {
        var endpoint = _clamAvContainer.GetDnsEndPoint();
        using TcpClient client = new(endpoint.Host, endpoint.Port);
        NetworkStream stream = client.GetStream();

        // send command
        stream.Write(command);

        // read response
        byte[] buffer = new byte[1024];
        int length = stream.Read(buffer, 0, buffer.Length);

        Assert.NotEqual(0, length);
        string response = Encoding.UTF8.GetString(buffer, 0, length);

        return response;
    }
}
