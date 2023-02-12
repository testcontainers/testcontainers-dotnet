namespace Testcontainers.Azurite.Tests.Fixtures;

[UsedImplicitly]
public sealed class AzuriteWithCustomWorkspaceBindingFixture : AzuriteDefaultFixture, IDisposable
{
  public AzuriteWithCustomWorkspaceBindingFixture()
    : base(builder => builder.WithWorkspaceLocationBinding(GetFolderName))
  {
    if (Configuration.WorkspaceLocationBinding != null)
    {
      Directory.CreateDirectory(Configuration.WorkspaceLocationBinding);
    }
  }

  private static string GetFolderName => Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("D"));

  public void Dispose()
  {
    if (Directory.Exists(Configuration.WorkspaceLocationBinding))
    {
      Directory.Delete(Configuration.WorkspaceLocationBinding, true);
    }
  }
}