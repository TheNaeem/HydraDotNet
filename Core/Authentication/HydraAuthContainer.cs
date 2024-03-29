﻿using HydraDotNet.Core.Api;
using HydraDotNet.Core.Models;
using RestSharp;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HydraDotNet.Core.Authentication;

public class HydraAuthContainer : IAuthContainer
{
    private string? _token;
    private string? _networkToken;

    public string Token => _token ?? string.Empty;
    public string NetworkToken 
    {
        get
        {
            if (HasAccessTokenExpired())
            {
                UpdateToken();
            }

            return _networkToken ?? string.Empty;
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

    private HydraAuthTokenResponse? GetNetworkAccessToken(string profileToken)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasAccessTokenExpired() => Sw.Elapsed >= Expiration;

    public virtual void UpdateToken()
    {
        var access = Client?.GetAccountAccessInfo();
        var content = access?.GetContent<HydraAccessAuthResponse>();

        if (content is null ||
            content.wb_network is null ||
            string.IsNullOrEmpty(content.wb_network.network_token) ||
            string.IsNullOrEmpty(content.token))
            return;

        _token = content.token;
        var response = GetNetworkAccessToken(content.wb_network.network_token);

        if (response is null ||
            string.IsNullOrEmpty(response.access_token))
            return;

        Sw = Stopwatch.StartNew();
        Expiration = TimeSpan.FromSeconds(response.expires_in);
        _networkToken = response.access_token;
    }
}
