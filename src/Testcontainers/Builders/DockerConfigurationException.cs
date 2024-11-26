namespace DotNet.Testcontainers.Builders
{
  using System;
  using JetBrains.Annotations;

  /// <summary>
  /// The exception that is thrown when the Docker configuration file cannot be read successfully.
  /// </summary>
  [PublicAPI]
  public sealed class DockerConfigurationException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerConfigurationException"/> class, using the provided message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public DockerConfigurationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerConfigurationException"/> class, using the provided message and exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DockerConfigurationException(string message, Exception innerException) : base(message, innerException)
    {
    }
  }
}
