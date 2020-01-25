namespace DotNet.Testcontainers.Containers.WaitStrategies.Windows
{
  using System;
  using System.Threading.Tasks;

  internal class UntilCommandIsCompleted : Unix.UntilCommandIsCompleted
  {
    public UntilCommandIsCompleted(string command) : base(command)
    {
    }

    public override Task<bool> Until(Uri endpoint, string id)
    {
      throw new NotImplementedException();
    }
  }
}
