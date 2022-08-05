using HydraDotNet.Core.Authentication;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace HydraDotNet.Core.Api;

/// <summary>
/// Api request that can be executed with a Hydra client.
/// </summary>
public class HydraApiRequest : RestRequest
{
    public HydraEndpoint Endpoint { get; init; }

    public HydraApiRequest(HydraEndpoint endpoint, Method method, Parameter? parameter = null) 
        : base(endpoint.BaseUrl + endpoint.Path, method)
    {
        Endpoint = endpoint;

        if (parameter is not null)
            AddParameter(parameter);
    }

    public HydraApiRequest(HydraEndpoint endpoint, Method method, ParametersCollection parameters) 
        : base(endpoint.BaseUrl + endpoint.Path, method)
    {
        Endpoint = endpoint;

        this.AddOrUpdateParameters(parameters);
    }

    public RestResponse GetResponse(RestClient client)
    {
        if (client.Authenticator is not HydraAuthenticator)
            throw new ArgumentException("Attempted to execute a request with a client not using a HydraAuthenticator.");

        return client.Execute(this);
    }

    public async Task<RestResponse> GetResponseAsync(RestClient client)
    {
        if (client.Authenticator is not HydraAuthenticator)
            throw new ArgumentException("Attempted to execute a request with a client not using a HydraAuthenticator.");

        return await client.ExecuteAsync(this);
    }
}
