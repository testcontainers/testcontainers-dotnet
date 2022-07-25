namespace DotNet.Testcontainers.Containers
{
  using System.Collections.Generic;
  using System.Net.Http;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using Microsoft.Extensions.Logging;
  using System.Text.Json;

  public sealed class CosmosDbTestcontainer : TestcontainerDatabase
  {
    private readonly string CosmosUrl;

    private HttpClient HttpClient;

    internal CosmosDbTestcontainer(ITestcontainersConfiguration configuration, ILogger logger)
    : base(configuration, logger)
    {
        CosmosUrl = $"https://{this.Username}.documents.azure.com/dbs/{this.Database}";
    }

    public override string ConnectionString
        => $"AccountEndpoint=https://{this.Username}:{this.Port}.documents.azure.com:443/;AccountKey={this.Password}";

    public async Task<HttpResponseMessage> QueryAsync(
        string queryString, IEnumerable<KeyValuePair<string, string>> parameters)
    {
        var client = GetHttpClient();
        var parJsonStr = JsonSerializer.Serialize(parameters);
        var body = new { Query = queryString, Parameters = parJsonStr };
        var reqBodyStr = JsonSerializer.Serialize(body);
        var content = new StringContent(reqBodyStr);

        var response = await HttpClient.PostAsync(CosmosUrl, content);

        return response;
    }
        
    // Setup HttpClient following:
    // https://docs.microsoft.com/en-us/rest/api/cosmos-db/querying-cosmosdb-resources-using-the-rest-api#how-do-i-query-a-resource-by-using-rest
    private HttpClient GetHttpClient() 
    {
        if (HttpClient != null) return HttpClient;

        var client = new HttpClient(); 
        var authHeaderValue = System.Web.HttpUtility.UrlEncode($"type=master&ver=1.0&sig={this.Password}");

        client.DefaultRequestHeaders.Add("x-ms-documentdb-isquery", "true");
        client.DefaultRequestHeaders.Add("x-ms-documentdb-query-enablecrosspartition", "true");
        client.DefaultRequestHeaders.Add("x-ms-version", "2018-12-31");
        client.DefaultRequestHeaders.Add("x-ms-date", System.DateTime.UtcNow.ToLongTimeString());
        client.DefaultRequestHeaders.Add("authorization", authHeaderValue);
        client.DefaultRequestHeaders.Add("Content-Type", "application/query+json");
        client.DefaultRequestHeaders.Add("Content-Type", "application/query+json");

        return HttpClient;
    }
  }
}