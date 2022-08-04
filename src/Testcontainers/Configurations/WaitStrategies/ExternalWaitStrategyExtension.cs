namespace DotNet.Testcontainers.Configurations
{
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  public static class ExternalWaitStrategyExtension
  {
    /// <summary>
    /// Waits until the http request is completed successfully.
    /// </summary>
    /// <param name="waitForContainerOs">A configured instance of <see cref="IWaitForContainerOS" />.</param>
    /// <param name="request">The http request to be executed.</param>
    ///<param name="frequency">The retry frequency in milliseconds.</param>
    /// <param name="timeout">The timeout in milliseconds.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    /// <remarks>Utilizes the HttpClient to send request from host. Expects either the response status code to match given status code(s) or a successful response.</remarks>
    [PublicAPI]
    public static IWaitForContainerOS UntilHttpRequestIsCompleted(this IWaitForContainerOS waitForContainerOs, HttpWaitRequest request, int frequency = 25, int timeout = -1)
    {
      waitForContainerOs.AddCustomWaitStrategy(new UntilHttpRequestIsCompleted(request, frequency, timeout));
      return waitForContainerOs;
    }
  }
}
