using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;

namespace Testcontainers.BigQuery
{
    public class EmptyCredentials:ICredential
    {
        public void Initialize(ConfigurableHttpClient httpClient)
        {
        
        }

        public async Task<string> GetAccessTokenForRequestAsync(string authUri = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return "empty";
        }
    }
}