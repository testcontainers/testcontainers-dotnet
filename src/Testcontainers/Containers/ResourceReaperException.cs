namespace DotNet.Testcontainers.Containers
{
  using System;

  public sealed class ResourceReaperException : Exception
  {
    public ResourceReaperException(string message)
      : base(message)
    {
    }
  }
}
