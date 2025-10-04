namespace DotNet.Testcontainers.Commons;

[PublicAPI]
public sealed class Certificates
{
    public const string Password = "password";

    private readonly string _baseDirectory = AppContext.BaseDirectory;

    private Certificates()
    {
        if (!Directory.Exists(CaDirectory))
        {
            throw new DirectoryNotFoundException($"CA directory not found: '{CaDirectory}'.");
        }

        if (!Directory.Exists(ClientDirectory))
        {
            throw new DirectoryNotFoundException($"Client directory not found: '{ClientDirectory}'.");
        }

        if (!Directory.Exists(ServerDirectory))
        {
            throw new DirectoryNotFoundException($"Server directory not found: '{ServerDirectory}'.");
        }
    }

    public static Certificates Instance { get; } = new Certificates();

    public string CaDirectory => Path.Combine(_baseDirectory, "ssl", "ca");

    public string ClientDirectory => Path.Combine(_baseDirectory, "ssl", "client");

    public string ServerDirectory => Path.Combine(_baseDirectory, "ssl", "server");

    public string GetFilePath(string directoryName, string fileName)
    {
        string directoryPath;

        switch (directoryName.ToLowerInvariant())
        {
            case "ca":
                directoryPath = CaDirectory;
                break;
            case "client":
                directoryPath = ClientDirectory;
                break;
            case "server":
                directoryPath = ServerDirectory;
                break;
            default:
                throw new ArgumentException($"Unknown directory: '{directoryName}'.");
        }

        var filePath = Path.Combine(directoryPath, fileName);
        return File.Exists(filePath) ? filePath : throw new FileNotFoundException($"File not found: '{filePath}'.");
    }
}