namespace DotNet.Testcontainers.Clients
{
  using System.Collections.Generic;

  internal abstract class DockerMetaDataClient<T> : DockerApiClient
  {
    public abstract ICollection<T> All { get; }

    public abstract T ById(string id);

    public abstract T ByName(string name);

    protected abstract T ByProperty(string property, string value);
  }
}
