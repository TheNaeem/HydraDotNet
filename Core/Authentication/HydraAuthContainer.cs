using HydraDotNet.Core.Api;
using HydraDotNet.Core.Models;
using RestSharp;
using System;
using System.Diagnostics;

namespace HydraDotNet.Core.Authentication;

public class HydraAuthContainer : IAuthContainer
{
    protected string? _accessToken;
    public string AccessToken
    {
        get
        {
            if (Sw.Elapsed >= Expiration)
            {
                UpdateToken();
            }

            return _accessToken ?? string.Empty;
        }
    }

    protected Stopwatch Sw { get; set; }
    protected TimeSpan Expiration { get; set; }
    private HydraClient? Client { get; set; }

    protected HydraAuthContainer()
    {
        Sw = new();
    }

    /// <summary>
    /// Hydra auth container constructor.
    /// </summary>
    /// <param name="client">Corresponding HydraClient</param>
    public HydraAuthContainer(HydraClient client)
    {
        Client = client;
        Sw = new();

        UpdateToken();
    }

    private HydraAuthTokenResponse? GetAccessToken(string profileToken)
    {
        var body = new HydraAuthTokenRequestBody()
        {
            code = profileToken,
            grant_type = "authorization_code"
        };

        var param = new BodyParameter(string.Empty, body, "application/json", DataFormat.Json);
        var request = Endpoints.AuthToken.CreateRequest(Method.Post, param);

        var response = Client?.DoRequest(request);

        if (response is null)
            return default;

        return response.GetContent<HydraAuthTokenResponse>();
    }

    public virtual void UpdateToken()
    {
        var access = Client?.GetAccountAccessInfo();
        var content = access?.GetContent<HydraAccessAuthResponse>();

        if (content is null ||
            content.wb_network is null ||
            string.IsNullOrEmpty(content.wb_network.network_token))
            return;

        var response = GetAccessToken(content.wb_network.network_token);

        if (response is null ||
            string.IsNullOrEmpty(response.access_token))
            return;

        Sw = Stopwatch.StartNew();
        Expiration = TimeSpan.FromSeconds(response.expires_in);
        _accessToken = response.access_token;
    }
}
