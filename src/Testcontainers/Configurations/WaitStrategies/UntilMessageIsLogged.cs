namespace DotNet.Testcontainers.Configurations
{
  using System.Text.RegularExpressions;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;

  internal class UntilMessageIsLogged : IWaitUntil
  {
    private readonly Regex _pattern;

    public UntilMessageIsLogged(string pattern)
      : this(new Regex(pattern))
    {
    }

    public UntilMessageIsLogged(Regex pattern)
    {
      _pattern = pattern;
    }

    public async Task<bool> UntilAsync(IContainer container)
    {
      var maxTime = container.StoppedTime > container.CreatedTime ? container.StoppedTime : container.CreatedTime;

      var (stdout, stderr) = await container.GetLogsAsync(since: maxTime, timestampsEnabled: false)
        .ConfigureAwait(false);

      return _pattern.IsMatch(stdout) || _pattern.IsMatch(stderr);
    }
  }
}
