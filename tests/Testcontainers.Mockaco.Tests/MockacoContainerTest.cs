using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using DotNet.Testcontainers.Commons;

namespace Testcontainers.Mockaco.Tests;

public sealed class MockacoContainerTest : IAsyncLifetime
{
  private static readonly string TemplatesPath = TestSession.TempDirectoryPath;
  private readonly MockacoContainer _mockacoContainer;

  public MockacoContainerTest()
  {
    var pingTemplate = new
    {
      request = new
      {
        method = "GET",
        route = "ping",
      },
      response = new
      {
        status = "OK",
        body = new
        {
          response = "pong",
        },
      },
    };

    var createUserTemplate = new
    {
      request = new
      {
        method = "POST",
        route = "users",
      },
      response = new
      {
        status = "Created",
        headers = new Dictionary<string, string>
        {
          ["Content-Type"] = "application/json",
          ["Location"] = "/users/123",
        },
        body = new
        {
          id = 123,
          message = "User created successfully",
        },
      },
    };

    var notFoundTemplate = new
    {
      request = new
      {
        method = "GET",
        route = "notfound",
      },
      response = new
      {
        status = "NotFound",
        body = new
        {
          error = "Resource not found",
        },
      },
    };

    var serverErrorTemplate = new
    {
      request = new
      {
        method = "GET",
        route = "error",
      },
      response = new
      {
        status = "InternalServerError",
        body = new
        {
          error = "Internal server error",
        },
      },
    };

    var templates = new (object template, string fileName)[]
    {
      (pingTemplate, "ping.json"),
      (createUserTemplate, "create-user.json"),
      (notFoundTemplate, "not-found.json"),
      (serverErrorTemplate, "server-error.json"),
    };

    foreach (var (template, fileName) in templates)
    {
      var templateFilePath = Path.Combine(TemplatesPath, fileName);
      var templateJson = JsonSerializer.Serialize(template);
      File.WriteAllText(templateFilePath, templateJson);
    }

    _mockacoContainer = new MockacoBuilder()
      .WithTemplatesPath(TemplatesPath)
      .Build();
  }

  public async ValueTask InitializeAsync()
  {
    await _mockacoContainer.StartAsync();
  }

  public ValueTask DisposeAsync()
  {
    return _mockacoContainer.DisposeAsync();
  }

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public async Task GetStatusReturnsHttpStatusCodeOk()
  {
    // Given
    using var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri(_mockacoContainer.GetEndpoint());
    using var request = new HttpRequestMessage(HttpMethod.Get, "ping");

    // When
    using var response = await httpClient.SendAsync(request, TestContext.Current.CancellationToken);

    // Then
    var responseContent = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    var responseJson = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
    Assert.Equal("pong", responseJson["response"].ToString());
  }

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public async Task PostRequest_ReturnsCreatedStatus()
  {
    // Given
    using var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri(_mockacoContainer.GetEndpoint());

    var requestBody = JsonSerializer.Serialize(new { name = "John Doe", email = "john@example.com" });
    using var request = new HttpRequestMessage(HttpMethod.Post, "users");
    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

    // When
    using var response = await httpClient.SendAsync(request, TestContext.Current.CancellationToken);

    // Then
    var responseContent = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    Assert.Equal("/users/123", response.Headers.Location?.ToString());

    var responseJson = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
    Assert.Equal("123", responseJson["id"].ToString());
    Assert.Equal("User created successfully", responseJson["message"].ToString());
  }

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public async Task NotFoundRequest_Returns404()
  {
    // Given
    using var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri(_mockacoContainer.GetEndpoint());
    using var request = new HttpRequestMessage(HttpMethod.Get, "notfound");

    // When
    using var response = await httpClient.SendAsync(request, TestContext.Current.CancellationToken);

    // Then
    var responseContent = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    var responseJson = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
    Assert.Equal("Resource not found", responseJson["error"].ToString());
  }

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public async Task ServerErrorRequest_Returns500()
  {
    // Given
    using var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri(_mockacoContainer.GetEndpoint());
    using var request = new HttpRequestMessage(HttpMethod.Get, "error");

    // When
    using var response = await httpClient.SendAsync(request, TestContext.Current.CancellationToken);

    // Then
    var responseContent = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    var responseJson = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
    Assert.Equal("Internal server error", responseJson["error"].ToString());
  }

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public async Task UnmatchedRequest_ReturnsNotImplemented()
  {
    // Given
    using var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri(_mockacoContainer.GetEndpoint());
    using var request = new HttpRequestMessage(HttpMethod.Get, "nonexistent-endpoint");

    // When
    using var response = await httpClient.SendAsync(request, TestContext.Current.CancellationToken);

    // Then
    var responseContent = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

    Assert.Equal(HttpStatusCode.NotImplemented, response.StatusCode);
    Assert.Contains("Incoming request didn't match any mock", responseContent);
  }

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public async Task HealthEndpoint_ReturnsHealthy()
  {
    // Given
    using var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri(_mockacoContainer.GetEndpoint());
    using var request = new HttpRequestMessage(HttpMethod.Get, "_mockaco/ready");

    // When
    using var response = await httpClient.SendAsync(request, TestContext.Current.CancellationToken);

    // Then
    Assert.True(response.IsSuccessStatusCode, "Health endpoint should return success");

    var responseContent = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
    Assert.Equal("Healthy", responseContent);
  }

  [Fact]
  public void GetEndpoint_ReturnsValidUri()
  {
    // Given & When
    var endpoint = _mockacoContainer.GetEndpoint();

    // Then
    Assert.True(Uri.TryCreate(endpoint, UriKind.Absolute, out var uri), "Endpoint should be a valid URI");
    Assert.Equal("http", uri.Scheme);
    Assert.True(uri.Port > 0, "Port should be greater than 0");
  }

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public async Task GetVerifyAsync_AfterRequest_ReturnsVerificationData()
  {
    // Given
    using var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri(_mockacoContainer.GetEndpoint());
    
    var requestBody = JsonSerializer.Serialize(new { name = "John Doe", email = "john@example.com" });
    using var request = new HttpRequestMessage(HttpMethod.Post, "users");
    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

    // When - Make a request first
    await httpClient.SendAsync(request, TestContext.Current.CancellationToken);
    
    // Then - Verify the request was recorded (try different route formats)
    var verification = await _mockacoContainer.GetVerifyAsync("/users");
    
    Assert.NotNull(verification);
    Assert.Equal("/users", verification.Route);
    Assert.False(string.IsNullOrEmpty(verification.Timestamp));
    Assert.Contains("John Doe", verification.Body);
  }

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public async Task GetVerifyAsync_RouteNotCalled_ReturnsNull()
  {
    // Given & When
    var verification = await _mockacoContainer.GetVerifyAsync("never-called-route");
    
    // Then
    Assert.Null(verification);
  }

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public async Task GetVerifyAsync_GetBodyAs_DeserializesCorrectly()
  {
    // Given
    using var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri(_mockacoContainer.GetEndpoint());
    
    var requestData = new { name = "Jane Doe", email = "jane@example.com", age = 30 };
    var requestBody = JsonSerializer.Serialize(requestData);
    using var request = new HttpRequestMessage(HttpMethod.Post, "users");
    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

    // When
    await httpClient.SendAsync(request, TestContext.Current.CancellationToken);
    var verification = await _mockacoContainer.GetVerifyAsync("/users");
    
    // Then
    Assert.NotNull(verification);
    var deserializedBody = verification.GetBodyAs<Dictionary<string, object>>();
    Assert.Equal("Jane Doe", deserializedBody["name"].ToString());
    Assert.Equal("jane@example.com", deserializedBody["email"].ToString());
  }

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public async Task GetVerifyAsync_GetBodyAsJson_ParsesCorrectly()
  {
    // Given
    using var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri(_mockacoContainer.GetEndpoint());
    
    var requestBody = JsonSerializer.Serialize(new { id = 123, active = true });
    using var request = new HttpRequestMessage(HttpMethod.Post, "users");
    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

    // When
    await httpClient.SendAsync(request, TestContext.Current.CancellationToken);
    var verification = await _mockacoContainer.GetVerifyAsync("/users");
    
    // Then
    Assert.NotNull(verification);
    using var jsonDoc = verification.GetBodyAsJson();
    var root = jsonDoc.RootElement;
    Assert.Equal(123, root.GetProperty("id").GetInt32());
    Assert.True(root.GetProperty("active").GetBoolean());
  }
}
