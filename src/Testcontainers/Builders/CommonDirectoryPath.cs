namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Runtime.CompilerServices;
  using JetBrains.Annotations;

  /// <summary>
  /// Resolves common directory paths.
  /// </summary>
  [PublicAPI]
  public readonly struct CommonDirectoryPath
  {
    private static readonly string WorkingDirectoryPath = Directory.GetCurrentDirectory();

    /// <summary>
    /// Initializes a new instance of the <see cref="CommonDirectoryPath" /> struct.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    [PublicAPI]
    public CommonDirectoryPath(string directoryPath)
    {
      DirectoryPath = directoryPath;
    }

    /// <summary>
    /// Gets the directory path.
    /// </summary>
    [PublicAPI]
    public string DirectoryPath { get; }

    /// <summary>
    /// Resolves the first bin directory upwards the directory tree.
    /// </summary>
    /// <returns>The first bin directory upwards the directory tree.</returns>
    /// <exception cref="DirectoryNotFoundException">Thrown when the bin directory was not found upwards the directory tree.</exception>
    [PublicAPI]
    public static CommonDirectoryPath GetBinDirectory()
    {
      var indexOfBinDirectory = WorkingDirectoryPath.LastIndexOf("bin", StringComparison.OrdinalIgnoreCase);

      if (indexOfBinDirectory > -1)
      {
        return new CommonDirectoryPath(WorkingDirectoryPath.Substring(0, indexOfBinDirectory));
      }

      const string message = "Cannot find 'bin' and resolve the base directory in the directory tree.";
      throw new DirectoryNotFoundException(message);
    }

    /// <summary>
    /// Resolves the first Git directory upwards the directory tree.
    /// </summary>
    /// <remarks>
    /// Start node is the caller file path directory. End node is the root directory.
    /// </remarks>
    /// <param name="filePath">The caller file path.</param>
    /// <returns>The first Git directory upwards the directory tree.</returns>
    /// <exception cref="DirectoryNotFoundException">Thrown when the Git directory was not found upwards the directory tree.</exception>
    [PublicAPI]
    public static CommonDirectoryPath GetGitDirectory([CallerFilePath, NotNull] string filePath = "")
    {
      return new CommonDirectoryPath(GetDirectoryPath(Path.GetDirectoryName(filePath), ".git"));
    }

    /// <summary>
    /// Resolves the first Visual Studio solution file upwards the directory tree.
    /// </summary>
    /// <remarks>
    /// Start node is the caller file path directory. End node is the root directory.
    /// </remarks>
    /// <param name="filePath">The caller file path.</param>
    /// <returns>The first Visual Studio solution file upwards the directory tree.</returns>
    /// <exception cref="DirectoryNotFoundException">Thrown when the Visual Studio solution file was not found upwards the directory tree.</exception>
    [PublicAPI]
    public static CommonDirectoryPath GetSolutionDirectory([CallerFilePath, NotNull] string filePath = "")
    {
      return new CommonDirectoryPath(GetDirectoryPath(Path.GetDirectoryName(filePath), "*.sln"));
    }

    /// <summary>
    /// Resolves the first CSharp project file upwards the directory tree.
    /// </summary>
    /// <remarks>
    /// Start node is the caller file path directory. End node is the root directory.
    /// </remarks>
    /// <param name="filePath">The caller file path.</param>
    /// <returns>The first CSharp project file upwards the directory tree.</returns>
    /// <exception cref="DirectoryNotFoundException">Thrown when the CSharp project file was not found upwards the directory tree.</exception>
    [PublicAPI]
    public static CommonDirectoryPath GetProjectDirectory([CallerFilePath, NotNull] string filePath = "")
    {
      return new CommonDirectoryPath(GetDirectoryPath(Path.GetDirectoryName(filePath), "*.csproj"));
    }

    /// <summary>
    /// Resolves the caller file path directory.
    /// </summary>
    /// <param name="filePath">The caller file path.</param>
    /// <returns>The caller file path directory.</returns>
    [PublicAPI]
    public static CommonDirectoryPath GetCallerFileDirectory([CallerFilePath, NotNull] string filePath = "")
    {
      return new CommonDirectoryPath(Path.GetDirectoryName(filePath));
    }

    private static string GetDirectoryPath(string path, string searchPattern)
    {
      return GetDirectoryPath(Directory.Exists(path) ? new DirectoryInfo(path) : null, searchPattern);
    }

    private static string GetDirectoryPath(DirectoryInfo path, string searchPattern)
    {
      if (path != null)
      {
        return path.EnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly).Any() ? path.FullName : GetDirectoryPath(path.Parent, searchPattern);
      }

      var message = $"Cannot find '{searchPattern}' and resolve the base directory in the directory tree.";
      throw new DirectoryNotFoundException(message);
    }
  }
}
