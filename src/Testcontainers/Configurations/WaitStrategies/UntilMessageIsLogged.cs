namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Text.RegularExpressions;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;

  internal class UntilMessageIsLogged : IWaitUntil
  {
    private readonly Regex _pattern;

    public UntilMessageIsLogged(string pattern)
      : this(new Regex(pattern, RegexOptions.None, TimeSpan.FromSeconds(5)))
    {
    }

    public UntilMessageIsLogged(Regex pattern)
    {
      _pattern = pattern;
    }

    public async Task<bool> UntilAsync(IContainer container)
    {
      var (stdout, stderr) = await container.GetLogsAsync(since: container.StoppedTime, timestampsEnabled: false)
        .ConfigureAwait(false);

      return _pattern.IsMatch(stdout) || _pattern.IsMatch(stderr);
    }
  }
}
