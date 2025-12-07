namespace DotNet.Testcontainers.Commons;

[PublicAPI]
public static class TestSession
{
    private static readonly Regex FromLinePattern = new Regex("^FROM\\s+(?<arg>--\\S+\\s)*(?<image>\\S+).*", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));

    public static readonly string TempDirectoryPath = Path.Combine(Path.GetTempPath(), "testcontainers-tests", Guid.NewGuid().ToString("D"));

    static TestSession()
    {
        Directory.CreateDirectory(TempDirectoryPath);
    }

    public static string GetImageFromDockerfile(string relativeFilePath = "Dockerfile", string stage = "")
    {
        const string imageGroup = "image";

        var absoluteFilePath = Path.GetFullPath(relativeFilePath);

        if (!File.Exists(absoluteFilePath))
        {
            throw new FileNotFoundException($"Dockerfile '{absoluteFilePath}' not found.");
        }

        var lines = File.ReadAllLines(absoluteFilePath)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrEmpty(line))
            .Where(line => !line.StartsWith('#'))
            .ToArray();

        var fromMatches = lines
            .Select(line => FromLinePattern.Match(line))
            .Where(match => match.Success)
            .ToArray();

        var stages = fromMatches
            .Select(match => new
            {
                Stage = match.Value.Split(new[] { " AS ", " As ", " aS ", " as " }, StringSplitOptions.RemoveEmptyEntries).Skip(1).DefaultIfEmpty(string.Empty).First(),
                Image = match.Groups[imageGroup].Value,
            })
            .ToDictionary(item => item.Stage, item => item.Image);

        if (stages.TryGetValue(stage, out var image))
        {
            return image;
        }

        throw new InvalidOperationException($"Stage '{stage}' not found in Dockerfile.");
    }
}