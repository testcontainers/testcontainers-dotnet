namespace DotNet.Testcontainers.Networks
{
  using System.Threading;
  using System.Threading.Tasks;

  /// <inheritdoc cref="IDockerNetwork" />
  public sealed class DockerNetwork : IDockerNetwork
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerNetwork" /> class.
    /// </summary>
    /// <param name="id">The Docker network id.</param>
    /// <param name="name">The Docker network name.</param>
    public DockerNetwork(
      string id,
      string name)
    {
      Guard.Argument(id, nameof(id))
        .NotNull()
        .NotEmpty();

      Guard.Argument(name, nameof(name))
        .NotNull()
        .NotEmpty();

      this.Id = id;
      this.Name = name;
    }

    /// <inheritdoc />
    public string Id { get; }

    /// <inheritdoc />
    public string Name { get; }

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
