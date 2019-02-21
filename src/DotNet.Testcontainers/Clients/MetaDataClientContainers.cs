namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Docker.DotNet.Models;
  using static LanguageExt.Prelude;

  internal sealed class MetaDataClientContainers : DockerMetaDataClient<ContainerListResponse>
  {
    private static readonly Lazy<DockerMetaDataClient<ContainerListResponse>> MetaDataClient = new Lazy<DockerMetaDataClient<ContainerListResponse>>(() =>
    {
      return new MetaDataClientContainers();
    });

    private MetaDataClientContainers()
    {
    }

    internal static DockerMetaDataClient<ContainerListResponse> Instance
    {
      get
      {
        return MetaDataClient.Value;
      }
    }

    internal override ICollection<ContainerListResponse> All
    {
      get
      {
        return Docker.Containers.ListContainersAsync(new ContainersListParameters { }).Result.ToList();
      }
    }

    internal override ContainerListResponse ById(string id)
    {
      return notnull(id) ? this.All.FirstOrDefault(value => id.Equals(value.ID)) : null;
    }

    internal override ContainerListResponse ByName(string name)
    {
      return this.ByProperty("name", name);
    }

    internal override ContainerListResponse ByProperty(string property, string value)
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
