namespace Testcontainers.Xunit;

/// <summary>
/// Fixture for sharing a container instance across multiple tests in a single class.
/// See <a href="https://xunit.net/docs/shared-context">Shared Context between Tests</a> from xUnit.net documentation for more information about fixtures.
/// A logger is automatically configured to write diagnostic messages to xUnit's <see cref="IMessageSink" />.
/// </summary>
/// <param name="messageSink">An optional <see cref="IMessageSink" /> where the logs are written to. Pass <c>null</c> to ignore logs.</param>
/// <typeparam name="TBuilderEntity">The builder entity.</typeparam>
/// <typeparam name="TContainerEntity">The container entity.</typeparam>
[PublicAPI]
public class ContainerFixture<TBuilderEntity, TContainerEntity>(IMessageSink messageSink)
    : ContainerLifetime<TBuilderEntity, TContainerEntity>(new MessageSinkLogger(messageSink))
    where TBuilderEntity : IContainerBuilder<TBuilderEntity, TContainerEntity>, new()
    where TContainerEntity : IContainer;