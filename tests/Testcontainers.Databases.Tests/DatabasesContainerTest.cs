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
            // TODO: If a module contains multiple container implementations, it would require all container implementations to implement the interface.
            foreach (var containerType in testAssembly.Value.Where(type => type.IsAssignableTo(typeof(IContainer))))
            {
                var testAssemblyName = testAssembly.Key.GetName().Name!;

                var containerTypeAssemblyName = containerType.Assembly.GetName().Name!;

                // If a module utilizes another one of our modules, do not include the container type
                // if it does not belong to the actual module. For example, the ServiceBus module
                // utilizes the MsSql module. We do not want to include the MsSqlContainer type
                // twice or place it in the wrong test.
                if (!testAssemblyName.Contains(containerTypeAssemblyName, StringComparison.OrdinalIgnoreCase))
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