namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Text.RegularExpressions;

  public readonly struct CommonDirectoryPath
  {
    public static readonly CommonDirectoryPath SolutionRoot = new CommonDirectoryPath(GetDirectoryPathWithFileExtension(".csproj"));

    public static readonly CommonDirectoryPath ProjectRoot = new CommonDirectoryPath(GetDirectoryPathWithFileExtension(".sln"));

    private CommonDirectoryPath(string directoryPath)
    {
      this.DirectoryPath = directoryPath;
    }

    public string DirectoryPath { get; }

    public static string GetDirectoryPathWithFileExtension(string fileExtension)
    {
      var currentDirPath = GetParentBinPath();
      while (currentDirPath != null)
      {
        var fileInCurrentDir = Directory.GetFiles(currentDirPath);
        var filePath = fileInCurrentDir.SingleOrDefault(f => f.EndsWith(fileExtension, StringComparison.InvariantCultureIgnoreCase));
        if (filePath != null)
        {
          return currentDirPath;
        }

        currentDirPath = Directory.GetParent(currentDirPath)?.FullName;
      }

      throw new FileNotFoundException("Cannot find solution file path");
    }

    private static string GetParentBinPath()
    {
      var rootDir = Path.GetDirectoryName(System.Reflection.Assembly.
        GetExecutingAssembly().CodeBase);
      var pattern = "(?<!fil)[A-Za-z]:\\\\+[\\S\\s]*?(?=\\\\+bin)";
      var match = Regex.Match(rootDir, pattern);
      return match.Value;
    }
  }
}
