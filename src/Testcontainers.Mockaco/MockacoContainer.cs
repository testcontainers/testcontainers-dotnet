using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Testcontainers.Mockaco;

[PublicAPI]
public class MockacoContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MockacoContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public MockacoContainer(MockacoConfiguration configuration) : base(configuration)
    {
    }

    /// <summary>
    /// Gets the Mockaco base address.
    /// </summary>
    /// <returns>The Mockaco base address.</returns>
    public string GetBaseAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(MockacoBuilder.MockacoPort)).ToString();
    }

    /// <summary>
    /// Gets the verification data for a specific route.
    /// </summary>
    /// <param name="route">The route to verify.</param>
    /// <returns>The verification response, or null if not found.</returns>
    [ItemCanBeNull]
    public async Task<MockacoVerificationResponse> GetVerifyAsync(string route)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(GetBaseAddress());

        try
        {
            return await httpClient.GetFromJsonAsync<MockacoVerificationResponse>(
                $"/_mockaco/verification?route={Uri.EscapeDataString(route)}");
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Represents a Mockaco verification response.
    /// </summary>
    public sealed class MockacoVerificationResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockacoVerificationResponse" /> class.
        /// </summary>
        /// <param name="route">The verified route.</param>
        /// <param name="timestamp">When the route was called.</param>
        /// <param name="body">The request body content.</param>
        /// <param name="headers">The request headers (optional).</param>
        public MockacoVerificationResponse(string route, string timestamp, string body, MockacoHeader[] headers = null)
        {
            Route = route;
            Timestamp = timestamp;
            Body = body;
            Headers = headers ?? new MockacoHeader[0];
        }

        /// <summary>
        /// Gets the verified route.
        /// </summary>
        public string Route { get; }

        /// <summary>
        /// Gets when the route was called.
        /// </summary>
        public string Timestamp { get; }

        /// <summary>
        /// Gets the request body content.
        /// </summary>
        public string Body { get; }

        /// <summary>
        /// Gets the request headers.
        /// </summary>
        public MockacoHeader[] Headers { get; }

        /// <summary>
        /// Deserializes the body to the specified type.
        /// </summary>
        public T GetBodyAs<T>() => JsonSerializer.Deserialize<T>(Body);

        /// <summary>
        /// Parses the body as JSON.
        /// </summary>
        public JsonDocument GetBodyAsJson() => JsonDocument.Parse(Body);
    }

    /// <summary>
    /// Represents a header in the Mockaco verification response.
    /// </summary>
    public sealed class MockacoHeader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockacoHeader" /> class.
        /// </summary>
        /// <param name="key">The header name.</param>
        /// <param name="value">The header value.</param>
        public MockacoHeader(string key, string value)
        {
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Gets the header name.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the header value.
        /// </summary>
        public string Value { get; }
    }
}