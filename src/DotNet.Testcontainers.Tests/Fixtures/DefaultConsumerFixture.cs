namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.IO;
  using DotNet.Testcontainers.Diagnostics;

  public class DefaultConsumerFixture : DefaultConsumer
  {
    public DefaultConsumerFixture() : base(new MemoryStream(), new MemoryStream())
    {
    }
  }
}
