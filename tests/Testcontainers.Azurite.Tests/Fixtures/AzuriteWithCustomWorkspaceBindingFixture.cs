namespace Testcontainers.Azurite.Tests.Fixtures
{
  [UsedImplicitly]
  public sealed class AzuriteWithWorkspaceBindingFixture : AzuriteDefaultFixture, IDisposable
  {
    public AzuriteWithWorkspaceBindingFixture()
      : base(builder => builder.WithBindMount(GetFolderName, AzuriteBuilder.DefaultWorkspaceDirectoryPath))
    {
      var workspaceFolder = this.Container.WorkspaceLocationBinding;
      if (workspaceFolder != null)
      {
        _ = Directory.CreateDirectory(workspaceFolder);
      }
    }

    private static string GetFolderName
    {
      get
      {
        return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("D"));
      }
    }

    public void Dispose()
    {
      var workspaceFolder = this.Container.WorkspaceLocationBinding;
      if (workspaceFolder != null && Directory.Exists(workspaceFolder))
      {
        Directory.Delete(workspaceFolder, true);
      }
    }
  }
}
