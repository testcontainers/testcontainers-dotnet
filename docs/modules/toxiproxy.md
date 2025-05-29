# Toxiproxy

[Toxiproxy](https://github.com/Shopify/toxiproxy) is a proxy to simulate network failure for testing. It can simulate latency, timeouts, bandwidth limits, and more between services.

This module integrates [Toxiproxy.Net](https://github.com/mdevilliers/Toxiproxy.Net), a .NET client for Toxiproxy's HTTP API. While the test suite includes examples for latency and timeout toxics, the implementation supports **all toxics and features** that Toxiproxy itself supports.

## Installation

Add the following dependency to your project file:

```shell
dotnet add package Testcontainers.Toxiproxy
```

## Usage Example

You can start a Toxiproxy container instance and configure proxies/toxics from any .NET test or application.

```csharp
var proxyPort = 12345;
var serverPort = 12346;

var container = new ToxiproxyBuilder()
    .WithProxy("my-proxy", $"0.0.0.0:{proxyPort}", $"host.docker.internal:{serverPort}")
    .WithPortBinding(proxyPort, false)
    .Build();

await container.StartAsync();

var proxy = container.Client.FindProxy("my-proxy");

proxy.Add(new LatencyToxic
{
    Name = "latency-toxic",
    Stream = ToxicDirection.DownStream,
    Attributes = { Latency = 500 }
});

// You can use the proxy (127.0.0.1:proxyPort) to connect with the injected network condition.
```

## Available Features

- Add and remove proxies dynamically
- Inject latency, timeout, bandwidth limit, and more via toxics
- Use `Toxiproxy.Net` to interact with the running Toxiproxy server
- Test fault tolerance of networked services in isolated environments

> Note: The library leverages the official [Toxiproxy.Net](https://github.com/mdevilliers/Toxiproxy.Net) client. Though the test suite demonstrates a couple of toxic types (e.g., latency, timeout), the module supports **all Toxiproxy features**.

## Running Tests

To execute the tests, use the command:

```shell
dotnet test
```
