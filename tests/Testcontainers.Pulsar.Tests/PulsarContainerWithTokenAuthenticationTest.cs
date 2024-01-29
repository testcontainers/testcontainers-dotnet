using System.Collections.Generic;
using System.Threading;
using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;
using DotPulsar.Internal;
using Xunit.Abstractions;

namespace Testcontainers.Pulsar.Tests;

public sealed class PulsarContainerWithTokenAuthenticationTest : IAsyncLifetime
{
  private readonly CancellationTokenSource _cts;
  private readonly PulsarContainer _pulsarContainer;
  private readonly ITestOutputHelper _testOutputHelper;

  public PulsarContainerWithTokenAuthenticationTest(ITestOutputHelper testOutputHelper)
  {
    _testOutputHelper = testOutputHelper;
    _pulsarContainer = new PulsarBuilder().WithTokenAuthentication().Build();
    _cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
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
  public async Task PulsarContainer_WhenBrokerWithTokenAuthenticationIsStarted_ShouldConnect()
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
      .Authentication(new TokenAuthentication(_pulsarContainer.CreateToken(Timeout.InfiniteTimeSpan).Result))
      .ExceptionHandler(context => _testOutputHelper.WriteLine($"PulsarClient got an exception: {context.Exception}"))
      .ServiceUrl(new Uri(_pulsarContainer.GetPulsarBrokerUrl()))
      .Build();
}
