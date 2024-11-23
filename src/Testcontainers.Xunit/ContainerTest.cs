namespace Testcontainers.Xunit;

/// <summary>
/// Base class for tests needing a container per test method.
/// </summary>
/// <typeparam name="TBuilderEntity">The builder entity.</typeparam>
/// <typeparam name="TContainerEntity">The container entity.</typeparam>
[PublicAPI]
public abstract class ContainerTest<TBuilderEntity, TContainerEntity> : ContainerLifetime<TBuilderEntity, TContainerEntity>
    where TBuilderEntity : IContainerBuilder<TBuilderEntity, TContainerEntity>, new()
    where TContainerEntity : IContainer
{
    private readonly Func<TBuilderEntity, TBuilderEntity> _configure;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerTest{TBuilderEntity,TContainerEntity}" /> class.
    /// </summary>
    /// <param name="configure">An optional callback to configure the container.</param>
    /// <remarks>Use the constructor taking an <see cref="ITestOutputHelper"/> for logging Testcontainers messages.</remarks>
    protected ContainerTest(Func<TBuilderEntity, TBuilderEntity> configure = null) : base(NullLogger.Instance)
    {
        _configure = configure;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerTest{TBuilderEntity,TContainerEntity}" /> class.
    /// </summary>
    /// <param name="testOutputHelper">The test output helper where the logs, prefixed with <c>[testcontainers.org]</c> and a timestamp, will be written to.</param>
    /// <param name="configure">An optional callback to configure the container.</param>
    protected ContainerTest(ITestOutputHelper testOutputHelper, Func<TBuilderEntity, TBuilderEntity> configure = null) : base(new TestOutputLogger(testOutputHelper))
    {
        _configure = configure;
    }

    protected override TBuilderEntity Configure(TBuilderEntity builder) => _configure != null ? _configure(builder) : builder;
}