namespace Testcontainers.Databases;

public sealed class DatabaseContainersTest
{
    [Theory]
    [MemberData(nameof(DatabaseContainersTheoryData))]
    public void ImplementsIDatabaseContainerInterface(Type type)
    {
        Assert.True(type.IsAssignableTo(typeof(IDatabaseContainer)));
    }

    public static IEnumerable<object[]> DatabaseContainersTheoryData
    {
        get
        {
            static bool HasGetConnectionStringMethod(Type type) => type.IsAssignableTo(typeof(IContainer)) && type.GetMethod("GetConnectionString") != null;
            var assembly = typeof(DatabaseContainersTest).Assembly;
            var dependencyContext = DependencyContext.Load(assembly) ?? throw new InvalidOperationException($"DependencyContext.Load({assembly}) returned null");
            return dependencyContext.RuntimeLibraries
                .Where(library => library.Name.StartsWith("Testcontainers."))
                .SelectMany(library => Assembly.Load(library.Name).GetExportedTypes().Where(HasGetConnectionStringMethod))
                .Select(type => new[] { type });
        }
    }
}