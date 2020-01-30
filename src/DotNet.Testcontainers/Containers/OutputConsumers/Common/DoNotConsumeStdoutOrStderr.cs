namespace DotNet.Testcontainers.Containers.OutputConsumers.Common
{
  using System.IO;

  /// <inheritdoc cref="IOutputConsumer" />
  internal sealed class DoNotConsumeStdoutOrStderr : IOutputConsumer
  {
    /// <inheritdoc />
    public Stream Stdout { get; } = Stream.Null;

    /// <inheritdoc />
    public Stream Stderr { get; } = Stream.Null;

    public void Dispose()
    {
      this.Stdout.Dispose();
      this.Stderr.Dispose();
    }
  }
}
