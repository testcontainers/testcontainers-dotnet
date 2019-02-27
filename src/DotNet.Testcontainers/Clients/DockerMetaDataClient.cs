namespace DotNet.Testcontainers.Clients
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using static LanguageExt.Prelude;

  internal abstract class DockerMetaDataClient<T> : DockerApiClient
  {
    internal abstract Task<IReadOnlyCollection<T>> GetAllAsync();

    internal abstract Task<T> ByIdAsync(string id);

    internal abstract Task<T> ByNameAsync(string name);

    internal abstract Task<T> ByPropertyAsync(string property, string value);

    internal async Task<bool> ExistsWithIdAsync(string id)
    {
      return notnull(await this.ByIdAsync(id));
    }

    internal async Task<bool> ExistsWithNameAsync(string name)
    {
      return notnull(await this.ByNameAsync(name));
    }
  }
}
