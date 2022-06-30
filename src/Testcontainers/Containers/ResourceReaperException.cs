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

    private ResourceReaperException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
