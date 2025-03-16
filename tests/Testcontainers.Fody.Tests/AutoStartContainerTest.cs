namespace Testcontainers.Fody;

public sealed class AutoStartContainerTest
{
    [AutoStartContainer]
    private readonly IContainer _container = new ContainerBuilder()
        .WithImage(CommonImages.Alpine)
        .WithEntrypoint(CommonCommands.SleepInfinity)
        .Build();

    // This test allows you to debug into the Fody module weaver implementation and
    // test the generated code. The two tests are meant to run in different assemblies,
    // as they test different things. For simplicity, I placed them in the same assembly.
    // To run this test, set the `DisableFody` project property to `true`.

    [Fact]
    public void ConstructorDecompilesToExpectedCodeWhenAutoStartContainerAttributeIsPresent()
    {
        // Given
        const string expectedCode = """
        using System.Threading;
        using DotNet.Testcontainers.Builders;
        using DotNet.Testcontainers.Commons;

        public AutoStartContainerTest()
        {
            _container = new ContainerBuilder().WithImage(CommonImages.Alpine).WithEntrypoint(CommonCommands.SleepInfinity).Build();
            base..ctor();
            Thread thread = new Thread(_container.StartAsync(CancellationToken.None).Wait);
            thread.Start();
            thread.Join();
        }
        """;

        const string assemblyName = "Testcontainers.Fody.Tests.dll";

        var currentDirectoryPath = Directory.GetCurrentDirectory();

        // When
        var testResult = new ModuleWeaver().ExecuteTestRun(assemblyName, false);

        var assemblyResolver = new UniversalAssemblyResolver(testResult.AssemblyPath, true, ".NETCOREAPP,VERSION=v9.0");
        assemblyResolver.AddSearchDirectory(currentDirectoryPath);

        var decompilerSettings = new DecompilerSettings();
        decompilerSettings.UsingDeclarations = true;
        decompilerSettings.CSharpFormattingOptions.IndentationString = "    ";

        using var peFile = new PEFile(testResult.AssemblyPath);

        var decompiler = new CSharpDecompiler(testResult.AssemblyPath, assemblyResolver, decompilerSettings);

        var decompilerTypeSystem = new DecompilerTypeSystem(peFile, assemblyResolver);

        var typeDefinition = decompilerTypeSystem.MainModule.Compilation.FindType(new FullTypeName(typeof(AutoStartContainerTest).FullName)).GetDefinition()!;

        var constructor = typeDefinition.Methods.Single(method => method.IsConstructor);

        var actualCode = decompiler.DecompileAsString(constructor.MetadataToken);

        // Then
        Assert.Equal(expectedCode, actualCode.Trim());
    }

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