namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;

  /// <inheritdoc cref="IResourceMapping" />
  internal sealed class UriResourceMapping : IResourceMapping
  {
    private readonly Uri _uri;

    /// <summary>
    /// Initializes a new instance of the <see cref="UriResourceMapping" /> class.
    /// </summary>
    /// <param name="uri">The URL of the file to download.</param>
    /// <param name="containerPath">The absolute path of the file to map in the container.</param>
    /// <param name="uid">The user ID to set for the copied resource.</param>
    /// <param name="gid">The group ID to set for the copied resource.</param>
    /// <param name="fileMode">The POSIX file mode permission.</param>
    public UriResourceMapping(Uri uri,
      string containerPath,
      uint uid,
      uint gid,
      UnixFileModes fileMode)
    {
      _uri = uri;
      Type = MountType.Bind;
      Source = uri.AbsoluteUri;
      Target = containerPath;
      UserId = uid;
      GroupId = gid;
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
    public uint UserId { get; }

    /// <inheritdoc />
    public uint GroupId { get; }

    /// <inheritdoc />
    public UnixFileModes FileMode { get; }

    /// <inheritdoc />
    public Task CreateAsync(CancellationToken ct = default) => Task.CompletedTask;

    /// <inheritdoc />
    public Task DeleteAsync(CancellationToken ct = default) => Task.CompletedTask;

    /// <inheritdoc />
    public async Task<byte[]> GetAllBytesAsync(CancellationToken ct = default)
    {
      using (var httpClient = new HttpClient())
      {
#if NET6_0_OR_GREATER
        return await httpClient.GetByteArrayAsync(_uri, ct)
          .ConfigureAwait(false);
#else
        return await httpClient.GetByteArrayAsync(_uri)
          .ConfigureAwait(false);
#endif
      }
    }
  }
}
