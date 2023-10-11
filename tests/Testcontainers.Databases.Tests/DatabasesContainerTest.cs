namespace Testcontainers.Databases;

public sealed class DatabaseContainersTest
{
    [Theory]
    [MemberData(nameof(GetContainerTypes), parameters: true)]
    public void ImplementsIDatabaseContainer(Type type)
    {
        Assert.True(type.IsAssignableTo(typeof(IDatabaseContainer)));
    }

    [Theory]
    [MemberData(nameof(GetContainerTypes), parameters: false)]
    public void DoesNotImplementIDatabaseContainer(Type type)
    {
        Assert.False(type.IsAssignableTo(typeof(IDatabaseContainer)));
    }

    public static IEnumerable<object[]> GetContainerTypes(bool adoNetContainers)
    {
        foreach (var dll in Directory.GetFiles(".", "Testcontainers.*.Tests.dll", SearchOption.TopDirectoryOnly))
        {
            var referencedAssemblies = Assembly.LoadFrom(Path.GetFullPath(dll)).GetReferencedAssemblies().Select(Assembly.Load).ToList();
            var hasAdoNetProvider = referencedAssemblies.Any(IsAdoNetProvider);
            if (adoNetContainers ? hasAdoNetProvider : !hasAdoNetProvider)
            {
                var containerAssembly = referencedAssemblies.SingleOrDefault(IsTestcontainer);
                if (containerAssembly != null)
                {
                    foreach (var containerType in containerAssembly.GetExportedTypes().Where(type => type.IsAssignableTo(typeof(IContainer))))
                    {
                        yield return new object[] { containerType };
                    }
                }
            }
        }
    }

    private static bool IsAdoNetProvider(Assembly assembly)
    {
        return assembly.GetExportedTypes().Any(a => a.IsSubclassOf(typeof(DbProviderFactory)));
    }

    private static bool IsTestcontainer(Assembly assembly)
    {
        var name = assembly.GetName().Name ?? "";
        return name.StartsWith("Testcontainers.") && name != "Testcontainers.Commons";
    }
}