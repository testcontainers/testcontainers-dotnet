namespace DotNet.Testcontainers.Configurations
{
  using System.IO;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;

  internal class UntilFileExistsInContainer : IWaitUntil
  {
    private readonly string _file;

    public UntilFileExistsInContainer(string file)
    {
      _file = file;
    }

    public async Task<bool> UntilAsync(IContainer container)
    {
      try
      {
        _ = await container.ReadFileAsync(_file)
          .ConfigureAwait(false);

        return true;
      }
      catch (FileNotFoundException)
      {
        return false;
      }
    }
  }
}
