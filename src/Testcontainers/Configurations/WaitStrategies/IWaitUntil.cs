namespace DotNet.Testcontainers.Configurations
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  [PublicAPI]
  public interface IWaitUntil
  {
    Task<bool> UntilAsync(IContainer container);
  }
}
