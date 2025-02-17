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
    public DockerUnavailableException(string message) : base(message)
    {
    }
  }
}
