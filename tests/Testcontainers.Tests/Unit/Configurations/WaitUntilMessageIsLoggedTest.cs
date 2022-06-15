namespace DotNet.Testcontainers.Tests.Unit
{
  using System.IO;
  using System.Linq;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using Xunit;

  public sealed class WaitUntilMessageIsLoggedTest
  {
    [Fact]
    public async Task UntilMessageIsLogged()
    {
      // Given
      const string expectedMessage = "Message has been logged.";

      await using var memoryStream = new MemoryStream();
      await using var streamWriter = new StreamWriter(memoryStream);

      // When
      await streamWriter.WriteAsync(expectedMessage);
      await streamWriter.FlushAsync();

      var wait = Wait.ForUnixContainer()
        .UntilMessageIsLogged(memoryStream, expectedMessage)
        .Build()
        .Skip(1)
        .First();

      // Then
      var exception = await Record.ExceptionAsync(() => WaitStrategy.WaitUntil(() => wait.Until(null, null)));
      Assert.Null(exception);
    }
  }
}
