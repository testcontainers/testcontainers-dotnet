using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Xunit;

namespace Testcontainers.SpiceDB;

public sealed class SpiceDBContainerTest : IAsyncLifetime
{
    private readonly SpiceDBContainer _spicedbContainer = new SpiceDBBuilder().Build();

    public async ValueTask InitializeAsync()
    {
        await _spicedbContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _spicedbContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ExpectedPortIsMapped()
    {
        // Given & When
        var mappedPort = _spicedbContainer.GetMappedPublicPort(SpiceDBBuilder.SpiceDBgRPCPort);

        // Then
        Assert.True(mappedPort > 0);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task VersionCommandReturnsSuccessful()
    {
        // Given
        List<string> commands = ["spicedb", "version"];

        // When
        var execResult = await _spicedbContainer.ExecAsync(commands, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void GetGrpcConnectionStringNotNull()
    {
        // Given & When
        var connectionString = _spicedbContainer.GetGrpcConnectionString();

        // Then
        Assert.NotNull(connectionString);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ShouldConnectToSpiceDB()
    {
        // Given
        var connectionString = _spicedbContainer.GetGrpcConnectionString();
        var handler = new SocketsHttpHandler();
        handler.SslOptions.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

        // When
        using var channel = GrpcChannel.ForAddress(connectionString, new GrpcChannelOptions
        {
            HttpHandler = handler
        });

        // Then
        Assert.NotNull(connectionString);
        Assert.NotNull(channel);

        // Test connectivity by attempting to connect
        await channel.ConnectAsync();
        Assert.Equal(ConnectivityState.Ready, channel.State);
    }
}
