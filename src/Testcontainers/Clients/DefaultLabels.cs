namespace DotNet.Testcontainers.Clients
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Reflection;
  using DotNet.Testcontainers.Containers;

  internal sealed class DefaultLabels : ReadOnlyDictionary<string, string>
  {
    static DefaultLabels()
    {
    }

    private DefaultLabels()
      : base(new Dictionary<string, string>
      {
        { TestcontainersClient.TestcontainersLabel, bool.TrueString.ToLowerInvariant() },
        { TestcontainersClient.TestcontainersLangLabel, "dotnet" },
        { TestcontainersClient.TestcontainersVersionLabel, typeof(DefaultLabels).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion },
        { ResourceReaper.ResourceReaperSessionLabel, ResourceReaper.DefaultSessionId.ToString("D") },
      })
    {
    }

    public static IReadOnlyDictionary<string, string> Instance { get; }
      = new DefaultLabels();
  }
}
