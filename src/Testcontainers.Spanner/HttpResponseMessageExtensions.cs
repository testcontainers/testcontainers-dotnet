using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Testcontainers.Spanner;
internal static class HttpResponseMessageExtensions
{
  internal static async Task<HttpResponseMessage> ValidateAsync(this Task<HttpResponseMessage> response, string operation, CancellationToken ct = default)
  {
    ct.ThrowIfCancellationRequested();

    var result = await response;

    if (!result.IsSuccessStatusCode)
    {
      string resultContent = await result.Content.ReadAsStringAsync();
      string message =
        $"Failed to {operation} with status code {result.StatusCode} response: {resultContent}";
      throw new InvalidOperationException(message);
    }

    return result;
  }
}
