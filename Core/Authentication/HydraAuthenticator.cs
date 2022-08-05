using HydraDotNet.Core.Api;
using RestSharp;
using RestSharp.Authenticators;
using System.Threading.Tasks;

namespace HydraDotNet.Core.Authentication;

public class HydraAuthenticator : IAuthenticator
{
    private HydraAuthContainer? HydraAuth { get; set; }

    public void SetAuthContainer(HydraAuthContainer container) => HydraAuth = container;
    
    public ValueTask Authenticate(RestClient client, RestRequest request)
    {
        if (request is not HydraApiRequest hydraRequest)
            return new();

        if (!string.IsNullOrEmpty(hydraRequest.Endpoint.ApiKey))
            request.AddOrUpdateHeader("x-hydra-api-key", hydraRequest.Endpoint.ApiKey);

        if (HydraAuth is not null)
        {
            request.AddOrUpdateHeader("X-Hydra-Access-Token", hydraRequest.Endpoint.UseNetworkToken ? HydraAuth.NetworkToken : HydraAuth.Token);
        }

        return new();
    }
}
