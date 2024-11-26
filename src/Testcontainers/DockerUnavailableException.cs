namespace DotNet.Testcontainers
{
  using System;
  using JetBrains.Annotations;

  /// <summary>
  /// The exception that is thrown when Docker is not available (because it is either not running or misconfigured).
  /// </summary>
  [PublicAPI]
  public sealed class DockerUnavailableException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerUnavailableException"/> class, using the provided message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    internal DockerUnavailableException(string message, Exception innerException) : base(message, innerException)
    {
    }
  }
}
