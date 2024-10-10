using Microsoft.Extensions.Logging;
using Seq.Api;
using System;
using System.Linq;

namespace Testcontainers.Seq;

public sealed class SeqContainerTest : IAsyncLifetime
{
    private readonly SeqContainer _seqContainer = new SeqBuilder().Build();

    public Task InitializeAsync()
    {
        return _seqContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _seqContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task CanLog()
    {
        var currentSeqApiPort = _seqContainer.GetMappedPublicPort(80);
        var currentSeqHostname = _seqContainer.Hostname;

        ILoggerFactory loggerFactory = new LoggerFactory();
        loggerFactory.AddSeq(_seqContainer.GetServerApiUrl());
        var testLogger = loggerFactory.CreateLogger("testlogger");
        testLogger.LogInformation("TRY THIS");

        var seqConnection = new SeqConnection(_seqContainer.GetServerApiUrl());
        var eventList = await seqConnection.Events.ListAsync(fromDateUtc: DateTime.Now.AddMinutes(-1));
        Assert.Contains(eventList, e => e.MessageTemplateTokens.Last().Text == "TRY THIS");
    }
}