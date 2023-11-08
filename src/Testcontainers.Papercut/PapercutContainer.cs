using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Testcontainers.Papercut;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class PapercutContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PapercutContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public PapercutContainer(PapercutConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }

    public string WebUrl => "http://" + Hostname + ":" + GetMappedPublicPort(37408);

    public async Task<PapercutMessageCollection> GetMessages()
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri(WebUrl);
        var messageText = await client.GetStringAsync("/api/messages");
        return JsonConvert.DeserializeObject<PapercutMessageCollection>(messageText);
    }
    
    public async Task<PapercutMessage> GetMessage(string id)
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri(WebUrl);
        var messageText = await client.GetStringAsync($"/api/messages/{id}");
        return JsonConvert.DeserializeObject<PapercutMessage>(messageText);
    }

    public async Task<bool> DeleteMessages()
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri(WebUrl);
        var response = await client.DeleteAsync("/api/messages");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteMessages(string id)
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri(WebUrl);
        var response = await client.DeleteAsync($"/api/messages/{id}");
        return response.IsSuccessStatusCode;
    }

    public Task<HttpResponseMessage> DownloadRaw(string id)
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri(WebUrl);
        return client.GetAsync($"/api/messages/{id}/raw");
    }

    public Task<HttpResponseMessage> DownloadSection(string id, int index)
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri(WebUrl);
        return client.GetAsync($"/api/messages/{id}/sections/{index}");
    }

    public Task<HttpResponseMessage> DownloadSectionContent(string id, string contentId)
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri(WebUrl);
        return client.GetAsync($"/api/messages/{id}/contents/{contentId}");
    }

}