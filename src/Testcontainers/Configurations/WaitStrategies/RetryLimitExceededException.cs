namespace DotNet.Testcontainers.Configurations
{
  using System;

  public sealed class RetryLimitExceededException : Exception
  {
    public RetryLimitExceededException(string message)
      : base(message)
    {
    }
  }
}
