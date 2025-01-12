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

    public static TheoryData<Type> GetContainerImplementations(bool expectDataProvider)
    {
        var theoryData = new TheoryData<Type>();

        var testAssemblies = Directory.GetFiles(".", "Testcontainers.*.Tests.dll", SearchOption.TopDirectoryOnly)
            .Where(fileName => !fileName.Contains("Testcontainers.Xunit.Tests"))
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
            var testAssemblyName = testAssembly.Key.GetName().Name;

            // TODO: If a module contains multiple container implementations, it would require all container implementations to implement the interface.
            foreach (var containerType in testAssembly.Value.Where(type => type.IsAssignableTo(typeof(IContainer))))
            {
                var typeAssemblyName = containerType.Assembly.GetName().Name;

                if (!string.IsNullOrWhiteSpace(testAssemblyName) && !string.IsNullOrWhiteSpace(typeAssemblyName) && !testAssemblyName.Contains(typeAssemblyName))
                {
                    continue;
                }

                var hasDataProvider = testAssembly.Value.Exists(type => type.IsSubclassOf(typeof(DbProviderFactory)));

                if (expectDataProvider && hasDataProvider)
                {
                    theoryData.Add(containerType);
                }

                if (!expectDataProvider && !hasDataProvider)
                {
                    theoryData.Add(containerType);
                }
            }
        }

        return theoryData;
    }
}