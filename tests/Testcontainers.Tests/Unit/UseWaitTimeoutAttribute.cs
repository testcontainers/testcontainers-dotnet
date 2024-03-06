namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Reflection;
  using DotNet.Testcontainers.Configurations;
  using Xunit.Sdk;

  class UseWaitTimeoutAttribute : BeforeAfterTestAttribute
  {
    private readonly TimeSpan _originalTimeout;
    private readonly TimeSpan _timeout;

    public UseWaitTimeoutAttribute(int seconds)
    {
      _originalTimeout = TestcontainersSettings.WaitTimeout;
      _timeout = TimeSpan.FromSeconds(seconds);
    }

    public override void Before(MethodInfo methodUnderTest)
    {
      TestcontainersSettings.WaitTimeout = _timeout;
    }

    public override void After(MethodInfo methodUnderTest)
    {
      TestcontainersSettings.WaitTimeout = _originalTimeout;
    }
  }
}
