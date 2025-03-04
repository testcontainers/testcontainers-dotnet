namespace Testcontainers.Fody;

public sealed class AutoStartContainerTest
{
    [AutoStartContainer]
    private readonly IContainer _container = new ContainerBuilder()
        .WithImage(CommonImages.Alpine)
        .WithEntrypoint(CommonCommands.SleepInfinity)
        .Build();

    // The `AutoStartContainer` attribute (annotation) is recognized by the Testcontainers
    // for .NET Fody implementation, which injects and appends the following lines into
    // the constructor. We could utilize this mechanism to inject the necessary code to
    // start a container into every constructor we find, or only into a specific one.
    // Depending on whether the field or property is static, we can inject the code into
    // the static constructor, starting the container only once for all test methods.
    // There may also be better approaches. The implemented Fody module essentially
    // generates the following Csharp code:
    //
    // public AutoStartContainerTest()
    // {
    //     Thread thread = new Thread(_container.StartAsync(CancellationToken.None).Wait);
    //     thread.Start();
    //     thread.Join();
    // }

    [Fact]
    public void ContainerIsRunning()
    {
        Assert.Equal(TestcontainersStates.Running, _container.State);
    }
}