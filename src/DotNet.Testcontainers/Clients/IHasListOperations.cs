namespace DotNet.Testcontainers.Clients
{
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;

  internal interface IHasListOperations<T>
  {
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);

    Task<T> ByIdAsync(string id, CancellationToken ct = default);

    Task<T> ByNameAsync(string name, CancellationToken ct = default);

    Task<T> ByPropertyAsync(string property, string value, CancellationToken ct = default);

    Task<bool> ExistsWithIdAsync(string id, CancellationToken ct = default);

    Task<bool> ExistsWithNameAsync(string name, CancellationToken ct = default);
  }
}
