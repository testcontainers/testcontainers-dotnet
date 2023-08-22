using System;

namespace Testcontainers.Qdrant.Tests.QdrantClient;

[Serializable]
public class QdrantException : Exception
{
  public QdrantException(string message) : base(message)
  {
  }
  public QdrantException(string message, string error) : base(message + " " + error)
  {
  }
}
