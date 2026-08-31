namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.IO;
  using System.Net;
  using System.Net.Http;
  using System.Net.Sockets;
  using System.Threading.Tasks;
  using Docker.DotNet;
  using DotNet.Testcontainers.Clients;
  using Xunit;

  public sealed class DockerApiErrorTest
  {
    public static TheoryData<Exception> TransientExceptions { get; }
      = new TheoryData<Exception>
      {
        new DockerApiException(HttpStatusCode.RequestTimeout, null),
        new DockerApiException((HttpStatusCode)429, null),
        new DockerApiException(HttpStatusCode.InternalServerError, null),
        new DockerApiException(HttpStatusCode.NotImplemented, null),
        new DockerApiException(HttpStatusCode.BadGateway, null),
        new DockerApiException(HttpStatusCode.ServiceUnavailable, null),
        new DockerApiException(HttpStatusCode.GatewayTimeout, null),
        new DockerApiException((HttpStatusCode)599, null),
        new HttpRequestException(),
        new IOException(),
        new SocketException(),
        new TimeoutException(),
        new TaskCanceledException(),
      };

    public static TheoryData<Exception> PermanentExceptions { get; }
      = new TheoryData<Exception>
      {
        new DockerApiException(HttpStatusCode.BadRequest, null),
        new DockerApiException(HttpStatusCode.Unauthorized, null),
        new DockerApiException(HttpStatusCode.Forbidden, null),
        new DockerApiException(HttpStatusCode.NotFound, null),
        new DockerApiException(HttpStatusCode.Conflict, null),
        new DockerApiException((HttpStatusCode)422, null),
        new DockerApiException((HttpStatusCode)600, null),
        new InvalidOperationException(),
      };

    [Theory]
    [MemberData(nameof(TransientExceptions))]
    public void IdentifiesTransientException(Exception exception)
    {
      Assert.True(DockerApiError.IsTransient(exception));
    }

    [Theory]
    [MemberData(nameof(PermanentExceptions))]
    public void IdentifiesPermanentException(Exception exception)
    {
      Assert.False(DockerApiError.IsTransient(exception));
    }

    [Fact]
    public void ReturnsStatusCodeReasonForDockerApiException()
    {
      // Given
      const string sensitiveResponse = "registry response containing a secret";

      // When
      var reason = DockerApiError.GetReason(new DockerApiException(HttpStatusCode.InternalServerError, sensitiveResponse));

      // Then
      Assert.Equal("Docker API status code 500 (InternalServerError)", reason);
      Assert.DoesNotContain(sensitiveResponse, reason);
    }

    [Fact]
    public void ReturnsTypeNameReasonForOtherException()
    {
      Assert.Equal(nameof(TimeoutException), DockerApiError.GetReason(new TimeoutException()));
    }
  }
}
