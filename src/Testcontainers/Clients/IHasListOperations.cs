namespace DotNet.Testcontainers.Clients
{
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;

  internal interface IHasListOperations<TListEntity, TInspectEntity>
  {
    Task<IEnumerable<TListEntity>> GetAllAsync(CancellationToken ct = default);

    Task<IEnumerable<TListEntity>> GetAllAsync(FilterByProperty filters, CancellationToken ct = default);

    Task<TInspectEntity> ByIdAsync(string id, CancellationToken ct = default);

    Task<TInspectEntity> ByNameAsync(string name, CancellationToken ct = default);

    Task<TInspectEntity> ByPropertyAsync(string property, string value, CancellationToken ct = default);

    Task<bool> ExistsWithIdAsync(string id, CancellationToken ct = default);

    Task<bool> ExistsWithNameAsync(string name, CancellationToken ct = default);
  }
}
