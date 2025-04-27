namespace DotNet.Testcontainers.Images
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IFutureDockerImage" />
  [PublicAPI]
  internal sealed class FutureDockerImage : Resource, IFutureDockerImage
  {
    private readonly ITestcontainersClient _client;

    private readonly IImageFromDockerfileConfiguration _configuration;

    private ImageInspectResponse _image = new ImageInspectResponse();

    /// <summary>
    /// Initializes a new instance of the <see cref="FutureDockerImage" /> class.
    /// </summary>
    /// <param name="configuration">The image configuration.</param>
    public FutureDockerImage(IImageFromDockerfileConfiguration configuration)
    {
      _client = new TestcontainersClient(configuration.SessionId, configuration.DockerEndpointAuthConfig, configuration.Logger);
      _configuration = configuration;
    }

    /// <inheritdoc />
    public string Repository
    {
      get
      {
        ThrowIfResourceNotFound();
        return _configuration.Image.Repository;
      }
    }

    /// <inheritdoc />
    public string Registry
    {
      get
      {
        ThrowIfResourceNotFound();
        return _configuration.Image.Registry;
      }
    }

    /// <inheritdoc />
    public string Tag
    {
      get
      {
        ThrowIfResourceNotFound();
        return _configuration.Image.Tag;
      }
    }

    /// <inheritdoc />
    public string Digest
    {
      get
      {
        ThrowIfResourceNotFound();
        return _configuration.Image.Digest;
      }
    }

    /// <inheritdoc />
    public string FullName
    {
      get
      {
        ThrowIfResourceNotFound();
        return _configuration.Image.FullName;
      }
    }

    /// <inheritdoc />
    public string GetHostname()
    {
      ThrowIfResourceNotFound();
      return _configuration.Image.GetHostname();
    }

    /// <inheritdoc />
    public bool MatchLatestOrNightly()
    {
      return _configuration.Image.MatchLatestOrNightly();
    }

    /// <inheritdoc />
    public bool MatchVersion(Predicate<string> predicate)
    {
      return _configuration.Image.MatchVersion(predicate);
    }

    /// <inheritdoc />
    public bool MatchVersion(Predicate<System.Version> predicate)
    {
      return _configuration.Image.MatchVersion(predicate);
    }

    /// <inheritdoc />
    public async Task CreateAsync(CancellationToken ct = default)
    {
      using var disposable = await AcquireLockAsync(ct)
        .ConfigureAwait(false);

      await UnsafeCreateAsync(ct)
        .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(CancellationToken ct = default)
    {
      using var disposable = await AcquireLockAsync(ct)
        .ConfigureAwait(false);

      await UnsafeDeleteAsync(ct)
        .ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override bool Exists()
    {
      return !string.IsNullOrEmpty(_image.ID);
    }

    /// <inheritdoc />
    protected override async Task UnsafeCreateAsync(CancellationToken ct = default)
    {
      ThrowIfLockNotAcquired();

      if (Exists())
      {
        return;
      }

      await _client.System.LogContainerRuntimeInfoAsync(ct)
        .ConfigureAwait(false);

      _ = await _client.BuildAsync(_configuration, ct)
        .ConfigureAwait(false);

      _image = await _client.Image.ByIdAsync(_configuration.Image.FullName, ct)
        .ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task UnsafeDeleteAsync(CancellationToken ct = default)
    {
      ThrowIfLockNotAcquired();

      if (!Exists())
      {
        return;
      }

      await _client.Image.DeleteAsync(_configuration.Image, ct)
        .ConfigureAwait(false);

      _image = new ImageInspectResponse();
    }
  }
}
