namespace DotNet.Testcontainers.Containers
{
  using System;

  [Serializable]
  public sealed class ResourceReaperException : Exception
  {
    public ResourceReaperException(string message)
      : base(message)
    {
    }
  }
}
