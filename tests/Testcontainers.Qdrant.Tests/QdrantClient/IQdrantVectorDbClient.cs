using OneOf;

using System.Threading;
using System.Threading.Tasks;

namespace Testcontainers.Qdrant.Tests.QdrantClient;

public interface IQdrantVectorDbClient
{
  /// <summary>
  /// <a href="https://qdrant.tech/documentation/concepts/collections/">Create collection</a>
  /// </summary>
  /// <param name="collectionName"></param>
  /// <param name="vectorParams"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  Task<OneOf<bool, ErrorResponse>> CreateCollection(string collectionName, VectorParams vectorParams, CancellationToken cancellationToken);

  Task<OneOf<CollectionInfo, ErrorResponse>> GetCollection(string collectionName, CancellationToken cancellationToken);
}
