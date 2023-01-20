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
    private static readonly IOperatingSystem OS = new Unix(dockerEndpointAuthConfig: null);

    private readonly DirectoryInfo dockerfileDirectory;

    private readonly FileInfo dockerfile;

    private readonly IImage image;

    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerfileArchive" /> class.
    /// </summary>
    /// <param name="dockerfileDirectory">Directory to Docker configuration files.</param>
    /// <param name="dockerfile">Name of the Dockerfile, which is necessary to start the Docker build.</param>
    /// <param name="image">Docker image information to create the tar archive for.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentException">Thrown when the Dockerfile directory does not exist or the directory does not contain a Dockerfile.</exception>
    public DockerfileArchive(string dockerfileDirectory, string dockerfile, IImage image, ILogger logger)
      : this(new DirectoryInfo(dockerfileDirectory), new FileInfo(dockerfile), image, logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerfileArchive" /> class.
    /// </summary>
    /// <param name="dockerfileDirectory">Directory to Docker configuration files.</param>
    /// <param name="dockerfile">Name of the Dockerfile, which is necessary to start the Docker build.</param>
    /// <param name="image">Docker image information to create the tar archive for.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentException">Thrown when the Dockerfile directory does not exist or the directory does not contain a Dockerfile.</exception>
    public DockerfileArchive(DirectoryInfo dockerfileDirectory, FileInfo dockerfile, IImage image, ILogger logger)
    {
      if (!dockerfileDirectory.Exists)
      {
        throw new ArgumentException($"Directory '{dockerfileDirectory.FullName}' does not exist.");
      }

      if (!dockerfileDirectory.GetFiles(dockerfile.ToString(), SearchOption.TopDirectoryOnly).Any())
      {
        throw new ArgumentException($"{dockerfile} does not exist in '{dockerfileDirectory.FullName}'.");
      }

      this.dockerfileDirectory = dockerfileDirectory;
      this.dockerfile = dockerfile;
      this.image = image;
      this.logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> Tar(CancellationToken ct = default)
    {
      var dockerfileDirectoryPath = OS.NormalizePath(this.dockerfileDirectory.FullName);

      var dockerfileFilePath = OS.NormalizePath(this.dockerfile.ToString());

      var dockerfileArchiveFileName = Regex.Replace(this.image.FullName, "[^a-zA-Z0-9]", "-", RegexOptions.None, TimeSpan.FromSeconds(1)).ToLowerInvariant();

      var dockerfileArchiveFilePath = Path.Combine(Path.GetTempPath(), $"{dockerfileArchiveFileName}.tar");

      var dockerIgnoreFile = new DockerIgnoreFile(dockerfileDirectoryPath, ".dockerignore", dockerfileFilePath, this.logger);

      using (var tarOutputFileStream = new FileStream(dockerfileArchiveFilePath, FileMode.Create, FileAccess.Write))
      {
        using (var tarOutputStream = new TarOutputStream(tarOutputFileStream, Encoding.Default))
        {
          tarOutputStream.IsStreamOwner = false;

          foreach (var absoluteFilePath in GetFiles(dockerfileDirectoryPath))
          {
            // SharpZipLib drops the root path: https://github.com/icsharpcode/SharpZipLib/pull/582.
            var relativeFilePath = absoluteFilePath.Substring(dockerfileDirectoryPath.TrimEnd(Path.AltDirectorySeparatorChar).Length + 1);

            if (dockerIgnoreFile.Denies(relativeFilePath))
            {
              continue;
            }

            try
            {
              using (var inputStream = new FileStream(absoluteFilePath, FileMode.Open, FileAccess.Read))
              {
                var entry = TarEntry.CreateTarEntry(relativeFilePath);
                entry.Size = inputStream.Length;

                await tarOutputStream.PutNextEntryAsync(entry, ct)
                  .ConfigureAwait(false);

                await inputStream.CopyToAsync(tarOutputStream, 4096, ct)
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
      }

      return dockerfileArchiveFilePath;
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
        .Select(OS.NormalizePath)
        .ToArray();
    }
  }
}
