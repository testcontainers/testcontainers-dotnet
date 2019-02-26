namespace DotNet.Testcontainers.Clients
{
  using System.Collections.Generic;
  using static LanguageExt.Prelude;

  internal abstract class DockerMetaDataClient<T> : DockerApiClient
  {
    internal abstract IReadOnlyCollection<T> GetAll();

    internal abstract T ById(string id);

    internal abstract T ByName(string name);

    internal abstract T ByProperty(string property, string value);

    internal bool ExistsWithId(string id)
    {
      return notnull(this.ById(id));
    }

    internal bool ExistsWithName(string name)
    {
      return notnull(this.ByName(name));
    }
  }
}
