namespace DotNet.Testcontainers.Core.Wait
{
  using System.Threading.Tasks;

  public interface IWaitUntil
  {
    Task<bool> Until(string id);
  }
}
