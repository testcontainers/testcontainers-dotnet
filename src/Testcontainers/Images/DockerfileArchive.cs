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
  using JetBrains.Annotations;

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
      [CanBeNull] string contextDirectory,
      [NotNull] string dockerfileDirectory,
      [NotNull] string dockerfile,
      [NotNull] IImage image,
      [NotNull] IReadOnlyDictionary<string, string> buildArguments,
      [NotNull] ILogger logger)
      : this(
        // The Docker build context wasn't originally supported. To stay backwards
        // compatible, the argument is optional and can be null. If it isn't set,
        // fall back to the Dockerfile directory.
        new DirectoryInfo(contextDirectory ?? dockerfileDirectory),
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
      [NotNull] DirectoryInfo contextDirectory,
      [NotNull] DirectoryInfo dockerfileDirectory,
      [NotNull] FileInfo dockerfile,
      [NotNull] FileInfo dockerignore,
      [NotNull] IImage image,
      [NotNull] IReadOnlyDictionary<string, string> buildArguments,
      [NotNull] ILogger logger)
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
      const string nameGroup = "name";

      const string valueGroup = "value";

      const string argGroup = "arg";

      const string imageGroup = "image";

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
        .Select(match => (Arg: match.Groups[argGroup], Image: match.Groups[imageGroup]))
        .Select(item => (Arg: ReplaceVariables(item.Arg.Value, args), Image: ReplaceVariables(item.Image.Value, args)))
        .Where(item => !item.Image.Any(char.IsUpper))
        .Where(item => !stages.Contains(item.Image))
        .Select(item =>
        {
          var fromArgs = ParseFromArgs(item.Arg).ToDictionary(arg => arg.Name, arg => arg.Value);
          _ = fromArgs.TryGetValue("platform", out var platform);
          return new DockerImage(item.Image, platform);
        })
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

          var dockerfileDirectoryLength = _dockerfileDirectory.FullName
            .TrimEnd(Path.DirectorySeparatorChar).Length + 1;

          var dockerfileRelativeFilePath = _dockerfile.FullName
            .Substring(dockerfileDirectoryLength);

          var dockerfileNormalizedRelativeFilePath = Unix.Instance.NormalizePath(dockerfileRelativeFilePath);

          await AddAsync(_dockerfile.FullName, dockerfileNormalizedRelativeFilePath, tarOutputStream)
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
    /// <param name="path">Directory to Docker configuration files.</param>
    /// <returns>Returns a list with all accepted Docker archive files.</returns>
    private static IEnumerable<string> GetFiles(string path)
    {
      return Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
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
    /// <param name="line">The line from a Dockerfile <c>FROM</c> statement.</param>
    /// <param name="variables">A dictionary containing variable names as keys and their replacement values as values.</param>
    /// <returns>A new image string where placeholders are replaced with their corresponding values.</returns>
    private static string ReplaceVariables(string line, IDictionary<string, string> variables)
    {
      const string nameGroup = "name";

      if (variables.Count == 0)
      {
        return line;
      }

      return VariablePattern.Replace(line, match =>
      {
        var name = match.Groups[nameGroup].Value;
        return variables.TryGetValue(name, out var value) ? value : match.Value;
      });
    }

    /// <summary>
    /// Parses a FROM statement arg string into flag and value pairs.
    /// </summary>
    /// <remarks>
    /// This method parses a string containing FROM statement style flags,
    /// respecting quoted values. Both double quotes (<c>"</c>) and single
    /// quotes (<c>'</c>) are supported. Whitespaces outside of quotes are
    /// treated as separators.
    ///
    /// For example, the line:
    /// <code>
    ///   --pull=always --platform="linux/amd64"
    /// </code>
    /// becomes:
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///       (<c>pull</c>, <c>always</c>)
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       (<c>platform</c>, <c>linux/amd64</c>)
    ///     </description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <param name="line">
    /// The FROM statement arg string containing flags and optional values.
    /// </param>
    /// <returns>
    /// A sequence of (<c>Name</c>, <c>Value</c>) tuples.
    /// </returns>
    /// <exception cref="FormatException">
    /// Thrown if a quoted value is missing a closing quote.
    /// </exception>
    private static IEnumerable<(string Name, string Value)> ParseFromArgs(string line)
    {
      if (string.IsNullOrEmpty(line))
      {
        yield break;
      }

      char? quote = null;

      var start = 0;

      for (var i = 0; i < line.Length; i++)
      {
        var c = line[i];

        if ((c == '"' || c == '\'') && (quote == null || quote == c))
        {
          quote = quote == null ? c : null;
        }

        if (quote != null || !char.IsWhiteSpace(c))
        {
          continue;
        }

        if (i > start)
        {
          yield return ParseFlag(line.Substring(start, i - start));
        }

        start = i + 1;
      }

      if (quote != null)
      {
        throw new FormatException($"Unmatched {quote} quote starting at position {start - 1} in line: '{line}'.");
      }

      if (line.Length > start)
      {
        yield return ParseFlag(line.Substring(start));
      }
    }

    /// <summary>
    /// Splits a single flag token into a flag name and an optional value.
    /// </summary>
    /// <param name="flag">A single flag token, optionally containing an equals sign and value.</param>
    /// <returns>A tuple containing the flag name and its value, or <c>null</c> if no value is specified.</returns>
    private static (string Name, string Value) ParseFlag(string flag)
    {
      var trimmed = flag.TrimStart('-');
      var eqIndex = trimmed.IndexOf('=');
      if (eqIndex == -1)
      {
        return (trimmed, null);
      }
      else
      {
        var name = trimmed.Substring(0, eqIndex);
        var value = trimmed.Substring(eqIndex + 1).Trim(' ', '"', '\'');
        return (name, value);
      }
    }
  }
}
