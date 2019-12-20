namespace DotNet.Testcontainers.Containers.OutputConsumers
{
  using System.IO;

  public class OutputConsumerNull : IOutputConsumer
  {
    public static readonly IOutputConsumer Consumer = new OutputConsumerNull();

    private OutputConsumerNull()
    {
    }

    public Stream Stdout { get; } = Stream.Null;

    public Stream Stderr { get; } = Stream.Null;
  }
}
