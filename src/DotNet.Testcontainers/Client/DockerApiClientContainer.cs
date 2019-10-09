namespace DotNet.Testcontainers.Client
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;

  /// <summary>
  /// This class represents a Docker API client and interacts with Docker containers.
  /// </summary>
  internal sealed class DockerApiClientContainer : AbstractContainerImageClient<ContainerListResponse>
  {
    private static readonly Lazy<DockerApiClientContainer> MetaDataClientLazy = new Lazy<DockerApiClientContainer>(() => new DockerApiClientContainer());

    private DockerApiClientContainer()
    {
    }

    internal static DockerApiClientContainer Instance { get; } = MetaDataClientLazy.Value;

    internal override async Task<IReadOnlyCollection<ContainerListResponse>> GetAllAsync()
    {
      return (await Docker.Containers.ListContainersAsync(new ContainersListParameters { All = true })).ToList();
    }

    internal override async Task<ContainerListResponse> ByIdAsync(string id)
    {
      return await this.ByPropertyAsync("id", id);
    }

    internal override async Task<ContainerListResponse> ByNameAsync(string name)
    {
      return await this.ByPropertyAsync("name", name);
    }

    internal override async Task<ContainerListResponse> ByPropertyAsync(string property, string value)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        return null;
      }

      var response = Docker.Containers.ListContainersAsync(new ContainersListParameters
      {
        All = true,
        Filters = new Dictionary<string, IDictionary<string, bool>>
        {
          {
            property, new Dictionary<string, bool>
            {
              { value, true },
            }
          },
        },
      });

      return (await response).FirstOrDefault();
    }

    internal async Task<long> GetExitCode(string id)
    {
      return (await Docker.Containers.WaitContainerAsync(id)).StatusCode;
    }
  }
}
