using System;
using JetBrains.Annotations;

namespace DotNet.Testcontainers.Configurations
{
  /// <summary>
  /// Represents an exception that is thrown when a Docker container fails to start.
  /// </summary>
  [PublicAPI]
  public sealed class ContainerException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ContainerException(string message)
      : base(message)
    {
    }
  }
}
