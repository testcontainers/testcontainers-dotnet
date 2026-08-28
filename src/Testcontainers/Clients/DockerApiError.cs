namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Globalization;
  using System.IO;
  using System.Net;
  using System.Net.Http;
  using System.Net.Sockets;
  using System.Threading;
  using Docker.DotNet;

  /// <summary>
  /// Classifies and describes Docker Engine API errors.
  /// </summary>
  internal static class DockerApiError
  {
    private const int TooManyRequests = 429;

    /// <summary>
    /// Gets a value indicating whether the exception represents a transient error that is worth retrying.
    /// </summary>
    /// <remarks>
    /// An <see cref="OperationCanceledException" /> is transient too. The Docker Engine API client throws it when a request times out.
    /// Callers are expected to check their own <see cref="CancellationToken" /> beforehand.
    /// </remarks>
    /// <param name="exception">The exception to classify.</param>
    /// <returns>True if the exception represents a transient error; otherwise, false.</returns>
    public static bool IsTransient(Exception exception)
    {
      if (exception is DockerApiException dockerApiException)
      {
        var statusCode = (int)dockerApiException.StatusCode;
        return statusCode == (int)HttpStatusCode.RequestTimeout || statusCode == TooManyRequests || (statusCode >= 500 && statusCode < 600);
      }

      return exception is OperationCanceledException
        || exception is HttpRequestException
        || exception is IOException
        || exception is SocketException
        || exception is TimeoutException;
    }

    /// <summary>
    /// Gets a short description of the exception that is safe to log.
    /// </summary>
    /// <remarks>
    /// The description does not include the Docker Engine API response, which may contain sensitive information.
    /// </remarks>
    /// <param name="exception">The exception to describe.</param>
    /// <returns>A short description of the exception.</returns>
    public static string GetReason(Exception exception)
    {
      if (exception is DockerApiException dockerApiException)
      {
        return string.Format(CultureInfo.InvariantCulture, "Docker API status code {0} ({1})", (int)dockerApiException.StatusCode, dockerApiException.StatusCode);
      }

      return exception.GetType().Name;
    }
  }
}
