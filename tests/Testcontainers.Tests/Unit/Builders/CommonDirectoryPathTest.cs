namespace DotNet.Testcontainers.Tests.Unit
{
  using System.IO;
  using DotNet.Testcontainers.Builders;
  using Xunit;

  public sealed class CommonDirectoryPathTest
  {
    public static TheoryData<CommonDirectoryPath> CommonDirectoryPaths()
    {
      var theoryData = new TheoryData<CommonDirectoryPath>();
      theoryData.Add(CommonDirectoryPath.GetBinDirectory());
      theoryData.Add(CommonDirectoryPath.GetGitDirectory());
      theoryData.Add(CommonDirectoryPath.GetProjectDirectory());
      theoryData.Add(CommonDirectoryPath.GetSolutionDirectory());
      theoryData.Add(CommonDirectoryPath.GetCallerFileDirectory());
      return theoryData;
    }

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
