namespace DotNet.Testcontainers.Builders
{
  using System;
  using JetBrains.Annotations;

  /// <summary>
  /// Represents an exception that is thrown when a connection to the Docker endpoint
  /// cannot be established successfully.
  /// </summary>
  [PublicAPI]
  public sealed class DockerUnavailableException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerUnavailableException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DockerUnavailableException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerUnavailableException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DockerUnavailableException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
