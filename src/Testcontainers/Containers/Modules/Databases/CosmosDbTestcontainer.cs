namespace DotNet.Testcontainers.Containers
{
  using System.Collections.Generic;
  using System.Net.Http;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using Microsoft.Extensions.Logging;
  using System.Text.Json;
  using System;
  using System.Threading;
  using System.Text;

  public sealed class CosmosDbTestcontainer : TestcontainerDatabase
  {
    private string CosmosUrl;

    private HttpClient HttpClient;

    internal CosmosDbTestcontainer(ITestcontainersConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        CosmosUrl = $"https://{this.Username}.documents.azure.com/dbs";
    }

    public override string ConnectionString
        => $"AccountEndpoint=https://{this.Username}:{this.Port}.documents.azure.com:443/;AccountKey={this.Password}";

    public async Task<HttpResponseMessage> QueryAsync(
        string queryString, IEnumerable<KeyValuePair<string, string>> parameters = default)
    {
        Console.WriteLine("Executing query...");
        var client = GetHttpClient();
        var parJsonStr =JsonSerializer.Serialize(parameters);
        var body = new { Query = queryString, Parameters = parJsonStr };
        var reqBodyStr = JsonSerializer.Serialize(body);
        var content = new StringContent(reqBodyStr);

        var response = await client.PostAsync(CosmosUrl, content);

        return response;
    }
    // TODO: Call this implicitly on initialize
    public async Task<HttpResponseMessage> CreateDatabaseAsync()
    {
        Console.WriteLine("Attempting to create database...");
        var url = $"https://localhost:8081/dbs";
        var client = GetHttpClient();
        var jsonData = JsonSerializer.Serialize(new { id = string.IsNullOrEmpty(this.Database) ? "testdb" : this.Database });
        var contentData = new StringContent(jsonData, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, contentData).ConfigureAwait(false);

        return response;
    }        

    private HttpClient GetHttpClient() 
    {
      if (HttpClient != null) return HttpClient;

      var handler = new CosmosDbHttpHandler(this.Password);
      var client = new HttpClient(handler);

      HttpClient = client;
      return HttpClient;
    }

    private sealed class CosmosDbHttpHandler : HttpClientHandler 
    {
      private readonly string _password;

      public CosmosDbHttpHandler(string password)
      {
        this._password = password;
        // Skip SSL certificate validation
        // https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator?tabs=ssl-netstd21#disable-ssl-validation
        this.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
      }

      // https://stackoverflow.com/questions/52262767/cosmos-db-rest-api-create-user-permissio
      protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken ct)
      {
        var hmac = new System.Security.Cryptography.HMACSHA256()
        {
          Key = Convert.FromBase64String(this._password)
        };

        var date = DateTime.UtcNow.ToString("r");
        var payload = "POST".ToLowerInvariant() + "\n" +
          "dbs".ToLowerInvariant() + "\n" +
          "" + "\n" +
          date.ToLowerInvariant() + "\n" +
          "" + "\n";

        var hashPayload = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var signature = Convert.ToBase64String(hashPayload);
        var authHeaderValue = System.Web.HttpUtility.UrlEncode($"type=master&ver=1.0&sig={signature}");

        request.Headers.Add("x-ms-documentdb-isquery", "true");
        request.Headers.Add("x-ms-documentdb-query-enablecrosspartition", "true");
        request.Headers.Add("x-ms-version", "2018-12-31");
        request.Headers.Add("x-ms-date", date);
        request.Headers.Add("authorization", authHeaderValue);
        //request.Headers.Add("Content-Type", "application/query+json");
         request.Headers.Add("Accept", "application/json");

        return base.SendAsync(request, ct);
      }
    }
  }
}
