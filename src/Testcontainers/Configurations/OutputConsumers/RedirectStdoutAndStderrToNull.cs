namespace DotNet.Testcontainers.Configurations
{
  using System.IO;

  /// <inheritdoc cref="IOutputConsumer" />
  internal sealed class RedirectStdoutAndStderrToNull : IOutputConsumer
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RedirectStdoutAndStderrToNull" /> class.
    /// </summary>
    public RedirectStdoutAndStderrToNull()
    {
      Enabled = false;
      Stdout = Stream.Null;
      Stderr = Stream.Null;
    }

    /// <inheritdoc />
    public bool Enabled { get; }

    /// <inheritdoc />
    public Stream Stdout { get; }

    /// <inheritdoc />
    public Stream Stderr { get; }

    /// <inheritdoc />
    public void Dispose()
    {
      Stdout.Dispose();
      Stderr.Dispose();
    }
  }
}
