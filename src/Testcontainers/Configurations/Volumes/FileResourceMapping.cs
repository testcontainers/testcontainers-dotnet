namespace DotNet.Testcontainers.Configurations
{
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;

  /// <inheritdoc cref="IResourceMapping" />
  internal class FileResourceMapping : IResourceMapping
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FileResourceMapping" /> class.
    /// </summary>
    /// <param name="hostPath">The absolute path of a file to map on the host system.</param>
    /// <param name="containerPath">The absolute path of a file to map in the container.</param>
    /// <param name="fileMode">The POSIX file mode permission.</param>
    public FileResourceMapping(string hostPath, string containerPath, UnixFileModes fileMode)
    {
      Type = MountType.Bind;
      Source = hostPath;
      Target = containerPath;
      FileMode = fileMode;
      AccessMode = AccessMode.ReadOnly;
    }

    /// <inheritdoc />
    public MountType Type { get; }

    /// <inheritdoc />
    public AccessMode AccessMode { get; }

    /// <inheritdoc />
    public string Source { get; }

    /// <inheritdoc />
    public string Target { get; }

    /// <inheritdoc />
    public UnixFileModes FileMode { get; }

    /// <inheritdoc />
    public Task CreateAsync(CancellationToken ct = default)
    {
      return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(CancellationToken ct = default)
    {
      return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task<byte[]> GetAllBytesAsync(CancellationToken ct = default)
    {
      return Task.FromResult(File.ReadAllBytes(Source));
    }
  }
}
