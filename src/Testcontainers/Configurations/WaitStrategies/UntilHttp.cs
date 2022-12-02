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
    private int RetryCount = 0;

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

      try
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
            if (++this.RetryCount > this.Options.MaxRetries)
            {
              throw new TimeoutException($"Http Wait Failed {this.Options.MaxRetries} Times");
            }

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
          throw new TimeoutException($"Http Wait Failed Timed Out after {this.Options.TimeOut}");
        }

        var responseContent = await sendTask.Result.Content.ReadAsStringAsync();
        return !this.Options.ValidateContent || Regex.Match(this.Options.ExpectedOutput, responseContent).Success;
      }
      catch (Exception)
      {
        if (++this.RetryCount > this.Options.MaxRetries)
        {
          throw new TimeoutException($"Http Wait Failed {this.Options.MaxRetries} Times");
        }

        return false;
      }
    }
  }
}
