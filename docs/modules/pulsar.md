# Apache Pulsar Module

Testcontainers can be used to automatically create [Apache Pulsar](https://pulsar.apache.org) containers without external services.

It's based on the official Apache Pulsar docker image, it is recommended to read the [official guide](https://pulsar.apache.org/docs/next/getting-started-docker/).

The following example uses the following NuGet packages:

```console title="Install the NuGet dependencies"
dotnet add package Testcontainers.Pulsar
dotnet add package DotPulsar
dotnet add package xunit
```
IDEs and editors may also require the following packages to run tests: `xunit.runner.visualstudio` and `Microsoft.NET.Test.Sdk`.

Copy and paste the following code into a new `.cs` test file within an existing test project.

```csharp
using System.Collections.Generic;
using System.Threading;
using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;
using Xunit.Abstractions;

namespace Testcontainers.Pulsar.Tests;

public sealed class PulsarContainerTest : IAsyncLifetime
{
  private readonly CancellationTokenSource _cts;
  private readonly PulsarContainer _pulsarContainer;
  private readonly ITestOutputHelper _testOutputHelper;

  public PulsarContainerTest(ITestOutputHelper testOutputHelper)
  {
    _testOutputHelper = testOutputHelper;
    _pulsarContainer = new PulsarBuilder().Build();
    _cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
  }

  public Task InitializeAsync()
  {
    return _pulsarContainer.StartAsync();
  }

  public Task DisposeAsync()
  {
    return _pulsarContainer.DisposeAsync().AsTask();
  }

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public async Task PulsarContainer_WhenBrokerIsStarted_ShouldConnect()
  {
    // Given
    await using var client = CreateClient();
    var expected = new List<MessageId> { MessageId.Earliest };
    await using var reader = CreateReader(client, MessageId.Earliest, await CreateTopic(_cts.Token));

    // When
    var actual = await reader.GetLastMessageIds(_cts.Token);

    // Then
    Assert.Equal(expected,actual);
  }

  private IReader<string> CreateReader(IPulsarClient pulsarClient, MessageId messageId, string topicName)
    => pulsarClient.NewReader(Schema.String)
      .StartMessageId(messageId)
      .Topic(topicName)
      .Create();

  private static string CreateTopicName() => $"persistent://public/default/{Guid.NewGuid():N}";

  private async Task CreateTopic(string topic, CancellationToken cancellationToken)
  {
    var arguments = $"bin/pulsar-admin topics create {topic}";

    var result = await _pulsarContainer.ExecAsync(new[] { "/bin/bash", "-c", arguments }, cancellationToken);

    if (result.ExitCode != 0)
      throw new Exception($"Could not create the topic: {result.Stderr}");
  }

  private async Task<string> CreateTopic(CancellationToken cancellationToken)
  {
    var topic = CreateTopicName();
    await CreateTopic(topic, cancellationToken);
    return topic;
  }

  private IPulsarClient CreateClient()
    => PulsarClient
      .Builder()
      .ExceptionHandler(context => _testOutputHelper.WriteLine($"PulsarClient got an exception: {context.Exception}"))
      .ServiceUrl(new Uri(_pulsarContainer.GetPulsarBrokerUrl()))
      .Build();
}
```

To execute the test, use the command `dotnet test` from a terminal.

## Builder

### Token authentication
If you need to use token authentication use the follow with method in the builder
```csharp
PulsarBuilder().WithTokenAuthentication().Build();
```

and get the token by using
```csharp
var token = await _pulsarContainer.CreateToken(Timeout.InfiniteTimeSpan);
```

#### Pulsar Functions
If you need to use Pulsar Functions use the follow with method in the builder
```csharp
PulsarBuilder().WithFunctions().Build();
```
## Access Pulsar
To get the the Pulsar broker url.
```csharp
string pulsarBrokerUrl = _pulsarContainer.GetPulsarBrokerUrl();
```

To get the the Pulsar service url.
```csharp
string pulsarBrokerUrl = _pulsarContainer.GetHttpServiceUrl();
```
