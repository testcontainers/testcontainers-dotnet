namespace DotNet.Testcontainers.Containers.WaitStrategies
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Text.RegularExpressions;
  using System.Threading.Tasks;

  internal class WaitUntilMessagesAreLogged : IWaitUntil
  {
    private readonly Stream outputConsumerStream;

    private readonly string[] messages;

    public WaitUntilMessagesAreLogged(Stream outputConsumerStream, params string[] messages)
    {
      this.outputConsumerStream = outputConsumerStream;
      this.messages = messages;
    }

    public async Task<bool> Until(Uri endpoint, string id)
    {
      await WaitStrategy.WaitUntil(() => WaitUntilContainerIsRunning.WaitStrategy.Until(endpoint, id));

      this.outputConsumerStream.Seek(0, SeekOrigin.Begin);

      using (var streamReader = new StreamReader(this.outputConsumerStream, Encoding.UTF8, false, 4096, true))
      {
        var output = streamReader.ReadToEnd();
        return this.messages.All(message => Regex.IsMatch(output, message));
      }
    }
  }
}
