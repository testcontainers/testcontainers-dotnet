namespace DotNet.Testcontainers.Commons;

internal sealed class DockerfileParser
{
    private static readonly Regex FromLinePattern = new Regex("^FROM\\s+(?<arg>--\\S+\\s)*(?<image>\\S+).*", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));

    private readonly string _filePath;

    public DockerfileParser(string filePath)
    {
        _filePath = Path.GetFullPath(filePath);

        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException($"Dockerfile '{_filePath}' not found.");
        }
    }

    public Dictionary<string, string> Parse()
    {
        const StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries;

        const string imageGroup = "image";

        var separator = new[] { " AS ", " As ", " aS ", " as " };

        var lines = File.ReadAllLines(_filePath)
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
                Stage = match.Value.Split(separator, options).Skip(1).FirstOrDefault(string.Empty),
                Image = match.Groups[imageGroup].Value,
            })
            .ToDictionary(
                item => item.Stage,
                item => item.Image,
                StringComparer.OrdinalIgnoreCase);

        return stages;
    }
}