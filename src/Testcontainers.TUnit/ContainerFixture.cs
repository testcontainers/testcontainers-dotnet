namespace Testcontainers.TUnit;

/// <summary>
/// Fixture for sharing a container instance across multiple tests.
/// Inject the fixture with <c>[ClassDataSource&lt;TFixture&gt;(Shared = SharedType.PerClass)]</c> (or any other <see cref="SharedType" />) into the test class.
/// See <a href="https://tunit.dev/docs/writing-tests/class-data-source">Injectable Class Data Source</a> from the TUnit documentation for more information about sharing instances.
/// A logger is automatically configured to write messages to the output of the running test.
/// </summary>
/// <typeparam name="TBuilderEntity">The builder entity.</typeparam>
/// <typeparam name="TContainerEntity">The container entity.</typeparam>
[PublicAPI]
public abstract class ContainerFixture<TBuilderEntity, TContainerEntity> : ContainerLifetime<TBuilderEntity, TContainerEntity>
    where TBuilderEntity : IContainerBuilder<TBuilderEntity, TContainerEntity, IContainerConfiguration>, new()
    where TContainerEntity : IContainer
{
    protected ContainerFixture()
        : base(new TestContextLogger())
    {
    }
}