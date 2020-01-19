namespace DotNet.Testcontainers.Containers.OutputConsumers
{
  using System.IO;

  public class DoNotConsumeStdoutOrStderr : IOutputConsumer
  {
    public static readonly IOutputConsumer OutputConsumer = new DoNotConsumeStdoutOrStderr();

    private DoNotConsumeStdoutOrStderr()
    {
    }

    public Stream Stdout { get; } = Stream.Null;

    public Stream Stderr { get; } = Stream.Null;
  }
}
