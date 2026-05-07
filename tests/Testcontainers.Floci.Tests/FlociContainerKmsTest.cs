using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;
using Amazon.Runtime;
using Xunit;

namespace Testcontainers.Floci.Tests;

[Collection("Floci")]
public sealed class FlociContainerKmsTest(FlociContainerFixture fixture)
{
    private AmazonKeyManagementServiceClient CreateClient() =>
        new(new BasicAWSCredentials(fixture.Container.GetAccessKey(), fixture.Container.GetSecretKey()),
            new AmazonKeyManagementServiceConfig
            {
                ServiceURL = fixture.Container.GetConnectionString(),
                AuthenticationRegion = FlociContainer.Region,
            });

    [Fact]
    public async Task Kms_CreateAndListKey_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();

        var created = await client.CreateKeyAsync(new CreateKeyRequest(), ct);
        var keys = await client.ListKeysAsync(new ListKeysRequest(), ct);

        Assert.Contains(keys.Keys, k => k.KeyId == created.KeyMetadata.KeyId);
    }

    [Fact]
    public async Task Kms_EncryptAndDecrypt_ReturnsOriginalPlaintext()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        const string plaintext = "Hello from Floci KMS!";

        var key = await client.CreateKeyAsync(new CreateKeyRequest(), ct);
        var encrypted = await client.EncryptAsync(new EncryptRequest
        {
            KeyId = key.KeyMetadata.KeyId,
            Plaintext = new MemoryStream(Encoding.UTF8.GetBytes(plaintext)),
        }, ct);
        var decrypted = await client.DecryptAsync(new DecryptRequest
        {
            KeyId = key.KeyMetadata.KeyId,
            CiphertextBlob = encrypted.CiphertextBlob,
        }, ct);

        Assert.Equal(plaintext, Encoding.UTF8.GetString(decrypted.Plaintext.ToArray()));
    }
}
