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
        string stage = "")
    {
        var fullpath = Path.GetFullPath(relativePath);
        if (!File.Exists(fullpath)) throw new Exception($"Dockerfile not found at '{fullpath}'.");
        var lines = File.ReadAllLines(fullpath);
        if (lines.Length == 0) throw new Exception($"Dockerfile located at '{fullpath}' is empty.");
        return FindStageOrAnyTag(lines, stage);

        DockerImage FindStageOrAnyTag(IEnumerable<string> lines, string stage)
        {
            bool shouldSearchStage = !string.IsNullOrEmpty(stage);
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                if (line.StartsWith('#')) continue;
                if (shouldSearchStage && !line.Trim().ToLowerInvariant().EndsWith($"as {stage}")) continue;
                var split = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (split.Length < 2) continue;
                var imageTag = split[1];
                return new DockerImage(imageTag);
            }
            if (shouldSearchStage)
                throw new Exception($"Failed to find image with stage '{stage}' in Dockerfile located at '{fullpath}'.");
            else
                throw new Exception($"Failed to find any image tag in Dockerfile located at '{fullpath}'.");
        }
    }
}