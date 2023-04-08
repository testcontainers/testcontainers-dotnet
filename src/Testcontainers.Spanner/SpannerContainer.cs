using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Testcontainers.Spanner.RestApi.v1;

namespace Testcontainers.Spanner;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class SpannerContainer : DockerContainer, IDisposable
{
  private const string EnvironmentVariableEmulatorHost = "SPANNER_EMULATOR_HOST";
  private readonly SpannerConfiguration _configuration;
  private readonly HttpClient _webClient = new();

  public int GrpcPort => GetMappedPublicPort(SpannerBuilder.InternalGrpcPort);
  public int RestPort => GetMappedPublicPort(SpannerBuilder.InternalRestPort);

  public string ConnectionString
  {
    get
    {
      return new StringBuilder("Data Source=projects/")
      .Append(_configuration.ProjectId)
      .Append("/instances/")
      .Append(_configuration.InstanceId)
      .Append("/databases/")
      .Append(_configuration.DatabaseId)
      .Append("; EmulatorDetection=EmulatorOnly; Host=")
      .Append(Hostname)
      .Append("; Port=")
      .Append(GrpcPort)
      .ToString();
    }
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="SpannerContainer" /> class.
  /// </summary>
  /// <param name="configuration">The container configuration.</param>
  /// <param name="logger">The logger.</param>
  public SpannerContainer(SpannerConfiguration configuration, ILogger logger)
        : base(configuration, logger)
  {
    _configuration = configuration;
  }

  public override async Task StartAsync(CancellationToken ct = default)
  {
    await base.StartAsync(ct);

    Environment.SetEnvironmentVariable(EnvironmentVariableEmulatorHost, $"{Hostname}:{GrpcPort}");
#pragma warning disable S5332
    // warning S5332: Using http protocol is insecure. Use https instead.
    // This doesn't apply here, as it is a testing setup only localhost connection encryption is not required.
    _webClient.BaseAddress = new Uri($"http://{Hostname}:{RestPort}");
#pragma warning restore S5332

    await CreateSpannerInstance(ct);
    await CreateDatabase(ct);
  }

  private async Task CreateSpannerInstance(CancellationToken ct = default)
  {
    var resource = new
    {
      instanceId = _configuration.InstanceId,
      instance = new
      {
        config = "emulator-config",
        displayName = _configuration.InstanceId,
      }
    };
    string requestUri = $"v1/projects/{_configuration.ProjectId}/instances";
    string resourceDescription = "instance";

    await CreateResource(requestUri, resource, resourceDescription, ct);
  }

  private async Task CreateDatabase(CancellationToken ct = default)
  {
    var resource = new
    {
      createStatement = $"CREATE DATABASE `{_configuration.DatabaseId}`",
    };


    string requestUri = $"v1/projects/{_configuration.ProjectId}/instances/{_configuration.InstanceId}/databases";
    string resourceDescription = "database";

    await CreateResource(requestUri, resource, resourceDescription, ct);
  }

  public async Task ExecuteDdlAsync(params string[] ddl)
  {
    string requestUri = $"v1/projects/{_configuration.ProjectId}/instances/{_configuration.InstanceId}/databases/{_configuration.DatabaseId}/ddl";
    var request = new { statements = ddl };

    var response = await _webClient.PatchAsJsonAsync(requestUri, request)
      .ValidateAsync("update ddl");

    var operation = await response.Content.ReadFromJsonAsync<SpannerOperationDto>() ??
                    throw new InvalidOperationException(
                      "Executing ddl resulted in a response with a body deserializing as SpannerOperation to null");

    await AwaitOperationAsync(operation);

    if (operation.Error != null)
    {
      throw new InvalidOperationException($"Failed to execute ddl operation with error {operation.Error}");
    }
  }

  private async Task AwaitOperationAsync(SpannerOperationDto operation)
  {
    int delayInSeconds = 1;
    const int maxDelayInSeconds = 10;

    while (!operation.Done)
    {
      await Task.Delay(TimeSpan.FromSeconds(delayInSeconds));
      string operationName = operation.Name;
      var operationResponse = await _webClient.GetAsync($"v1/{operationName}").ValidateAsync("get operation");

      operation = await operationResponse.Content.ReadFromJsonAsync<SpannerOperationDto>() ??
                  throw new InvalidOperationException(
                    $"Getting operation resulted in null deserialization of body for operation name {operationName}");
      if (delayInSeconds * 2 < maxDelayInSeconds)
      {
        delayInSeconds *= 2;
      }
    }
  }


  private async Task CreateResource(string requestUri, object resource, string resourceDescription, CancellationToken ct = default)
  {
    await _webClient.PostAsJsonAsync(requestUri, resource, cancellationToken: ct)
      .ValidateAsync($"create {resourceDescription}", ct: ct);
  }

  public void Dispose() => _webClient.Dispose();

  protected override async ValueTask DisposeAsyncCore()
  {
    _webClient.Dispose();
    await base.DisposeAsyncCore();
  }
}
