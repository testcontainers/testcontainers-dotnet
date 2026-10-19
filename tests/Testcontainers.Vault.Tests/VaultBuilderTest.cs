namespace Testcontainers.Vault;

public sealed class VaultBuilderTest
{
    [Fact]
    public void RootTokenWithPeriodThrowsArgumentException()
    {
        // Given
        var vaultBuilder = new VaultBuilder(TestSession.GetImageFromDockerfile())
            .WithRootToken("vault.root.token");

        // When
        var exception = Assert.Throws<ArgumentException>(() => vaultBuilder.Build());

        // Then
        Assert.Equal("RootToken", exception.ParamName);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task FailingInitCommandThrowsExecFailedException()
    {
        // Given
        await using var vaultContainer = new VaultBuilder(TestSession.GetImageFromDockerfile())
            .WithInitCommand("secrets enable transit", "secrets enable transit")
            .Build();

        // When
        var exception = await Assert.ThrowsAsync<ExecFailedException>(() => vaultContainer.StartAsync(TestContext.Current.CancellationToken))
            .ConfigureAwait(true);

        // Then
        Assert.NotEqual(0, exception.ExecResult.ExitCode);
        Assert.Contains("path is already in use", exception.ExecResult.Stderr);
    }
}
