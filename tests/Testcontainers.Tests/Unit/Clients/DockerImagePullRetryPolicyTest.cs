namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Net.Http;
  using System.Net.Sockets;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet;
  using DotNet.Testcontainers.Clients;
  using Xunit;

  public sealed class DockerImagePullRetryPolicyTest
  {
    public static IEnumerable<object[]> TransientExceptions { get; } = new Exception[]
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
    }.Select(exception => new object[] { exception });

    public static IEnumerable<object[]> PermanentExceptions { get; } = new Exception[]
    {
      new DockerApiException(HttpStatusCode.BadRequest, null),
      new DockerApiException(HttpStatusCode.Unauthorized, null),
      new DockerApiException(HttpStatusCode.Forbidden, null),
      new DockerApiException(HttpStatusCode.NotFound, null),
      new DockerApiException(HttpStatusCode.Conflict, null),
      new DockerApiException((HttpStatusCode)422, null),
      new DockerApiException((HttpStatusCode)600, null),
      new InvalidOperationException(),
    }.Select(exception => new object[] { exception });

    [Fact]
    public async Task DoesNotRetrySuccessfulPull()
    {
      var attempts = 0;
      var retries = 0;
      var delays = 0;

      await DockerImagePullRetryPolicy.ExecuteAsync(
          _ =>
          {
            attempts++;
            return Task.CompletedTask;
          },
          (_, _, _) => retries++,
          _ => TimeSpan.Zero,
          (_, _) =>
          {
            delays++;
            return Task.CompletedTask;
          },
          TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      Assert.Equal(1, attempts);
      Assert.Equal(0, retries);
      Assert.Equal(0, delays);
    }

    [Theory]
    [MemberData(nameof(TransientExceptions))]
    public async Task RetriesTransientFailure(Exception exception)
    {
      var attempts = 0;

      await DockerImagePullRetryPolicy.ExecuteAsync(
          _ =>
          {
            attempts++;
            return attempts == 1 ? Task.FromException(exception) : Task.CompletedTask;
          },
          (_, _, _) => { },
          _ => TimeSpan.Zero,
          (_, _) => Task.CompletedTask,
          TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      Assert.Equal(2, attempts);
    }

    [Theory]
    [MemberData(nameof(PermanentExceptions))]
    public async Task DoesNotRetryPermanentFailure(Exception exception)
    {
      var attempts = 0;

      var actualException = await Assert.ThrowsAsync(exception.GetType(), () => DockerImagePullRetryPolicy.ExecuteAsync(
          _ =>
          {
            attempts++;
            return Task.FromException(exception);
          },
          (_, _, _) => { },
          _ => TimeSpan.Zero,
          (_, _) => Task.CompletedTask,
          TestContext.Current.CancellationToken));

      Assert.Same(exception, actualException);
      Assert.Equal(1, attempts);
    }

    [Fact]
    public async Task SucceedsOnFinalAttempt()
    {
      var attempts = 0;
      var retryAttempts = new List<int>();
      var requestedDelays = new List<TimeSpan>();

      await DockerImagePullRetryPolicy.ExecuteAsync(
          _ => ++attempts < DockerImagePullRetryPolicy.MaxAttempts
            ? Task.FromException(new DockerApiException(HttpStatusCode.ServiceUnavailable, null))
            : Task.CompletedTask,
          (attempt, delay, _) =>
          {
            retryAttempts.Add(attempt);
            requestedDelays.Add(delay);
          },
          attempt => TimeSpan.FromSeconds(attempt),
          (_, _) => Task.CompletedTask,
          TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      Assert.Equal(DockerImagePullRetryPolicy.MaxAttempts, attempts);
      Assert.Equal(new[] { 2, 3 }, retryAttempts);
      Assert.Equal(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) }, requestedDelays);
    }

    [Fact]
    public async Task RethrowsFinalFailureAfterMaximumAttempts()
    {
      var attempts = 0;
      var finalException = new DockerApiException(HttpStatusCode.InternalServerError, null);

      var actualException = await Assert.ThrowsAsync<DockerApiException>(() => DockerImagePullRetryPolicy.ExecuteAsync(
          _ =>
          {
            attempts++;
            return Task.FromException(finalException);
          },
          (_, _, _) => { },
          _ => TimeSpan.Zero,
          (_, _) => Task.CompletedTask,
          TestContext.Current.CancellationToken));

      Assert.Same(finalException, actualException);
      Assert.Equal(DockerImagePullRetryPolicy.MaxAttempts, attempts);
    }

    [Fact]
    public async Task StopsWhenCancellationIsRequestedDuringDelay()
    {
      var attempts = 0;
      using var cts = new CancellationTokenSource();

      await Assert.ThrowsAnyAsync<OperationCanceledException>(() => DockerImagePullRetryPolicy.ExecuteAsync(
          _ =>
          {
            attempts++;
            return Task.FromException(new DockerApiException(HttpStatusCode.InternalServerError, null));
          },
          (_, _, _) => { },
          _ => TimeSpan.Zero,
          (_, token) =>
          {
            cts.Cancel();
            return Task.FromCanceled(token);
          },
          cts.Token));

      Assert.Equal(1, attempts);
    }

    [Fact]
    public async Task DoesNotStartPullWhenAlreadyCanceled()
    {
      var attempts = 0;
      using var cts = new CancellationTokenSource();
      cts.Cancel();

      await Assert.ThrowsAnyAsync<OperationCanceledException>(() => DockerImagePullRetryPolicy.ExecuteAsync(
          _ =>
          {
            attempts++;
            return Task.CompletedTask;
          },
          (_, _, _) => { },
          _ => TimeSpan.Zero,
          (_, _) => Task.CompletedTask,
          cts.Token));

      Assert.Equal(0, attempts);
    }

    [Fact]
    public async Task DoesNotRetryCallerCancellation()
    {
      var attempts = 0;
      using var cts = new CancellationTokenSource();

      await Assert.ThrowsAnyAsync<OperationCanceledException>(() => DockerImagePullRetryPolicy.ExecuteAsync(
          _ =>
          {
            attempts++;
            cts.Cancel();
            return Task.FromCanceled(cts.Token);
          },
          (_, _, _) => { },
          _ => TimeSpan.Zero,
          (_, _) => Task.CompletedTask,
          cts.Token));

      Assert.Equal(1, attempts);
    }

    [Fact]
    public void UsesExponentialBackoffWithBoundedJitter()
    {
      var firstDelay = DockerImagePullRetryPolicy.GetRetryDelay(1);
      var secondDelay = DockerImagePullRetryPolicy.GetRetryDelay(2);

      Assert.InRange(firstDelay, TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(1250));
      Assert.InRange(secondDelay, TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(2500));
    }

    [Fact]
    public async Task ReportsSanitizedDockerApiFailureReason()
    {
      const string SensitiveResponse = "registry response containing a secret";
      string reason = null;
      var attempts = 0;

      await DockerImagePullRetryPolicy.ExecuteAsync(
          _ => ++attempts == 1
            ? Task.FromException(new DockerApiException(HttpStatusCode.InternalServerError, SensitiveResponse))
            : Task.CompletedTask,
          (_, _, failureReason) => reason = failureReason,
          _ => TimeSpan.Zero,
          (_, _) => Task.CompletedTask,
          TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      Assert.Equal("Docker API status code 500 (InternalServerError)", reason);
      Assert.DoesNotContain(SensitiveResponse, reason, StringComparison.Ordinal);
    }
  }
}
