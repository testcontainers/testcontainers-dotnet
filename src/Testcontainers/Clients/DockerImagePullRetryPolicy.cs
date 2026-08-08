namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.IO;
  using System.Net;
  using System.Net.Http;
  using System.Net.Sockets;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet;

  internal static class DockerImagePullRetryPolicy
  {
    internal const int MaxAttempts = 3;

    private const int InitialDelayInMilliseconds = 1000;

    private static readonly Random Random = new Random();

    public static Task ExecuteAsync(Func<CancellationToken, Task> pull, Action<int, TimeSpan, string> onRetry, CancellationToken ct)
    {
      return ExecuteAsync(pull, onRetry, GetRetryDelay, (delay, token) => Task.Delay(delay, token), ct);
    }

    internal static async Task ExecuteAsync(Func<CancellationToken, Task> pull, Action<int, TimeSpan, string> onRetry, Func<int, TimeSpan> getRetryDelay, Func<TimeSpan, CancellationToken, Task> delay, CancellationToken ct)
    {
      for (var attempt = 1; ; attempt++)
      {
        ct.ThrowIfCancellationRequested();

        try
        {
          await pull(ct)
            .ConfigureAwait(false);

          return;
        }
        catch (Exception exception) when (attempt < MaxAttempts && IsTransient(exception, ct))
        {
          var retryDelay = getRetryDelay(attempt);
          onRetry(attempt + 1, retryDelay, GetFailureReason(exception));

          await delay(retryDelay, ct)
            .ConfigureAwait(false);
        }
      }
    }

    internal static bool IsTransient(Exception exception, CancellationToken ct)
    {
      if (exception is DockerApiException dockerApiException)
      {
        var statusCode = (int)dockerApiException.StatusCode;
        return HttpStatusCode.RequestTimeout.Equals(dockerApiException.StatusCode)
          || (HttpStatusCode)429 == dockerApiException.StatusCode
          || statusCode >= 500 && statusCode <= 599;
      }

      if (exception is OperationCanceledException)
      {
        return !ct.IsCancellationRequested;
      }

      return exception is HttpRequestException
        || exception is IOException
        || exception is SocketException
        || exception is TimeoutException;
    }

    internal static TimeSpan GetRetryDelay(int attempt)
    {
      var exponentialDelay = InitialDelayInMilliseconds * (1 << (attempt - 1));
      int jitter;

      lock (Random)
      {
        jitter = Random.Next(0, exponentialDelay / 4 + 1);
      }

      return TimeSpan.FromMilliseconds(exponentialDelay + jitter);
    }

    private static string GetFailureReason(Exception exception)
    {
      if (exception is DockerApiException dockerApiException)
      {
        return $"Docker API status code {(int)dockerApiException.StatusCode} ({dockerApiException.StatusCode})";
      }

      return exception.GetType().Name;
    }
  }
}
