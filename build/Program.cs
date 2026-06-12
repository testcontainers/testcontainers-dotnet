namespace Testcontainers.Build;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .InstallTool(new Uri("nuget:?package=dotnet-sonarscanner&version=11.2.1"))
            .UseContext<BuildContext>()
            .UseLifetime<BuildLifetime>()
            .Run(args);
    }
}