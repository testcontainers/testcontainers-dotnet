namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Net.Http;
  using System.Net.Http.Headers;
  using System.Text.RegularExpressions;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Extensions.Logging;

  public class UntilHttp : IWaitUntil
  {
    private readonly UntilHttpOptions Options;

    public UntilHttp(string path)
    {
      this.Options = new() { Path = path, Method = HttpMethod.Get };
    }

    public UntilHttp(UntilHttpOptions options)
    {
      this.Options = options;
    }

    public async Task<bool> Until(ITestcontainersContainer testcontainers, ILogger logger)
    {
      var mappedPort = testcontainers.GetMappedPublicPort(this.Options.Port);
      this.Options.Port = mappedPort;
      var client = new HttpClient();
      var message = new HttpRequestMessage(this.Options.Method, this.Options.Uri);
      if (this.Options.RequestContent is not null && (this.Options.Method == HttpMethod.Post || this.Options.Method == HttpMethod.Put))
      {
        message.Content = this.Options.RequestContent;
      }

      if (this.Options.UseAuth && this.Options.AuthString is not null)
      {
        message.Headers.Authorization = AuthenticationHeaderValue.Parse(this.Options.AuthString.ToString());
      }

      var sendTask = Task.Run(async () =>
      {
        HttpResponseMessage response = null;
        while (response is null || !this.Options.ExpectedResponseCodes.Contains(response.StatusCode))
        {
          response = await client.SendAsync(message);
          if (!this.Options.ExpectedResponseCodes.Contains(response.StatusCode))
          {
            Thread.Sleep(TimeSpan.FromSeconds(this.Options.RequestDelay));
          }
        }

        return response;
      });
      var completed = sendTask.Wait(this.Options.TimeOut);
      if (!completed)
      {
        return false;
      }

      var responseContent = await sendTask.Result.Content.ReadAsStringAsync();
      return !this.Options.ValidateContent || Regex.Match(this.Options.ExpectedOutput, responseContent).Success;
    }
  }
}
