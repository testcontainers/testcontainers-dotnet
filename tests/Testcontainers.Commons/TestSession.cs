namespace DotNet.Testcontainers.Commons;

[PublicAPI]
public static class TestSession
{
    private static readonly ConcurrentDictionary<string, Dictionary<string, string>> Stages = new ConcurrentDictionary<string, Dictionary<string, string>>();

    public static readonly string TempDirectoryPath = Path.Combine(Path.GetTempPath(), "testcontainers-tests", Guid.NewGuid().ToString("D"));

    static TestSession()
    {
        Directory.CreateDirectory(TempDirectoryPath);
    }

    public static string GetImageFromDockerfile(string relativeFilePath = "Dockerfile", string stage = "")
    {
        var absoluteFilePath = Path.GetFullPath(relativeFilePath);

        var stages = Stages.GetOrAdd(absoluteFilePath, filePath => new DockerfileParser(filePath).Parse());

        if (stages.TryGetValue(stage, out var image))
        {
            return image;
        }

        throw new InvalidOperationException($"Stage '{stage}' not found in Dockerfile.");
    }
}