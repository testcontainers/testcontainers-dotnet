namespace DotNet.Testcontainers.Builders
{
  using System.IO;

  public readonly struct CommonDirectoryPath
  {
    public static readonly CommonDirectoryPath SolutionRoot = new CommonDirectoryPath(".");

    public static readonly CommonDirectoryPath ProjectRoot = new CommonDirectoryPath(".");

    private CommonDirectoryPath(string directoryPath)
    {
      var rootDir = new DirectoryInfo(directoryPath);
      this.DirectoryPath = directoryPath;
    }

    public string DirectoryPath { get; }
  }
}
