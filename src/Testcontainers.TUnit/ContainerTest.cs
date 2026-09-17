namespace Testcontainers.TUnit;

/// <summary>
/// Base class for tests needing a container per test method.
/// TUnit creates a new instance of the test class for each test, starts the container before the test runs and disposes of it afterward.
/// A logger is automatically configured to write messages to the output of the running test.
/// </summary>
/// <typeparam name="TBuilderEntity">The builder entity.</typeparam>
/// <typeparam name="TContainerEntity">The container entity.</typeparam>
[PublicAPI]
public abstract class ContainerTest<TBuilderEntity, TContainerEntity> : ContainerLifetime<TBuilderEntity, TContainerEntity>
    where TBuilderEntity : IContainerBuilder<TBuilderEntity, TContainerEntity, IContainerConfiguration>, new()
    where TContainerEntity : IContainer
{
    protected ContainerTest()
        : base(new TestContextLogger())
    {
    }
}