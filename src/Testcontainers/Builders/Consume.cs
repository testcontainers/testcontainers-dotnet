namespace DotNet.Testcontainers.Builders
{
  using System.IO;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <summary>
  /// Collection of pre-configured output consumers.
  /// </summary>
  [PublicAPI]
  public static class Consume
  {
    /// <summary>
    /// Does not consume the output of the Testcontainer.
    /// </summary>
    /// <returns>A output consumer.</returns>
    [PublicAPI]
    public static IOutputConsumer DoNotConsumeStdoutAndStderr()
    {
      return RedirectStdoutAndStderrToNull.Instance;
    }

    /// <summary>
    /// Redirects the output of the Testcontainer to <see cref="System.Console" />.
    /// </summary>
    /// <returns>A output consumer.</returns>
    [PublicAPI]
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
    [PublicAPI]
    public static IOutputConsumer RedirectStdoutAndStderrToStream(Stream stdout, Stream stderr)
    {
      return new RedirectStdoutAndStderrToStream(stdout, stderr);
    }
  }
}
