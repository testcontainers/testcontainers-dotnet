namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Text.RegularExpressions;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;

  internal class UntilMessageIsLogged : IWaitUntil
  {
    private readonly Regex pattern;

    public UntilMessageIsLogged(string pattern)
      : this(new Regex(pattern, RegexOptions.None, TimeSpan.FromSeconds(5)))
    {
    }

    public UntilMessageIsLogged(Regex pattern)
    {
      this.pattern = pattern;
    }

    public async Task<bool> UntilAsync(IContainer container)
    {
      var (stdout, stderr) = await container.GetLogsAsync(timestampsEnabled: false)
        .ConfigureAwait(false);

      return this.pattern.IsMatch(stdout) || this.pattern.IsMatch(stderr);
    }
  }
}
