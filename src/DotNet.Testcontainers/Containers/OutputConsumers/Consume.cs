namespace DotNet.Testcontainers.Containers.OutputConsumers
{
  using System.IO;
  using DotNet.Testcontainers.Containers.OutputConsumers.Common;

  /// <summary>
  /// Collection of pre-configured output consumers.
  /// </summary>
  public static class Consume
  {
    /// <summary>
    /// Does not consume the output of the Testcontainer.
    /// </summary>
    /// <returns>A output consumer.</returns>
    public static IOutputConsumer DoNotConsumeStdoutAndStderr()
    {
      return new DoNotConsumeStdoutOrStderr();
    }

    /// <summary>
    /// Redirects the output of the Testcontainer to <see cref="System.Console" />.
    /// </summary>
    /// <returns>A output consumer.</returns>
    public static IOutputConsumer RedirectStdoutAndStderrToConsole()
    {
      return new RedirectStdoutAndStderrToStream();
    }

    /// <summary>
    /// Redirects the output of the Testcontainer to the given streams..
    /// </summary>
    /// <param name="stdout">Receives Stdout.</param>
    /// <param name="stderr">Receives Stderr.</param>
    /// <returns>A output consumer.</returns>
    public static IOutputConsumer RedirectStdoutAndStderrToStream(Stream stdout, Stream stderr)
    {
      return new RedirectStdoutAndStderrToStream(stdout, stderr);
    }
  }
}
