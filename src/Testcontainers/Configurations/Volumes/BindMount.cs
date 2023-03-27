namespace DotNet.Testcontainers.Configurations
{
  using System.Threading;
  using System.Threading.Tasks;

  /// <inheritdoc cref="IMount" />
  internal readonly struct BindMount : IMount
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="BindMount" /> struct.
    /// </summary>
    /// <param name="hostPath">The absolute path of a file or directory to mount on the host system.</param>
    /// <param name="containerPath">The absolute path of a file or directory to mount in the container.</param>
    /// <param name="accessMode">The Docker volume access mode.</param>
    public BindMount(string hostPath, string containerPath, AccessMode accessMode)
    {
      Type = MountType.Bind;
      Source = hostPath;
      Target = containerPath;
      AccessMode = accessMode;
    }

    public MountType Type { get; }

    /// <inheritdoc />
    public string Source { get; }

    /// <inheritdoc />
    public string Target { get; }

    /// <inheritdoc />
    public AccessMode AccessMode { get; }

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
  }
}
