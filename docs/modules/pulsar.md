# Apache Pulsar

Testcontainers can be used to automatically create [Apache Pulsar](https://pulsar.apache.org) containers without the need for external services. Based on the official Apache Pulsar Docker image, it is recommended to read the official [getting started](https://pulsar.apache.org/docs/next/getting-started-docker/) guide.

The following example uses the following NuGet packages:

```console title="Install the NuGet dependencies"
dotnet add package Testcontainers.Pulsar
dotnet add package DotPulsar
dotnet add package xunit
```

IDEs and editors may also require the following packages to run tests: `xunit.runner.visualstudio` and `Microsoft.NET.Test.Sdk`.

Copy and paste the following code into a new `.cs` test file within an existing test project.

```csharp
using System;
using System.Text;
using System.Threading.Tasks;
using DotPulsar;
using DotPulsar.Extensions;
using Xunit;

namespace Testcontainers.Pulsar;

public sealed class PulsarContainerTest : IAsyncLifetime
{
    private readonly PulsarContainer _pulsarContainer =
        new PulsarBuilder().Build();

    [Fact]
    public async Task ConsumerReceivesSendMessage()
    {
        const string helloPulsar = "Hello, Pulsar!";

        var topic = $"persistent://public/default/{Guid.NewGuid():D}";

        var name = Guid.NewGuid().ToString("D");

        await using var client = PulsarClient.Builder()
            .ServiceUrl(new Uri(_pulsarContainer.GetBrokerAddress()))
            .Build();

        await using var producer = client.NewProducer(Schema.String)
            .Topic(topic)
            .Create();

        await using var consumer = client.NewConsumer(Schema.String)
            .Topic(topic)
            .SubscriptionName(name)
            .InitialPosition(SubscriptionInitialPosition.Earliest)
            .Create();

        _ = await producer.Send(helloPulsar)
            .ConfigureAwait(true);

        var message = await consumer.Receive()
            .ConfigureAwait(true);

        Assert.Equal(helloPulsar, Encoding.Default.GetString(message.Data));
    }

    public Task InitializeAsync()
        => _pulsarContainer.StartAsync();

    public Task DisposeAsync()
        => _pulsarContainer.DisposeAsync().AsTask();
}
```

To execute the tests, use the command `dotnet test` from a terminal.

## Access Pulsar

To get the Pulsar broker URL use:

```csharp
string pulsarBrokerUrl = _pulsarContainer.GetPulsarBrokerUrl();
```

To get the Pulsar service URL use:
```csharp
string pulsarServiceUrl = _pulsarContainer.GetHttpServiceUrl();
```

## Enable token authentication

If you need to use token authentication, use the following builder configuration to enable authentication:

```csharp
PulsarContainer _pulsarContainer = PulsarBuilder().WithTokenAuthentication().Build();
```

Start the container and obtain an authentication token with a specified expiration time

```csharp
var authToken = await container.CreateAuthenticationTokenAsync(TimeSpan.FromHours(1))
    .ConfigureAwait(false);
```

Alternatively, set the token to never expire

```csharp
var authToken = await container.CreateAuthenticationTokenAsync(Timeout.InfiniteTimeSpan)
    .ConfigureAwait(false);
```

## Enable Pulsar Functions

If you need to use Pulsar Functions, use the following builder configuration to enable it:

```csharp
PulsarContainer _pulsarContainer = PulsarBuilder().WithFunctions().Build();
```
