using RestSharp;
using RestSharp.Authenticators;
using System.Threading.Tasks;

namespace HydraDotNet.Core.Authentication;

public class HydraAuthenticator : IAuthenticator
{
    private string ApiKey { get; init; }

    public HydraAuthenticator(HydraClient client)
    {
        ApiKey = client.ApiKey;
    }

    public ValueTask Authenticate(RestClient client, RestRequest request)
    {
        request.AddOrUpdateHeader("x-hydra-api-key", ApiKey);

        return new();

        // TODO: this
    }
}
