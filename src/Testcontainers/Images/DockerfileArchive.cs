namespace DotNet.Testcontainers.Images
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Text.RegularExpressions;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using ICSharpCode.SharpZipLib.Tar;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// Generates a tar archive with Docker configuration files. The tar archive can be used to build a Docker image.
  /// </summary>
  internal sealed class DockerfileArchive : ITarArchive
  {
    private static readonly Regex ArgLinePattern = new Regex("^ARG\\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)=(?:\"(?<value>[^\"]*)\"|'(?<value>[^']*)'|(?<value>\\S+))", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));

    private static readonly Regex FromLinePattern = new Regex("^FROM\\s+(?<arg>--\\S+\\s)*(?<image>\\S+).*", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));

    private static readonly Regex VariablePattern = new Regex("\\$(\\{(?<name>[A-Za-z_][A-Za-z0-9_]*)\\}|(?<name>[A-Za-z_][A-Za-z0-9_]*))", RegexOptions.None, TimeSpan.FromSeconds(1));

    private readonly DirectoryInfo _contextDirectory;

    private readonly DirectoryInfo _dockerfileDirectory;

    private readonly FileInfo _dockerfile;

    private readonly FileInfo _dockerignore;

    private readonly IImage _image;

    private readonly IReadOnlyDictionary<string, string> _buildArguments;

    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerfileArchive" /> class.
    /// </summary>
    /// <param name="contextDirectory">Directory to Docker build context.</param>
    /// <param name="dockerfileDirectory">Directory to Docker configuration files.</param>
    /// <param name="dockerfile">Name of the Dockerfile, which is necessary to start the Docker build.</param>
    /// <param name="image">Docker image information to create the tar archive for.</param>
    /// <param name="buildArguments">Docker build arguments.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentException">Thrown when the Dockerfile directory does not exist or the directory does not contain a Dockerfile.</exception>
    public DockerfileArchive(
      string contextDirectory,
      string dockerfileDirectory,
      string dockerfile,
      IImage image,
      IReadOnlyDictionary<string, string> buildArguments,
      ILogger logger)
      : this(
        // The Docker build context wasn't originally supported. To stay backwards
        // compatible, the argument is optional and can be null. If it isn't set,
        // fall back to the Dockerfile directory.
        new DirectoryInfo(contextDirectory),
        new DirectoryInfo(dockerfileDirectory),
        new FileInfo(Path.Combine(dockerfileDirectory, dockerfile)),
        new FileInfo(Path.Combine(dockerfileDirectory, dockerfile + ".dockerignore")),
        image,
        buildArguments,
        logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerfileArchive" /> class.
    /// </summary>
    /// <param name="contextDirectory">Directory to Docker build context.</param>
    /// <param name="dockerfileDirectory">Directory to Docker configuration files.</param>
    /// <param name="dockerfile">Name of the Dockerfile, which is necessary to start the Docker build.</param>
    /// <param name="dockerignore">Name of the .dockerignore file.</param>
    /// <param name="image">Docker image information to create the tar archive for.</param>
    /// <param name="buildArguments">Docker build arguments.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentException">Thrown when the Dockerfile directory does not exist or the directory does not contain a Dockerfile.</exception>
    private DockerfileArchive(
      DirectoryInfo contextDirectory,
      DirectoryInfo dockerfileDirectory,
      FileInfo dockerfile,
      FileInfo dockerignore,
      IImage image,
      IReadOnlyDictionary<string, string> buildArguments,
      ILogger logger)
    {
      if (!dockerfileDirectory.Exists)
      {
        throw new ArgumentException($"Directory '{dockerfileDirectory.FullName}' does not exist.");
      }

      if (!dockerfile.Exists)
      {
        throw new ArgumentException($"{dockerfile.Name} does not exist in '{dockerfileDirectory.FullName}'.");
      }

      _contextDirectory = contextDirectory;
      _dockerfileDirectory = dockerfileDirectory;
      _dockerfile = dockerfile;
      _dockerignore = dockerignore;
      _image = image;
      _buildArguments = buildArguments;
      _logger = logger;
    }

    /// <summary>
    /// Gets a collection of base images.
    /// </summary>
    /// <remarks>
    /// This method reads the Dockerfile and collects a list of base images. It
    /// excludes stages that do not correspond to base images. For example, it will not include
    /// the second line from the following Dockerfile configuration:
    /// <code>
    ///   FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
    ///   FROM build
    /// </code>
    /// </remarks>
    /// <returns>An <see cref="IEnumerable{T}" /> of <see cref="IImage" />.</returns>
    public IEnumerable<IImage> GetBaseImages()
    {
      const string imageGroup = "image";

      const string nameGroup = "name";

      const string valueGroup = "value";

      var lines = File.ReadAllLines(_dockerfile.FullName)
        .Select(line => line.Trim())
        .Where(line => !string.IsNullOrEmpty(line))
        .Where(line => !line.StartsWith("#", StringComparison.Ordinal))
        .ToArray();

      var argMatches = lines
        .Select(line => ArgLinePattern.Match(line))
        .Where(match => match.Success)
        .ToArray();

      var fromMatches = lines
        .Select(line => FromLinePattern.Match(line))
        .Where(match => match.Success)
        .ToArray();

      var args = argMatches
        .Select(match => new KeyValuePair<string, string>(match.Groups[nameGroup].Value, match.Groups[valueGroup].Value))
        .Concat(_buildArguments)
        .GroupBy(kvp => kvp.Key)
        .ToDictionary(group => group.Key, group => group.Last().Value);

      var stages = fromMatches
        .Select(line => line.Value)
        .Select(line => line.Split(new[] { " AS ", " As ", " aS ", " as " }, StringSplitOptions.RemoveEmptyEntries))
        .Where(substrings => substrings.Length > 1)
        .Select(substrings => substrings[substrings.Length - 1])
        .Distinct()
        .ToArray();

      var images = fromMatches
        .Select(match => match.Groups[imageGroup])
        .Select(match => match.Value)
        .Select(line => ReplaceVariables(line, args))
        .Where(line => !line.Any(char.IsUpper))
        .Where(value => !stages.Contains(value))
        .Distinct()
        .Select(value => new DockerImage(value))
        .ToArray();

      return images;
    }

    /// <inheritdoc />
    public async Task<string> Tar(CancellationToken ct = default)
    {
      var dockerIgnoreFileName = _dockerignore.Exists ? _dockerignore.Name : ".dockerignore";

      var dockerIgnoreFile = new DockerIgnoreFile(_dockerfileDirectory, dockerIgnoreFileName, _dockerfile.Name, _logger);

      var dockerfileArchiveFileName = Regex.Replace(_image.FullName, "[^a-zA-Z0-9]", "-", RegexOptions.None, TimeSpan.FromSeconds(1)).ToLowerInvariant();

      var dockerfileArchiveFilePath = Path.Combine(Path.GetTempPath(), $"{dockerfileArchiveFileName}.tar");

      var baseDirectoryLength = _contextDirectory.FullName.TrimEnd(Path.DirectorySeparatorChar).Length + 1;

      using (var tarOutputFileStream = new FileStream(dockerfileArchiveFilePath, FileMode.Create, FileAccess.Write))
      {
        using (var tarOutputStream = new TarOutputStream(tarOutputFileStream, Encoding.Default))
        {
          tarOutputStream.IsStreamOwner = false;

          foreach (var absoluteFilePath in GetFiles(_contextDirectory.FullName))
          {
            // SharpZipLib drops the root path: https://github.com/icsharpcode/SharpZipLib/pull/582.
            var relativeFilePath = absoluteFilePath.Substring(baseDirectoryLength);

            if (dockerIgnoreFile.Denies(relativeFilePath))
            {
              continue;
            }

            // If the build context already has a `Dockerfile`, we need to ignore it and
            // instead use the one from the specified Dockerfile directory, which might be
            // different.
            if (_dockerfile.Name.Equals(relativeFilePath, StringComparison.Ordinal))
            {
              continue;
            }

            await AddAsync(absoluteFilePath, relativeFilePath, tarOutputStream)
              .ConfigureAwait(false);
          }

          await AddAsync(_dockerfile.FullName, _dockerfile.Name, tarOutputStream)
            .ConfigureAwait(false);
        }
      }

      return dockerfileArchiveFilePath;

      async Task AddAsync(string absoluteFilePath, string relativeFilePath, TarOutputStream tarOutputStream)
      {
        const int bufferSize = 4096;

        try
        {
          using (var stream = new FileStream(
            absoluteFilePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize,
            FileOptions.Asynchronous | FileOptions.SequentialScan))
          {
            var fileMode = GetUnixFileMode(absoluteFilePath);

            var tarEntry = new TarEntry(new TarHeader());
            tarEntry.TarHeader.Name = relativeFilePath;
            tarEntry.TarHeader.Mode = fileMode;
            tarEntry.Size = stream.Length;

            await tarOutputStream.PutNextEntryAsync(tarEntry, ct)
              .ConfigureAwait(false);

            await stream.CopyToAsync(tarOutputStream, bufferSize, ct)
              .ConfigureAwait(false);

            await tarOutputStream.CloseEntryAsync(ct)
              .ConfigureAwait(false);
          }
        }
        catch (IOException e)
        {
          throw new IOException("Cannot create Docker image tar archive.", e);
        }
      }
    }

    /// <summary>
    /// Gets all accepted Docker archive files.
    /// </summary>
    /// <param name="directory">Directory to Docker configuration files.</param>
    /// <returns>Returns a list with all accepted Docker archive files.</returns>
    private static IEnumerable<string> GetFiles(string directory)
    {
      return Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories)
        .AsParallel()
        .Select(Path.GetFullPath)
        .Select(Unix.Instance.NormalizePath)
        .ToArray();
    }

    /// <summary>
    /// Gets the Unix file mode of the file on the path.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <returns>The Unix file mode of the file on the path.</returns>
    private static int GetUnixFileMode(string filePath)
    {
#if NET7_0_OR_GREATER
      if (!OperatingSystem.IsWindows())
      {
        return (int)File.GetUnixFileMode(filePath);
      }
#endif

      // Default to 755 for Windows and fall back to 755 for Unix when `GetUnixFileMode`
      // is not available.
      _ = filePath;
      return (int)Unix.FileMode755;
    }

    /// <summary>
    /// Replaces placeholders in the Dockerfile <c>FROM</c> image string with the values
    /// provided in <paramref name="variables" />. Each placeholder is replaced with the
    /// corresponding build argument if present; otherwise, the default value in the
    /// Dockerfile is preserved.
    /// </summary>
    /// <param name="image">The image string from a Dockerfile <c>FROM</c> statement.</param>
    /// <param name="variables">A dictionary containing variable names as keys and their replacement values as values.</param>
    /// <returns>A new image string where placeholders are replaced with their corresponding values.</returns>
    private static string ReplaceVariables(string image, IDictionary<string, string> variables)
    {
      const string nameGroup = "name";

      if (variables.Count == 0)
      {
        return image;
      }

      return VariablePattern.Replace(image, match =>
      {
        var name = match.Groups[nameGroup].Value;
        return variables.TryGetValue(name, out var value) ? value : match.Value;
      });
    }
  }
}
