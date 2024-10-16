namespace Testcontainers.Camunda;

public sealed class CamundaContainerTest : IAsyncLifetime
{
    private readonly CamundaContainer _camundaContainer = new CamundaBuilder().Build();

    public Task InitializeAsync()
    {
        return _camundaContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _camundaContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task EstablishesConnection()
    {
        // Given
        var client = CamundaClient.Create(_camundaContainer.GetConnectionString());

        // When
        var response = (await client.ProcessDefinitions.Query(new ProcessDefinitionQuery()).List())
            .Select(x => x.Resource);

        // Then
        Assert.Equal(new[] { "reviewInvoice.bpmn", "invoice.v1.bpmn", "invoice.v2.bpmn" }, response);
    }
}