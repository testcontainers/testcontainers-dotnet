namespace Testcontainers.Xunit;

/// <summary>
/// Fixture for sharing a container instance across multiple tests in a single class.
/// See <a href="https://xunit.net/docs/shared-context">Shared Context between Tests</a> from xUnit.net documentation for more information about fixtures.
/// A logger is automatically configured to write diagnostic messages to xUnit's <see cref="IMessageSink"/>.
/// </summary>
/// <typeparam name="TBuilderEntity">The builder entity.</typeparam>
/// <typeparam name="TContainerEntity">The container entity.</typeparam>
[PublicAPI]
#if XUNIT_V3
public class ContainerFixture<TBuilderEntity, TContainerEntity> : ContainerLifetime<TBuilderEntity, TContainerEntity>
#else
public class ContainerFixture<TBuilderEntity, TContainerEntity>(IMessageSink messageSink) : ContainerLifetime<TBuilderEntity, TContainerEntity>
#endif
    where TBuilderEntity : IContainerBuilder<TBuilderEntity, TContainerEntity>, new()
    where TContainerEntity : IContainer
{
#if XUNIT_V3
    protected override ILogger Logger { get; } = TestContextLogger.Instance;
#else
    /// <summary>
    /// The message sink used for reporting diagnostic messages.
    /// </summary>
    protected IMessageSink MessageSink { get; } = messageSink;

    protected override ILogger Logger { get; } = new MessageSinkLogger(messageSink);
#endif
}