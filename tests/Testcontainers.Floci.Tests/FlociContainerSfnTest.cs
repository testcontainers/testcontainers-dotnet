using System;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.StepFunctions;
using Amazon.StepFunctions.Model;
using Xunit;

namespace Testcontainers.Floci.Tests;

[Collection("Floci")]
public sealed class FlociContainerSfnTest(FlociContainerFixture fixture)
{
    private AmazonStepFunctionsClient CreateClient() =>
        new(new BasicAWSCredentials(fixture.Container.GetAccessKey(), fixture.Container.GetSecretKey()),
            new AmazonStepFunctionsConfig
            {
                ServiceURL = fixture.Container.GetConnectionString(),
                AuthenticationRegion = FlociContainer.Region,
            });

    [Fact]
    public async Task Sfn_CreateAndListStateMachine_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var name = $"sm-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        const string definition = """{"Comment":"Test","StartAt":"Pass","States":{"Pass":{"Type":"Pass","End":true}}}""";

        var created = await client.CreateStateMachineAsync(new CreateStateMachineRequest
        {
            Name = name,
            Definition = definition,
            RoleArn = "arn:aws:iam::000000000000:role/test-role",
            Type = StateMachineType.STANDARD,
        }, ct);
        var response = await client.ListStateMachinesAsync(new ListStateMachinesRequest(), ct);

        Assert.Contains(name, created.StateMachineArn);
        Assert.Contains(response.StateMachines, sm => sm.Name == name);
    }
}
