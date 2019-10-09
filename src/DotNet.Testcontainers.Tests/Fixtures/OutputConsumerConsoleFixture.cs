namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.IO;
  using DotNet.Testcontainers.Containers.OutputConsumers;

  public class OutputConsumerConsoleFixture : OutputConsumerConsole
  {
    public OutputConsumerConsoleFixture() : base(new MemoryStream(), new MemoryStream())
    {
    }
  }
}
