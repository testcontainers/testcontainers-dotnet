namespace DotNet.Testcontainers.Configurations
{
  using System;

  public sealed class RetryLimitExceededException : Exception
  {
    public RetryLimitExceededException()
    {
    }

    public RetryLimitExceededException(string message)
      : base(message)
    {
    }

    public RetryLimitExceededException(string message, Exception inner)
      : base(message, inner)
    {
    }
  }
}
