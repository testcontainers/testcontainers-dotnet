namespace DotNet.Testcontainers.Containers.OutputConsumers.Common
{
  using System;
  using System.IO;

  /// <inheritdoc cref="IOutputConsumer" />
  internal sealed class DoNotConsumeStdoutOrStderr : IOutputConsumer
  {
    public DoNotConsumeStdoutOrStderr()
    {
      this.Stdout = Stream.Null;
      this.Stderr = Stream.Null;
    }

    ~DoNotConsumeStdoutOrStderr()
    {
      this.Dispose();
    }

    /// <inheritdoc />
    public Stream Stdout { get; }

    /// <inheritdoc />
    public Stream Stderr { get; }

    public void Dispose()
    {
      this.Stdout.Dispose();
      this.Stderr.Dispose();
      GC.SuppressFinalize(this);
    }
  }
}
