namespace Testcontainers.Build;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .InstallTool(new Uri("nuget:?package=dotnet-sonarscanner&version=10.1.2"))
            .UseContext<BuildContext>()
            .UseLifetime<BuildLifetime>()
            .Run(args);
    }
}
