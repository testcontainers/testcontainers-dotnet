namespace DotNet.Testcontainers.Tests.Unit.Services
{
  using System;
  using System.IO;
  using DotNet.Testcontainers.Services;
  using Microsoft.Extensions.Logging;
  using Xunit;

  public static class TestcontainersHostServiceTest
  {
    public class Logger
    {
      private static readonly string LogFile = $"testcontainers{DateTime.Now:yyyyMMdd}.log";

      [Fact]
      public void FromCategoryName()
      {
        // Given
        const string message = nameof(this.FromCategoryName);

        var log = TestcontainersHostService.GetLogger(typeof(TestcontainersHostServiceTest).FullName);

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

        var log = TestcontainersHostService.GetLogger<Logger>();

        // When
        log.LogInformation(message);

        // Then
        Assert.Contains(message, File.ReadAllText(LogFile));
      }
    }
  }
}
