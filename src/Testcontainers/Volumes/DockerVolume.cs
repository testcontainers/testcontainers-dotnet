namespace DotNet.Testcontainers.Volumes
{
  using System.Threading;
  using System.Threading.Tasks;

  /// <inheritdoc cref="IDockerVolume" />
  internal sealed class DockerVolume : IDockerVolume
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerVolume" /> class.
    /// </summary>
    /// <param name="name">The Docker volume name.</param>
    public DockerVolume(
      string name)
    {
      Guard.Argument(name, nameof(name))
        .NotNull()
        .NotEmpty();

      this.Name = name;
    }

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
