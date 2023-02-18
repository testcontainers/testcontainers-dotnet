namespace Testcontainers.Azurite.Tests.Fixtures
{
  [UsedImplicitly]
  public sealed class AzuriteWithWorkspaceBindingFixture : AzuriteDefaultFixture, IDisposable
  {
    public AzuriteWithWorkspaceBindingFixture()
      : base(Setup)
    {
    }

    public void Dispose()
    {
      var workspaceFolder = this.Container.WorkspaceLocationBinding;
      if (workspaceFolder != null && Directory.Exists(workspaceFolder))
      {
        Directory.Delete(workspaceFolder, true);
      }
    }

    private static AzuriteBuilder Setup(AzuriteBuilder builder)
    {
      var workspaceLocation = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("D"));

      _ = Directory.CreateDirectory(workspaceLocation);

      return builder.WithBindMount(workspaceLocation, AzuriteBuilder.DefaultWorkspaceDirectoryPath);
    }
  }
}
