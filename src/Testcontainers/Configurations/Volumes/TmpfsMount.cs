namespace DotNet.Testcontainers.Configurations
{
  using System.Threading;
  using System.Threading.Tasks;

  /// <inheritdoc cref="IMount" />
  internal readonly struct TmpfsMount : IMount
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TmpfsMount" /> struct.
    /// </summary>
    /// <param name="containerPath">The absolute path to mount the tmpfs in the container.</param>
    /// <param name="accessMode">The Docker volume access mode.</param>
    public TmpfsMount(string containerPath, AccessMode accessMode)
    {
      Type = MountType.Tmpfs;
      Source = string.Empty;
      Target = containerPath;
      AccessMode = accessMode;
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
