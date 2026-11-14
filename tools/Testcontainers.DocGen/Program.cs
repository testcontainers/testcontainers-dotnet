namespace Testcontainers.DocGen;

public static class Program
{
    private const string GeneratedXmlFileSuffix = ".Generated.xml";

    private const string GeneratedMarkdownFileSuffix = ".Generated.md";

    private static readonly Regex IncludePattern = new Regex("<include file=\"(?<file>[^\"]+" + Regex.Escape(GeneratedXmlFileSuffix) + ")\" path=\"(?<path>[^\"]+)\"");

    public static int Main()
    {
        var srcDirectoryPath = Path.GetFullPath("src");
        var testsDirectoryPath = Path.GetFullPath("tests");

        foreach (var builderType in GetBuilderTypes())
        {
            var moduleName = builderType.Assembly.GetName().Name;
            var moduleDirectoryPath = Path.Combine(srcDirectoryPath, moduleName);
            var builderFilePath = Path.Combine(moduleDirectoryPath, builderType.Name + ".cs");
            var generatedXmlFilePath = Path.Combine(moduleDirectoryPath, builderType.Name + GeneratedXmlFileSuffix);
            var generatedMarkdownFilePath = Path.Combine(moduleDirectoryPath, builderType.Name + GeneratedMarkdownFileSuffix);
            var images = GetImages(Path.Combine(testsDirectoryPath, moduleName + ".Tests", "Dockerfile"));

            // Render the Markdown from the XML documentation, so that the
            // website and the IDE show the same default configuration.
            var document = new BuilderDocumentation(builderType, File.ReadAllText(builderFilePath), images).Create();
            File.WriteAllText(generatedXmlFilePath, document.ToString());
            File.WriteAllText(generatedMarkdownFilePath, MarkdownDocumentation.Create(document));
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
            foreach (var groups in IncludePattern.Matches(File.ReadAllText(filePath)).Select(match => match.Groups))
            {
                var includeFilePath = Path.Combine(Path.GetDirectoryName(filePath), groups["file"].Value);

                if (File.Exists(includeFilePath) && XDocument.Load(includeFilePath).XPathSelectElements(groups["path"].Value).Any())
                {
                    continue;
                }

                Console.Error.WriteLine($"{filePath}: The include path '{groups["path"].Value}' does not match any element in '{groups["file"].Value}'.");
                isValid = false;
            }
        }

        return isValid;
    }
}