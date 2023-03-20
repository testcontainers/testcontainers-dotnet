namespace DotNet.Testcontainers.Commons;

[PublicAPI]
public static class TestSession
{
    public static readonly string TempDirectoryPath = Path.Combine(Path.GetTempPath(), "testcontainers-tests", Guid.NewGuid().ToString("D"));

    static TestSession()
    {
        Directory.CreateDirectory(TempDirectoryPath);
    }
}