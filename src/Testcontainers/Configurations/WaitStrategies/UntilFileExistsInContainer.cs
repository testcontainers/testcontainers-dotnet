namespace DotNet.Testcontainers.Configurations
{
  using System.IO;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;

  internal class UntilFilesExistsInContainer : IWaitUntil
  {
    private readonly string _file;

    public UntilFilesExistsInContainer(string file)
    {
      _file = file;
    }

    public async Task<bool> UntilAsync(IContainer container)
    {
      try
      {
        await container.ReadFileAsync(_file);
        return true;
      }
      catch (FileNotFoundException)
      {
        return false;
      }
    }
  }
}
