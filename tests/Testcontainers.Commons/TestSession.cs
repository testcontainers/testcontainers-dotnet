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
        if (!string.IsNullOrEmpty(stage)) return FindStage(lines, stage);
        else return FindFirstTag(lines);

        DockerImage FindFirstTag(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                var split = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (split.Length < 2) continue;
                var imageTag = split[1];
                return new DockerImage(imageTag);
            }
            throw new Exception($"Failed to find any image tag in Dockerfile located at '{fullpath}'.");
        }

        DockerImage FindStage(IEnumerable<string> lines, string stage)
        {
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                if (!line.Trim().ToLowerInvariant().EndsWith($"as {stage}")) continue;
                var split = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (split.Length < 2) continue;
                var imageTag = split[1];
                return new DockerImage(imageTag);
            }
            throw new Exception($"Failed to find image with stage {stage} in Dockerfile located at '{fullpath}'.");
        }
    }
}