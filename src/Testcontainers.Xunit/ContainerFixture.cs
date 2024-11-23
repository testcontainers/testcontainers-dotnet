namespace Testcontainers.Xunit;

/// <summary>
/// Fixture for sharing a container instance across multiple tests in a single class.
/// See <a href="https://xunit.net/docs/shared-context">Shared Context between Tests</a> from xUnit.net documentation for more information about fixtures.
/// </summary>
/// <typeparam name="TBuilderEntity">The builder entity.</typeparam>
/// <typeparam name="TContainerEntity">The container entity.</typeparam>
[PublicAPI]
public class ContainerFixture<TBuilderEntity, TContainerEntity> : ContainerLifetime<TBuilderEntity, TContainerEntity>
    where TBuilderEntity : IContainerBuilder<TBuilderEntity, TContainerEntity>, new()
    where TContainerEntity : IContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerFixture{TBuilderEntity,TContainerEntity}" /> class.
    /// </summary>
    /// <remarks>Use the constructor taking an <see cref="IMessageSink"/> for logging Testcontainers messages.</remarks>
    public ContainerFixture() : base(NullLogger.Instance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerFixture{TBuilderEntity,TContainerEntity}" /> class.
    /// </summary>
    /// <param name="messageSink">The message sink where the logs, prefixed with <c>[testcontainers.org]</c>, will be written to.</param>
    public ContainerFixture(IMessageSink messageSink) : base(new MessageSinkLogger(messageSink))
    {
    }
}