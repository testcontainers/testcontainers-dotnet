namespace Testcontainers.Xunit;

/// <summary>
/// Base class for tests needing a container per test method.
/// A logger is automatically configured to write messages to xUnit's <see cref="ITestOutputHelper" />.
/// </summary>
/// <param name="testOutputHelper">An optional <see cref="ITestOutputHelper" /> where the logs are written to. Pass <c>null</c> to ignore logs.</param>
/// <param name="configure">An optional callback to configure the container.</param>
/// <typeparam name="TBuilderEntity">The builder entity.</typeparam>
/// <typeparam name="TContainerEntity">The container entity.</typeparam>
[PublicAPI]
public abstract class ContainerTest<TBuilderEntity, TContainerEntity>(ITestOutputHelper testOutputHelper, Func<TBuilderEntity, TBuilderEntity> configure = null)
    : ContainerLifetime<TBuilderEntity, TContainerEntity>(new TestOutputLogger(testOutputHelper))
    where TBuilderEntity : IContainerBuilder<TBuilderEntity, TContainerEntity>, new()
    where TContainerEntity : IContainer
{
    protected override TBuilderEntity Configure(TBuilderEntity builder) => configure != null ? configure(builder) : builder;
}