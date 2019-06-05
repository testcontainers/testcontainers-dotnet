namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;

  internal sealed class MetaDataClientContainers : DockerMetaDataClient<ContainerListResponse>
  {
    private static readonly Lazy<DockerMetaDataClient<ContainerListResponse>> MetaDataClient = new Lazy<DockerMetaDataClient<ContainerListResponse>>(() => new MetaDataClientContainers());

    private MetaDataClientContainers()
    {
    }

    internal static DockerMetaDataClient<ContainerListResponse> Instance => MetaDataClient.Value;

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
  }
}
