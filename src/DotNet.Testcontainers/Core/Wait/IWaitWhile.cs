namespace DotNet.Testcontainers.Core.Wait
{
  using System.Threading.Tasks;

  public interface IWaitWhile
  {
    Task<bool> While(string id);
  }
}
