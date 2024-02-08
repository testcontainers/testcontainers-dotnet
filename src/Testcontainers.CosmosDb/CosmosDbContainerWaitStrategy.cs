namespace Testcontainers.CosmosDb;

public sealed class CosmosDbContainerWaitStrategy : IWaitUntil
{
    private const string ReadyProbeUrl = "https://localhost/_explorer/index.html";
        
    public async Task<bool> UntilAsync(IContainer container)
    {
        try
        {
            using var httpClient = ((CosmosDbContainer)container).HttpClient;

            var result = await httpClient.GetAsync(ReadyProbeUrl);
                
            return result.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
