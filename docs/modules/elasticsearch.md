# Elasticsearch

[Elasticsearch](https://www.elastic.co/elasticsearch/) is a distributed, RESTful search and analytics engine capable of addressing a growing number of use cases. As the heart of the Elastic Stack, it centrally stores data for lightning fast search, fine‑tuned relevancy, and powerful analytics that scale with ease.

The following example uses the following NuGet packages:

```console title="Install the NuGet dependencies"
dotnet add package Testcontainers.Elasticsearch
dotnet add package Elastic.Clients.Elasticsearch
dotnet add package xunit
```

IDEs and editors may also require the following packages to run tests: `xunit.runner.visualstudio` and `Microsoft.NET.Test.Sdk`.

Copy and paste the following code into a new `.cs` test file within an existing test project.

```csharp
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Testcontainers.Elasticsearch;
using Xunit;

public sealed class ElasticsearchContainerTest : IAsyncLifetime
{
    private readonly ElasticsearchContainer _elasticsearch
        = new ElasticsearchBuilder().Build();

    [Fact]
    public async Task ReadFromElasticsearch()
    {
        var settings = new ElasticsearchClientSettings(new Uri(_elasticsearch.GetConnectionString()));
        settings.ServerCertificateValidationCallback(CertificateValidations.AllowAll);

        var client = new ElasticsearchClient(settings);

        var stats = await client.PingAsync();

        Assert.True(stats.IsValidResponse);
    }

    public Task InitializeAsync()
        => _elasticsearch.StartAsync();

    public Task DisposeAsync()
        => _elasticsearch.DisposeAsync().AsTask();
}
```

To execute the tests, use the command `dotnet test` from a terminal.

## A Note To Developers

The Testcontainers module creates a container that listens to requests over **HTTPS**. To communicate with the Elasticsearch instance, developers must create a `ElasticsearchClientSettings` instance and set the `ServerCertificateValidationCallback` delegate to `CertificateValidations.AllowAll`. Failing to do so will result in a communication failure as the .NET will reject the certificate coming from the container.

[xunit]: https://xunit.net/
