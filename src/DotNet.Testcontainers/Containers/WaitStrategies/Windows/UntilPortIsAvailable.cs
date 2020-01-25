namespace DotNet.Testcontainers.Containers.WaitStrategies.Windows
{
  using System;
  using System.Threading.Tasks;

  internal class UntilPortIsAvailable : Unix.UntilPortIsAvailable
  {
    public UntilPortIsAvailable(int port) : base(port)
    {
    }

    public override Task<bool> Until(Uri endpoint, string id)
    {
      throw new NotImplementedException();
    }
  }
}
