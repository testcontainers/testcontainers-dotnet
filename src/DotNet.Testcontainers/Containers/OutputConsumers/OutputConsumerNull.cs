namespace DotNet.Testcontainers.Containers.OutputConsumers
{
  using System.IO;

  public class OutputConsumerNull : IOutputConsumer
  {
    public Stream Stdout { get; } = Stream.Null;

    public Stream Stderr { get; } = Stream.Null;
  }
}
