using Testcontainers.Gitlab.Models;
using Testcontainers.Gitlab.RegexPatterns;
using System.Net.Http;
using System;

namespace Testcontainers.Gitlab;

public sealed class GitlabContainerTest : IAsyncLifetime
{
    private readonly GitlabContainer _gitlabContainer = new GitlabBuilder().Build();

    public Task InitializeAsync()
    {
        return _gitlabContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _gitlabContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ConnectionStateReturnsOpen()
    {
        var pat = await _gitlabContainer.GenerateAccessToken("root",PersonalAccessTokenScopes.api);
        Assert.True(!string.IsNullOrEmpty(pat.Token));
    }
}