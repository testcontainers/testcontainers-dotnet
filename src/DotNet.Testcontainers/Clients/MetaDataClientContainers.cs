namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Docker.DotNet.Models;

  internal sealed class MetaDataClientContainers : DockerMetaDataClient<ContainerListResponse>
  {
    private static readonly Lazy<DockerMetaDataClient<ContainerListResponse>> MetaDataClient = new Lazy<DockerMetaDataClient<ContainerListResponse>>(() =>
    {
      return new MetaDataClientContainers();
    });

    private MetaDataClientContainers()
    {
    }

    public static DockerMetaDataClient<ContainerListResponse> Instance
    {
      get
      {
        return MetaDataClient.Value;
      }
    }

    public override ICollection<ContainerListResponse> All
    {
      get
      {
        return Docker.Containers.ListContainersAsync(new ContainersListParameters { }).Result.ToList();
      }
    }

    public override ContainerListResponse ById(string id)
    {
      return this.ByProperty("id", id);
    }

    public override ContainerListResponse ByName(string name)
    {
      return this.ByProperty("name", name);
    }

    protected override ContainerListResponse ByProperty(string property, string value)
    {
      return Docker.Containers.ListContainersAsync(new ContainersListParameters
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
      }).Result.FirstOrDefault();
    }
  }
}
