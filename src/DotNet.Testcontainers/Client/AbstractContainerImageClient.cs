namespace DotNet.Testcontainers.Client
{
  using System.Collections.Generic;
  using System.Threading.Tasks;

  internal abstract class AbstractContainerImageClient<T> : DockerApiClient
    where T : class
  {
    internal abstract Task<IReadOnlyCollection<T>> GetAllAsync();

    internal abstract Task<T> ByIdAsync(string id);

    internal abstract Task<T> ByNameAsync(string name);

    internal abstract Task<T> ByPropertyAsync(string property, string value);

    internal async Task<bool> ExistsWithIdAsync(string id)
    {
      return !(await this.ByIdAsync(id) is null);
    }

    internal async Task<bool> ExistsWithNameAsync(string name)
    {
      return !(await this.ByNameAsync(name) is null);
    }
  }
}
