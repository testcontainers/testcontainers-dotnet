namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.Runtime.Serialization;

  [Serializable]
  public sealed class ResourceReaperException : Exception
  {
    public ResourceReaperException(string message)
      : base(message)
    {
    }

    public ResourceReaperException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    private ResourceReaperException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
