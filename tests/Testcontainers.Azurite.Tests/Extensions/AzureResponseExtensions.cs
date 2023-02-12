namespace Testcontainers.Azurite.Tests;

internal static class AzureResponseExtensions
{
  public static bool IsError<T>(this Response<T> response)
  {
    using (var rawResponse = response.GetRawResponse())
    {
      return rawResponse.IsError;
    }
  }
}