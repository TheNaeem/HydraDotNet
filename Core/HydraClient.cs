using HydraDotNet.Core.Authentication;
using HydraDotNet.Core.Endpoints;
using RestSharp;
using System.Threading.Tasks;

namespace HydraDotNet.Core;

public class HydraClient
{
    private const string _DEFAULT_HYDRA_HOST = "https://dokken-api.wbagora.com";
    private const string _SERVER_API_KEY = "51586fdcbd214feb84b0e475b130fce0";

    private string? _overrideApiKey;
    private RestClient _client;
    private HydraClientConfiguration _config;

    public string ApiKey 
    { 
        get
        {
            if (string.IsNullOrEmpty(_overrideApiKey))
                return _SERVER_API_KEY;
            else return _overrideApiKey;
        }
    }

    public HydraClient(HydraClientConfiguration? config = null, string? apiKey = null)
    {
        _overrideApiKey = apiKey;

        if (config is null) config = new();

        _config = config;

        _client = new RestClient(_DEFAULT_HYDRA_HOST)
        {
            Authenticator = new HydraAuthenticator(this),
            AcceptedContentTypes = new[] { _config.ForceJSONRequest ? "application/json" : "application/x-ag-binary" }
        };
    }

    /// <summary>
    /// Executes a request to a Hydra endpoint.
    /// </summary>
    /// <param name="endpoint">Endpoint to execute the request upon.</param>
    /// <returns>Response from the endpoint.</returns>
    public HydraApiResponse DoRequest(HydraEndpoint endpoint)
    {
        var restResponse = endpoint.GetResponse(_client);

        return new(restResponse);
    }

    /// <summary>
    /// Asynchronously executes a request to a Hydra endpoint.
    /// </summary>
    /// <param name="endpoint">Endpoint to execute the request upon.</param>
    /// <returns>Response from the endpoint.</returns>
    public async ValueTask<HydraApiResponse> DoRequestAsync(HydraEndpoint endpoint)
    {
        var restResponse = await endpoint.GetResponseAsync(_client);

        return new(restResponse);
    }

    /// <summary>
    /// Executes a request to a Hydra endpoint.
    /// </summary>
    /// <param name="endpoint">Endpoint to execute the request upon.</param>
    /// <param name="response">Response from the endpoint.</param>
    /// <returns>If the response returned an OK status code.</returns>
    public bool TryDoRequest(HydraEndpoint endpoint, out HydraApiResponse response)
    {
        response = DoRequest(endpoint);

        return response.StatusCode == System.Net.HttpStatusCode.OK;
    }
}
