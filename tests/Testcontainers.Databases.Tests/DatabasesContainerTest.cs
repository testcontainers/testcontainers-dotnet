namespace Testcontainers.Databases;

public sealed class DatabaseContainersTest
{
    [Theory]
    [MemberData(nameof(GetContainerImplementations), parameters: true)]
    public void ShouldImplementIDatabaseContainer(Type type)
    {
        Assert.True(type.IsAssignableTo(typeof(IDatabaseContainer)), $"The type '{type.Name}' does not implement the database interface.");
    }

    [Theory]
    [MemberData(nameof(GetContainerImplementations), parameters: false)]
    public void ShouldNotImplementIDatabaseContainer(Type type)
    {
        Assert.False(type.IsAssignableTo(typeof(IDatabaseContainer)), $"The type '{type.Name}' does implement the database interface.");
    }

    public static IEnumerable<object[]> GetContainerImplementations(bool expectDataProvider)
    {
        var testAssemblies = Directory.GetFiles(".", "Testcontainers.*.Tests.dll", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFullPath)
            .Select(Assembly.LoadFrom)
            .ToDictionary(assembly => assembly, assembly => assembly.GetReferencedAssemblies()
                .Where(referencedAssembly => referencedAssembly.Name != null)
                .Where(referencedAssembly => !referencedAssembly.Name.StartsWith("System"))
                .Where(referencedAssembly => !referencedAssembly.Name.StartsWith("xunit"))
                .Where(referencedAssembly => !referencedAssembly.Name.Equals("Microsoft.VisualStudio.TestPlatform.ObjectModel"))
                .Where(referencedAssembly => !referencedAssembly.Name.Equals("Docker.DotNet"))
                .Where(referencedAssembly => !referencedAssembly.Name.Equals("Testcontainers"))
                .Select(Assembly.Load)
                .SelectMany(referencedAssembly => referencedAssembly.ExportedTypes)
                .ToImmutableList());

        foreach (var testAssembly in testAssemblies)
        {
            // TODO: If a module contains multiple container implementations, it would require all container implementations to implement the interface.
            foreach (var containerType in testAssembly.Value.Where(type => type.IsAssignableTo(typeof(IContainer))))
            {
                var hasDataProvider = testAssembly.Value.Exists(type => type.IsSubclassOf(typeof(DbProviderFactory)));

                if (expectDataProvider && hasDataProvider)
                {
                    yield return new object[] { containerType };
                }

                if (!expectDataProvider && !hasDataProvider)
                {
                    yield return new object[] { containerType };
                }
            }
        }
    }
}