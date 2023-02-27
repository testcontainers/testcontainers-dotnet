namespace DotNet.Testcontainers.Commons
{
  using System;
  using System.IO;
  using JetBrains.Annotations;

  [PublicAPI]
  public static class TestSession
  {
    public static readonly string TempDirectoryPath = Path.Combine(Path.GetTempPath(), "testcontainers-tests", Guid.NewGuid().ToString("D"));

    static TestSession()
    {
      Directory.CreateDirectory(TempDirectoryPath);
    }
  }
}
