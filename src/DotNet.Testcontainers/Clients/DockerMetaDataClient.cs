namespace DotNet.Testcontainers.Clients
{
  using System.Collections.Generic;

  internal abstract class DockerMetaDataClient<T> : DockerApiClient
  {
    internal abstract ICollection<T> All { get; }

    internal abstract T ById(string id);

    internal abstract T ByName(string name);

    internal abstract T ByProperty(string property, string value);
  }
}
