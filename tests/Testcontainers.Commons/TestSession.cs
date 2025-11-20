namespace DotNet.Testcontainers.Commons;

[PublicAPI]
public static class TestSession
{
    public static readonly string TempDirectoryPath = Path.Combine(Path.GetTempPath(), "testcontainers-tests", Guid.NewGuid().ToString("D"));

    static TestSession()
    {
        Directory.CreateDirectory(TempDirectoryPath);
    }

    public static IImage GetImageFromDockerfile(
        string relativePath = "Dockerfile",
        int lineIndex = 0)
    {
        var fullpath = Path.GetFullPath(relativePath);
        if (!File.Exists(fullpath)) throw new Exception($"Dockerfile not found at '{fullpath}'.");
        var lines = File.ReadAllLines(fullpath);
        if (lines.Length == 0 || lines.Length <= lineIndex) throw new Exception($"Dockerfile located at '{fullpath}' is empty or shorter than {lineIndex + 1} line(s).");
        var imageLine = lines[lineIndex];
        var imageLineSplit = imageLine.Split(" ");
        if (imageLineSplit.Length < 2) throw new Exception($"Dockerfile located at '{fullpath}' has invalid image tag at line {lineIndex + 1}. The line should start with 'FROM' instruction, followed by space and image tag. For example: 'FROM postgres:17'.");
        var imageTag = imageLineSplit[1];
        var image = new DockerImage(imageTag);
        return image;
    }
}