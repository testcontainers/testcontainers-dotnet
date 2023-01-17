namespace DotNet.Testcontainers.Images
{
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="IFutureDockerImage" />
  [PublicAPI]
  internal sealed class FutureDockerImage : Resource, IFutureDockerImage
  {
    private readonly IDockerImageOperations dockerImageOperations;

    private readonly IImageFromDockerfileConfiguration configuration;

    private ImagesListResponse image = new ImagesListResponse();

    /// <summary>
    /// Initializes a new instance of the <see cref="FutureDockerImage" /> class.
    /// </summary>
    /// <param name="configuration">The image configuration.</param>
    /// <param name="logger">The logger.</param>
    public FutureDockerImage(IImageFromDockerfileConfiguration configuration, ILogger logger)
    {
      this.dockerImageOperations = new DockerImageOperations(configuration.SessionId, configuration.DockerEndpointAuthConfig, logger);
      this.configuration = configuration;
    }

    /// <inheritdoc />
    public string Repository
    {
      get
      {
        this.ThrowIfResourceNotFound();
        return this.configuration.Image.Repository;
      }
    }

    /// <inheritdoc />
    public string Name
    {
      get
      {
        this.ThrowIfResourceNotFound();
        return this.configuration.Image.Name;
      }
    }

    /// <inheritdoc />
    public string Tag
    {
      get
      {
        this.ThrowIfResourceNotFound();
        return this.configuration.Image.Tag;
      }
    }

    /// <inheritdoc />
    public string FullName
    {
      get
      {
        this.ThrowIfResourceNotFound();
        return this.configuration.Image.FullName;
      }
    }

    /// <inheritdoc />
    public string GetHostname()
    {
      this.ThrowIfResourceNotFound();
      return this.configuration.Image.GetHostname();
    }

    /// <inheritdoc />
    public async Task CreateAsync(CancellationToken ct = default)
    {
      _ = await this.dockerImageOperations.BuildAsync(this.configuration, ct)
        .ConfigureAwait(false);

      this.image = await this.dockerImageOperations.ByNameAsync(this.configuration.Image.FullName, ct)
        .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(CancellationToken ct = default)
    {
      await this.dockerImageOperations.DeleteAsync(this.configuration.Image, ct)
        .ConfigureAwait(false);

      this.image = new ImagesListResponse();
    }

    /// <inheritdoc />
    protected override bool Exists()
    {
      return !string.IsNullOrEmpty(this.image.ID);
    }
  }
}
