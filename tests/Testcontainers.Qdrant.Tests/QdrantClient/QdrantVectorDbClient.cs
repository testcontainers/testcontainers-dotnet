using OneOf;

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Testcontainers.Qdrant.Tests.QdrantClient;

public class QdrantVectorDbClient : IQdrantVectorDbClient
{
  private static readonly HttpClient httpClient = new HttpClient();

  private readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
  {
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
  };

  public QdrantVectorDbClient(string url)
  {
    httpClient.BaseAddress = new Uri(url);
  }

  private void PrepareClient()
  {
    httpClient.DefaultRequestHeaders.Clear();
  }

  private ErrorResponse HandleError(Exception ex, string subUri)
  {
    return new ErrorResponse($"Operation Failed for {subUri} {ex.Message}");
  }

  private async Task<ErrorResponse> HandleError(HttpResponseMessage response, CancellationToken cancellationToken, Exception ex = null)
  {
    if (ex is not null)
    {
      return new ErrorResponse("Fatal Error Calling Qdrant.\n" + ex.Message);
    }
    if (response is not null && response.StatusCode == System.Net.HttpStatusCode.BadRequest)
    {
      var result = await response.Content.ReadFromJsonAsync<HttpStatusResponse>(cancellationToken: cancellationToken);
      if (result is null)
      {
        return new ErrorResponse($"Operation failed.");
      }

      if (result.Status is not null)
      {
        return new ErrorResponse($"Operation failed with status: {result.Status.Error}.");
      }
    }
    return new ErrorResponse("Fatal Error Calling Qdrant");
  }

  private OneOf<TR, ErrorResponse> VerifyResult<TR>(QdrantHttpResponse<TR> response)
  {
    if (response is null)
    {
      return new ErrorResponse($"Operation failed.");
    }

    if (response.Status is not OperationStatus.Succeeded)
    {
      return new ErrorResponse($"Operation failed with status {response.Status}.");
    }
    return response!.Result;
  }

  private async Task<OneOf<TR, ErrorResponse>> VerifyResult<TR>(HttpResponseMessage response, CancellationToken cancellationToken)
  {
    if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
    {
      var result = await response.Content.ReadFromJsonAsync<QdrantHttpResponse<TR>>(cancellationToken: cancellationToken);
      if (result is null)
      {
        return await HandleError(response, cancellationToken);
      }
      if (result.Status is not OperationStatus.Succeeded)
      {
        return await HandleError(response, cancellationToken);
      }
      return result!.Result;
    }
    return await HandleError(response, cancellationToken);
  }

  private async Task<OneOf<TR, ErrorResponse>> PutAsync<T, TR>(string subUri, T payload, CancellationToken cancellationToken)
  {
    HttpResponseMessage response = null;
    try
    {
      PrepareClient();
      response = await httpClient.PutAsJsonAsync(subUri, payload, serializerOptions, cancellationToken);
      return await VerifyResult<TR>(response, cancellationToken);
    }
    catch (Exception ex)
    {
      return await HandleError(response, cancellationToken, ex);
    }
  }

  private async Task<OneOf<TR, ErrorResponse>> GetAsync<TR>(string subUri, CancellationToken cancellationToken)
  {
    try
    {
      PrepareClient();
      var response = await httpClient.GetFromJsonAsync<QdrantHttpResponse<TR>>(subUri, cancellationToken)!;
      return VerifyResult(response);
    }
    catch (Exception ex)
    {
      return HandleError(ex, subUri);
    }
  }

  /// <summary>
  /// Create a Collection
  /// </summary>
  /// <param name="collectionName"></param>
  /// <param name="vectorParams"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<OneOf<bool, ErrorResponse>> CreateCollection(string collectionName, VectorParams vectorParams, CancellationToken cancellationToken)
  {
    var payLoad = new CreateCollectionWithVectorRequest()
    {
      Vectors = vectorParams
    };
    var res = await PutAsync<CreateCollectionWithVectorRequest, bool>($"/collections/{collectionName}", payLoad, cancellationToken);
    return res;
  }
  public async Task<OneOf<CollectionInfo, ErrorResponse>> GetCollection(string collectionName, CancellationToken cancellationToken)
  {
    return await GetAsync<CollectionInfo>($"/collections/{collectionName}", cancellationToken);
  }
}
