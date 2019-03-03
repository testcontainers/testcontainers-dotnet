namespace DotNet.Testcontainers.Core
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;

  public sealed class DefaultWaitStrategy : WaitStrategy
  {
    private string id;

    public WaitStrategy ForContainer(string id)
    {
      this.id = id;
      return this;
    }

    protected override async Task<bool> While()
    {
      return await Task.Run(() => true);
    }

    protected override async Task<bool> Until()
    {
      var container = await MetaDataClientContainers.Instance.ByIdAsync(this.id);
      return !container?.Status.Equals("Created") ?? false;
    }
  }
}
