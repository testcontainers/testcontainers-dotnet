namespace DotNet.Testcontainers.Core.Wait
{
  public static class Wait
  {
    public static IWaitUntil UntilContainerIsRunning()
    {
      return new WaitUntilContainerIsRunning();
    }

    public static IWaitUntil UntilFilesExists(params string[] files)
    {
      return new WaitUntilFilesExists(files);
    }

    public static IWaitUntil UntilPortsAreAvailable(params int[] ports)
    {
      return new WaitUntilPortsAreAvailable(ports);
    }
  }
}
