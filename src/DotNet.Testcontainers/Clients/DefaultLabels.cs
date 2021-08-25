namespace DotNet.Testcontainers.Clients
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;

  internal sealed class DefaultLabels : ReadOnlyDictionary<string, string>
  {
    private DefaultLabels()
      : base(new Dictionary<string, string>
      {
        { TestcontainersClient.TestcontainersLabel, bool.TrueString },
        { TestcontainersClient.TestcontainersCleanUpLabel, bool.TrueString },
      })
    {
    }

    public static IReadOnlyDictionary<string, string> Instance { get; }
      = new DefaultLabels();
  }
}
