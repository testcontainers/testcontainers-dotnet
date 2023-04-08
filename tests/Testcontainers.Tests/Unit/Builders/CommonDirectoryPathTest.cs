namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Collections.Generic;
  using System.IO;
  using DotNet.Testcontainers.Builders;
  using Xunit;

  public sealed class CommonDirectoryPathTest
  {
    public static IEnumerable<object[]> CommonDirectoryPaths { get; }
      = new[]
      {
        new[] { (object)CommonDirectoryPath.GetBinDirectory() },
        new[] { (object)CommonDirectoryPath.GetGitDirectory() },
        new[] { (object)CommonDirectoryPath.GetProjectDirectory() },
        new[] { (object)CommonDirectoryPath.GetSolutionDirectory() },
        new[] { (object)CommonDirectoryPath.GetCallerFileDirectory() },
      };

    [Theory]
    [MemberData(nameof(CommonDirectoryPaths))]
    public void CommonDirectoryPathExists(CommonDirectoryPath commonDirectoryPath)
    {
      Assert.True(Directory.Exists(commonDirectoryPath.DirectoryPath));
    }

    [Fact]
    public void CommonDirectoryPathNotExists()
    {
      var callerFilePath = Path.GetPathRoot(Directory.GetCurrentDirectory());
      Assert.Throws<DirectoryNotFoundException>(() => CommonDirectoryPath.GetGitDirectory(callerFilePath));
    }
  }
}
