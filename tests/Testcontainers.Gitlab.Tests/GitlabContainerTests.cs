using Testcontainers.Gitlab.Models;
using System.Net.Http;

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
    public async Task GetUser()
    {
        var pat = await _gitlabContainer.GenerateAccessToken("root", PersonalAccessTokenScopes.api);

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("PRIVATE-TOKEN", pat.Token);
        var port = _gitlabContainer.GetMappedPublicPort(80);
        var result = await client.GetAsync($"http://localhost:{port}/api/v4/user");

        Assert.True(result.IsSuccessStatusCode);
    }
}