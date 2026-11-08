namespace Testcontainers.DocGen;

public static class Program
{
    private const string GeneratedFileSuffix = ".Generated.xml";

    private static readonly Regex IncludePattern = new Regex("<include file=\"(?<file>[^\"]+" + Regex.Escape(GeneratedFileSuffix) + ")\" path=\"(?<path>[^\"]+)\"");

    public static int Main()
    {
        var srcDirectoryPath = Path.GetFullPath("src");
        var testsDirectoryPath = Path.GetFullPath("tests");

        foreach (var builderType in GetBuilderTypes())
        {
            var moduleName = builderType.Assembly.GetName().Name;
            var moduleDirectoryPath = Path.Combine(srcDirectoryPath, moduleName);
            var builderFilePath = Path.Combine(moduleDirectoryPath, builderType.Name + ".cs");
            var generatedFilePath = Path.Combine(moduleDirectoryPath, builderType.Name + GeneratedFileSuffix);
            var images = GetImages(Path.Combine(testsDirectoryPath, moduleName + ".Tests", "Dockerfile"));

            var document = new BuilderDocumentation(builderType, File.ReadAllText(builderFilePath), images).Create();
            File.WriteAllText(generatedFilePath, document.ToString());
        }

        return ValidateIncludes(srcDirectoryPath) ? 0 : 1;
    }

    private static IEnumerable<Type> GetBuilderTypes()
    {
        // Load only the referenced module assemblies. The output directory may
        // still contain assemblies of renamed or removed modules.
        return ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))
            .Split(Path.PathSeparator)
            .Select(Path.GetFileNameWithoutExtension)
            .Where(assemblyName => assemblyName.StartsWith("Testcontainers."))
            .Select(Assembly.Load)
            .SelectMany(assembly => assembly.GetExportedTypes())
            .Where(type => !type.IsAbstract && type.BaseType is { IsGenericType: true } baseType && baseType.GetGenericTypeDefinition() == typeof(ContainerBuilder<,,>));
    }

    private static List<string> GetImages(string dockerfilePath)
    {
        // Return the images of the module tests. The default image, which is
        // the image of the Dockerfile stage without a name, comes first, see
        // TestSession.GetImageFromDockerfile.
        return File.ReadLines(dockerfilePath)
            .Select(line => line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries))
            .Where(tokens => tokens.Length > 1 && "FROM".Equals(tokens[0], StringComparison.OrdinalIgnoreCase))
            .OrderBy(tokens => tokens.Any(token => "AS".Equals(token, StringComparison.OrdinalIgnoreCase)))
            .Select(tokens => tokens.Skip(1).First(token => !token.StartsWith("--")))
            .ToList();
    }

    private static bool ValidateIncludes(string srcDirectoryPath)
    {
        var isValid = true;

        // The compiler ignores an include path that does not match any
        // element. Report it, so that the documentation does not silently
        // lose content when a builder changes.
        foreach (var filePath in Directory.EnumerateFiles(srcDirectoryPath, "*.cs", SearchOption.AllDirectories))
        {
            foreach (Match match in IncludePattern.Matches(File.ReadAllText(filePath)))
            {
                var includeFilePath = Path.Combine(Path.GetDirectoryName(filePath), match.Groups["file"].Value);

                if (File.Exists(includeFilePath) && XDocument.Load(includeFilePath).XPathSelectElements(match.Groups["path"].Value).Any())
                {
                    continue;
                }

                Console.Error.WriteLine($"{filePath}: The include path '{match.Groups["path"].Value}' does not match any element in '{match.Groups["file"].Value}'.");
                isValid = false;
            }
        }

        return isValid;
    }
}