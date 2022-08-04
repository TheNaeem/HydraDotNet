using HydraDotNet.Core.Authentication;
using HydraDotNet.Core.Encoding;
using HydraDotNet.Core.Endpoints;
using HydraDotNet.Core.Models;
using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;

namespace HydraDotNet.Core;

public class HydraClient
{
    private const string _DEFAULT_HYDRA_HOST = "https://dokken-api.wbagora.com";
    private const string _SERVER_API_KEY = "51586fdcbd214feb84b0e475b130fce0";

    private string? _overrideApiKey;
    private RestClient _client;
    private HydraClientConfiguration _config;
    private EpicAuthContainer _auth;

    public string ApiKey 
    { 
        get
        {
            if (string.IsNullOrEmpty(_overrideApiKey))
                return _SERVER_API_KEY;
            else return _overrideApiKey;
        }
    }

    /// <summary>
    /// Hydra client constructor.
    /// </summary>
    /// <param name="epicAuth">Container with external Epic games authentication information.</param>
    /// <param name="config">Optional: Configuration for Hydra client.</param>
    /// <param name="apiKey">Optional: Overrides the api key.</param>
    public HydraClient(ExternalEpicAuthContainer epicAuth, HydraClientConfiguration? config = null, string? apiKey = null)
    {
        _auth = epicAuth;
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
    /// Creates access to authorized endpoints.
    /// </summary>
    /// <param name="onLoginSuccessful">Optional: Executes after logging in successfully.</param>
    /// <param name="onLoginFailed">Optional: Executes if logging in fails. If this is not passed in then an exception will be thrown on failure.</param>
    /// <returns></returns>
    public async Task LoginAsync(Action? onLoginSuccessful = null, Action<string?, HttpStatusCode, Exception>? onLoginFailed = null)
    {
        var body = new HydraAccessRequestBody();
        body.auth.epic = _auth.AccessToken;

        await using var encoder = new HydraEncoder();
        encoder.WriteValue(body);

        var accessEndpoint = new HydraEndpoint("/access", await encoder.GetBufferAsync(), Method.Post);

        var response = await DoRequestAsync(accessEndpoint);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            var ex = new TaskCanceledException($"Access token request unsuccessful with response status code {response.StatusCode}");

            if (onLoginFailed is null)
                throw ex;

            onLoginFailed(response.GetContentString(), response.StatusCode, ex);

            return;
        }

        if (onLoginSuccessful is not null)
            onLoginSuccessful();
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
