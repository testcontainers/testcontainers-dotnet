namespace DotNet.Testcontainers.Tests.Unit.Containers
{
  using System.IO;
  using System.Linq;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.WaitStrategies;
  using Xunit;

  public class WaitUntilMessageIsLogged
  {
    [Fact]
    public async Task UntilMessageIsLogged()
    {
      // Given
      const string expectedMessage = nameof(this.UntilMessageIsLogged);

      // When
      // Then
      using (var memoryStream = new MemoryStream())
      {
        using (var streamWriter = new StreamWriter(memoryStream))
        {
          await streamWriter.WriteAsync(expectedMessage);
          await streamWriter.FlushAsync();

          var wait = Wait.ForUnixContainer().UntilMessageIsLogged(memoryStream, expectedMessage);
          await WaitStrategy.WaitUntil(() => wait.Build().Skip(1).First().Until(null, string.Empty));
        }
      }
    }
  }
}
