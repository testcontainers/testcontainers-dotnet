namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using System.Net;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Moq;
  using Xunit;

  public sealed class WaitUntilHttpRequestIsCompletedTest
  {
    public static IEnumerable<object[]> RequestData()
    {
      yield return new object[] { 80, HttpMethod.Head, string.Empty, 200, 200, false };
      yield return new object[] { 8080, HttpMethod.Head, "abs", 200, 200, false };
      yield return new object[] { 3000, HttpMethod.Options, "/", 200, 200, false };
      yield return new object[] { 5000, HttpMethod.Get, "/health", 204, 204, false };
      yield return new object[] { 8080, HttpMethod.Get, "/actuator/health", 200, 500, true };
    }

    [Theory]
    [MemberData(nameof(RequestData))]
    public async Task WaitUntilRequestIsCompleted(ushort port, HttpMethod method, string path, int expectedStatusCode, int resultStatusCode, bool shouldTimeout)
    {
      // Given
      var waitRequestBuilder = HttpWaitRequest.ForPort(port)
        .WithMethod(method)
        .ForPath(path)
        .ForStatusCode(expectedStatusCode);

      var callCounter = 0;

      HttpRequestMessage requestMessage = null;
      var handler = new MockClientHandler((request, _) =>
      {
        callCounter++;

        if (callCounter > 1)
        {
          throw new ArtificialTimeoutException();
        }

        requestMessage = request;
        return Task.FromResult(new HttpResponseMessage((HttpStatusCode)resultStatusCode));
      });

      const string hostname = "localhost";
      var testContainer = new Mock<ITestcontainersContainer>();
      testContainer.SetupGet(c => c.Hostname).Returns(hostname);
      testContainer.Setup(c => c.GetMappedPublicPort(port)).Returns(port);

      var wait = new UntilHttpRequestIsCompleted(waitRequestBuilder.Build(), 0, -1, handler, true);
      try
      {
        // When
        await wait.Until(testContainer.Object, null);

        // Then
        Assert.False(shouldTimeout);
        Assert.NotNull(requestMessage?.RequestUri);
        Assert.Equal(method, requestMessage.Method);
        Assert.Equal(hostname, requestMessage.RequestUri!.Host);
        Assert.Equal(port, requestMessage.RequestUri!.Port);
        Assert.Contains(path, requestMessage.RequestUri!.AbsolutePath);
      }
      catch (TimeoutException)
      {
        // Then
        Assert.True(shouldTimeout);
      }
    }

    private class ArtificialTimeoutException : TimeoutException
    {
    }

    private class MockClientHandler : HttpClientHandler
    {
      private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler;

      public MockClientHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler)
      {
        this.handler = handler;
      }

      protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
        return this.handler(request, cancellationToken);
      }
    }
  }
}
