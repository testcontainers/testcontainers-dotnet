namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.IO;
  using Microsoft.Extensions.Logging;
  using Xunit;

  public static class TestcontainersHostTest
  {
    public class ILogger
    {
      private static readonly string LogFile = $"testcontainers{DateTime.Now:yyyyMMdd}.log";

      [Fact]
      public void FromCategoryName()
      {
        // Given
        const string message = nameof(this.FromCategoryName);

        var log = TestcontainersHost.GetLogger(typeof(TestcontainersHostTest).FullName);

        // When
        log.LogInformation(message);

        // Then
        Assert.Contains(message, File.ReadAllText(LogFile));
      }

      [Fact]
      public void FromClassName()
      {
        // Given
        const string message = nameof(this.FromClassName);

        var log = TestcontainersHost.GetLogger<ILogger>();

        // When
        log.LogInformation(message);

        // Then
        Assert.Contains(message, File.ReadAllText(LogFile));
      }
    }
  }
}
