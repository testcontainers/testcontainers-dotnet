namespace DotNet.Testcontainers.Containers.Configurations
{
  using System;
  using DotNet.Testcontainers.Clients;

  /// <inheritdoc cref="IDockerClientAuthenticationConfiguration" />
  internal sealed class DockerClientAuthenticationConfiguration : IDockerClientAuthenticationConfiguration
  {
    // TODO: Improve null check; Add other envs variables too (maybe in a different way dunno yet).
    // Do not forget content of DockerClientAuthConfig.
    private static readonly Lazy<Uri> todo = new Lazy<Uri>(() =>
    {
      try
      {
        return new Uri(Environment.GetEnvironmentVariable("DOCKER_HOST"));
      }
      catch (Exception)
      {
        return DockerApiEndpoint.Local;
      }
    });

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerClientAuthenticationConfiguration" /> class.
    /// </summary>
    public DockerClientAuthenticationConfiguration()
      : this(todo.Value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerClientAuthenticationConfiguration" /> class.
    /// </summary>
    /// <param name="clientEndpoint">The Docker client endpoint.</param>
    public DockerClientAuthenticationConfiguration(
      Uri clientEndpoint)
    {
      this.Endpoint = clientEndpoint;
    }

    /// <inheritdoc />
    public Uri Endpoint { get; }
  }
}
