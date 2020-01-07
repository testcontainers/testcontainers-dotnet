namespace DotNet.Testcontainers.Containers.WaitStrategies
{
  using System.IO;

  public static class Wait
  {
    public static IWaitUntil UntilContainerIsRunning()
    {
      return WaitUntilContainerIsRunning.WaitStrategy;
    }

    public static IWaitUntil UntilBashCommandsAreCompleted(params string[] commands)
    {
      return new WaitUntilShellCommandsAreCompleted(commands);
    }

    public static IWaitUntil UntilFilesExists(params string[] files)
    {
      return new WaitUntilFilesExists(files);
    }

    public static IWaitUntil UntilMessagesAreLogged(Stream outputConsumerStream, params string[] messages)
    {
      return new WaitUntilMessagesAreLogged(outputConsumerStream, messages);
    }

    public static IWaitUntil UntilPortsAreAvailable(params int[] ports)
    {
      return new WaitUntilPortsAreAvailable(ports);
    }
  }
}
